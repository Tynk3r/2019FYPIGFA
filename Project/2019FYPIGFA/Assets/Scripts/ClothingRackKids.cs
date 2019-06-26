using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class ClothingRackKids : AIManager
{
    public enum STATES
    {
        IDLE,           // LOOKING FOR TARGET
        ORIENTING,      // FOUND TARGET, ROTATING TOWARDS
        SPEEDINGUP,     // ACCELERATING TOWARDS TARGET
        REORIENT,       // HEADING TOWARD TARGET, WILL TRY AND RAM BY ROTATING TOWARDS
        SLOWINGDOWN,    // OVERSHOT, WILL TRY TO DECELERATE
        DEATH,          // HEALTH IS DEPLETED, FALL OVER
    }

    public STATES currentState = STATES.ORIENTING;
    public Transform target = null;
    private Quaternion _lookRotation = Quaternion.identity;
    private Vector3 _direction = Vector3.zero;
    private Rigidbody rb = null;
    public float impactForce = 30f;

    public float rotationSpeed = 0.5f;
    public float reorientSpeed = 0.1f;

    public float topSpeed = 50f;
    private float moveSpeed = 0f;

    public float pushForce = 10f;
    public float pushRate = 1f;
    private float pushTimer = 0f;

    private float slowDownSpeed = 0f;
    private float slowDownTimer = 0f;
    public float slowDownDelta = 1f;

    private float prevFrameDist = 0f;
    private float currDist = 0f;

    // Start is called before the first frame update
    public override void Start()
    {
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
        enemyType = ENEMY_TYPE.CLOTHING_RACK_KIDS;
        ChangeSpeed(moveSpeed, 0f);

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.detectCollisions = true;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        switch (currentState)
        {
            case STATES.ORIENTING:
                // Rotate towrds target
                _direction = Vector3.ProjectOnPlane((target.position - transform.position).normalized, new Vector3(0, 1, 0)).normalized;
                _lookRotation = Quaternion.LookRotation(_direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, _lookRotation, rotationSpeed);

                // Check if need to change state
                if (Vector3.Dot(_direction, transform.forward) > 0.9999f)
                {
                    currentState = STATES.SPEEDINGUP;
                    currDist = (target.position - transform.position).magnitude;
                    prevFrameDist = currDist;
                    break;
                }
                break;
            case STATES.SPEEDINGUP:
                // Applies speed to object
                SimpleMove(transform.forward, moveSpeed);

                // Every interval, increases speed (simulates children pushing rack)
                if (pushTimer <= 0f)
                {
                    pushTimer = 1f;
                    moveSpeed = Mathf.Min(moveSpeed + pushForce, topSpeed);
                }
                else
                    pushTimer -= Time.deltaTime * pushRate;

                // Check if need to change state
                if (moveSpeed >= topSpeed)
                {
                    //Debug.Log("Reached Top Speed");
                    currentState = STATES.REORIENT;
                    prevFrameDist = currDist;
                    currDist = (target.position - transform.position).magnitude;
                    break;
                }

                // if heading away from target, slow down
                prevFrameDist = currDist;
                currDist = (target.position - transform.position).magnitude;
                //Debug.Log(currState + ", " + (currDist - prevFrameDist));
                if (currDist - prevFrameDist > 0.1f)
                    currentState = STATES.SLOWINGDOWN;
                break;
            case STATES.REORIENT:
                // Applies speed to object
                SimpleMove(transform.forward, moveSpeed);

                // Try to reorient towards player
                _direction = Vector3.ProjectOnPlane((target.position - transform.position).normalized, new Vector3(0, 1, 0)).normalized;
                _lookRotation = Quaternion.LookRotation(_direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, _lookRotation, reorientSpeed);
                // TODO: Z AXIS ROTATION (CAREENING)

                // if heading away from target, slow down
                prevFrameDist = currDist;
                currDist = (target.position - transform.position).magnitude;
                //Debug.Log(currState + ", " + (currDist - prevFrameDist));
                if (currDist - prevFrameDist > 0.1f)
                    currentState = STATES.SLOWINGDOWN;
                break;
            case STATES.SLOWINGDOWN:
                // Set speed coming into slowdown state
                if (slowDownSpeed == 0f)
                    slowDownSpeed = moveSpeed;

                // Applies speed to object
                SimpleMove(transform.forward, moveSpeed);

                // Decrease speed linearly (simulate kids putting foot on ground to stop rack)
                slowDownTimer = Mathf.Clamp(slowDownTimer + (Time.deltaTime * slowDownDelta), 0, 1);
                moveSpeed = Mathf.Lerp(slowDownSpeed, 0f, slowDownTimer);

                // TODO: SPARK PARTICLES AT WHEELS

                // Check if need to change state
                if (moveSpeed <= 0f)
                {
                    slowDownSpeed = 0f;
                    slowDownTimer = 0f;
                    currentState = STATES.ORIENTING;
                    break;
                }
                break;
            default:
                break;
        }

        if (health <= 0f)
            Die();
    }

    public override void Die()
    {
        base.Die();
        currentState = STATES.DEATH;
        rb.isKinematic = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(gameObject + " Collided with " + collision.gameObject);
        RaycastHit hit;
        // Make sure not colliding with ground
        if (Physics.Raycast(new Ray(transform.position, -transform.up), out hit, Mathf.Infinity) && currentState != STATES.DEATH)
        {
            //Debug.Log("Grounded to " + hit.collider.gameObject);
            if (collision.gameObject != hit.collider.gameObject)
            {
                //Debug.Log(gameObject + " collided with " + collision.gameObject);
                if (collision.rigidbody != null)
                {
                    collision.rigidbody.AddForce(-collision.GetContact(0).normal * impactForce);
                }
                float damage = (collision.collider.bounds.size.magnitude - GetComponent<Collider>().bounds.size.magnitude) * 10;
                TakeDamage(damage);
            }
        }
    }
}
