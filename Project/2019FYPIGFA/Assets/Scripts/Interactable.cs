using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Interactable : MonoBehaviour
{
    public ItemTemplate itemTemplate;
    public bool canPickUp = true;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshFilter>().mesh = itemTemplate.itemData.mesh;
        GetComponent<MeshRenderer>().material = itemTemplate.itemData.material;
        GetComponent<MeshCollider>().sharedMesh = itemTemplate.itemData.mesh;
        GetComponent<MeshCollider>().convex = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPickedUp(GameObject player)
    {
        if (player.GetComponent<Inventory>() != null)
            player.GetComponent<Inventory>().AddItem(itemTemplate.itemData.Clone());
        Destroy(gameObject);
        Debug.Log("Destwoyed");
    }
}
