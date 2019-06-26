using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ClothingRackKids : Enemy
{
    public enum STATES
    {
        IDLE,           // LOOKING FOR TARGET
        ORIENTING,      // FOUND TARGET, ROTATING TOWARDS
        SPEEDINGUP,     // ACCELERATING TOWARDS TARGET
        REORIENT,       // HEADING TOWARD TARGET, WILL TRY AND RAM BY ROTATING TOWARDS
        SLOWINGDOWN,    // DECELERATING 
    }

    public  STATES currState = STATES.ORIENTING;
    private CharacterController character;
    public  Transform target = null;
    private Quaternion _lookRotation = Quaternion.identity;
    private Vector3 _direction = Vector3.zero;
    public  float impactForce = 30f;

    public  float rotationSpeed = 0.5f;
    public  float reorientSpeed = 0.1f;

    public  float topSpeed = 50f;
    private float moveSpeed = 0f;

    public  float pushForce = 10f;
    public  float pushRate = 1f;
    private float pushTimer = 0f;

    private float slowDownSpeed = 0f;
    private float slowDownTimer = 0f;
    public  float slowDownDelta = 1f;

    private float prevFrameDist = 0f;
    private float currDist = 0f;
    
    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<CharacterController>();
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        switch (currState)
        {
            case STATES.ORIENTING:
                // Rotate towrds target
                _direction = Vector3.ProjectOnPlane((target.position - transform.position).normalized, new Vector3(0, 1, 0)).normalized;
                _lookRotation = Quaternion.LookRotation(_direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, _lookRotation, rotationSpeed);

                // Check if need to change state
                if (Vector3.Dot(_direction, transform.forward) > 0.9999f)
                {
                    currState = STATES.SPEEDINGUP;
                    currDist = (target.position - transform.position).magnitude;
                    prevFrameDist = currDist;
                    break;
                }
                break;
            case STATES.SPEEDINGUP:

                // Applies speed to object
                character.SimpleMove(transform.forward * moveSpeed);

                // Every interval, increases speed (simulates children pushing rack)
                if (pushTimer <= 0f)
                {
                    pushTimer = 1f;
                    moveSpeed = Mathf.Min(moveSpeed+pushForce,topSpeed);
                }
                else
                    pushTimer -= Time.deltaTime * pushRate;

                // Check if need to change state
                if (moveSpeed >= topSpeed)
                {
                    Debug.Log("Reached Top Speed");
                    currState = STATES.REORIENT;
                    prevFrameDist = currDist;
                    currDist = (target.position - transform.position).magnitude;
                    break;
                }

                // if heading away from target, slow down
                prevFrameDist = currDist;
                currDist = (target.position - transform.position).magnitude;
                //Debug.Log(currState + ", " + (currDist - prevFrameDist));
                if (currDist - prevFrameDist > 0.1f)
                    currState = STATES.SLOWINGDOWN;
                break;
            case STATES.REORIENT:
                // Applies speed to object
                character.SimpleMove(transform.forward * topSpeed);

                // Try to reorient towards player
                _direction = Vector3.ProjectOnPlane((target.position - transform.position).normalized, new Vector3(0, 1, 0)).normalized;
                _lookRotation = Quaternion.LookRotation(_direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, _lookRotation, reorientSpeed);
                // TODO: Z AXIS ROTATION (CAREENING)
                //transform.Rotate(0f, 0f, reorientSpeed, Space.Self);

                // if heading away from target, slow down
                prevFrameDist = currDist;
                currDist = (target.position - transform.position).magnitude;
                //Debug.Log(currState + ", " + (currDist - prevFrameDist));
                if (currDist - prevFrameDist > 0.1f)
                    currState = STATES.SLOWINGDOWN;
                break;
            case STATES.SLOWINGDOWN:
                // Set speed coming into slowdown state
                if (slowDownSpeed == 0f)
                    slowDownSpeed = moveSpeed;

                // Applies speed to object
                character.SimpleMove(transform.forward * moveSpeed);

                // Decrease speed linearly (simulate kids putting foot on ground to stop rack)
                slowDownTimer = Mathf.Clamp(slowDownTimer + (Time.deltaTime * slowDownDelta), 0, 1);
                moveSpeed = Mathf.Lerp(slowDownSpeed, 0f, slowDownTimer);

                // TODO: SPARK PARTICLES AT WHEELS

                // Check if need to change state
                if (moveSpeed <= 0f)
                {
                    slowDownSpeed = 0f;
                    slowDownTimer = 0f;
                    currState = STATES.ORIENTING;
                    break;
                }
                break;
            default:
                break;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.rigidbody != null)
        {
            hit.rigidbody.AddForce(-hit.normal * impactForce);

        }
    }
}
