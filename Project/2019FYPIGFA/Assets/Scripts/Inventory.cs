﻿using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<ItemData> itemList = new List<ItemData>();
    public HeldWeapon currentWeapon;
    public TextMeshProUGUI weaponTextOne, weaponTextTwo, weaponTextThree;
    public RectTransform selectionOutline;

    public void AddItem(ItemData itemToAdd)
    {
        itemList.Add(itemToAdd);
    }

    public void RemoveItem(ItemData itemToRemove)
    {
        itemList.Remove(itemToRemove);
    }

    public void PrintAllItems()
    {
        string toPrint = "";
        foreach (ItemData item in itemList)
        {
            toPrint += (itemList.IndexOf(item) + 1) + ". " + item.type;
            if (item == currentWeapon.itemData)
                toPrint += " (Currently Equipped)";
            toPrint += "\n";
        }
        if (toPrint == "")
            toPrint = "No Items Found In Inventory";
        Debug.Log(toPrint);
    }

    // Update is called once per frame
    void Update()
    {
        if (itemList.Count == 0)
        {
            selectionOutline.gameObject.SetActive(false);
        }
        if (itemList.Count > 0 && itemList[0] != null)
        {
            if (weaponTextOne.text != itemList[0].type)
                weaponTextOne.text = itemList[0].type;
            if (!selectionOutline.gameObject.activeSelf)
                selectionOutline.gameObject.SetActive(true);
            if (itemList[0] == currentWeapon.itemData && selectionOutline.anchoredPosition.x != weaponTextOne.rectTransform.anchoredPosition.x)
            {
                selectionOutline.anchoredPosition = new Vector2(weaponTextOne.rectTransform.anchoredPosition.x, weaponTextOne.rectTransform.anchoredPosition.y);
            }

        }
        else if (weaponTextOne.text != "Empty")
            weaponTextOne.text = "Empty";
        if (itemList.Count > 1 && itemList[1] != null)
        {
            if (weaponTextTwo.text != itemList[1].type)
                weaponTextTwo.text = itemList[1].type;
            if (!selectionOutline.gameObject.activeSelf)
                selectionOutline.gameObject.SetActive(true);
            if (itemList[1] == currentWeapon.itemData && selectionOutline.anchoredPosition.x != weaponTextTwo.rectTransform.anchoredPosition.x)
            {
                selectionOutline.anchoredPosition = new Vector2(weaponTextTwo.rectTransform.anchoredPosition.x, weaponTextTwo.rectTransform.anchoredPosition.y);
            }
        }
        else if (weaponTextTwo.text != "Empty")
            weaponTextTwo.text = "Empty";
        if (itemList.Count > 2 && itemList[2] != null)
        {
            if (weaponTextThree.text != itemList[2].type)
                weaponTextThree.text = itemList[2].type;
            if (!selectionOutline.gameObject.activeSelf)
                selectionOutline.gameObject.SetActive(true);
            if (itemList[2] == currentWeapon.itemData && selectionOutline.anchoredPosition.x != weaponTextThree.rectTransform.anchoredPosition.x)
            {
                selectionOutline.anchoredPosition = new Vector2(weaponTextThree.rectTransform.anchoredPosition.x, weaponTextThree.rectTransform.anchoredPosition.y);
            }
        }
        else if (weaponTextThree.text != "Empty")
            weaponTextThree.text = "Empty";
    }
}
