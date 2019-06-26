using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class AhMa : AIManager
{
    public float enragedHealthThreshold = 50f;
    public float baseMoveSpeed = 3.5f;
    public float enragedMoveSpeed = 3.5f;
    private float m_countDown = 0f;
    public float acceleration = 60f;
    public Transform D_PLAYERTARGET;
    enum STATES
    {
        IDLE,
        HOSTILE,
        ENRAGED,
        DEAD
    }
    STATES currState = STATES.HOSTILE;
    // Start is called before the first frame update
    override public void Start()
    {
        base.Start();
        enemyType = ENEMY_TYPE.AHMA;
        ChangeSpeed(baseMoveSpeed, acceleration);
        ChangeState(STATES.ENRAGED); //TODO : REMOVE

        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.detectCollisions = true;
    }

    // Update is called once per frame
    override public void Update()
    {
        base.Update();
        UpdateStates();
        if (health <= 0f)
            Die();
    }
    void UpdateStates()
    {
        switch (currState)
        {
            case STATES.ENRAGED:
                MoveToPosition(D_PLAYERTARGET.position);
                break;
            case STATES.HOSTILE:
                if (health <= enragedHealthThreshold)
                    ChangeState(STATES.ENRAGED);
                MoveToPosition(D_PLAYERTARGET.position);
                break;
            case STATES.DEAD:
                if (m_countDown <= 0f)
                {
                    var rb = GetComponent<Rigidbody>();
                    rb.detectCollisions = false;
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }
                break;
        }
    }

    void ChangeState(STATES _newState)
    {
        currState = _newState;
            // Transition animation or effects here
        switch (currState)
        {
            case STATES.HOSTILE:
                ChangeSpeed(baseMoveSpeed, acceleration);
                break;
            case STATES.ENRAGED:
                ChangeSpeed(enragedMoveSpeed, acceleration);
                break;
            case STATES.DEAD:
                var rb = GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.detectCollisions = true;
                m_countDown = 5f;
                break;
        }
    }

    public override bool TakeDamage(float _damage)
    {
        Debug.Log("Took damage in AhMa.cs");
        Die();
        return (health -= _damage) <= 0f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name != "Plane")
            Debug.Log("Just collided with " + collision.gameObject);
    }

    public override void Die()
    {
        base.Die();
        m_countDown = 500f;
        ChangeState(STATES.DEAD);
    }
}
