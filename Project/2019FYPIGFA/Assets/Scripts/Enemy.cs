using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameController.AGGRESSION_LEVELS;

public class Enemy : AIManager
{
    public GameController gameController;
    public Player player;
    const float BURN_DAMAGE_MULT = 0.03f;
    public enum ENEMY_TYPE
    {
        CLOTHING_RACK_KIDS,
        AHMA,
        FERAL_SHOPPER
    }
    [SerializeField]
    private EnemyBuffCanvas buffCanvas;
    private List<Buffable.Buff> m_buffList;
    public ENEMY_TYPE enemyType;
    public float health = 1;
    public float maxHealth;
    private float m_speedMultiplier = 1f;
    [HideInInspector]
    public bool alive = true;

    public virtual void Start()
    {
        base.Start();
        gameController = FindObjectOfType<GameController>();
        player = FindObjectOfType<Player>();
        m_buffList = new List<Buffable.Buff>();

        if (target == null
            || (target.GetComponent<Enemy>() != null && !target.GetComponent<Enemy>().alive))
            SwitchTarget();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (target == null
            || (target.GetComponent<Enemy>() != null && !target.GetComponent<Enemy>().alive))
            SwitchTarget();
        UpdateBuffs();
    }

    public virtual bool TakeDamage(float _damage)
    {
        float trueDamage = Mathf.Clamp(_damage, 0, health);
        health -= trueDamage;
        if (health <= 0f)
        {
            Die();
            return true;
        }
        return false;
    }

    void UpdateBuffs()
    {
        for (int i = 0; i < m_buffList.Count; ++i)
        {
            if ((m_buffList[i].duration - Time.deltaTime) < m_buffList[i].nextTickVal)
            {
                BuffTick(m_buffList[i]);
                m_buffList[i].nextTickVal -= 0.5f;
            }
            m_buffList[i].duration -= Time.deltaTime;
            if (m_buffList[i].duration <= 0f)
            {
                BuffEnd(m_buffList[i].buff);
                m_buffList.Remove(m_buffList[i]);
            }
        }
    }
    public void ApplyBuff(Buffable.CHAR_BUFF _buffType, float _duration = 0f)
    {
        Debug.Log("Applied a buff of type " + _buffType);
        // First find out if the buff has already been applied
        for (int i = 0; i < m_buffList.Count; ++i)
        {
            if (m_buffList[i].buff == _buffType)
            {
                m_buffList[i].duration = _duration;  // Reset duration and return
                return;
            }
        }
        // Create and add new buff instead
        Buffable.Buff newBuff = new Buffable.Buff();
        newBuff.duration = _duration;
        newBuff.buff = _buffType;
        newBuff.nextTickVal = newBuff.duration - 0.5f;
        m_buffList.Add(newBuff);
        // Initiate a starting effect for the new buff
        switch (_buffType)
        {
            case Buffable.CHAR_BUFF.DEBUFF_BURN:
                break;
            case Buffable.CHAR_BUFF.DEBUFF_SLOW:
                ChangeSpeedMultiplier(0.5f);
                break;
        }
        buffCanvas.AddBuff(_buffType);
    }
    void BuffEnd(Buffable.CHAR_BUFF _buffType)
    {
        switch (_buffType)
        {
            case Buffable.CHAR_BUFF.DEBUFF_BURN:
                break;
            case Buffable.CHAR_BUFF.DEBUFF_SLOW:
                ChangeSpeedMultiplier(1f);
                break;
        }
        buffCanvas.RemoveBuff(_buffType);
    }
    void BuffTick(Buffable.Buff _buff)
    {
        Debug.Log("Ticking buff");
        switch (_buff.buff)
        {
            case Buffable.CHAR_BUFF.DEBUFF_BURN:
                TakeDamage(maxHealth * BURN_DAMAGE_MULT);
                break;
            default:
                //Debug.LogError("No buff found!");
                break;
        }
    }

    public IEnumerator DeathAnimation()
    {
        yield return new WaitForSecondsRealtime(5f);
        if (GetComponent<Rigidbody>() != null)
            GetComponent<Rigidbody>().isKinematic = true;
        while (transform.position.y > -5f)
        {
            transform.Translate(new Vector3(0f, -0.5f * Time.deltaTime, 0f), Space.World);
            yield return null;
        }
        Destroy(this.gameObject);
    }

    public IEnumerator CheckAggressionLevel()
    {
        while (true)
        {
            int rand = Random.Range(1, 11);
            if (rand == 1)
                SwitchTarget();
            yield return new WaitForSecondsRealtime(1f);
        }
    }

    public void SwitchTarget()
    {
        if (gameController.enemyList.Count <= 1)
            target = player.transform;
        else
        {
            int rand1 = Random.Range(0, gameController.enemyList.Count);
            int rand2 = Random.Range(1, 11);
            while (gameController.enemyList[rand1] == this)
                rand1 = Random.Range(0, gameController.enemyList.Count);
            switch (gameController.aggressionLevel)
            {
                case DOCILE:
                    target = gameController.enemyList[rand1].transform;
                    break;
                case ANGRY:
                    switch (rand2)
                    {
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                            target = gameController.enemyList[rand1].transform;
                            break;
                        default:
                            target = player.transform;
                            break;
                    }
                    break;
                case ENRAGED:
                    switch (rand2)
                    {
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                            target = player.transform;
                            break;
                        default:
                            target = gameController.enemyList[rand1].transform;
                            break;
                    }
                    break;
                case INSANE:
                    target = player.transform;
                    break;
                default:
                    target = player.transform;
                    break;
            }
            if (target.GetComponent<Enemy>() != null)
                target.GetComponent<Enemy>().target = this.transform;
            Debug.Log(target);
        }
    }

    public virtual void ChangeSpeedMultiplier(float _newMult)
    {
        ChangeSpeed(agent.speed / m_speedMultiplier * _newMult);
        m_speedMultiplier = _newMult;
    }

    public float GetSpeedMultiplier()
    {
        return m_speedMultiplier;
    }

    public override void Die()
    {
        base.Die();
        gameController.enemyList.Remove(this);
        alive = false;
    }
}
