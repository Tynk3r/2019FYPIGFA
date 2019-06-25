using System;
using UnityEngine;

[Serializable]
public class ItemData
{
    public enum WEAPON_TYPE
    {
        NONE,
        CLOSE_RANGE,
        CONDIMENT_MUSTARD,
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
    public float attackRange;
    public string type;
    public Mesh mesh;
    public Material material;
    public Vector3 heldPosition;
    public Vector3 heldRotation;

    public ItemData Clone()
    {
        ItemData clone = new ItemData
        {
            weaponType = this.weaponType,
            weaponDamage = this.weaponDamage,
            attackRate = this.attackRate,
            durability = this.durability,
            durabilityDecay = this.durabilityDecay,
            attackRange = this.attackRange,
            type = this.type,
            mesh = this.mesh,
            material = this.material,
            heldPosition = this.heldPosition,
            heldRotation = this.heldRotation
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
