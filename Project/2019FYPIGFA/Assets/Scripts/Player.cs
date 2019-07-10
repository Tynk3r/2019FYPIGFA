using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEditor;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    private CharacterController characterController;
    public GameController gameController;
    public string[] collectedObjectives;
    private Vector3 externalForce;
    private bool hasExternalForce;

    [Header("Stats")]
    public float maxHealth = 100f;
    public float maxStamina = 100f;
    public float staminaRegenMultiplier = 1f;
    private float stamRegenTimer = 0f;
    private bool stamRegenTimerDone = true;
    private float staminaDecayMultiplier = 1f;
    private float health, stamina;
    private bool staminaRecovering;
    private List<Buffable.Buff> buffList;

    [Header("UI")]
    public GameObject staminaBarOutline;
    public GameObject staminaBar;
    public Vector2 staminaBarPosition;
    public GameObject healthBarOutline;
    public GameObject healthBar;
    public Vector2 healthBarPosition;
    public GameObject enemyName;
    public GameObject enemyHealthBarOutline;
    public GameObject enemyHealthBar;
    public Vector2 enemyHealthBarPosition;
    private Enemy currTarget = null;
    public GameObject shoppingList;
    public GameObject objectiveArrow;
    public SpawnPoint.POINT_TYPE arrowLocationType;
    private Transform nextObjective = null;

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
    public RectTransform inventoryPanel;
    private GameObject floorWeapon = null;
    public GameObject pickupInfoText;

    void Start()
    {
        // Variable Initialisation
        health = maxHealth;
        stamina = maxStamina;
        maxFOV = Camera.main.fieldOfView * Mathf.Clamp(1f + ((sprintSpeedModifier - 1f) / 2.5f), 1f, 2f);
        characterController = GetComponent<CharacterController>();
        smoothWeaponLandingDistanceMultiplier = weaponLandingDistanceMultiplier;
        inventoryPanel.gameObject.SetActive(false);
        shoppingList.SetActive(false);
        externalForce = new Vector3(0f, 0f, 0f);
        hasExternalForce = false;
        // Misc QOL Stuff
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        buffList = new List<Buffable.Buff>();
    }

    void Update()
    {
        UpdateMove();
        if (hasExternalForce)
            UpdateExternalForce();
        UpdateWeapon();
        UpdateBuffs();
        UpdateLook();
        UpdateInventory();
        UpdateUI();
    }

    void UpdateWeapon()
    {
        if (ItemData.BUFF_TYPE.NONE != currentWeapon.itemData.weaponBuff.buff)
        {
            currentWeapon.itemData.weaponBuff.duration -= Time.deltaTime;
            if (currentWeapon.itemData.weaponBuff.duration <= 0f)
            {
                currentWeapon.itemData.weaponBuff.duration = 0f;
                currentWeapon.itemData.weaponBuff.buff = ItemData.BUFF_TYPE.NONE;
                Debug.Log("Buff ran out");
            }
        }
        if (currentWeapon && currentWeapon.itemData != null && currentWeapon.itemData.weaponType != ItemData.WEAPON_TYPE.NONE)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (currentWeapon.Fire())
                {
                    //hit
                }
                else
                {
                    //weapon cooldown
                }
                if (currTarget != null)
                    enemyHealthBar.GetComponent<RectTransform>().localScale = new Vector3(currTarget.health / currTarget.maxHealth, enemyHealthBar.transform.localScale.y, enemyHealthBar.transform.localScale.z);
            }
            if (Input.GetButtonDown("Fire2"))
            {
                if (currentWeapon.Skill())
                {
                    // Skill used
                }
                else
                {
                    // Skill cooldown
                }
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

    void UpdateInventory()
    {
        // Pickup Interactables
        /*if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)), out RaycastHit hit, 100))
        {
            Debug.DrawLine(Camera.main.transform.position, hit.point);
            if (hit.collider.GetComponent<Interactable>() != null)
            {
                GameObject interactable = hit.transform.gameObject;
                if (Input.GetButtonDown("Pick Up"))
                {
                    if (weaponInventory.itemList.Count >= 3)
                        Debug.Log("No Space Left in Inventory");
                    else
                        interactable.GetComponent<Interactable>().OnPickedUp(this.gameObject);
                }
            }
        }*/
        if (Input.GetButtonDown("Pick Up") && floorWeapon != null)
        {
            if (weaponInventory.itemList.Count >= 3)
                Debug.Log("No Space Left in Inventory");
            else
                floorWeapon.GetComponent<Interactable>().OnPickedUp(this.gameObject);
        }
        if (Input.GetButtonDown("Use") && currentWeapon.itemData.weaponType != ItemData.WEAPON_TYPE.NONE)
        {
            // Ray cast to check for buff machine
            RaycastHit hit;
            const float useRange = 1f;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out hit, useRange))
            {
                BuffMachineBase machine = hit.collider.gameObject.GetComponent<BuffMachineBase>();
                if (null != machine)
                {
                    Debug.Log("Found a machine to use");
                    ItemData.WeaponBuff newBuff;
                    if (machine.DispenseBuff(out newBuff)) // If a buff has been found
                    {
                        Debug.Log("Used the dispenser successfully!");
                        //ApplyBuff(newBuff.buff, newBuff.duration);
                        currentWeapon.ApplyBuff(newBuff);
                    }
                }
                else
                    Debug.Log("There's no machine to use");
            }
        }
        // Drop Weapons From Inventory as Interactables
        if (Input.GetButtonDown("Drop Weapon"))
        {
            if (!currentWeapon || currentWeapon.itemData == null || currentWeapon.itemData.weaponType == ItemData.WEAPON_TYPE.NONE)
            {
                Debug.Log("No weapon is currently being held.");
            }
            else
            {
                weaponInventory.RemoveItem(currentWeapon.itemData);
                GameObject droppedWeapon = new GameObject("dropped" + currentWeapon.itemData.type, typeof(Interactable));
                droppedWeapon.GetComponent<Interactable>().Initialize(currentWeapon.RemoveWeapon());
                droppedWeapon.transform.position = transform.position;
                //droppedWeapon.GetComponent<Rigidbody>().AddForce(transform.forward);

                if (currentWeapon.itemData != null)
                {
                    Debug.LogError("Held Weapon was not destroyed");
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
                //Debug.Log("Landed with velocity of " + landingVelocity);
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

    void UpdateBuffs()
    {
        for(int i = 0; i < buffList.Count; ++i)
        {
            if ((buffList[i].duration - Time.deltaTime) < buffList[i].nextTickVal)
            {
                BuffTick(buffList[i]);
                buffList[i].nextTickVal -= 0.5f; // Buff will run one more time past 0.0f
            }
            buffList[i].duration -= Time.deltaTime;
            if (buffList[i].duration <= 0f)
            {
                BuffEnd(buffList[i].buff);
                buffList.Remove(buffList[i]);
            }
            if (buffList[i].duration != buffList[i].duration)
                Debug.LogError("Reference doesn't work here");
            // TODO: function to play sound on buff expunge?

        }
    }

    public void ApplyBuff(Buffable.CHAR_BUFF _buffType, float _duration = 0f)
    {
        // First find out if the buff has already been applied
        for (int i = 0; i < buffList.Count; ++i)
        {
            if (buffList[i].buff == _buffType)
            {
                buffList[i].duration = _duration;  // Reset duration and return
                return;
            }
        }
        // Create and add new buff instead
        Buffable.Buff newBuff = new Buffable.Buff();
        newBuff.duration = _duration;
        newBuff.buff = _buffType;
        buffList.Add(newBuff);
        // Initiate a starting effect for the new buff
        switch(_buffType)
        {
            case Buffable.CHAR_BUFF.BUFF_SLOMO:
                Time.timeScale = 0.5f;
                break;
        }
    }

    void BuffEnd(Buffable.CHAR_BUFF _buffType)
    {
        switch(_buffType)
        {
            case Buffable.CHAR_BUFF.BUFF_SLOMO:
                Time.timeScale = 1f;
                break;
        }
    }

    void BuffTick(Buffable.Buff _buff)
    {
        switch(_buff.buff)
        {
            case Buffable.CHAR_BUFF.BUFF_SLOMO:
                break;
            default:
                Debug.LogError("No buff found!");
                break;
        }
    }

    void UpdateExternalForce()
    {
        if (externalForce.sqrMagnitude < 30f)
        {
            externalForce = Vector3.zero;
            hasExternalForce = false;
            return;
        }
        characterController.Move(externalForce * Time.deltaTime);
        externalForce = Vector3.Lerp(externalForce, Vector3.zero, 1 * Time.deltaTime); // TODO: check decay on externalForce
    }

    IEnumerator LandingSink(float landingVelocity)
    {
        float displacement = 0f;
        while (landingVelocity < 0)
        {
            displacement = landingVelocity * Time.deltaTime * landingDistanceMultiplier;
            Camera.main.transform.Translate(0, displacement, 0, Space.World);
            currentWeapon.transform.Translate(0, displacement * smoothWeaponLandingDistanceMultiplier, 0, Space.World);
            landingVelocity += landingSpeedMultiplier;
            if (landingVelocity >= 0 || Camera.main.transform.position.y <= transform.position.y + (transform.localScale.y * 0.5f))
            {
                landingVelocity = 0;
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
            if (currentWeapon && currentWeapon.itemData != null)
                currentWeapon.transform.localPosition = currentWeapon.itemData.heldPosition;
            StopCoroutine(recoveringCo);
        }
    }

    void UpdateUI()
    {
        // Inventory
        if (Input.GetKeyDown(KeyCode.U))
        {
            inventoryPanel.gameObject.SetActive(!inventoryPanel.gameObject.activeSelf);
        }

        // Check Objectives
        if (Input.GetKeyDown(KeyCode.O))
        {
            gameController.UpdateShoppingList();
            shoppingList.SetActive(!shoppingList.activeSelf);
        }

        // Update Pickup Info
        if (floorWeapon)
        {
            if(!pickupInfoText.activeSelf)
                pickupInfoText.SetActive(true);
            string s = "Press [E] to pick up " + floorWeapon.GetComponent<Interactable>().itemData.type;
            pickupInfoText.GetComponent<TextMeshProUGUI>().text = s;
        }
        else
            pickupInfoText.SetActive(false);

        // Objective Arrow (MAYBE INEFFICIENT CONSIDER REDOING)
        nextObjective = gameController.GetClosestPoint(transform.position, arrowLocationType).transform;
        if (nextObjective.GetComponent<SpawnPoint>().GetPointType() == SpawnPoint.POINT_TYPE.EMPTY)
            objectiveArrow.SetActive(false);
        else if (nextObjective)
        {
            if (!objectiveArrow.activeSelf)
                objectiveArrow.SetActive(true);
            objectiveArrow.transform.LookAt(new Vector3(nextObjective.transform.position.x, objectiveArrow.transform.position.y, nextObjective.transform.position.z), transform.up);
            objectiveArrow.transform.Rotate(90, 90, 0);
        }

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

        // Health
        if (healthBarOutline.GetComponent<RectTransform>().localPosition != new Vector3(healthBarPosition.x, healthBarPosition.y, 0))
            healthBarOutline.GetComponent<RectTransform>().localPosition = new Vector3(healthBarPosition.x, healthBarPosition.y, 0);
        if (healthBar.GetComponent<RectTransform>().localScale != new Vector3(health / maxHealth, healthBar.transform.localScale.y, healthBar.transform.localScale.z))
            healthBar.GetComponent<RectTransform>().localScale = new Vector3(health / maxHealth, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
        if (health == 0)
            EditorApplication.isPlaying = false;

        // Target Info
        if (enemyName.GetComponent<RectTransform>().localPosition != new Vector3(enemyHealthBarPosition.x, enemyHealthBarPosition.y, 0))
            enemyName.GetComponent<RectTransform>().localPosition = new Vector3(enemyHealthBarPosition.x, enemyHealthBarPosition.y, 0);
        float range = 100f; // Default
        if (currentWeapon && currentWeapon.itemData != null && currentWeapon.itemData.weaponType == ItemData.WEAPON_TYPE.RAYCAST)
            range = currentWeapon.itemData.attackRange;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)), out RaycastHit hit, range) && hit.collider.GetComponent<Enemy>())
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
        else
        {
            if (currTarget != null)
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

    IEnumerator PickUpObjective(ControllerColliderHit hit)
    {
        string objective = gameController.RemovePoint(hit.collider.GetComponent<SpawnPoint>());
        yield return 0;
        gameController.UpdateShoppingList();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Pick up weapons
        if (other.GetComponent<SpawnPoint>() != null
            && other.GetComponent<Interactable>() != null
            && other.GetComponent<SpawnPoint>().GetPointType() == SpawnPoint.POINT_TYPE.WEAPON
            && other.gameObject != floorWeapon
            && floorWeapon == null)
        {
            floorWeapon = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Pick up weapons
        if (other.gameObject == floorWeapon)
        {
            floorWeapon = null;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Pick up Objectives on collide
        if (hit.collider.GetComponent<SpawnPoint>() != null 
            && hit.collider.GetComponent<SpawnPoint>().GetPointType() == SpawnPoint.POINT_TYPE.OBJECTIVE)
            StartCoroutine(PickUpObjective(hit));

        // Pushing Objects
        Rigidbody body = hit.collider.attachedRigidbody;
        float magnitude = externalForce.magnitude;
        // No rigidbody
        if (null == body || body.isKinematic)
        {
            if (1f == Mathf.Abs(hit.normal.x) || 1f == Mathf.Abs(hit.normal.z))
            {
                externalForce *= 0.7f;
                externalForce += magnitude * 0.1f * hit.normal;
            }
            return;
        }
        // We don't want to push objects below us
        if (hit.moveDirection.y < -0.3f)
            return;
        Vector3 pushDir = new Vector3(hit.moveDirection.x, hit.moveDirection.y, hit.moveDirection.z);
        //Debug.Log("The magnitude is " + Vector3.Project(externalForce, pushDir).magnitude );
        Vector3 proj = Vector3.Project(externalForce, pushDir) / hit.rigidbody.mass;
        if (hasExternalForce)
            body.velocity += proj;
        else
            body.velocity += pushDir * 2f / hit.rigidbody.mass;
        externalForce *= -0.1f; //* magnitude * 0.7f;
    }

    ref CharacterController GetCharacterController()
    {
        return ref characterController;
    }

    /// <summary>
    /// Apply damage to the player.
    /// </summary>
    /// <param name="_damage"></param>
    /// <returns>True if the player took damage.</returns>
    public bool TakeDamage(float _damage)
    {
        float trueDamage = Mathf.Clamp(_damage, 0, health);
        Debug.Log("Player took " + trueDamage + " damage.");
        health -= trueDamage;
        return trueDamage <= 0f;
    }


    public void AddExternalForce(Vector3 _force)
    {
        externalForce += _force;
        if (externalForce.sqrMagnitude > 0.2f)
            hasExternalForce = true;
    }
}