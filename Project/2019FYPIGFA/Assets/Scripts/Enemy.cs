using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum ENEMY_TYPE
    {
        CLOTHING_RACK_KIDS,
        AHMA,

    }

    public ENEMY_TYPE enemyType;
    public float health = 1;
    public float maxHealth;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public virtual void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);

        if (health <= 0)
        {
            health = 0;
            Die();
        }
    }
    
    public void Die()
    {
        // Def :(
    }

}
