using System.Collections;
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

}
