using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_Egg : MonoBehaviour, I_Projectile
{
    public static float damage = 50f;
    public static float MAX_LIFETIME = 5.0f;
    public static float EXPLOSIVE_RANGE = 5f;
    public static float EXPLOSION_FORCE = 500f;
    public static float LINGER_TIME = 1f; // Only for aesthetics

    private float m_lifeTime;
    private Rigidbody m_rb;
    private bool m_playerOwned;
    // Update is called once per frame
    public void Initialize(bool _playerOwned = false)
    {
        m_lifeTime = MAX_LIFETIME;
        m_rb = GetComponent<Rigidbody>();
        m_playerOwned = _playerOwned;
    }
    void Update()
    {
        m_lifeTime -= Time.deltaTime;
        if (m_lifeTime <= 0f)
            Detonate();
    }
    public void Discharge(Vector3 _force, Vector3 _position)
    {
        m_rb.velocity = _force;
        transform.position = _position;
        m_lifeTime = MAX_LIFETIME;  // Set the lifetime
        gameObject.SetActive(true);
    }
    public void Detonate()
    {
        // Play egg particle effects and deals damage to all enemies in area
        // Get all game objects within radius of explosion and deal damage to them
        //LayerMask layerMask = 11 << 8;
        //int layerMaskEnemy = 1 << 9;
        //int layerMaskDestructible = 1 << 10;
        //int finalLayerMask = layerMaskEnemy | layerMaskDestructible;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, EXPLOSIVE_RANGE/*, finalLayerMask*/);
        int i = 0;
        while (i < hitColliders.Length)
        {
            // Check if it's an enemy. If it is, it takes damage
            Enemy enemyHit = null;
            Player playerHit = null;
            if (m_playerOwned)
                playerHit = hitColliders[i].GetComponent<Player>();
            else
                enemyHit = hitColliders[i].GetComponent<Enemy>();
            if (enemyHit != null)
            {
                // Apply force away from eggsplosion
                //enemyHit.GetComponent<Rigidbody>().AddForce((hitColliders[i].gameObject.transform.position - transform.position).normalized * EXPLOSION_FORCE);
                if (enemyHit.TakeDamage(damage))
                    enemyHit.GetComponent<Rigidbody>().AddExplosionForce(EXPLOSION_FORCE, transform.position, EXPLOSIVE_RANGE);
            }
            else if (playerHit != null)
            {
                playerHit.TakeDamage(damage);
            }
            ++i;
        }
        gameObject.SetActive(false);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Player")
            Detonate();
    }
}
