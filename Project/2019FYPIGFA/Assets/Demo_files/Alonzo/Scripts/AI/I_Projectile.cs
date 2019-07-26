using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_Projectile
{
    /// <summary>
    /// Initializes the projectile. Intended to replace Start()
    /// This should be called to initialize the projectile when spawned from pooler
    /// </summary>
    void Initialize(bool _playerOwned);
    /// <summary>
    /// Discharges the projectile
    /// </summary>
    /// <param name="_force">Force to apply onto object</param>
    /// <param name="_position">The new position for the projectile to spawn from</param>
    void Discharge(Vector3 _force, Vector3 _position); // Shoot bullet
    /// <summary>
    /// Detonates the projectile. Intended to play any effect on impact or expiry
    /// </summary>
    void Detonate();    // For particle effects on detonation or damage
}
