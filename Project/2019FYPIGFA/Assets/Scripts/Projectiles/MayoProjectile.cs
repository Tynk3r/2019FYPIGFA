using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MayoProjectile : MonoBehaviour, I_Projectile
{
    [SerializeField]
    float maxLifetime = 3f;
    [SerializeField]
    float detonateRange = 3f;
    [SerializeField]
    float detonateForce = 500f;
    [SerializeField]
    float damage = 10;
    private float m_countdown;
    [SerializeField]
    float duration = 2f;

    private int m_splashEffectID;
    [SerializeField]
    string splashEffectName = "Mayo2";

    private Rigidbody m_rb = null;
    private bool m_playerOwned;
    private void Awake()
    {
        m_rb = gameObject.GetComponent<Rigidbody>();
        m_splashEffectID = ProjectilePool.g_sharedInstance.GetPooledObjectIndex(splashEffectName);
    }

    // Update is called once per frame
    void Update()
    {
        m_countdown -= Time.deltaTime;
        if (m_countdown <= 0f)
            Detonate();
    }

    public void Initialize(bool _playerOwned = false)
    {
        m_countdown = maxLifetime;
        m_playerOwned = _playerOwned;
    }

    public void Discharge(Vector3 _force, Vector3 _position)
    {
        m_rb.velocity = _force;
        transform.position = _position;
        m_countdown = maxLifetime;
        gameObject.SetActive(true);
    }

    public void Detonate()
    {
        // Play deal area damage at point of destruction
        // Get all game objects within radius of explosion and deal damage to them
        //int layerMaskMinimap = 1 << 9;
        //int layerMaskProjectiles = 1 << 10;
        int layerMaskDefault = 1 << 0;
        int finalLayerMask = layerMaskDefault;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detonateRange);
        for (int i = 0; i < hitColliders.Length; ++i)
        {
            // Check if it's an enemy. If it is, it takes damage
            Enemy enemyHit = null;
            Player playerHit = null;
            if (m_playerOwned)
                playerHit = hitColliders[i].GetComponent<Player>();
            else
                enemyHit = hitColliders[i].GetComponent<Enemy>();
            if (null != enemyHit)
            {
                if(enemyHit.TakeDamage(damage))
                    enemyHit.GetComponent<Rigidbody>().AddExplosionForce(detonateForce, transform.position, detonateRange);
                enemyHit.ApplyBuff(Buffable.CHAR_BUFF.DEBUFF_SLOW, duration);
            }
            else if (null != playerHit)
            {
                playerHit.TakeDamage(damage);
            }
        }


        // Play the particle explosion
        // Get the splash effect
        GameObject splashEffect = ProjectilePool.g_sharedInstance.FetchObjectInPool(m_splashEffectID);
        splashEffect.SetActive(true);
        // Get the I_Projectile component
        I_Projectile splashProjectile = splashEffect.GetComponent<I_Projectile>();
        // Get the splash direction from contact point
        // Prevent animation from playing straight away
        splashProjectile.Initialize(true);
        // Play the animation at the right place
        splashProjectile.Discharge(new Vector3(), transform.position);
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Detonate();
    }
}
