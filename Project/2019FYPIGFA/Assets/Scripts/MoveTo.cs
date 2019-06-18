// MoveTo.cs
using UnityEngine;
using System.Collections;

public class MoveTo : MonoBehaviour
{

    public Transform goal;
    private UnityEngine.AI.NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.destination = goal.position;
    }

    void Update()
    {
        if (agent.destination != goal.position)
            agent.destination = goal.position;
    }
}