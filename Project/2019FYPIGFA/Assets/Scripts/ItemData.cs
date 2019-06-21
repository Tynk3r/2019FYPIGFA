using System;
using UnityEngine;

[Serializable]
public class ItemData
{
    public enum WEAPON_TYPE
    {
        CLOSE_RANGE,
        PROJECTILE,
    }

    public WEAPON_TYPE weaponType;
    public string type;
    public Mesh mesh;
    public Material material;

    public ItemData Clone()
    {
        ItemData clone = new ItemData
        {
            type = this.type,
            mesh = this.mesh,
            material = this.material
        };
        return clone;
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
