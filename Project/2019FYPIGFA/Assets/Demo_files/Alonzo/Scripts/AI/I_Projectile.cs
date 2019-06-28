using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_Projectile
{
    void Initialize();
    void Discharge(Vector3 _force, Vector3 _position); // Shoot bullet
    void Detonate();    // For particle effects on detonation or damage
}
