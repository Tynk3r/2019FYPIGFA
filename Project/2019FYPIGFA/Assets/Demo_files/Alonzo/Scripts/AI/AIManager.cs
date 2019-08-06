using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIManager : MonoBehaviour
{
    [HideInInspector]
    public NavMeshAgent agent;
    public Transform target = null;
    
    // Start is called before the first frame update
    public virtual void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
    }
    /// <summary>
    /// Disables the navmesh agent when the enemy dies, preventing any lively actions
    /// </summary>
    public virtual void Die()
    {
        agent.enabled = false;
    }
    /// <summary>
    /// Moves towards specified transform.
    /// Agent will always move towards transform if moved away from it, even if it reached it before
    /// </summary>
    /// <param name="_targetTransform">Transform to move to</param>
    public void MoveToTarget(Transform _targetTransform)
    {
        agent.SetDestination(_targetTransform.position);
        target = _targetTransform;
    }
    /// <summary>
    /// Changes the speed of the navmesh agent
    /// </summary>
    /// <param name="_speed">The new speed of the agent</param>
    /// <param name="_acceleration">The new acceleration of the navmesh agent</param>
    public void ChangeSpeed(float _speed, float _acceleration = -1)
    {
        agent.speed = _speed;
        if (-1 != _acceleration)
            agent.acceleration = _acceleration;
    }
    /// <summary>
    /// Alternate move function for the navmesh agent
    /// Moves towards specified target. Only a Vector3 position is required instead of transform.
    /// Agent will always move towards target if moved away from it, even if it reached it before
    /// </summary>
    /// <param name="_target">Vector3 target to move to</param>
    public void MoveToPosition(Vector3 _target)
    {
        agent.SetDestination(_target);
    }
    /// <summary>
    /// Moves manually a specified direction.
    /// Speed is required to allow smooth motion based on time
    /// </summary>
    /// <param name="direction">Direction to move</param>
    /// <param name="speed">speed to move</param>
    public void SimpleMove(Vector3 direction, float speed)
    {
        agent.Move(direction * speed * Time.deltaTime);
    }
    /// <summary>
    /// Checks if agent is at any edge of the navmesh
    /// </summary>
    /// <returns>Returns true if it is at an edge of the navmesh</returns>
    public bool IsAtEdge()
    {
        agent.FindClosestEdge(out NavMeshHit hit);
        if ((hit.position - agent.transform.position).magnitude < 0.1f)
            return true;
        return false;
    }

}
