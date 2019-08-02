using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_PizzaSlice : MonoBehaviour, I_Projectile
{
    public float damage = 50f;
    public float MAX_LIFETIME = 5f;
    public float EXPLOSIVE_RANGE = 5f;
    public float EXPLOSION_FORCE = 500f;
    public float rotationSpeed = 1f; // Only for aesthetics
    public const float lingerTime = 3f;
    private bool m_playerOwned;
    private float m_lifeTime;
    private Rigidbody m_rb;

    private bool m_live; // To make sure trail finishes before unrendering

    // Update is called once per frame
    public void Initialize(bool _playerOwned = false)
    {
        m_lifeTime = MAX_LIFETIME;
        m_rb = GetComponent<Rigidbody>();
        m_playerOwned = _playerOwned;
        m_live = true;
    }
    void Update()
    {
        transform.Rotate(0f, rotationSpeed, 0f, Space.Self);
        m_lifeTime -= Time.deltaTime;
        if(m_live)
        {
            if (m_lifeTime <= 0f)
                Detonate();
        }
        else
        {
            if (m_lifeTime <= 0f)
            {
                gameObject.SetActive(false);
            }
        }
    }
    public void Discharge(Vector3 _force, Vector3 _position)
    {
        m_rb.velocity = _force;
        transform.position = _position;
        transform.rotation = Quaternion.LookRotation(_force.normalized);
        m_lifeTime = MAX_LIFETIME;  // Set the lifetime
        gameObject.SetActive(true);
    }
    public void Detonate(GameObject g)
    {
        // Check if it's an enemy. If it is, it takes damage
        Debug.Log("Collided with an enemy");
        Enemy enemyHit = null;
        Player playerHit = null;
        if (!m_playerOwned)
            playerHit = g.GetComponent<Player>();
        else
            enemyHit = g.GetComponent<Enemy>();
        if (enemyHit != null)
        {
            // Apply force away from eggsplosion sorry i mean Pizzasplosio
            if (enemyHit.TakeDamage(damage))
            {
                Rigidbody[] rigidbodies = enemyHit.GetComponentsInChildren<Rigidbody>();
                foreach (Rigidbody rigidbody in rigidbodies)
                {
                    rigidbody.AddExplosionForce(EXPLOSION_FORCE, transform.position, EXPLOSIVE_RANGE);
                }
            }
        }
        else if (playerHit != null)
            playerHit.TakeDamage(damage);
        Detonate();
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (m_live)
            Detonate(collision.gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger detected pizza");
        if (other.gameObject.tag != "Player")
            Detonate(other.gameObject);
    }

    public void Detonate()
    {
        m_live = false;
        m_lifeTime = lingerTime;
    }
}
