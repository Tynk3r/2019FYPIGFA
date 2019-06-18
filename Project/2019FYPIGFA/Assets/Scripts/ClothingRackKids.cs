using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ClothingRackKids : MonoBehaviour
{
    enum STATES
    {
        IDLE,           // LOOKING FOR TARGET
        ORIENTING,      // FOUND TARGET, ROTATING TOWARDS
        SPEEDINGUP,     // ACCELERATING TOWARDS TARGET
        REORIENT,      // HEADING TOWARD TARGET, WILL TRY AND RAM BY ROTATING TOWARDS
        SLOWINGDOWN,    // DECELERATING 
    }

    private STATES currState = STATES.ORIENTING;
    private float moveTimer = 0f;
    private Transform target = null;
    public float rotationSpeed;
    public float moveSpeed;
    public float moveSpeedDelta;
    private float smoothMoveSpeed = 0f;
    private float tempSpd = 0f;

    private float prevFrameDist = 0f;
    private float currDist = 0f;

    private Quaternion _lookRotation = Quaternion.identity;
    private Vector3 _direction = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currState)
        {
            case STATES.IDLE:
                if ((target.position - transform.position).magnitude < 10f)
                {

                }
                break;
            case STATES.ORIENTING:
                //find the vector pointing from our position to the target
                _direction = (target.position - transform.position).normalized;
                // need to project onto xz plane!!!!!!!!!!!!!!!!
                _direction = Vector3.ProjectOnPlane(_direction, new Vector3(0, 1, 0)).normalized/* * Input.GetAxis("Vertical")*/;

                //create the rotation we need to be in to look at the target
                _lookRotation = Quaternion.LookRotation(_direction);

                //rotate us over time according to speed until we are in the required rotation
                transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * rotationSpeed);

                //if mostly facing target, start accelerating
                if (Vector3.Dot(_direction, transform.forward) > 0.99f)
                {
                    currState = STATES.SPEEDINGUP;
                    moveTimer = 0f;
                    currDist = (target.position - transform.position).magnitude;
                    prevFrameDist = currDist;
                    Debug.Log("Changing to " + currState);
                }
                break;
            case STATES.SPEEDINGUP:
                currDist = (target.position - transform.position).magnitude;

                moveTimer += Time.deltaTime;
                smoothMoveSpeed = Mathf.Lerp(0, moveSpeed, moveTimer * moveSpeedDelta);
                GetComponent<CharacterController>().SimpleMove(transform.forward * smoothMoveSpeed);

                if (smoothMoveSpeed == moveSpeed)
                {
                    currState = STATES.REORIENT;
                    Debug.Log("Changing to " + currState);
                    currDist = (target.position - transform.position).magnitude;
                    prevFrameDist = currDist;
                }
                if (currDist > prevFrameDist)
                {
                    currState = STATES.SLOWINGDOWN;
                    moveTimer = 0f;
                    tempSpd = smoothMoveSpeed;
                    Debug.Log("Changing to " + currState);
                }

                prevFrameDist = currDist;
                break;
            case STATES.REORIENT:
                currDist = (target.position - transform.position).magnitude;

                GetComponent<CharacterController>().SimpleMove(transform.forward * smoothMoveSpeed);

                if (currDist > prevFrameDist)
                {
                    currState = STATES.SLOWINGDOWN;
                    moveTimer = 0f;
                    tempSpd = smoothMoveSpeed;
                    Debug.Log("Changing to " + currState);
                }

                prevFrameDist = currDist;
                break;
            case STATES.SLOWINGDOWN:
                moveTimer += Time.deltaTime;
                smoothMoveSpeed = Mathf.Lerp(tempSpd, 0, moveTimer * moveSpeedDelta);
                GetComponent<CharacterController>().SimpleMove(transform.forward * smoothMoveSpeed);
                if (smoothMoveSpeed == 0)
                {
                    currState = STATES.ORIENTING;
                    tempSpd = moveSpeed;
                    Debug.Log("Changing to " + currState);
                }
                break;
            default:
                break;
        }
    }
}
