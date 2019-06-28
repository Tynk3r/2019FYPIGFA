using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class FeralShopper : AIManager
{
    
    enum STATES
    {
        IDLE,
        HOSTILE,
        ENRAGED,
        DEAD
    }
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        enemyType = ENEMY_TYPE.FERAL_SHOPPER;
        //ChangeSpeed()
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
