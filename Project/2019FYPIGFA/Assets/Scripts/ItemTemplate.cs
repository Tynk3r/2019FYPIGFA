using UnityEngine;

[System.Serializable]
public sealed class ItemTemplate : MonoBehaviour
{
    public ItemTemplate(ItemData itemdata)
    {
        this.itemData = itemdata;
    }
    public ItemData itemData;
}