using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    List<ItemData> itemList = new List<ItemData>();

    public void AddItem(ItemData itemToAdd)
    {
        itemList.Add(itemToAdd);
    }

    public void PrintAllItems()
    {
        int i = 0;
        string toPrint = "";
        foreach (ItemData item in itemList)
        {
            i++;
            toPrint += i + ". " + item.type + "\n";
        }
        if (i <= 0)
            toPrint = "No Items Found In Inventory";
        Debug.Log(toPrint);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
