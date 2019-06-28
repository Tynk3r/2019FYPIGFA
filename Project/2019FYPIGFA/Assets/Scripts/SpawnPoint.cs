using System;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public enum POINT_TYPE
    {
        OBJECTIVE,
        EMPTY,
    }

    private POINT_TYPE pointType = POINT_TYPE.EMPTY;
    private string pointName = null;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetPointTo(Objective objective)
    {
        pointType = POINT_TYPE.OBJECTIVE;
        pointName = objective.GetItemType().ToString();
    }

    public POINT_TYPE GetPointType()
    {
        return pointType;
    }

    public string GetPointName()
    {
        return pointName;
    }

    public string OnPickedUp()
    {
        string temp = pointName;
        Destroy(gameObject);
        return temp;
    }
}
