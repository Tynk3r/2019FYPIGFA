using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffMachineBase : MonoBehaviour
{
    [SerializeField]
    public ItemData.WeaponBuff buff;
    public int uses = 0;
    public virtual void Start()
    {
    }
    /// <summary>
    /// Gives a buff to user, if there are uses remaining
    /// Uses go down by 1 whenever this is run
    /// </summary>
    /// <param name="_buff">output buff to be given to user</param>
    /// <returns> Returns true if there are uses remaining</returns>
    public virtual bool DispenseBuff(out ItemData.WeaponBuff _buff)
    {
        if (uses == 0)
        {
            _buff = null;
            return false;
        }
        _buff = new ItemData.WeaponBuff();
        --uses;
        _buff.buff = buff.buff;
        _buff.duration = buff.duration;
        _buff.magnitude = buff.magnitude;
        --buff.magnitude;
        if (_buff.magnitude == buff.magnitude)
            Debug.LogError("reference error");
        ++buff.magnitude;
        return true;
    }
}
