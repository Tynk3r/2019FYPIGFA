using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class FeralShopper : Enemy
{
    EnemyLootManager lootManager;
    public float rangedAttackRange = 10f;
    public float meleeStartDistThreshold = 5f;
    public float meleeEndDistThreshold = 20f;
    public float meleeMoveSpeed = 5f;
    public float normalMoveSpeed = 3.5f;
    public Player D_PLAYERTARGET;
    private float m_countDown;
    STATES currState;
    public static float RATE_OF_FIRE = 1f;
    
    private float m_attackCooldown;
    ProjectilePool poolInstance;

    // For picking up weapons
    [SerializeField]
    private ItemData m_rangedWeapon = null;
    [SerializeField]
    private ItemData m_meleeWeapon = null;

    public GameObject heldWeapon;

    private GameObject m_itemGoal; // The weapon the AI is trying to get
    const int MAX_INVENTORY_SPACE = 2;
    
    enum STATES
    {
        IDLE,
        HOSTILE_CLOSE_GAP,
        RANGED_ATTACK,
        KITING,
        SEARCH_WEAPON_RANGED,
        SEARCH_WEAPON_MELEE,
        MELEE_ATTACK,
        DEAD
    }
    // Start is called before the first frame update
    override public void Start()
    {
        base.Start();
        enemyType = ENEMY_TYPE.FERAL_SHOPPER;
        currState = STATES.HOSTILE_CLOSE_GAP;
        ChangeSpeed(normalMoveSpeed);
        m_countDown = 0f;
        m_attackCooldown = 0f;
        if (null == poolInstance)
            poolInstance = ProjectilePool.g_sharedInstance;

        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.detectCollisions = true;
        m_meleeWeapon = m_rangedWeapon = null;
        if (null == heldWeapon)
            Debug.LogError("Missing the held weapon gameObject");
        //else if (null == heldWeapon.GetComponent<MeshFilter>() || null == heldWeapon.GetComponent<MeshRenderer>())
        //    Debug.LogError("Missing MeshFilter or MeshRenderer in the held weapon");
        //if (null == m_meleeWeapon || null == m_rangedWeapon)
        //    Debug.Log("Missing weapon");
        lootManager = (EnemyLootManager)FindObjectOfType(typeof(EnemyLootManager));
        if (null == lootManager)
            Debug.LogError("Missing loot manager");
    }

    // Update is called once per frame
    override public void Update()
    {
        Debug.Log(currState);
        if (0f < m_countDown)
            m_countDown -= Time.deltaTime;
        if (0f < m_attackCooldown)
            m_attackCooldown -= Time.deltaTime;
        else
            m_attackCooldown = 0f;

        UpdateStates();
    }
    // CHECK: countdown needed to control state performance
    void UpdateStates()
    {
        switch (currState)
        {
            case STATES.HOSTILE_CLOSE_GAP:
                MoveToPosition(D_PLAYERTARGET.transform.position);
                if ((transform.position - D_PLAYERTARGET.transform.position).sqrMagnitude <= rangedAttackRange * rangedAttackRange)
                {
                    // TODO: raycast for covers
                    //int layerMask = 1 << 9; // For now it's just enemy
                    //RaycastHit hit;
                    if (Physics.Raycast(transform.position, (D_PLAYERTARGET.transform.position -
                        transform.position), rangedAttackRange) && m_attackCooldown <= 0f)
                        ChangeState(STATES.RANGED_ATTACK);
                }
                break;
            case STATES.RANGED_ATTACK:
                if (null == m_rangedWeapon)
                {
                    
                }
                else if ((transform.position - D_PLAYERTARGET.transform.position).sqrMagnitude > rangedAttackRange * rangedAttackRange || m_attackCooldown > 0f)
                {
                    ChangeState(STATES.HOSTILE_CLOSE_GAP);
                    break;
                }
                else if ((transform.position - D_PLAYERTARGET.transform.position).sqrMagnitude < meleeStartDistThreshold * meleeStartDistThreshold)
                {
                    ChangeState(STATES.MELEE_ATTACK);
                }
                //if (0 < m_attackCooldown)
                //    break;
                if (!Attack()) // If can't attack because of obstacle
                    MoveToPosition(D_PLAYERTARGET.transform.position);
                else
                {
                    MoveToPosition(transform.position);
                    m_attackCooldown += RATE_OF_FIRE;
                }
                
                break;
            case STATES.SEARCH_WEAPON_RANGED:
                if (agent.remainingDistance > 0f)
                    break;
                m_rangedWeapon = lootManager.GetWeapon(true);
                ChangeState(STATES.RANGED_ATTACK);
                break;
            case STATES.SEARCH_WEAPON_MELEE:
                if (agent.remainingDistance > 0f)
                    break;
                m_meleeWeapon = lootManager.GetWeapon(false);
                ChangeState(STATES.MELEE_ATTACK);
                break;
            case STATES.MELEE_ATTACK:
                MoveToPosition(D_PLAYERTARGET.transform.position);
                if (null == m_meleeWeapon)
                {

                    break;
                }
                else if ((D_PLAYERTARGET.transform.position - transform.position).sqrMagnitude < rangedAttackRange && m_countDown <= 0f)
                {
                    // Attack the player here
                    m_countDown += m_attackCooldown;
                }
                else if ((D_PLAYERTARGET.transform.position - transform.position).sqrMagnitude > meleeEndDistThreshold * meleeEndDistThreshold)
                {
                    ChangeState(STATES.HOSTILE_CLOSE_GAP);
                }
                break;
            case STATES.DEAD:

                break;
        }
    }

    bool Attack()
    {
        {
            //Vector3 newPos = D_PLAYERTARGET.transform.position;
            transform.LookAt(D_PLAYERTARGET.transform.position);
        }
        if (STATES.RANGED_ATTACK == currState)
            {
            // RayCast to the player and if they are not in sight, move closer
            RaycastHit hit;
            Vector3 toPlayer = (D_PLAYERTARGET.transform.position - heldWeapon.transform.position).normalized;
            if (Physics.Raycast(heldWeapon.transform.position, toPlayer, out hit, Mathf.Infinity))
            {
                Debug.DrawRay(heldWeapon.transform.position, toPlayer * hit.distance, Color.red);
                if (null == hit.collider.gameObject.GetComponent<Player>())
                    return false;
            }
            else
                return false;
            // Spawn a projectile to be shot to the player
            I_Projectile projectile = poolInstance.FetchObjectInPool(m_rangedWeapon.projectileID).GetComponent<I_Projectile>();
            if (null == projectile)
            {
                Debug.LogWarning("Did not get any egg");
                return false;
            }
            //egg.Discharge(transform.forward * 10f, transform.position + 
            //    (D_PLAYERTARGET.transform.position - transform.forward).normalized * 0.5f); JFL
            projectile.Discharge(toPlayer * m_rangedWeapon.projectileMagnitude, transform.position +
                (D_PLAYERTARGET.transform.position - transform.position).normalized * 1f);
            return true;
        }
        else
        {
            // Raycast to the player but with limited distance
            RaycastHit hit;
            Vector3 toPlayer = D_PLAYERTARGET.transform.position - heldWeapon.transform.position;
            if (Physics.Raycast(heldWeapon.transform.position, toPlayer, out hit, m_meleeWeapon.attackRange))
            {
                Debug.DrawRay(heldWeapon.transform.position, toPlayer * hit.distance, Color.red);
                if (null == hit.collider.gameObject.GetComponent<Player>())
                    return false;
            }
            else
                return false;
            // Attack codes here

            return true;
        }
    }

    void ChangeState(STATES _newState)
    {
        currState = _newState;
        agent.stoppingDistance = 0f;
        // Transition animation or effects here
        switch (currState)
        {
            case STATES.HOSTILE_CLOSE_GAP:
                ChangeSpeed(normalMoveSpeed);
                break;
            case STATES.RANGED_ATTACK:
                MoveToPosition(transform.position);
                if (!ChangeWeapon(ItemData.WEAPON_TYPE.PROJECTILE))
                    ChangeState(STATES.SEARCH_WEAPON_RANGED);
                break;
            case STATES.MELEE_ATTACK:
                agent.stoppingDistance = 2f;
                ChangeSpeed(meleeMoveSpeed);
                m_countDown = 0f;
                if (!ChangeWeapon(ItemData.WEAPON_TYPE.RAYCAST))
                    ChangeState(STATES.SEARCH_WEAPON_MELEE);
                break;
            case STATES.DEAD:
                var rb = GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.detectCollisions = true;
                m_countDown = 5f;
                break;
            case STATES.SEARCH_WEAPON_RANGED:
            case STATES.SEARCH_WEAPON_MELEE:
                ChangeSpeed(meleeMoveSpeed);
                m_countDown = 0f;
                SearchWeapon();
                break;
        }
    }

    bool ChangeWeapon(ItemData.WEAPON_TYPE _type)
    {
        if (ItemData.WEAPON_TYPE.PROJECTILE == _type)
        {
            if (null != m_rangedWeapon)
            {
                EquipWeapon(_type);
                return true;
            }
        }
        else
        {
            if (null != m_meleeWeapon)
            {
                EquipWeapon(_type);
                return true;
            }
        }
        return false;
    }

    void EquipWeapon(ItemData.WEAPON_TYPE _type)
    {
        if (ItemData.WEAPON_TYPE.PROJECTILE == _type)
        {
            heldWeapon.GetComponent<MeshFilter>().mesh = m_rangedWeapon.mesh;
            heldWeapon.GetComponent<MeshRenderer>().material = m_rangedWeapon.material;
            heldWeapon.transform.localPosition = m_rangedWeapon.heldPosition;
            heldWeapon.transform.localRotation = Quaternion.Euler(m_rangedWeapon.heldRotation);
        }
        else
        {
            heldWeapon.GetComponent<MeshFilter>().mesh = m_meleeWeapon.mesh;
            heldWeapon.GetComponent<MeshRenderer>().material = m_meleeWeapon.material;
            heldWeapon.transform.localPosition = m_meleeWeapon.heldPosition;
            heldWeapon.transform.localRotation = Quaternion.Euler(m_meleeWeapon.heldRotation);
        }
    }

    void SearchWeapon()
    {
        EnemyLoot lootShelf;
        Vector3 goalPos = lootManager.GetLootLocation(transform.position, out lootShelf);
        MoveToPosition(goalPos);
        Debug.Log("Searching for weapon at " + lootShelf);
    }

    public override bool TakeDamage(float _damage)
    {
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
