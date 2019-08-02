using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceAnimation : MonoBehaviour
{
    [Header("Bounce")]
    public bool bounce = true;
    private Vector3 startPosition = Vector3.zero;

    [Header("Spin")]
    public bool spin = true;
    [DrawIf("spin", true)]
    public float spinRate = 1f;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (bounce)
            transform.localPosition = new Vector3(
                transform.localPosition.x,
                startPosition.y + (Mathf.PingPong(Time.time, 1f) + 1) * transform.GetComponent<MeshRenderer>().bounds.size.y * 0.5f,
                transform.localPosition.z);
        if (spin)
            transform.Rotate(0f, spinRate, 0f, Space.World);
    }
}
