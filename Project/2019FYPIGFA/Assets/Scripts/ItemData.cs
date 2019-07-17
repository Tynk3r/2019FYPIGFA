using System;
using UnityEngine;

[Serializable]
public class ItemData
{
    public enum WEAPON_TYPE
    {
        NONE,
        RAYCAST,
        PROJECTILE,
    }
    public enum SKILL_TYPE
    {
        NONE,
        LUNGE,
        MAYO_DRINK,
    }
    public enum BUFF_TYPE
    {
        NONE,
        HOT_SAUCE
    }
    public class WeaponBuff
    {
        public BUFF_TYPE buff = BUFF_TYPE.NONE;
        public float duration = 0f;
        public float magnitude = 0f;
    }

    public WeaponBuff weaponBuff = new WeaponBuff();
    public SKILL_TYPE skillType;
    public WEAPON_TYPE weaponType;
    public string type;
    public Mesh mesh;
    public Material material;
    [Tooltip("Determines orientation when held by player.")]
    public Vector3 heldPosition;
    [Tooltip("Determines orientation when held by player.")]
    public Vector3 heldRotation;
    [Tooltip("Determines the position of projectiles spawning, relative to y look object transform")]
    public float shootOffset = 0.5f;
    [Tooltip("Attacks Per Second")]
    public float attackRate;
    [Tooltip("Expressed as a Percentage")]
    [Range(1f, 100f)]
    public float durability;
    [Tooltip("Durability Damage Done Per Attack (in %)")]
    public float durabilityDecay;

    [DrawIf("weaponType", WEAPON_TYPE.RAYCAST)]
    public float weaponDamage;
    [DrawIf("weaponType", WEAPON_TYPE.RAYCAST)]
    public float attackRange;
    [DrawIf("weaponType", WEAPON_TYPE.RAYCAST)]
    public GameObject impactEffect;

    [DrawIf("weaponType", WEAPON_TYPE.PROJECTILE)]
    public string projectile;


    public ItemData Clone()
    {
        ItemData clone = new ItemData
        {
            skillType = this.skillType,
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
            heldRotation = this.heldRotation,
            impactEffect = this.impactEffect,
            projectile = this.projectile,
            weaponBuff = this.weaponBuff,
            shootOffset = this.shootOffset
        };
        return clone;
    }
}
