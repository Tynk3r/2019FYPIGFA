using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int testInt = 5;
        TestPoint testPoint = new TestPoint();
        TestPoint mainPoint = testPoint;
        Debug.Log("Original main point proof " + mainPoint.proof);
        testPoint.proof = true;
        Debug.Log("Now it's " + mainPoint.proof);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class TestPoint
{
    public bool proof = false;
    public TestPoint next = null;
    public int nextint;
}