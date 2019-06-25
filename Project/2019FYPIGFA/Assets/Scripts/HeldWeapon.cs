﻿using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Collider))]
public class HeldWeapon : MonoBehaviour
{
    [HideInInspector]
    public ItemData itemData;
    private float attackTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        attackTimer = 0f;
        GetComponent<MeshFilter>().mesh = itemData.mesh;
        GetComponent<MeshRenderer>().material = itemData.material;
        transform.localPosition = itemData.heldPosition;
        transform.localRotation = Quaternion.Euler(itemData.heldRotation);
    }

    // Update is called once per frame
    void Update()
    {
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;
        else
            attackTimer = 0f;
    }

    public void ChangeWeapon(ItemData newWeap)
    {
        itemData = newWeap;
        GetComponent<MeshFilter>().mesh = itemData.mesh;
        GetComponent<MeshRenderer>().material = itemData.material;
        transform.localPosition = itemData.heldPosition;
        transform.localRotation = Quaternion.Euler(itemData.heldRotation);
    }

    public void RemoveWeapon()
    {
        itemData.weaponType = ItemData.WEAPON_TYPE.NONE;
        itemData.weaponDamage = 0;
        itemData.attackRate = 0;
        itemData.durability = 0;
        itemData.durabilityDecay = 0;
        itemData.attackRange = 0;
        itemData.type = null;
        itemData.mesh = null;
        itemData.material = null;
        itemData.heldPosition = Vector3.zero;
        itemData.heldRotation = Vector3.zero;
        Destroy(GetComponent<MeshFilter>().mesh);
        Destroy(GetComponent<MeshRenderer>().material);
    }

    public bool Fire()
    {
        RaycastHit hit;
        // attackrate = times per second can attack
        // attacktimer = when zero, can attack
        // attack timer set to (1 / attackrate) when attack

        switch (itemData.weaponType)
        {
            case ItemData.WEAPON_TYPE.CLOSE_RANGE:
                if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)), out hit, itemData.attackRange))
                {
                    if (attackTimer > 0f)
                        return false;

                    if (hit.collider.GetComponent<Enemy>() != null)
                    {
                        Enemy enemy = hit.collider.GetComponent<Enemy>();
                        enemy.health = Mathf.Clamp(enemy.health - itemData.weaponDamage, 0, enemy.maxHealth);
                        attackTimer = 1 / itemData.attackRate;
                        itemData.durability = Mathf.Clamp(itemData.durability - itemData.durabilityDecay, 0, 100);
                        Debug.Log("Hit " + enemy.enemyType + " for " + itemData.weaponDamage + " damage with " + itemData.type + ". Durability Left: " + itemData.durability + "%");
                    }
                }
                break;
            case ItemData.WEAPON_TYPE.CONDIMENT_MUSTARD:
                if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)), out hit, itemData.attackRange))
                {
                    if (attackTimer > 0f)
                        return false;

                    if (hit.collider.GetComponent<Enemy>() != null)
                    {
                        Enemy enemy = hit.collider.GetComponent<Enemy>();
                        enemy.health = Mathf.Clamp(enemy.health - itemData.weaponDamage, 0, enemy.maxHealth);
                        attackTimer = 1 / itemData.attackRate;
                        itemData.durability = Mathf.Clamp(itemData.durability - itemData.durabilityDecay, 0, 100);
                        Debug.Log("Hit " + enemy.enemyType + " for " + itemData.weaponDamage + " damage with " + itemData.type + ". Durability Left: " + itemData.durability + "%");
                    }
                }

                break;
            default:
                break;
        }

        return true;
    }
}
