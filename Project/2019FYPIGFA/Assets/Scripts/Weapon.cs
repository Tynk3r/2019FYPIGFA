using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : ItemData
{
    public enum TYPE
    {
        CLOSE_RANGE,
        PROJECTILE,
        RANGED,
        TOTAL,
    }

    protected TYPE weaponType;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
