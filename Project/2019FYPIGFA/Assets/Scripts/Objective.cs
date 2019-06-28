using UnityEngine;

public sealed class Objective
{
    public enum ITEM_TYPES
    {
        LAWNMOWER = 0,
        TOILETPAPER,
        POT,
        TOOTHBRUSH,
        SHAMPOO,
        TOTAL
    }
    ITEM_TYPES itemType = ITEM_TYPES.TOTAL;
    public Objective()
    {
        // Randomise objective item for shoppping list
        itemType = (ITEM_TYPES)Random.Range(0, (int)ITEM_TYPES.TOTAL);
    }
    public ITEM_TYPES GetItemType()
    {
        return itemType;
    }
}
