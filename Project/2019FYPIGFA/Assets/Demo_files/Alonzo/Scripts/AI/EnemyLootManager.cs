using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLootManager : MonoBehaviour
{
    public List<GameObject> baseWeaponList;
    List<EnemyLoot> m_lootShelves;
    List<ItemData> m_rangedWeapons = new List<ItemData>();
    List<ItemData> m_meleeWeapons = new List<ItemData>();
    private void Start()
    {
        m_lootShelves = new List<EnemyLoot>();
        EnemyLoot[] foundShelves= (EnemyLoot[])FindObjectsOfType(typeof(EnemyLoot));
        foreach (EnemyLoot shelf in foundShelves)
        {
            m_lootShelves.Add(shelf);
        }
        foreach(GameObject weaponObj in baseWeaponList)
        {
            ItemTemplate template = weaponObj.GetComponent<ItemTemplate>();
            ItemData data = template.itemData;
            if (ItemData.WEAPON_TYPE.PROJECTILE == data.weaponType)
            {
                m_rangedWeapons.Add(data);
                // Update the ranged weapon projectile ID
                data.projectileID = ProjectilePool.g_sharedInstance.GetPooledObjectIndex(data.projectile);
            }
            else if (ItemData.WEAPON_TYPE.RAYCAST == data.weaponType)
            {
                m_meleeWeapons.Add(data);
            }
            else
                Debug.LogError("One of the weapons in the loot manager does not have a type");
        }
        Debug.Log("Found " + m_rangedWeapons.Count + " ranged weapons and " + m_meleeWeapons.Count + " melee weapons");
    }

    public Vector3 GetLootLocation(Vector3 _enemyPos, out EnemyLoot _lootShelf)
    {
        float closest = float.MaxValue;
        _lootShelf = null;
        Vector3 closestPoint = new Vector3();
        foreach (EnemyLoot lootShelf in m_lootShelves)
        {
            closestPoint = lootShelf.GetClosestPoint(_enemyPos);
            float distanceSq = (closestPoint - _enemyPos).sqrMagnitude;
            if (distanceSq > closest)
                continue;
            closest = distanceSq;
            _lootShelf = lootShelf;
        }
        return closestPoint;
    }

    public ItemData GetWeapon(bool _ranged)
    {
        if (_ranged)
        {
            int randomWeaponID = Random.Range(0, m_rangedWeapons.Count - 1);
            return m_rangedWeapons[randomWeaponID];
        }
        else
        {
            int randomWeaponID = Random.Range(0, m_meleeWeapons.Count - 1);
            return m_meleeWeapons[randomWeaponID];
        }
    }
}
