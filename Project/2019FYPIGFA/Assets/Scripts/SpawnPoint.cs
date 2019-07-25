using System;
using UnityEngine;
public class SpawnPoint : MonoBehaviour
{
    public enum POINT_TYPE
    {
        OBJECTIVE,
        WEAPON,
        HEALTH,
        EMPTY,
    }

    private POINT_TYPE pointType = POINT_TYPE.EMPTY;
    private string pointName = null;
    
    public void SetPointTo(POINT_TYPE p)
    {
        pointType = p;
        switch (pointType)
        {
            case POINT_TYPE.HEALTH:
                pointName = "HealthPickup";
                transform.localScale = new Vector3(1f,1f,1f);
                GetComponent<BoxCollider>().size = GetComponent<MeshRenderer>().bounds.size;
                gameObject.AddComponent<BounceAnimation>();
                GetComponent<BounceAnimation>().bounce = false;
                GetComponent<BounceAnimation>().spinRate = 1.5f;
                break;
        }
    }

    public void SetPointTo(Objective objective)
    {
        pointType = POINT_TYPE.OBJECTIVE;
        pointName = objective.GetItemType().ToString();
    }

    public void SetPointTo(ItemData itemData)
    {
        pointType = POINT_TYPE.WEAPON;
        pointName = itemData.type;
        gameObject.AddComponent<Interactable>().Initialize(itemData);
        Destroy(gameObject.GetComponent<BoxCollider>());
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
