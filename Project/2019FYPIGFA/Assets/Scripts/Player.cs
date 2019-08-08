using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEditor;
using static SpawnPoint;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    public enum OBJECTIVE_PROGRESSION
    {
        LINEAR,
        CIRCUITOUS
    }
    public OBJECTIVE_PROGRESSION gameMode = OBJECTIVE_PROGRESSION.LINEAR;
    private CharacterController characterController;
    private SoundManager soundController;
    public GameController gameController;
    private Vector3 externalForce;
    private bool hasExternalForce;
    private List<Buffable.Buff> buffList;

    [Header("Stats")]
    public float maxHealth = 100f;
    public float maxStamina = 100f;
    public float staminaRegenMultiplier = 1f;
    public float healthPickupValue = 50f;
    public float doubleJumpCost = 10f;
    private float stamRegenTimer = 0f;
    private bool stamRegenTimerDone = true;
    private float staminaDecayMultiplier = 1f;
    private float health, stamina;
    private bool staminaRecovering;

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
    public Transform objectiveFloaterParent;
    public Camera minimapCamera;
    [Range(10f, 1f)]
    public float minimapZoom = 6f;

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
    [ReadOnly]
    public GameObject floorWeapon = null;
    [ReadOnly]
    public Collider[] hitColliders;
    public GameObject pickupInfoText;

    [Header("Objective")]
    public List<string> submittedObjectives = new List<string>();
    public List<string> heldObjectives = new List<string>();
    private Transform nextObjective = null;
    private bool walkingSound = false;
    private bool sprinting;

    ref CharacterController GetCharacterController()
    {
        return ref characterController;
    }

    void Start()
    {
        // Variable Initialisation
        health = maxHealth;
        stamina = maxStamina;
        maxFOV = Camera.main.fieldOfView * Mathf.Clamp(1f + ((sprintSpeedModifier - 1f) / 2.5f), 1f, 2f);
        characterController = GetComponent<CharacterController>();
        soundController = gameController.GetComponent<SoundManager>();
        smoothWeaponLandingDistanceMultiplier = weaponLandingDistanceMultiplier;
        inventoryPanel.gameObject.SetActive(false);
        shoppingList.SetActive(false);
        externalForce = new Vector3(0f, 0f, 0f);
        characterController.detectCollisions = false;
        hasExternalForce = false;
        heldObjectives.Clear();
        // Misc QOL Stuff
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        buffList = new List<Buffable.Buff>();
        currentWeapon.m_animator = (Animator)GetComponentInChildren(typeof(Animator));
    }

    void Update()
    {
        UpdateLook();
        if (hasExternalForce)
            UpdateExternalForce();
        UpdateMove();
        UpdatePickup();
        UpdateWeapon();
        UpdateInventory();
        UpdateUI();
    }

    void UpdateLook()
    {
        // FOV Change Whilst Sprintng
        if (sprinting)
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

    void UpdateMove()
    {
        if (characterController.isGrounded)
        {
            float walkModifier = 1f;
            doubleJump = false;
            // moveDirection = (transform.right * Input.GetAxis("Horizontal")) + (Vector3.ProjectOnPlane(transform.forward, new Vector3(0, 1, 0)) * Input.GetAxis("Vertical")); // deprecated movement that ignored y look
            if (Input.GetButton("Sprint")
                && Input.GetAxis("Vertical") > 0f
                && !staminaRecovering)
                sprinting = true;
            else
                sprinting = false;

            if (sprinting)
                walkModifier = sprintSpeedModifier;
            else if (Input.GetAxis("Vertical") < 0f)
                walkModifier = retreatSpeedModifier;
            else
                walkModifier = 1f;

            moveDirection = 
                transform.forward * Input.GetAxis("Vertical") * walkSpeed * walkModifier
                + transform.right * Input.GetAxis("Horizontal") * walkSpeed * strafeSpeedModifier;

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
            if (doubleJump 
                && Input.GetButtonDown("Jump")
                && !staminaRecovering)
            {
                doubleJump = false;
                moveDirection.y = jumpSpeed;
                stamina = Mathf.Max(stamina - doubleJumpCost, 0f);
            }
            if (smoothWeaponLandingDistanceMultiplier != 1)
                smoothWeaponLandingDistanceMultiplier = 1;
        }
        moveDirection.y -= gravity * Time.deltaTime; // Ensure a Stunk to floor
        characterController.Move(moveDirection * Time.deltaTime);
    }

    void UpdatePickup()
    {
        // Check what colliders in range
        hitColliders = Physics.OverlapCapsule(
            transform.position + new Vector3(0f, characterController.height, 0f),
            transform.position - new Vector3(0f, characterController.height, 0f),
            characterController.radius);
        floorWeapon = null;
        foreach (Collider c in hitColliders)
        {
            GameObject g = c.gameObject;

            // Update closest weapon
            if (g.GetComponent<Interactable>() != null)
                if (floorWeapon == null 
                    || (g.transform.position - transform.position).magnitude <= (floorWeapon.transform.position - transform.position).magnitude)
                    floorWeapon = g;

            // Pick Up SpawnPoint
            SpawnPoint pt = g.GetComponent<SpawnPoint>();
            if (pt != null)
                switch (pt.GetPointType())
                {
                    case POINT_TYPE.OBJECTIVE: 
                        // if less than one objective held or can pickup multiple objectives
                        if (heldObjectives.Count < 1 || gameMode == OBJECTIVE_PROGRESSION.LINEAR)
                            StartCoroutine(PickUpObjective(pt));
                        break;
                    case POINT_TYPE.HEALTH:
                        if (health < maxHealth)
                        {
                            gameController.RemovePoint(pt);
                            health = Mathf.Clamp(health + healthPickupValue, 0, maxHealth);
                            soundController.PlaySingle(gameController.healthSound);
                        }
                        break;
                }

            // Stairs trigger
            if (g.GetComponent<LevelTrigger>() != null
                && gameController.collectedAll)
                g.GetComponent<LevelTrigger>().Activate();
        }

        // Submit objectives
        if (Physics.Raycast(new Ray(transform.position, yLookObject.transform.forward), out RaycastHit hit, 10f)
            && hit.collider.GetComponent<Cashier>() != null
            && Input.GetButtonDown("Pick Up")
            && heldObjectives.Count > 0)
        {
            for (int i = 0; i < heldObjectives.Count; ++i)
            {
                Debug.Log("Submitted " + heldObjectives[i] + " to Cashier.");
                submittedObjectives.Add(heldObjectives[i]);
                gameController.shoppingListText.Remove(heldObjectives[i]);
            }
            heldObjectives.Clear();
            if (gameController.shoppingListText.Count <= 0)
                gameController.collectedAll = true;
            gameController.UpdateShoppingList();
            soundController.PlaySingle(gameController.submitSound);
        }

        // Pick up Weapon you are currently standing over
        if (Input.GetButtonDown("Pick Up") && floorWeapon != null)
        {
            if (weaponInventory.itemList.Count >= 3)
                Debug.Log("No Space Left in Inventory");
            else
            {
                floorWeapon.GetComponent<Interactable>().OnPickedUp(this.gameObject);
                if (floorWeapon.GetComponent<SpawnPoint>() != null)
                    gameController.RemovePoint(floorWeapon.GetComponent<SpawnPoint>());

            }
        }
    }

    void UpdateWeapon()
    {
        currentWeapon.UpdateAttack();
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
            else if (Input.GetButtonDown("Use") && ItemData.WEAPON_TYPE.NONE != currentWeapon.itemData.weaponType)
            {
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
                            // ApplyBuff(newBuff.buff, newBuff.duration);
                            currentWeapon.ApplyBuff(newBuff);
                        }
                    }
                    else
                        Debug.Log("There's no machine to use");
                }
            }
            if (currentWeapon.itemData.durability <= 0)
            {
                weaponInventory.RemoveItem(currentWeapon.itemData);
                currentWeapon.RemoveWeapon();
                Debug.Log("Weapon Broke!");
            }
            if (Input.GetButtonDown("Next Weapon") && !currentWeapon.m_attacking)
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
        }
        else if (weaponInventory.itemList.Count != 0)
            currentWeapon.ChangeWeapon(weaponInventory.itemList[0]);
    }

    void UpdateInventory()
    {
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
                if (Physics.Raycast(new Ray(transform.position, -transform.up), out RaycastHit hit, Mathf.Infinity))
                    droppedWeapon.transform.position = new Vector3(transform.position.x, hit.transform.position.y + 1, transform.position.z);
                else
                    droppedWeapon.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

                if (currentWeapon.itemData != null)
                {
                    Debug.LogError("Held Weapon was not destroyed");
                }
            }
        }
    }

    void UpdateUI()
    {
        // Inventory
        if (Input.GetKeyDown(KeyCode.U))
        {
            inventoryPanel.gameObject.SetActive(!inventoryPanel.gameObject.activeSelf);
            if (inventoryPanel.gameObject.activeSelf)
                shoppingList.SetActive(false);
        }

        // Check Objectives
        if (Input.GetKeyDown(KeyCode.O))
        {
            gameController.UpdateShoppingList();
            shoppingList.SetActive(!shoppingList.activeSelf);
            if (shoppingList.gameObject.activeSelf)
                inventoryPanel.gameObject.SetActive(false);
            string s = "Collected Items: ";
            int i = 0;
            foreach (string ss in submittedObjectives)
            {
                i++;
                if (i >= submittedObjectives.Count)
                    s += ss;
                else
                    s += ss + ", ";
            }
            Debug.Log(s);
        }

        // Update Pickup Info
        if (floorWeapon)
        {
            if (!pickupInfoText.activeSelf)
                pickupInfoText.SetActive(true);
            string s = "Press [E] to pick up " + floorWeapon.GetComponent<Interactable>().itemData.type;
            pickupInfoText.GetComponent<TextMeshProUGUI>().text = s;
        }
        else
            pickupInfoText.SetActive(false);

        // Objective Arrow (MAYBE INEFFICIENT CONSIDER REDOING)
        GameObject pt = null;
        if (arrowLocationType == POINT_TYPE.OBJECTIVE
         && gameMode == OBJECTIVE_PROGRESSION.CIRCUITOUS
         && heldObjectives.Count > 0
         || (gameMode == OBJECTIVE_PROGRESSION.LINEAR 
            && heldObjectives.Count + submittedObjectives.Count >= gameController.numberOfObjectives))
            pt = ((Cashier)FindObjectOfType(typeof(Cashier))).gameObject;
        if (submittedObjectives.Count == gameController.numberOfObjectives)
            pt = ((LevelTrigger)FindObjectOfType(typeof(LevelTrigger))).gameObject;

        SpawnPoint pt2 = gameController.GetClosestPoint(transform.position, arrowLocationType);
        if (pt2 != null && pt == null)
            pt = pt2.gameObject;

        if (pt != null)
        {
            nextObjective = pt.transform;
            if (!objectiveArrow.activeSelf)
                objectiveArrow.SetActive(true);
            objectiveArrow.transform.LookAt(new Vector3(nextObjective.transform.position.x, objectiveArrow.transform.position.y, nextObjective.transform.position.z), transform.up);
            objectiveArrow.transform.Rotate(90, 90, 0);
            if (!objectiveFloaterParent.gameObject.activeSelf)
                objectiveFloaterParent.gameObject.SetActive(true);
            objectiveFloaterParent.position = nextObjective.position + new Vector3(
                0f, 
                nextObjective.localScale.y * 0.5f, 
                0f);
        }
        else
        {
            objectiveArrow.SetActive(false);
            objectiveFloaterParent.gameObject.SetActive(false);
        }

        // Minimap
        if (Input.GetButtonDown("MinimapZoomIn"))
            minimapZoom = Mathf.Clamp(minimapZoom + 1, 1f, 10f);
        else if (Input.GetButtonDown("MinimapZoomOut"))
            minimapZoom = Mathf.Clamp(minimapZoom - 1, 1f, 10f);
        if (minimapCamera.orthographicSize != minimapZoom)
            minimapCamera.orthographicSize = minimapZoom;

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

        if (sprinting)
            stamina = Mathf.Max(stamina - (Time.deltaTime * staminaDecayMultiplier), 0f);
        else if (stamRegenTimerDone)
            stamina = Mathf.Min(stamina + (Time.deltaTime * 0.5f * staminaRegenMultiplier), maxStamina);

        staminaBar.GetComponent<RectTransform>().localScale = new Vector3(stamina / maxStamina, staminaBar.transform.localScale.y, staminaBar.transform.localScale.z);

        // Health
        if (healthBarOutline.GetComponent<RectTransform>().localPosition != new Vector3(healthBarPosition.x, healthBarPosition.y, 0))
            healthBarOutline.GetComponent<RectTransform>().localPosition = new Vector3(healthBarPosition.x, healthBarPosition.y, 0);
        if (healthBar.GetComponent<RectTransform>().localScale != new Vector3(health / maxHealth, healthBar.transform.localScale.y, healthBar.transform.localScale.z))
            healthBar.GetComponent<RectTransform>().localScale = new Vector3(health / maxHealth, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
        if (health == 0)
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }

        // Target Info
        if (enemyName.GetComponent<RectTransform>().localPosition != new Vector3(enemyHealthBarPosition.x, enemyHealthBarPosition.y, 0))
            enemyName.GetComponent<RectTransform>().localPosition = new Vector3(enemyHealthBarPosition.x, enemyHealthBarPosition.y, 0);
        float range = 100f; // Default
        if (currentWeapon && currentWeapon.itemData != null && currentWeapon.itemData.weaponType == ItemData.WEAPON_TYPE.RAYCAST)
            range = currentWeapon.itemData.attackRange;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)), out RaycastHit hit, range) && hit.collider.GetComponent<Enemy>() && hit.collider.GetComponent<Enemy>().alive)
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (currTarget != enemy)
                currTarget = enemy;
            if (!enemyName.activeSelf)
                enemyName.SetActive(true);
            if (enemyName.GetComponent<TextMeshProUGUI>().text != Enum.GetName(typeof(Enemy.ENEMY_TYPE), enemy.enemyType))
                enemyName.GetComponent<TextMeshProUGUI>().SetText(Enum.GetName(typeof(Enemy.ENEMY_TYPE), enemy.enemyType));
            Vector3 vector3 = new Vector3(enemy.health / enemy.maxHealth, enemyHealthBar.transform.localScale.y, enemyHealthBar.transform.localScale.z);
            if (enemyHealthBar.GetComponent<RectTransform>().localScale != vector3)
                enemyHealthBar.GetComponent<RectTransform>().localScale = vector3;
        }
        else
        {
            if (currTarget != null)
                currTarget = null;
            if (enemyName.activeSelf)
                enemyName.SetActive(false);
        }
    }

    void UpdateBuffs()
    {
        for (int i = 0; i < buffList.Count; ++i)
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
        switch (_buffType)
        {
            case Buffable.CHAR_BUFF.BUFF_SLOMO:
                Time.timeScale = 0.5f;
                break;
        }
    }

    void BuffEnd(Buffable.CHAR_BUFF _buffType)
    {
        switch (_buffType)
        {
            case Buffable.CHAR_BUFF.BUFF_SLOMO:
                Time.timeScale = 1f;
                break;
        }
    }

    void BuffTick(Buffable.Buff _buff)
    {
        switch (_buff.buff)
        {
            case Buffable.CHAR_BUFF.BUFF_SLOMO:
                break;
            default:
                Debug.LogError("No buff found!");
                break;
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
        soundController.PlaySingle(gameController.hitSound);
        return trueDamage <= 0f;
    }

    public void AddExternalForce(Vector3 _force)
    {
        externalForce += _force;
        if (externalForce.sqrMagnitude > 0.2f)
            hasExternalForce = true;
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
        while (Camera.main.transform.localPosition.y < 0)
        {
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

    IEnumerator PickUpObjective(SpawnPoint pt)
    {
        // TODO : Update Shopping List only when submitted
        string objective = gameController.RemovePoint(pt);
        heldObjectives.Add(objective);
        soundController.PlaySingle(gameController.pickUpSound);
        yield return 0;
        float percentageObjectsCollected = (float)(submittedObjectives.Count + heldObjectives.Count) / (float)(gameController.numberOfObjectives);
        Debug.Log(submittedObjectives.Count + " submitted " + heldObjectives.Count + " held " + gameController.shoppingListText.Count + " left " + (int)(percentageObjectsCollected * 100) + "% Collected");

        if (percentageObjectsCollected < 0.25f 
            && gameController.aggressionLevel != GameController.AGGRESSION_LEVELS.DOCILE)
            gameController.aggressionLevel = GameController.AGGRESSION_LEVELS.DOCILE;
        else if (percentageObjectsCollected >= 0.25f && percentageObjectsCollected < 0.5f
            && gameController.aggressionLevel != GameController.AGGRESSION_LEVELS.ANGRY)
            gameController.aggressionLevel = GameController.AGGRESSION_LEVELS.ANGRY;
        else if (percentageObjectsCollected >= 0.5f && percentageObjectsCollected < 0.75f
            && gameController.aggressionLevel != GameController.AGGRESSION_LEVELS.ENRAGED)
            gameController.aggressionLevel = GameController.AGGRESSION_LEVELS.ENRAGED;
        else if (percentageObjectsCollected >= 0.75f
            && gameController.aggressionLevel != GameController.AGGRESSION_LEVELS.INSANE)
            gameController.aggressionLevel = GameController.AGGRESSION_LEVELS.INSANE;

        Debug.Log(gameController.aggressionLevel);
        gameController.UpdateShoppingList();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
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
}