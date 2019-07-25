using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Buffable
{
    [SerializeField]
    public enum CHAR_BUFF
    {
        NONE,
        BUFF_SLOMO,
        DEBUFF, // For checking
        DEBUFF_BURN,
        DEBUFF_SLOW,
    }
    [System.Serializable]
    public class Buff
    {
        public CHAR_BUFF buff = CHAR_BUFF.DEBUFF_BURN;
        public float tickValue = 0f;
        public float duration = 0f;
        public float nextTickVal = 0f;
    }
}
