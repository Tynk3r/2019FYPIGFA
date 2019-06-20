using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<ItemData> itemList = new List<ItemData>();

    public void AddItem(ItemData itemToAdd)
    {
        itemList.Add(itemToAdd);
    }

    public void PrintAllItems()
    {
        string toPrint = "";
        foreach (ItemData item in itemList)
        {
            toPrint += (itemList.IndexOf(item)+1) + ". " + item.type + "\n";
        }
        if (toPrint == "")
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
