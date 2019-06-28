﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPoolItem
{
    public string name;
    public GameObject objectToPool;
    public int amountToPool;
    public bool shouldExpand = true;
    public ObjectPoolItem(string name, GameObject obj, int amount, bool expand = true)
    {
        this.name = name;
        objectToPool = obj;
        amountToPool = amount;
        shouldExpand = expand;
    }
}
public class ProjectilePool : MonoBehaviour
{
    public static ProjectilePool g_sharedInstance;
    public List<ObjectPoolItem> itemsToPool;            // List of items to pool

    // Dictionary containing the names of each bullet
    public List<List<GameObject>> pooledObjectsList;    // Actual object pool
    //private List<GameObject> pooledObjects; // CHECK: shift to be local variable?
    private List<string> pooledObjectNames;
    private List<int> positions;
    private void Awake()
    {
        g_sharedInstance = this;
        pooledObjectNames = new List<string>();
        pooledObjectsList = new List<List<GameObject>>();
        //pooledObjects = new List<GameObject>(); // TODO: Check if appears in editor
        positions = new List<int>(); // To store offsets of current projectile to use
        for (int i = 0; i < itemsToPool.Count; ++i)
        {
            StoreObjectPoolElement(i); // Pools items based on element index
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject FetchObjectInPool(int _index)
    {
        int currSize = pooledObjectsList[_index].Count; // Size of pool
        for (int i = positions[_index] + 1; i < positions[_index] + pooledObjectsList[_index].Count; ++i) // Possible lag if bullets live too long
        {
            if (!pooledObjectsList[_index][i % currSize].activeSelf) // Does not check if parent is inactive
            {
                positions[_index] = i % currSize;
                return pooledObjectsList[_index][i % currSize]; // Returns the game object requested
            }
        } // NOTE: slow bullets will impair performance if there's too little objects

        // If pool can expand and too no objects were found
        if (itemsToPool[_index].shouldExpand)
        {
            GameObject obj = Instantiate(itemsToPool[_index].objectToPool); // Instantiate another game object
            obj.GetComponent<I_Projectile>().Initialize();
            obj.SetActive(false);
            obj.transform.parent = this.transform; // CHECK: this necessary?
            pooledObjectsList[_index].Add(obj); // Add to the list
            return obj; // Return the new bullet
        }
        return null;
    }

    public struct bullets
    {
        GameObject bullet;
        int maxCount;
    }

    public int GetPooledObjectIndex(string _name)
    {
        int index = 0;
        foreach(ObjectPoolItem item in itemsToPool)
        {
            if (_name == item.name)
                return index;
            ++index;
        }
        return -1;
    }

    void StoreObjectPoolElement(int _index)
    {
        ObjectPoolItem item = itemsToPool[_index];   // Get the item using index\
        pooledObjectNames.Add(item.name);
        var pooledObjects = new List<GameObject>(); // List of pooled objects of 1 type
        for (int i = 0; i < item.amountToPool; ++i)
        {
            GameObject obj = Instantiate(item.objectToPool);
            obj.GetComponent<I_Projectile>().Initialize();
            obj.SetActive(false);
            obj.transform.parent = this.transform; // CHECK: this necessary?
            pooledObjects.Add(obj);
        }
        pooledObjectsList.Add(pooledObjects);
        positions.Add(0); // Offset for finding object
    }
}
