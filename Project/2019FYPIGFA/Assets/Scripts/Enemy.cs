﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
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

    // Update is called once per frame
    public virtual void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
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
            if (m_buffList[i].duration != m_buffList[i].duration)
                Debug.LogError("Reference doesn't work here");
            // TODO: function to play sound on buff expunge?
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
        }
    }
    void BuffEnd(Buffable.CHAR_BUFF _buffType)
    {
        switch (_buffType)
        {
            case Buffable.CHAR_BUFF.DEBUFF_BURN:
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
                Debug.LogError("No buff found!");
                break;
        }
    }
}
