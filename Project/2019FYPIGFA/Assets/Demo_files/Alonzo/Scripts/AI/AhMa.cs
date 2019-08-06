using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class AhMa : Enemy
{
    public float enragedHealthThreshold = 50f;
    public float baseMoveSpeed = 3.5f;
    public float enragedMoveSpeed = 3.5f;
    public static float ATTACK_RATE = 0.3f;
    public static float ATTACK_DAMAGE = 5f;
    private float m_countDown = 0f;
    public float acceleration = 60f;
    public float attackRangeSquared = 4f;
    
    private Rigidbody rb;
    enum STATES
    {
        IDLE,
        HOSTILE,
        ENRAGED,
        ATTACK,
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

        Rigidbody[] rigidBodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rigidbody in rigidBodies)
        {
            rigidbody.isKinematic = true;
            rigidbody.detectCollisions = false;
        }

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.detectCollisions = true;
        alive = true;
    }

    // Update is called once per frame
    override public void Update()
    {
        base.Update();
        m_countDown = Mathf.Max(m_countDown - Time.deltaTime, 0f);
        UpdateStates();
        if (health <= 0f && alive)
            Die();
        //Debug.Log("Detect collisions is " + rb.detectCollisions);
    }
    void UpdateStates()
    {
        switch (currState)
        {
            case STATES.ENRAGED:
                if ((target.position - transform.position).sqrMagnitude < attackRangeSquared && m_countDown == 0f)
                {
                    ChangeState(STATES.ATTACK);
                }
                else
                {
                    agent.updatePosition = true;
                    MoveToPosition(target.position);
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
                if ((target.position - transform.position).sqrMagnitude < attackRangeSquared && m_countDown == 0f)
                {
                    ChangeState(STATES.ATTACK);
                }
                else
                {
                    agent.updatePosition = true;
                    MoveToPosition(target.position);
                }
                //if ((D_PLAYERTARGET.transform.position - transform.position).sqrMagnitude < attackRangeSquared && m_countDown == 0f)
                //{
                //    m_countDown = ATTACK_RATE;
                //    D_PLAYERTARGET.TakeDamage(5f);
                //}
                break;
            case STATES.ATTACK:
                if ((target.position - transform.position).sqrMagnitude < attackRangeSquared)
                {
                    // Stop moving,
                    agent.updatePosition = false;
                    // Look at the player
                    Vector3 positionToLook = target.position;
                    positionToLook.y = transform.position.y;
                    transform.LookAt(positionToLook, new Vector3(0f, 1f, 0f));
                    // Attack
                    Attack();
                    m_countDown = ATTACK_RATE;
                }
                ChangeState(STATES.HOSTILE);
                break;
            case STATES.DEAD:
                //if (m_countDown <= 0f)
                //{
                //    StartCoroutine(DeathAnimation());
                //}
                break;
        }
    }

    bool Attack()
    {
        // TODO: check any other conditions like raycast?
        // Play the attack animation
        Animator anim = GetComponentInChildren<Animator>();
        anim.SetTrigger("attack");
        if (target.GetComponent<Player>() != null)
            target.GetComponent<Player>().TakeDamage(5f);
        else if (target.GetComponent<Enemy>() != null)
        {
            if (target.GetComponent<Enemy>().TakeDamage(1f))
                SwitchTarget();
        }
        return false;
    }

    void ChangeState(STATES _newState)
    {
        currState = _newState;
            // Transition animation or effects here
        switch (currState)
        {
            case STATES.HOSTILE:
                ChangeSpeed(baseMoveSpeed * GetSpeedMultiplier(), acceleration);
                break;
            case STATES.ENRAGED:
                ChangeSpeed(enragedMoveSpeed * GetSpeedMultiplier(), acceleration);
                break;
            case STATES.ATTACK:
                ChangeSpeed(0f);
                break;
            case STATES.DEAD:
                Animator anim = GetComponentInChildren<Animator>();
                anim.SetBool("run", false);
                anim.enabled = false;
                GetComponent<Collider>().enabled = false;
                Rigidbody[] rigidBodies = GetComponentsInChildren<Rigidbody>();
                foreach (Rigidbody rigidbody in rigidBodies)
                {
                    rigidbody.isKinematic = false;
                    rigidbody.detectCollisions = true;
                }
                //rb.isKinematic = true;
                //rb.detectCollisions = false;
                m_countDown = 5f;
                break;
        }
    }

    public override bool TakeDamage(float _damage)
    {
        return base.TakeDamage(_damage);
    }

    public override void Die()
    {
        base.Die();
        m_countDown = 5f;
        ChangeState(STATES.DEAD);
        StartCoroutine(DeathAnimation());
    }
    
}
