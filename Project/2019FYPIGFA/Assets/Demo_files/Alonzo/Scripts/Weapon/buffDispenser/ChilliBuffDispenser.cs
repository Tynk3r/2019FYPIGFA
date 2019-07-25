using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChilliBuffDispenser : BuffMachineBase
{
    public Renderer rend;
    public override void Start()
    {
        base.Start();
        if (null == rend)
            Debug.LogError("Failed to get renderer");
    }
    public override bool DispenseBuff(out ItemData.WeaponBuff _buff)
    {
        // Visual stuff happens here
        if (uses > 0)
        {
            int newUses = uses;
            switch (--newUses)
            {
                case 2:
                    rend.material.color = new Color(1f, 1f, 0f);
                    break;
                case 1:
                    rend.material.color = new Color(0.901f, 0.494f, 0f);
                    break;
                case 0:
                    rend.material.color = new Color(1f, 0f, 0f);
                    break;
            }
        }
        return base.DispenseBuff(out _buff);
    }
}
