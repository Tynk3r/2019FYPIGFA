using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mayo2Projectile : MonoBehaviour, I_Projectile
{
    [SerializeField]
    float maxLifeTime = 10f;
    private ParticleSystem m_particleSystem;
    private float m_countdown;
    // Start is called before the first frame update
    void Awake()
    {
        m_particleSystem = gameObject.GetComponent<ParticleSystem>();
        m_countdown = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        m_countdown -= Time.deltaTime;
        if (m_countdown <= 0f)
            Detonate();
    }

    public void Initialize()
    {
        m_particleSystem.Stop();
        m_countdown = maxLifeTime;
    }

    public void Discharge(Vector3 _lookAtDir, Vector3 _position)
    {
        // TODO: LOOKAT DIRECTION
        // set the location of the particle system
        gameObject.SetActive(true);
        transform.position = _position;
        m_countdown = maxLifeTime;
        m_particleSystem.Play(); // Play the particleSystem
    }

    public void Detonate()
    {
        gameObject.SetActive(false);
    }
}
