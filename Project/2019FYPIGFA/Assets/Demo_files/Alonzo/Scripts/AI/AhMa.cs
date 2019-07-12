using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class AhMa : AIManager
{
    public float enragedHealthThreshold = 50f;
    public float baseMoveSpeed = 3.5f;
    public float enragedMoveSpeed = 3.5f;
    public static float ATTACK_RATE = 0.3f;
    private float m_countDown = 0f;
    public float acceleration = 60f;
    public float attackRangeSquared = 4f;

    public Player D_PLAYERTARGET;
    enum STATES
    {
        IDLE,
        HOSTILE,
        ENRAGED,
        DEAD
    }
    STATES currState = STATES.HOSTILE;
    // Start is called before the first frame update
    override public void Start()
    {
        base.Start();
        enemyType = ENEMY_TYPE.AHMA;
        ChangeSpeed(baseMoveSpeed, acceleration);
        ChangeState(STATES.HOSTILE); //TODO : REMOVE

        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.detectCollisions = true;
    }

    // Update is called once per frame
    override public void Update()
    {
        m_countDown = Mathf.Max(m_countDown - Time.deltaTime, 0f);
        UpdateStates();
        if (health <= 0f)
            Die();
    }
    void UpdateStates()
    {
        switch (currState)
        {
            case STATES.ENRAGED:
                if ((D_PLAYERTARGET.transform.position - transform.position).sqrMagnitude < attackRangeSquared && m_countDown == 0f)
                {
                    // Stop moving,
                    agent.updatePosition = false;
                    // Look at the player
                    transform.LookAt(D_PLAYERTARGET.transform.position, transform.up);
                    // Attack
                    m_countDown = ATTACK_RATE;
                    D_PLAYERTARGET.TakeDamage(5f);
                }
                else
                {
                    agent.updatePosition = true;
                    MoveToPosition(D_PLAYERTARGET.transform.position);
                }
                //if ((D_PLAYERTARGET.transform.position - transform.position).sqrMagnitude < attackRangeSquared && m_countDown == 0f)
                //{
                //    m_countDown = ATTACK_RATE;
                //    D_PLAYERTARGET.TakeDamage(10f);
                //}
                break;
            case STATES.HOSTILE:
                if (health <= enragedHealthThreshold)
                    ChangeState(STATES.ENRAGED);
                if ((D_PLAYERTARGET.transform.position - transform.position).sqrMagnitude < attackRangeSquared && m_countDown == 0f)
                {
                    // Stop moving,
                    agent.updatePosition = false;
                    // Look at the player
                    Vector3 positionToLook = D_PLAYERTARGET.transform.position;
                    positionToLook.y = transform.position.y;
                    transform.LookAt(positionToLook, new Vector3(0f, 1f, 0f));
                    // Attack
                    m_countDown = ATTACK_RATE;
                    D_PLAYERTARGET.TakeDamage(5f);
                }
                else
                {
                    agent.updatePosition = true;
                    MoveToPosition(D_PLAYERTARGET.transform.position);
                }
                //if ((D_PLAYERTARGET.transform.position - transform.position).sqrMagnitude < attackRangeSquared && m_countDown == 0f)
                //{
                //    m_countDown = ATTACK_RATE;
                //    D_PLAYERTARGET.TakeDamage(5f);
                //}
                break;
            case STATES.DEAD:
                if (m_countDown <= 0f)
                {
                    var rb = GetComponent<Rigidbody>();
                    rb.detectCollisions = false;
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }
                break;
        }
    }

    bool Attack()
    {
        // Play the attack animation
        return false;
    }

    void ChangeState(STATES _newState)
    {
        currState = _newState;
            // Transition animation or effects here
        switch (currState)
        {
            case STATES.HOSTILE:
                ChangeSpeed(baseMoveSpeed, acceleration);
                break;
            case STATES.ENRAGED:
                ChangeSpeed(enragedMoveSpeed, acceleration);
                break;
            case STATES.DEAD:
                var rb = GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.detectCollisions = true;
                m_countDown = 5f;
                break;
        }
    }

    public override bool TakeDamage(float _damage)
    {
        Debug.Log("Took damage in AhMa.cs");
        if ((health -= _damage) <= 0f)
        {
            Die();
            return true;
        }
        return false;
    }

    public override void Die()
    {
        base.Die();
        m_countDown = 500f;
        ChangeState(STATES.DEAD);
    }
}
