using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : AIManager
{
    public enum ENEMY_TYPE
    {
        CLOTHING_RACK_KIDS,
        AHMA,
        FERAL_SHOPPER
    }

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
        m_buffList = new List<Buffable.Buff>();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateBuffs();
    }

    public virtual bool TakeDamage(float _damage)
    {
        float trueDamage = Mathf.Clamp(_damage, 0, health);
        //Debug.Log(enemyType + " took " + trueDamage + " damage.");
        return (health -= trueDamage) <= 0f;
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
    }
    void BuffTick(Buffable.Buff _buff)
    {
        switch (_buff.buff)
        {
            case Buffable.CHAR_BUFF.DEBUFF_BURN:
                TakeDamage(_buff.tickValue);
                break;
            default:
                //Debug.LogError("No buff found!");
                break;
        }
    }

    public IEnumerator DeathAnimation()
    {
        yield return new WaitForSecondsRealtime(2f);
        while(GetComponent<Rigidbody>().velocity.magnitude > 0.001f)
        {
            yield return null;
        }
        if (GetComponent<Rigidbody>() != null)
            GetComponent<Rigidbody>().isKinematic = true;
        while (transform.position.y > -transform.localScale.y)
        {
            transform.Translate(new Vector3(0f, -0.5f * Time.deltaTime, 0f), Space.World);
            yield return null;
        }
        Destroy(this.gameObject);
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
        alive = false;
    }
}
