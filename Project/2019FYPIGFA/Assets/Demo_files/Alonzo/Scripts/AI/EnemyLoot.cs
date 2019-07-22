using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLoot : MonoBehaviour
{
    public BoxCollider collider;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<BoxCollider>();
    }
    public Vector3 GetClosestPoint(Vector3 _position)
    {
        return collider.ClosestPoint(_position);
    }
}
