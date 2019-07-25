using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lift : MonoBehaviour
{
    public GameObject leftDoor;
    public GameObject rightDoor;    
    public LiftTrigger liftObj;
    public MapGenerator mapGenObj;

    public float doorSpeed = 1f;
    public float doorDisplacementValue;
    private Vector3 m_leftDoorStartPos;
    private Vector3 m_rightDoorStartPos;

    private float m_doorDisplacementFrac;
    private float m_countDown;
    LIFT_STATE currState;
    // Start is called before the first frame update
    void Start()
    {
        m_doorDisplacementFrac = 0f;
        currState = LIFT_STATE.TRANSITIONING;
        m_countDown = 0f;
        m_leftDoorStartPos = leftDoor.transform.position;
        m_rightDoorStartPos = rightDoor.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        switch(currState)
        {
            case LIFT_STATE.UNLOCKED:
                if (liftObj.playerInLift)
                    ChangeState(LIFT_STATE.CLOSING);
                break;
            case LIFT_STATE.LOCKED:
                m_countDown -= Time.deltaTime;
                if (m_countDown <= 0f)
                    ChangeState(LIFT_STATE.UNLOCKED);
                break;
            case LIFT_STATE.TRANSITIONING:
                m_countDown -= Time.deltaTime;
                if (m_countDown <= 0f)
                    ChangeState(LIFT_STATE.GENERATING);
                break;
            case LIFT_STATE.GENERATING:
                m_countDown -= Time.deltaTime;
                if (m_countDown <= 0f)
                    ChangeState(LIFT_STATE.OPENING);
                    break;
            case LIFT_STATE.OPENING:
                m_doorDisplacementFrac = Mathf.Min(1f, m_doorDisplacementFrac + doorSpeed * Time.deltaTime);
                leftDoor.transform.position = Vector3.Slerp(m_leftDoorStartPos, new Vector3(m_leftDoorStartPos.x - doorDisplacementValue, m_leftDoorStartPos.y, m_leftDoorStartPos.z), m_doorDisplacementFrac);
                rightDoor.transform.position = Vector3.Slerp(m_rightDoorStartPos, new Vector3(m_rightDoorStartPos.x + doorDisplacementValue, m_rightDoorStartPos.y, m_rightDoorStartPos.z), m_doorDisplacementFrac);
                if (m_doorDisplacementFrac == 1f)
                    ChangeState(LIFT_STATE.LOCKED);
                break;
            case LIFT_STATE.CLOSING:
                m_doorDisplacementFrac = Mathf.Min(1f, m_doorDisplacementFrac + doorSpeed * Time.deltaTime);
                leftDoor.transform.position = Vector3.Slerp(new Vector3(m_leftDoorStartPos.x - doorDisplacementValue, m_leftDoorStartPos.y, m_leftDoorStartPos.z), m_leftDoorStartPos, m_doorDisplacementFrac);
                rightDoor.transform.position = Vector3.Slerp(new Vector3(m_rightDoorStartPos.x + doorDisplacementValue, m_rightDoorStartPos.y, m_rightDoorStartPos.z), m_rightDoorStartPos, m_doorDisplacementFrac);
                if (m_doorDisplacementFrac == 1f)
                    ChangeState(LIFT_STATE.TRANSITIONING);
                break;
        }
    }

    /// <summary>
    /// Changes the state of the lift.
    /// Also runs the starting effect at the beginning of a new state, based on new state
    /// </summary>
    /// <param name="_newState">The new state to change into</param>
    void ChangeState(LIFT_STATE _newState)
    {
        // TODO: REMOVE
        if (currState == _newState)
            Debug.LogError("New state of lift is same as initial state");
        currState = _newState;
        switch (currState)
        {
            case LIFT_STATE.UNLOCKED:
                break;
            case LIFT_STATE.LOCKED:
                m_countDown = 5f;
                break;
            case LIFT_STATE.TRANSITIONING:
                m_countDown = 1f;
                break;
            case LIFT_STATE.GENERATING:
                mapGenObj.GenerateMap();
                m_countDown = 2f;
                break;
            case LIFT_STATE.OPENING:
            case LIFT_STATE.CLOSING:
                m_doorDisplacementFrac = 0f;
                break;
            default:
                break;
        }
    }

    enum LIFT_STATE
    {
        UNLOCKED,
        LOCKED,
        TRANSITIONING,
        GENERATING,
        OPENING,
        CLOSING,
    }
}
