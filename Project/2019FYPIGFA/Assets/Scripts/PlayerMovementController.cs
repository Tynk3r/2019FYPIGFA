using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementController : MonoBehaviour
{
    CharacterController characterController;

    public float walkSpeed = 6.0f;
    public float sprintSpeedModifier = 1.25f;
    private float sprintSpeed = 7.5f;
    public float jumpSpeed = 6.0f;
    public float gravity = 20.0f;
    private bool doubleJump = false;

    private Vector3 moveDirection = Vector3.zero;

    void Start()
    {
        sprintSpeed = walkSpeed * sprintSpeedModifier;
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Input Update
        if (characterController.isGrounded)
        {
            doubleJump = false;
            Vector3 localDir = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            localDir *= walkSpeed;
            moveDirection = transform.TransformDirection(localDir);

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

        // Movement Update
        moveDirection.y -= gravity * Time.deltaTime;
        characterController.Move(moveDirection * Time.deltaTime);
    }
}