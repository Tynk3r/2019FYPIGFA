using UnityEngine;
using System.Collections;
using System;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{

    [Header("Stats")]
    public float maxHealth = 10f;
    public float maxStamina = 1f;
    public float staminaRegenMultiplier = 1f;
    private float stamRegenTimer = 0f;
    private bool stamRegenTimerDone = true;
    private float staminaDecayMultiplier = 1f;
    private float health, stamina;
    private bool staminaRecovering;

    [Header("UI")]
    public GameObject staminaBarOutline;
    public GameObject staminaBar;
    public Vector2 staminaBarPosition;
    public GameObject enemyName;
    public GameObject enemyHealthBarOutline;
    public GameObject enemyHealthBar;
    public Vector2 enemyHealthBarPosition;
    private Enemy currTarget = null;

    [Header("Movement")]
    public float walkSpeed = 5.0f;
    [Range(1.0f, 3.0f)]
    public float sprintSpeedModifier = 2f;
    [Range(0.1f, 1f)]
    public float strafeSpeedModifier = 0.75f;
    [Range(0.1f, 1f)]
    public float retreatSpeedModifier = 0.75f;
    private float maxFOV = 0f;
    [Range(10f, 100f)]
    public float FOVDeltaChange = 50f;
    public float jumpSpeed = 6.0f;
    public float gravity = 20.0f;
    private bool doubleJump = false;
    private Vector3 moveDirection = Vector3.zero;

    [Header("Look")]
    public float mouseXSpeed = 3f;
    public float mouseYSpeed = 3f;
    public float maxYLookRange = 15f;
    public GameObject yLookObject;
    public GameObject viewBobObject;
    private Vector2 rotation = Vector2.zero;
    private int defaultFOV = 60;
    private float cameraSwayAngle = 0f;
    private float cameraSwayMaxAngle = 0.5f;
    // View Bobbing
    public float maximumXBobFront;
    public float maximumYBobFront;
    public float maximumXBobBack;
    public float maximumYBobBack;
    public float viewBobSpeedFront;
    public float viewBobSpeedBack;
    private float viewBobTimer = 0.5f;
    // Landing Animation
    private bool falling = false;
    private float landingVelocity = 0;
    public float landingSpeedMultiplier;
    public float landingDistanceMultiplier;
    public float weaponLandingDistanceMultiplier = 0.5f;
    private float smoothWeaponLandingDistanceMultiplier = 0f;
    public float recoverSpeed;
    private IEnumerator landingCo;
    private IEnumerator recoveringCo;

    [Header("Inventory")]
    public Inventory weaponInventory;
    public HeldWeapon currentWeapon;

    private CharacterController characterController;

    void Start()
    {
        // Variable Initialisation
        health = maxHealth;
        stamina = maxStamina;
        maxFOV = Camera.main.fieldOfView * Mathf.Clamp(1f + ((sprintSpeedModifier - 1f) / 2.5f), 1f, 2f);
        characterController = GetComponent<CharacterController>();
        smoothWeaponLandingDistanceMultiplier = weaponLandingDistanceMultiplier;

        // Misc QOL Stuff
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        UpdateMove();
        UpdateWeapon();
        UpdateLook();
        UpdatePickup();
        UpdateUI();
    }

    void UpdateWeapon()
    {
        if (currentWeapon && currentWeapon.itemData != null && currentWeapon.itemData.weaponType != ItemData.WEAPON_TYPE.NONE)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                currentWeapon.Fire();
                if (currTarget != null)
                    enemyHealthBar.GetComponent<RectTransform>().localScale = new Vector3(currTarget.health / currTarget.maxHealth, enemyHealthBar.transform.localScale.y, enemyHealthBar.transform.localScale.z);
            }
            if (currentWeapon.itemData.durability <= 0)
            {
                weaponInventory.RemoveItem(currentWeapon.itemData);
                currentWeapon.RemoveWeapon();
                Debug.Log("Weapon Broke!");
            }
            if (Input.GetButtonDown("Next Weapon"))
            {
                if (weaponInventory.itemList.Count == 0)
                    Debug.Log("You Don't Have Any Weapons!");
                else if (weaponInventory.itemList.Count == 1)
                    Debug.Log("You Don't Have Any Other Weapons!");
                else if (weaponInventory.itemList.Count > 1)
                {
                    int nextWeaponIndex = weaponInventory.itemList.IndexOf(currentWeapon.itemData) + 1;
                    if (weaponInventory.itemList.IndexOf(currentWeapon.itemData) == weaponInventory.itemList.Count - 1)
                        nextWeaponIndex = 0;
                    currentWeapon.ChangeWeapon(weaponInventory.itemList[nextWeaponIndex]);
                }
            }
        }
        else if (weaponInventory.itemList.Count != 0)
            currentWeapon.ChangeWeapon(weaponInventory.itemList[0]);
    }

    void UpdatePickup()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)), out hit, 100))
        {
            Debug.DrawLine(Camera.main.transform.position, hit.point);
            if (hit.collider.GetComponent<Interactable>() != null)
            {
                GameObject interactable = hit.transform.gameObject;
                if (Input.GetAxis("Pick Up") > 0)
                {
                    if (weaponInventory.itemList.Count >= 3)
                        Debug.Log("No Space Left in Inventory");
                    else
                        interactable.GetComponent<Interactable>().OnPickedUp(this.gameObject);
                }
            }
        }
    }

    void UpdateMove()
    {
        if (characterController.isGrounded)
        {
            doubleJump = false;
            // moveDirection = (transform.right * Input.GetAxis("Horizontal")) + (Vector3.ProjectOnPlane(transform.forward, new Vector3(0, 1, 0)) * Input.GetAxis("Vertical")); // deprecated movement that ignored y look
            if (Input.GetButton("Sprint") && Input.GetAxis("Vertical") > 0f && !staminaRecovering)
            {
                stamina = Mathf.Max(stamina - (Time.deltaTime * staminaDecayMultiplier), 0f);
                moveDirection = (transform.forward * Input.GetAxis("Vertical") * walkSpeed * sprintSpeedModifier) + (transform.right * Input.GetAxis("Horizontal") * walkSpeed * strafeSpeedModifier);
            }
            else if (Input.GetAxis("Vertical") < 0f)
                moveDirection = (transform.forward * Input.GetAxis("Vertical") * walkSpeed * retreatSpeedModifier) + (transform.right * Input.GetAxis("Horizontal") * walkSpeed * strafeSpeedModifier);
            else
                moveDirection = (transform.forward * Input.GetAxis("Vertical") * walkSpeed) + (transform.right * Input.GetAxis("Horizontal") * walkSpeed * strafeSpeedModifier);

            if (Input.GetButton("Jump"))
            {
                doubleJump = true;
                moveDirection.y = jumpSpeed;
            }
            if (smoothWeaponLandingDistanceMultiplier != weaponLandingDistanceMultiplier)
                smoothWeaponLandingDistanceMultiplier = weaponLandingDistanceMultiplier;
        }
        else
        {
            if (Input.GetButton("Sprint") && Input.GetAxis("Vertical") > 0f && !staminaRecovering)
                stamina = Mathf.Max(stamina - (Time.deltaTime * staminaDecayMultiplier), 0f);

            if (doubleJump && Input.GetButtonDown("Jump"))
            {
                doubleJump = false;
                moveDirection.y = jumpSpeed;
            }
            if (smoothWeaponLandingDistanceMultiplier != 1)
                smoothWeaponLandingDistanceMultiplier = 1;
        }
        moveDirection.y -= gravity * Time.deltaTime; // Ensure a Stunk to floor
        characterController.Move(moveDirection * Time.deltaTime);
    }

    void UpdateLook()
    {
        // FOV Change Whilst Sprintng
        if (Input.GetButton("Sprint") && Input.GetAxis("Vertical") > 0f && !staminaRecovering)
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView + (Time.deltaTime * FOVDeltaChange), defaultFOV, maxFOV);
        else
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView - (Time.deltaTime * FOVDeltaChange), defaultFOV, Camera.main.fieldOfView);

        // Strafing Camera Sway
        if (characterController.isGrounded)
            cameraSwayAngle = Input.GetAxis("Horizontal") * -cameraSwayMaxAngle;

        // View Bobbing
        if (Input.GetAxis("Vertical") > 0 && characterController.isGrounded)
        {
            viewBobTimer += Time.deltaTime * viewBobSpeedFront;
            viewBobObject.transform.localPosition = new Vector3(Input.GetAxis("Vertical") * maximumXBobFront * Mathf.Sin(viewBobTimer * (2 * Mathf.PI)),
                                                                Input.GetAxis("Vertical") * maximumYBobFront * Mathf.Sin(viewBobTimer * (4 * Mathf.PI)),
                                                                0);
            if (viewBobTimer >= 1)
                viewBobTimer = 0f;
        }
        else if (Input.GetAxis("Vertical") < 0 && characterController.isGrounded)
        {
            viewBobTimer += Time.deltaTime * viewBobSpeedBack;
            viewBobObject.transform.localPosition = new Vector3(Input.GetAxis("Vertical") * maximumXBobBack * Mathf.Sin(viewBobTimer * (2 * Mathf.PI)),
                                                                Input.GetAxis("Vertical") * maximumYBobBack * Mathf.Sin(viewBobTimer * (4 * Mathf.PI)),
                                                                0);
            if (viewBobTimer >= 1)
                viewBobTimer = 0f;
        }
        else
            viewBobObject.transform.localPosition = new Vector3(Input.GetAxis("Vertical") * maximumXBobFront * Mathf.Sin(viewBobTimer * (2 * Mathf.PI)),
                                                                Input.GetAxis("Vertical") * maximumYBobFront * Mathf.Sin(viewBobTimer * (4 * Mathf.PI)),
                                                                0);

        // Landing Animation
        if (falling)
        {
            if (characterController.isGrounded)
            {
                falling = false;
                Debug.Log("Landed with velocity of " + landingVelocity);
                landingCo = LandingSink(landingVelocity);
                StartCoroutine(landingCo);
            }
            else
                landingVelocity = characterController.velocity.y;
        }
        else if (!characterController.isGrounded)
            falling = true;

        // Mouse Controls
        rotation.y += Input.GetAxis("Mouse X");
        rotation.x += Input.GetAxis("Mouse Y");
        rotation.x = Mathf.Clamp(rotation.x, -maxYLookRange, maxYLookRange); // lock Y look
        // Left to Right Look on Player, Up Down Look on Camera Look to isolate movement to XZ plane
        transform.localRotation = Quaternion.Euler(0, rotation.y * mouseYSpeed, 0);
        yLookObject.transform.localRotation = Quaternion.Euler(rotation.x * mouseXSpeed, 0, 0);
        viewBobObject.transform.localRotation = Quaternion.Euler(0, 0, cameraSwayAngle);
    }

    IEnumerator LandingSink(float landingVelocity)
    {
        float displacement = 0f;
        while (landingVelocity < 0)
        {
            //displacement += landingVelocity * Time.deltaTime * landingDistanceMultiplier;
            //Camera.main.transform.localPosition = new Vector3(0, displacement, 0);
            //landingVelocity += landingSpeedMultiplier;
            //if (landingVelocity >= 0)
            //    StartCoroutine(LandingRecovery());
            //yield return null;
            displacement = landingVelocity * Time.deltaTime * landingDistanceMultiplier;
            Camera.main.transform.Translate(0, displacement, 0, Space.World);
            currentWeapon.transform.Translate(0, displacement * smoothWeaponLandingDistanceMultiplier, 0, Space.World);
            landingVelocity += landingSpeedMultiplier;
            if (landingVelocity >= 0)
            {
                recoveringCo = LandingRecovery();
                StartCoroutine(recoveringCo);
                StopCoroutine(landingCo);
            }
            yield return null;
        }
    }

    IEnumerator LandingRecovery()
    {
        float displacement = Camera.main.transform.localPosition.y;
        //float recoverTimer = 0;
        while (Camera.main.transform.localPosition.y < 0)
        {
            //recoverTimer += Time.deltaTime * recoverSpeed;
            //Camera.main.transform.localPosition = new Vector3(0, Mathf.Lerp(displacement, 0, recoverTimer), 0);
            //yield return null;
            Camera.main.transform.Translate(0, recoverSpeed, 0, Space.World);
            currentWeapon.transform.Translate(0, recoverSpeed * smoothWeaponLandingDistanceMultiplier, 0, Space.World);
            yield return null;
        }
        if (Camera.main.transform.localPosition.y >= 0)
        {
            Camera.main.transform.localPosition = Vector3.zero;
            currentWeapon.transform.localPosition = currentWeapon.itemData.heldPosition;
            StopCoroutine(recoveringCo);
        }
    }

    void UpdateUI()
    {
        // Inventory
        if (Input.GetKeyDown(KeyCode.U))
            weaponInventory.PrintAllItems(currentWeapon.itemData);

        // Stamina
        if (staminaBarOutline.GetComponent<RectTransform>().localPosition != new Vector3(staminaBarPosition.x, staminaBarPosition.y, 0))
            staminaBarOutline.GetComponent<RectTransform>().localPosition = new Vector3(staminaBarPosition.x, staminaBarPosition.y, 0);
        if (GetStam() <= 0f && !staminaRecovering && stamRegenTimerDone)
        {
            staminaBarOutline.GetComponentInChildren<Blink>().StartBlink();
            stamRegenTimer = 2f;
            stamRegenTimerDone = false;
            staminaRecovering = true;
        }
        else if (staminaRecovering && stamRegenTimer <= 0f && !stamRegenTimerDone)
        {
            stamRegenTimer = 0f;
            stamRegenTimerDone = true;
        }
        else if (GetStam() >= 1f && staminaRecovering)
        {
            staminaRecovering = false;
            staminaBarOutline.GetComponentInChildren<Blink>().StopBlink();
        }
        else
        {
            stamRegenTimer -= Time.deltaTime;
        }
        if (stamRegenTimerDone)
            stamina = Mathf.Min(stamina + (Time.deltaTime * 0.5f * staminaRegenMultiplier), maxStamina);
        staminaBar.GetComponent<RectTransform>().localScale = new Vector3(stamina / maxStamina, staminaBar.transform.localScale.y, staminaBar.transform.localScale.z);

        // Current Enemy Health Bar
        if (enemyName.GetComponent<RectTransform>().localPosition != new Vector3(enemyHealthBarPosition.x, enemyHealthBarPosition.y, 0))
            enemyName.GetComponent<RectTransform>().localPosition = new Vector3(enemyHealthBarPosition.x, enemyHealthBarPosition.y, 0);
        RaycastHit hit;
        float range = 0f;
        if (currentWeapon && currentWeapon.itemData != null && currentWeapon.itemData.weaponType == ItemData.WEAPON_TYPE.CLOSE_RANGE)
            range = currentWeapon.itemData.attackRange;
        else
            range = 100f;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)), out hit, range) && hit.collider.GetComponent<Enemy>())
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (currTarget != enemy)
            {
                currTarget = enemy;
                if (!enemyName.activeSelf)
                    enemyName.SetActive(true);
                enemyName.GetComponent<TextMeshProUGUI>().SetText(Enum.GetName(typeof(Enemy.ENEMY_TYPE), enemy.enemyType));
                enemyHealthBar.GetComponent<RectTransform>().localScale = new Vector3(enemy.health / enemy.maxHealth, enemyHealthBar.transform.localScale.y, enemyHealthBar.transform.localScale.z);
            }
        }
        else if (currTarget != null)
        {
            currTarget = null;
            if (enemyName.activeSelf)
                enemyName.SetActive(false);
        }
    }

    /// <summary>
    /// Returns the percentage of health out of the max health of the player.
    /// </summary>
    /// <returns>
    /// Percentage of health out of the max health of the player.
    /// </returns>
    public float GetHealth()
    {
        return health / maxHealth;
    }
    /// <summary>
    /// Returns the percentage of stamina out of the max stamina of the player.
    /// </summary>
    /// <returns>
    /// The percentage of stamina out of the max stamina of the player.
    /// </returns>
    public float GetStam()
    {
        return stamina / maxStamina;
    }


}