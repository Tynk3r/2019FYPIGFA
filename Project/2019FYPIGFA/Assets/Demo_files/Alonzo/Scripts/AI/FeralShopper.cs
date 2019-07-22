using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class FeralShopper : Enemy
{
    public float attackRange = 10f;
    public float fleeHealthThreshold = 50f;
    public float fleeDistThreshold = 10f;
    public static float fleePointMaxDist = 2f;
    public Player D_PLAYERTARGET;
    private float m_countDown;
    STATES currState;
    public static float RATE_OF_FIRE = 1f;

    private float m_attackCooldown;
    ProjectilePool poolInstance;
    private static int projectileID;
    enum STATES
    {
        IDLE,
        HOSTILE_CLOSE_GAP,
        HOSTILE_ATTACK,
        KITING,
        DEAD
    }
    // Start is called before the first frame update
    override public void Start()
    {
        base.Start();
        enemyType = ENEMY_TYPE.FERAL_SHOPPER;
        currState = STATES.HOSTILE_CLOSE_GAP;
        m_countDown = 0f;
        m_attackCooldown = 0f;
        if (null == poolInstance)
            poolInstance = ProjectilePool.g_sharedInstance;

        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.detectCollisions = true;
    }

    // Update is called once per frame
    override public void Update()
    {
        if (0f < m_countDown)
            m_countDown -= Time.deltaTime;
        if (0f < m_attackCooldown)
            m_attackCooldown -= Time.deltaTime;
        else
            m_attackCooldown = 0f;

        UpdateStates();
        //if ((transform.position - D_PLAYERTARGET.transform.position).sqrMagnitude > fleeDistThreshold * fleeDistThreshold)
        //{
        //    MoveToPosition(D_PLAYERTARGET.transform.position);
        //    return;
        //}
        //Vector3 runPos;
        //Kite(out runPos);
        //MoveToPosition(runPos);
    }
    // CHECK: countdown needed to control state performance
    void UpdateStates()
    {
        switch (currState)
        {
            case STATES.HOSTILE_CLOSE_GAP:
                MoveToPosition(D_PLAYERTARGET.transform.position);
                if ((transform.position - D_PLAYERTARGET.transform.position).sqrMagnitude <= attackRange * attackRange)
                {
                    // TODO: raycast for covers
                    //int layerMask = 1 << 9; // For now it's just enemy
                    //RaycastHit hit;
                    if (Physics.Raycast(transform.position, (D_PLAYERTARGET.transform.position -
                        transform.position), attackRange))
                        ChangeState(STATES.HOSTILE_ATTACK);
                }
                break;
            case STATES.HOSTILE_ATTACK:
                if ((transform.position - D_PLAYERTARGET.transform.position).sqrMagnitude < fleeDistThreshold
                    && health < fleeHealthThreshold)
                    ChangeState(STATES.KITING);
                else if ((transform.position - D_PLAYERTARGET.transform.position).sqrMagnitude > attackRange * attackRange)
                    ChangeState(STATES.HOSTILE_CLOSE_GAP);
                if (0 < m_attackCooldown)
                    break;
                Attack();
                m_attackCooldown += RATE_OF_FIRE;
                break;
            case STATES.KITING:
                // TODO: fight if unable to run 
                // Runs away from player by a set distance
                Vector3 runPos;
                Kite(out runPos);
                MoveToPosition(runPos);
                break;
            default:

                break;
        }
    }

    void Attack()
    {
        // Spawn a projectile to be shot to the player
        B_Egg egg = poolInstance.FetchObjectInPool(projectileID).GetComponent<B_Egg>();
        if (null == egg)
        {
            Debug.LogWarning("Did not get any egg");
            return;
        }
        //egg.Discharge(transform.forward * 10f, transform.position + 
        //    (D_PLAYERTARGET.transform.position - transform.forward).normalized * 0.5f); JFL
        egg.Discharge(transform.forward * 30f, transform.position +
            (D_PLAYERTARGET.transform.position - transform.position).normalized * 1f);
    }

    void ChangeState(STATES _newState)
    {
        currState = _newState;
        // Transition animation or effects here
        switch (currState)
        {
            case STATES.HOSTILE_CLOSE_GAP:

                break;
            case STATES.HOSTILE_ATTACK:
                MoveToPosition(transform.position);
                break;
            case STATES.KITING:

                break;
            case STATES.DEAD:
                var rb = GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.detectCollisions = true;
                m_countDown = 5f;
                break;
        }
    }

    bool Kite(out Vector3 _result)
    {
        Vector3 directionToPlayer = transform.position - D_PLAYERTARGET.transform.position;
        Vector3 targetFleePos = transform.position + directionToPlayer;// Target flee pos
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetFleePos, out hit, fleePointMaxDist, NavMesh.AllAreas))
        {
            _result = hit.position;
            return true;
        }
        else
        {
            _result = transform.position;
            return false;
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
