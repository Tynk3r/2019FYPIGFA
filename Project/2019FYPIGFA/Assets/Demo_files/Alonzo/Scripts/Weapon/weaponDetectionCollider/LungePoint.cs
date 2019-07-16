using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LungePoint : MonoBehaviour, I_Projectile
{
    public float damage;
    public static float lifetime = 3f;
    private float m_countdown;
    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log("yes");
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
        // Not needed
        gameObject.SetActive(true);
        m_countdown = lifetime;
    }
    public void Discharge(Vector3 _Scale, Vector3 _position)
    {
        // Make the collider have the appropriate dimensions
        gameObject.transform.localScale = _Scale;
        gameObject.transform.position = _position;
    }
    public void Detonate()
    {
        gameObject.SetActive(false);
        // TODO: Effects?
    }


    //private void Trig(Collision collision)
    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        if (null == enemy)
        {
            return;
        }
        Debug.Log("Dealt the damage");
        if (enemy.TakeDamage(damage))
        {
            // Do something here if enemy dies?
        }
        Detonate(); // Made to only damage one target
    }
}
