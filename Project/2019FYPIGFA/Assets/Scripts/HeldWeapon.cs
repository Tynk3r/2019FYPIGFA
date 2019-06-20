using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class HeldWeapon : MonoBehaviour
{
    [HideInInspector]
    public ItemData itemData = null;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshFilter>().mesh = itemData.mesh;
        GetComponent<MeshRenderer>().material = itemData.material;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeWeap(ItemData newWeap)
    {
        itemData = newWeap;
        GetComponent<MeshFilter>().mesh = itemData.mesh;
        GetComponent<MeshRenderer>().material = itemData.material;
    }
}
