using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W_EggLauncher : MonoBehaviour
{
    ProjectilePool poolInstance;
    private static int projectileID;
    private void Start()
    {
        poolInstance = ProjectilePool.g_sharedInstance;
        projectileID = poolInstance.GetPooledObjectIndex("Egg");
    }
    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
            Debug.Log("Firing");
        }
    }
    void Attack()
    {
        // Uses the object pooler to spawn the bullet
        B_Egg egg = poolInstance.FetchObjectInPool(projectileID).GetComponent<B_Egg>();
        if (null == egg)
        {
            Debug.LogWarning("Did not get any egg");
            return;
        }
        egg.Discharge(transform.forward * 10f, transform.position + transform.forward * 0.5f);
    }
}
