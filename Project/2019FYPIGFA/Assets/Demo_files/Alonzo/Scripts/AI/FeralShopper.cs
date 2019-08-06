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
    public float meleeRange = 4f;
    public float meleeStoppingDistance = 1;
    private bool m_attacking;
    private Rigidbody rb;
    private Animator anim;
    private float m_countDown;
    STATES currState;
    public static float RATE_OF_FIRE = 1f;
    private const float TURNING_SPEED = 100f;

    private float m_attackCooldown;
    ProjectilePool poolInstance;

    // For picking up weapons
    [SerializeField]
    private ItemData m_rangedWeapon;
    [SerializeField]
    private ItemData m_meleeWeapon;

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

        m_attacking = false;
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        m_meleeWeapon.weaponType = ItemData.WEAPON_TYPE.NONE;
        m_rangedWeapon.weaponType = ItemData.WEAPON_TYPE.NONE;
        target = FindObjectOfType<Player>().transform;
        if (null == heldWeapon)
            Debug.LogError("Missing the held weapon gameObject");
        //else if (null == heldWeapon.GetComponent<MeshFilter>() || null == heldWeapon.GetComponent<MeshRenderer>())
        //    Debug.LogError("Missing MeshFilter or MeshRenderer in the held weapon");
        //if (null == m_meleeWeapon || null == m_rangedWeapon)
        //    Debug.Log("Missing weapon");
        lootManager = (EnemyLootManager)FindObjectOfType(typeof(EnemyLootManager));
        if (null == lootManager)
            Debug.LogError("Missing loot manager");

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
                MoveToPosition(target.position);
                if ((transform.position - target.position).sqrMagnitude <= rangedAttackRange * rangedAttackRange)
                {
                    // TODO: raycast for covers
                    //int layerMask = 1 << 9; // For now it's just enemy
                    //RaycastHit hit;
                    if (Physics.Raycast(transform.position, (target.position -
                        transform.position), rangedAttackRange) && m_attackCooldown <= 0f)
                        ChangeState(STATES.RANGED_ATTACK);
                }
                break;
            case STATES.RANGED_ATTACK:
                {
                    Vector3 newView = target.position - transform.position;
                    newView.y = 0;
                    newView.Normalize();
                    Quaternion newRotation = new Quaternion();
                    newRotation.SetLookRotation(newView);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, Time.deltaTime * TURNING_SPEED);
                    if (ItemData.WEAPON_TYPE.NONE == m_rangedWeapon.weaponType)
                    {
                        ChangeState(STATES.SEARCH_WEAPON_RANGED);
                        break;
                    }
                    else if (m_attacking)
                    {
                        // Check if animation finished
                        var stateInfo = anim.GetCurrentAnimatorStateInfo(0);
                        // If it has, deal damage
                        if (stateInfo.IsName("attack"))
                            FinishAttack();
                        break;
                    }
                    else if ((transform.position - target.position).sqrMagnitude > rangedAttackRange * rangedAttackRange || m_attackCooldown > 0f)
                    {
                        ChangeState(STATES.HOSTILE_CLOSE_GAP);
                        break;
                    }
                    else if ((transform.position - target.position).sqrMagnitude < meleeStartDistThreshold * meleeStartDistThreshold)
                    {
                        ChangeState(STATES.MELEE_ATTACK);
                    }
                    //if (0 < m_attackCooldown)
                    //    break;
                    if (!BeginAttack()) // If can't attack because of obstacle
                        MoveToPosition(target.position);
                    else
                    {
                        MoveToPosition(transform.position);
                        m_attackCooldown += RATE_OF_FIRE;
                    }
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
                {
                    Vector3 newView = target.position - transform.position;
                    newView.y = 0;
                    newView.Normalize();
                    Quaternion newRotation = new Quaternion();
                    newRotation.SetLookRotation(newView);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, Time.deltaTime * TURNING_SPEED);
                    MoveToPosition(target.position);
                    if (ItemData.WEAPON_TYPE.NONE == m_meleeWeapon.weaponType)
                    {
                        ChangeState(STATES.SEARCH_WEAPON_MELEE);
                        break;
                    }
                    else if (m_attacking)
                    {
                        // Check if animation finished
                        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
                        // If it has, deal damage
                        if (stateInfo.IsName("attack"))
                            FinishAttack();
                        break;
                    }
                    else if ((target.position - transform.position).sqrMagnitude < meleeRange * meleeRange && m_attackCooldown <= 0f)
                    {
                        // Attack the player here
                    
                        if (BeginAttack())
                            m_attackCooldown += RATE_OF_FIRE;
                    }
                    else if ((target.position - transform.position).sqrMagnitude > meleeEndDistThreshold * meleeEndDistThreshold)
                    {
                        ChangeState(STATES.HOSTILE_CLOSE_GAP);
                    }
                }
                break;
            default:

                break;
        }
    }

    bool BeginAttack()
    {
        {
            Vector3 newView = target.position - transform.position;
            newView.y = 0;
            newView.Normalize();
            Quaternion newRotation = new Quaternion();
            newRotation.SetLookRotation(newView);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, Time.deltaTime * 10f);
        }
        if (STATES.RANGED_ATTACK == currState)
            {
            // RayCast to the player and if they are not in sight, move closer
            RaycastHit hit;
            Vector3 toPlayer = (target.position - heldWeapon.transform.position).normalized;
            if (Physics.Raycast(heldWeapon.transform.position, toPlayer, out hit, Mathf.Infinity))
            {
                Debug.DrawRay(heldWeapon.transform.position, toPlayer * hit.distance, Color.red);
                if (null == hit.collider.gameObject.GetComponent<Player>())
                    return false;
            }
            else
                return false;
            //egg.Discharge(transform.forward * 10f, transform.position + 
            //    (D_PLAYERTARGET.transform.position - transform.forward).normalized * 0.5f); JFL
            anim.SetTrigger("attack");
            m_attacking = true;
            return true;
        }
        else
        {
            // Raycast to the player but with limited distance
            RaycastHit hit;
            Vector3 toPlayer = target.position - heldWeapon.transform.position;
            if (Physics.Raycast(heldWeapon.transform.position, toPlayer, out hit, m_meleeWeapon.attackRange))
            {
                Debug.DrawRay(heldWeapon.transform.position, toPlayer * hit.distance, Color.red);
                if (null == hit.collider.gameObject.GetComponent<Player>())
                    return false;
            }
            else
                return false;
            // Attack codes here
            anim.SetTrigger("attack");
            m_attacking = true;
            return true;
        }
    }

    void FinishAttack()
    {
        m_attacking = false;
        Debug.Log("Finished attack");
        if (STATES.RANGED_ATTACK == currState)
        {
            // Spawn a projectile to be shot to the player
            Vector3 toPlayer = (target.position - heldWeapon.transform.position).normalized;
            I_Projectile projectile = poolInstance.FetchObjectInPool(m_rangedWeapon.projectileID).GetComponent<I_Projectile>();
            if (null == projectile)
            {
                Debug.LogWarning("Did not get any egg");
                return;
            }
            projectile.Initialize(false);
            projectile.Discharge(toPlayer * m_rangedWeapon.projectileMagnitude, transform.position +
                (target.position - transform.position).normalized * 1f);
        }
        else
        {
            // Deal damage to the player if the range is close enough
            // Raycast to the player but with limited distance
            RaycastHit hit;
            Vector3 toPlayer = target.position - heldWeapon.transform.position;
            if (Physics.Raycast(heldWeapon.transform.position, toPlayer, out hit, m_meleeWeapon.attackRange))
            {
                Debug.DrawRay(heldWeapon.transform.position, toPlayer * hit.distance, Color.red);
                if (null == hit.collider.gameObject.GetComponent<Player>())
                    return;
            }
            else
                return;
            // Attack codes here
            if (target.GetComponent<Player>() != null)
                target.GetComponent<Player>().TakeDamage(m_meleeWeapon.weaponDamage * 0.5f);
            else if (target.GetComponent<Enemy>() != null)
                target.GetComponent<Enemy>().TakeDamage(m_meleeWeapon.weaponDamage * 0.5f);
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
        lootManager.GetLootLocation(transform.position, out lootShelf);
        MoveToPosition(lootShelf.transform.position);
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("It's there");
        if (STATES.SEARCH_WEAPON_MELEE != currState && STATES.SEARCH_WEAPON_RANGED != currState)
            return;
        EnemyLoot loot = other.gameObject.GetComponent<EnemyLoot>();
        if (null == loot)
            return;
        if (STATES.SEARCH_WEAPON_RANGED == currState)
        {
            m_rangedWeapon = lootManager.GetWeapon(true);
        }
        else
        {
            m_meleeWeapon = lootManager.GetWeapon(false);
        }
        ChangeState(STATES.HOSTILE_CLOSE_GAP);
        anim.SetTrigger("attack");
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
        StartCoroutine(DeathAnimation());
    }
}
