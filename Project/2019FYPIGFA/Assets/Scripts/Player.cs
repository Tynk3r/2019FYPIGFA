using UnityEngine;
using System.Collections;

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

    [Header("Movement")]
    public float walkSpeed = 5.0f;
    private float smoothWalkSpeed;
    [Range(1.0f, 3.0f)]
    public float sprintSpeedModifier = 2f;
    private float sprintSpeed = 7.5f;
    private float smoothSprintSpeed;
    public float jumpSpeed = 6.0f;
    public float gravity = 20.0f;
    private bool doubleJump = false;
    private Vector3 moveDirection = Vector3.zero;

    [Header("Look")]
    public float mouseXSpeed = 3f;
    public float mouseYSpeed = 3f;
    public float maxYLookRange = 15f;
    public GameObject cameraLookObject;
    private Vector2 rotation = Vector2.zero;
    private int defaultFOV = 60;
    private float cameraSwayAngle = 0f;
    private float cameraSwayMaxAngle = 0.5f;

    private CharacterController characterController;

    void Start()
    {
        // Variable Initialisation
        health = maxHealth;
        stamina = maxStamina;
        sprintSpeed = walkSpeed * sprintSpeedModifier;
        characterController = GetComponent<CharacterController>();

        // Misc QOL Stuff
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        UpdateMove();
        UpdateLook();
        UpdateUI();
    }

    void UpdateMove()
    {

        if (characterController.isGrounded)
        {
            doubleJump = false;
            moveDirection = (transform.right * Input.GetAxis("Horizontal")) + (Vector3.ProjectOnPlane(transform.forward, new Vector3(0, 1, 0)) * Input.GetAxis("Vertical"));
            // Deplete stamina as you sprint/only activates if player is moving and holding sprint button
            if (Input.GetButton("Sprint") && moveDirection.magnitude > 0f && !staminaRecovering)
            {
                stamina = Mathf.Max(stamina - (Time.deltaTime * staminaDecayMultiplier), 0f);
                float maxFOV = defaultFOV * Mathf.Clamp(sprintSpeedModifier * 0.6f, 1f, 100f);
                Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView + (Time.deltaTime * 50), defaultFOV, maxFOV);
                moveDirection *= sprintSpeed;
            }
            else if (moveDirection.magnitude > 0f)
            {
                Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView - (Time.deltaTime * 50), defaultFOV, Camera.main.fieldOfView); // defaultFOV
                moveDirection *= walkSpeed;
            }
            else
            {
                Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView - (Time.deltaTime * 50), defaultFOV, Camera.main.fieldOfView); // defaultFOV
            }

            if (Input.GetButton("Jump"))
            {
                doubleJump = true;
                moveDirection.y = jumpSpeed;
            }
        }
        else // Whilst In Air 
        {
            if (Input.GetButton("Sprint") && moveDirection.magnitude > 0f && !staminaRecovering)
            {
                stamina = Mathf.Max(stamina - (Time.deltaTime * staminaDecayMultiplier), 0f);
                float maxFOV = defaultFOV * Mathf.Clamp(sprintSpeedModifier * 0.6f, 1f, 100f);
                Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView + (Time.deltaTime * 50), defaultFOV, maxFOV);
            }
            else
            {
                Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView - (Time.deltaTime * 50), defaultFOV, Camera.main.fieldOfView);
            }

            if (doubleJump && Input.GetButtonDown("Jump"))
            {
                doubleJump = false;
                moveDirection.y = jumpSpeed;
            }
        }
        moveDirection.y -= gravity * Time.deltaTime; // Ensure a Stunk to floor
        characterController.Move(moveDirection * Time.deltaTime);
    }

    void UpdateLook()
    {
        if (characterController.isGrounded)
        {
            // Strafing Camera Sway
            cameraSwayAngle = Input.GetAxis("Horizontal") * -cameraSwayMaxAngle;
        }
        Camera.main.transform.localRotation = Quaternion.Euler(0, 0, cameraSwayAngle);

        // Mouse Controls
        rotation.y += Input.GetAxis("Mouse X");
        rotation.x += Input.GetAxis("Mouse Y");
        rotation.x = Mathf.Clamp(rotation.x, -maxYLookRange, maxYLookRange); // lock Y look
        transform.localRotation = Quaternion.Euler(rotation.x * mouseXSpeed, rotation.y * mouseYSpeed, 0);
    }

    void UpdateUI()
    {
        if (GetStam() <= 0f && !staminaRecovering && stamRegenTimerDone)
        {
            GameObject.FindGameObjectWithTag("Stam Bar Outline").GetComponentInChildren<Blink>().StartBlink();
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
            GameObject.FindGameObjectWithTag("Stam Bar Outline").GetComponentInChildren<Blink>().StopBlink();
        }
        else
        {
            stamRegenTimer -= Time.deltaTime;
        }
        if (stamRegenTimerDone)
            stamina = Mathf.Min(stamina + (Time.deltaTime * 0.5f * staminaRegenMultiplier), maxStamina);
        GameObject staminaBar = GameObject.FindGameObjectWithTag("Stam Bar");
        staminaBar.GetComponent<RectTransform>().localScale = new Vector3(GetStam() * 4, staminaBar.transform.localScale.y, staminaBar.transform.localScale.z);
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