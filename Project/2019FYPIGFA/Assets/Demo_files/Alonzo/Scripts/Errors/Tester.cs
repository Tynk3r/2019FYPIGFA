using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var list = new List<TestPoint>();
        list.Add(new TestPoint(9));
        list.Add(new TestPoint(8));
        list.Add(new TestPoint(7));
        list.Add(new TestPoint(6));
        list.Add(new TestPoint(5));
        list.Add(new TestPoint(4));
        list.Add(new TestPoint(3));
        list.Add(new TestPoint(2));
        list.Add(new TestPoint(1));
        list.Add(new TestPoint(0));
        list.Sort((x, y) => x.nextint.CompareTo(y.nextint));

        //Debug.Log("the sorted list goes by this order: ");
        //foreach(TestPoint i in list)
        //{
        //    Debug.Log("Tester: " + i.nextint);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class TestPoint
{
    public TestPoint(int i)
    {
        nextint = i;   
    }
    public bool proof = false;
    public TestPoint next = null;
    public int nextint;
}