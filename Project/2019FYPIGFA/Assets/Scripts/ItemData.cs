using System;
using UnityEngine;

[Serializable]
public class ItemData
{
    public enum WEAPON_TYPE
    {
        NONE,
        CLOSE_RANGE,
        PROJECTILE,
    }

    public WEAPON_TYPE weaponType;
    public float weaponDamage;
    [Tooltip("Attacks Per Second")]
    public float attackRate;
    [Tooltip("Expressed as a Percentage")]
    [Range(1f,100f)]
    public float durability;
    [Tooltip("Durability Damage Done Per Attack (in %)")]
    public float durabilityDecay;
    public float range;
    public string type;
    public Mesh mesh;
    public Material material;

    public ItemData Clone()
    {
        ItemData clone = new ItemData
        {
            weaponType = this.weaponType,
            weaponDamage = this.weaponDamage,
            attackRate = this.attackRate,
            durability = this.durability,
            durabilityDecay = this.durabilityDecay,
            range = this.range,
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
