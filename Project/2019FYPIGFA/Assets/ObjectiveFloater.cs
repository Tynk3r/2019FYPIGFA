using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveFloater : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.Translate((Mathf.PingPong(Time.time, 1f) - 0.5f) * 0.01f, 0f, 0f);
        transform.Rotate(0f, 1f, 0f, Space.World);
    }
}
