using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
        HIT,            // HIT AN OBJECT 
        SLOWINGDOWN,    // OVERSHOT, WILL TRY TO DECELERATE
        DEATH,          // HEALTH IS DEPLETED, FALL OVER
    }

    public STATES currentState = STATES.ORIENTING;
    public Transform target = null;
    private Quaternion _lookRotation = Quaternion.identity;
    private Vector3 _direction = Vector3.zero;
    private Rigidbody rb = null;


    public float rotationSpeed = 0.5f;
    public float reorientSpeed = 0.1f;

    public float topSpeed = 50f;
    private float moveSpeed = 0f;

    public float sightDistance = 10f;
    public float pushForce = 10f;
    public float pushRate = 1f;
    private float pushTimer = 0f;

    private float slowDownSpeed = 0f;
    private float slowDownTimer = 0f;
    public float slowDownDelta = 1f;

    public float collideDamageMultiplier = 10f;
    public float damageToPlayer = 10f;

    public float impactForce = 30f;
    public float damageThreshold = 10f;
    private Vector3 bounceDirection = Vector3.zero;

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
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.detectCollisions = true;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        switch (currentState)
        {
            case STATES.IDLE:
                // Temp see if player is within view
                prevFrameDist = currDist;
                currDist = (target.position - transform.position).magnitude;
                if (currDist <= sightDistance)
                {
                    _direction = (target.position - transform.position).normalized;
                    Physics.Raycast(new Ray(transform.position, _direction), out RaycastHit hit, sightDistance);
                    if (hit.transform == target.transform)
                    {
                        currentState = STATES.ORIENTING;
                        break;
                    }
                }
                break;
            case STATES.ORIENTING:
                if (currDist <= sightDistance)
                {
                    _direction = (target.position - transform.position).normalized;
                    Physics.Raycast(new Ray(transform.position, _direction), out RaycastHit hit, sightDistance);
                    if (hit.transform != target.transform)
                    {
                        currentState = STATES.IDLE;
                        break;
                    }
                }
                else
                {
                    currentState = STATES.IDLE;
                    break;
                }

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
                    prevFrameDist = currDist;
                    currDist = (target.position - transform.position).magnitude;
                    currentState = STATES.REORIENT;
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
            case STATES.HIT:
                // Set speed coming into slowdown state
                if (slowDownSpeed == 0f)
                    slowDownSpeed = moveSpeed * 0.25f;

                // Applies speed to object
                SimpleMove(bounceDirection, moveSpeed);

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

        if (currentState != STATES.DEATH)
        {
            agent.FindClosestEdge(out NavMeshHit hit);
            if ((hit.position - agent.transform.position).magnitude <= agent.radius)
                OnHit(hit.normal);

            if (health <= 0f)
                Die();

        }
    }

    public override void Die()
    {
        base.Die();
        currentState = STATES.DEATH;
        rb.constraints = RigidbodyConstraints.None;
        rb.velocity = transform.forward * (moveSpeed*0.5f) + transform.right * 2.5f;
        if (rb.velocity.magnitude <= 0f)
            rb.velocity = transform.forward * 1.25f + transform.right * 2.5f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnHit(collision);
    }


    private void OnHit(Collision collision = null)
    {
        // Make sure not colliding with ground
        if (Physics.Raycast(new Ray(transform.position, -transform.up), out RaycastHit hit, Mathf.Infinity) && currentState != STATES.DEATH)
        {
            if (collision.gameObject != hit.collider.gameObject)
            {
                if (collision.rigidbody != null)
                {
                    collision.rigidbody.AddForce(-collision.GetContact(0).normal * impactForce);
                }

                float damage = (collision.collider.bounds.size.magnitude - GetComponent<Collider>().bounds.size.magnitude) * collideDamageMultiplier;
                TakeDamage(damage);
                if (collision.gameObject.transform == target && moveSpeed > 0)
                {
                    target.GetComponent<Player>().TakeDamage(damageToPlayer);
                }
                if (collision.collider.bounds.size.magnitude - GetComponent<Collider>().bounds.size.magnitude >= damageThreshold)
                {
                    bounceDirection = hit.normal;
                    currentState = STATES.HIT;
                }
            }
        }
    }

    private void OnHit(Vector3 normal)
    {
        if (currentState != STATES.DEATH)
        {
            TakeDamage(10);
            bounceDirection = normal;
            currentState = STATES.HIT;
        }
    }
}
