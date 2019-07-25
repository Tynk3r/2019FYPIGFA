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

    private float m_lifeTime;
    private Rigidbody m_rb;

    // Update is called once per frame
    public void Initialize()
    {
        m_lifeTime = MAX_LIFETIME;
        m_rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        transform.Rotate(0f, rotationSpeed, 0f, Space.Self);
        m_lifeTime -= Time.deltaTime;
        if (m_lifeTime <= 0f)
            Detonate();
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
        Enemy enemyHit = g.GetComponent<Enemy>();
        if (enemyHit != null && enemyHit.TakeDamage(damage))
        {
            // Apply force away from eggsplosion sorry i mean Pizzasplosion
            enemyHit.GetComponent<Rigidbody>().AddExplosionForce(EXPLOSION_FORCE, transform.position, EXPLOSIVE_RANGE);
        }
        gameObject.SetActive(false);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Player")
            Detonate(collision.gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player")
            Detonate(other.gameObject);
    }

    public void Detonate()
    {
        gameObject.SetActive(false);
    }
}
