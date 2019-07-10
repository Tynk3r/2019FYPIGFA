using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffMachineBase : MonoBehaviour
{
    public ItemData.WeaponBuff buff;
    public int uses = 0;
    public virtual bool DispenseBuff(out ItemData.WeaponBuff _buff)
    {
        if (uses == 0)
        {
            _buff = null;
            return false;
        }
        --uses;
        _buff = buff;
        --buff.magnitude;
        if (_buff == buff)
            Debug.LogError("reference error");
        ++buff.magnitude;
        return true;
    }
}
