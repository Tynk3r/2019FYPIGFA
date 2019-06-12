using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    CharacterController characterController;

    public float health = 10f, maxHealth = 10f, stamina = 10f, maxStamina = 10f;
    public float walkSpeed = 6.0f;
    public float sprintSpeedModifier = 1.25f;
    private float sprintSpeed = 7.5f;
    public float jumpSpeed = 6.0f;
    public float gravity = 20.0f;
    private bool doubleJump = false;
    public Camera mainCamera;
    public float mouseXSpeed = 2f, mouseYSpeed = 2f;

    private Vector3 moveDirection = Vector3.zero;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        sprintSpeed = walkSpeed * sprintSpeedModifier;
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Input Update
        if (characterController.isGrounded)
        {
            doubleJump = false;
            moveDirection = transform.TransformDirection(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            // Deplete stamina as you sprint/only activates if player is moving and holding sprint button
            if(Input.GetButton("Sprint") && stamina > 0f && moveDirection.magnitude > 0f)
            {
                stamina = Mathf.Max(stamina - Time.deltaTime, 0f);
                moveDirection *= sprintSpeed;
            }
            else
            {
                stamina = Mathf.Min(stamina + Time.deltaTime, maxStamina);
                moveDirection *= walkSpeed;
            }
    
            if (Input.GetButtonDown("Jump"))
            {
                doubleJump = true;
                moveDirection.y = jumpSpeed;
            }
        }
        else // Whilst In Air
        {
            if (doubleJump && Input.GetButtonDown("Jump"))
            {
                doubleJump = false;
                moveDirection.y = jumpSpeed;
            }
        }

        // Movement/Look Update
        moveDirection.y -= gravity * Time.deltaTime;
        characterController.Move(moveDirection * Time.deltaTime);
        transform.Rotate(0, mouseXSpeed * Input.GetAxis("Mouse X"), 0);
        mainCamera.transform.Rotate(mouseYSpeed * Input.GetAxis("Mouse Y"), 0, 0);
    }
}