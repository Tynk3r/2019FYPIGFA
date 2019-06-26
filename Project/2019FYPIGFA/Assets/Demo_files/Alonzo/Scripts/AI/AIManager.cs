using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIManager : Enemy
{
    NavMeshAgent agent;
    
    // Start is called before the first frame update
    public virtual void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
    }

    public virtual void Die()
    {
        agent.enabled = false;
    }

    public void MoveToTarget(Transform _targetTransform)
    {
        agent.SetDestination(_targetTransform.position);
    }

    public void ChangeSpeed(float _speed, float _acceleration = 8f)
    {
        agent.speed = _speed;
        agent.acceleration = _acceleration;
    }

    public void MoveToPosition(Vector3 _target)
    {
        agent.SetDestination(_target);
    }

    public void SimpleMove(Vector3 direction, float speed)
    {
        agent.Move(direction * speed * Time.deltaTime);
    }

}
