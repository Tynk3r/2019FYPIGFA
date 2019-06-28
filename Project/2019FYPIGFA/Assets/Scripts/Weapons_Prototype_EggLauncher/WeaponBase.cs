using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : I_Weapon
{
    public static float ATTACK_RATE;


    public abstract void Attack();
    public abstract void Drop();
}
