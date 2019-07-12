using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftTrigger : MonoBehaviour
{
    public bool playerInLift;
    private void Start()
    {
        playerInLift = false;
    }
    private void OnTriggerExit(Collider other)
    {
        Player playerObject = other.gameObject.GetComponent<Player>();
        if (null != other.gameObject)
            playerInLift = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        Player playerObject = other.gameObject.GetComponent<Player>();
        if (null != other.gameObject)
            playerInLift = true;
    }
}
