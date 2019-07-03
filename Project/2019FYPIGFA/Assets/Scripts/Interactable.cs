using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
[ExecuteInEditMode]
public class Interactable : MonoBehaviour
{
    [SerializeField]
    private ItemTemplate itemTemplate = null;
    [HideInInspector]
    public ItemData itemData = null;
    public bool canPickUp = true;
    
    public void Initialize(ItemData itemdata)
    {
        itemData = itemdata;
        //Debug.Log("Initialize");
    }

    // Start is called before the first frame update
    void Start()
    {
        if (itemTemplate)
            itemData = itemTemplate.itemData;
        else if (itemData == null)
        {
            Debug.LogError("Initiate this Interactable with an ItemData. Object erroring: " + gameObject);
            Destroy(this);
        }
        GetComponent<MeshFilter>().mesh = itemData.mesh;
        GetComponent<MeshRenderer>().material = itemData.material;
        GetComponent<MeshCollider>().sharedMesh = itemData.mesh;
        GetComponent<MeshCollider>().convex = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPickedUp(GameObject player)
    {
        if (player.GetComponent<Inventory>() != null)
            player.GetComponent<Inventory>().AddItem(itemData.Clone());
        Destroy(gameObject);
    }
}
