using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    [Header("Main")]
    public Player player = null;
    [ReadOnly] public bool collectedAll = false;
    public List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

    [System.Serializable]
    public enum AGGRESSION_LEVELS
    {
        DOCILE,     // NIL - 1/4 
        ANGRY,      // 1/4 - 1/2
        ENRAGED,    // 1/2 - 3/4
        INSANE,     // 3/4 - ALL
    }
    [Header("Enemy")]
    public AGGRESSION_LEVELS aggressionLevel = AGGRESSION_LEVELS.DOCILE;
    public List<Enemy> enemyList = new List<Enemy>();

    [Header("Shopping List")]
    public int numberOfObjectives;
    public List<string> shoppingListText = new List<string>();
    public TextMeshProUGUI shoppingList;
    public AudioClip pickUpSound;
    public AudioClip submitSound;
    public AudioClip footstepSound;
    public AudioClip hitSound;
    public AudioClip healthSound;

    [Header("Powerups")]
    public int numberOfHealthPickups;
    public Mesh healthPickupMesh;
    public Material healthPickupMaterial;

    [Header("Weapons")]
    public int numberOfWeapons;
    public List<ItemTemplate> weaponsToSpawn = new List<ItemTemplate>();

    void Awake()
    {
        InitPoints();
        InitEnemyList();
        foreach (SpawnPoint pt in FindObjectsOfType<SpawnPoint>())
        {
            if (pt.GetPointType() == SpawnPoint.POINT_TYPE.OBJECTIVE)
                shoppingListText.Add(pt.GetPointName());
        }
        UpdateShoppingList();
    }

    public void InitPoints()
    {
        // Find all spawn points and stop function if not enough spawn points
        int totalPointsUsed = numberOfObjectives + numberOfWeapons/*+ whatever*/;
        foreach (SpawnPoint s in FindObjectsOfType<SpawnPoint>())
            spawnPoints.Add(s);
        if (spawnPoints.Count < 1 || spawnPoints.Count < totalPointsUsed)
        {
            Debug.LogError("Not Enough Spawn Points in Scene.");
            return;
        }
        else if (weaponsToSpawn.Count < 1 && numberOfWeapons > 0)
        {
            Debug.LogError("You haven't put in any ItemTemplates to spawn. Weapons will not be spawned.");
        }

        // Spawn Objectives
        float objectivesSpawned = 0;
        while (objectivesSpawned < numberOfObjectives)
        {
            int rand = Random.Range(0, spawnPoints.Count);
            SpawnPoint objective = spawnPoints[rand];
            if (objective.GetPointType() == SpawnPoint.POINT_TYPE.EMPTY)
            {
                objective.SetPointTo(new Objective());
                objectivesSpawned++;
            }
        }

        // Spawn Weapons
        float weaponsSpawned = 0;
        if (!(weaponsToSpawn.Count < 1 && numberOfWeapons > 0))
        {
            while (weaponsSpawned < numberOfWeapons)
            {
                int rand = Random.Range(0, spawnPoints.Count);
                int rand2 = Random.Range(0, weaponsToSpawn.Count);
                SpawnPoint weapon = spawnPoints[rand];
                if (weapon.GetPointType() == SpawnPoint.POINT_TYPE.EMPTY)
                {
                    weapon.SetPointTo(weaponsToSpawn[rand2].itemData);
                    weaponsSpawned++;
                }
            }
        }

        // Spawn Health Pickups
        float healthPickupsSpawned = 0;
        while (healthPickupsSpawned < numberOfHealthPickups)
        {
            int rand = Random.Range(0, spawnPoints.Count);
            SpawnPoint healthPickup = spawnPoints[rand];
            if (healthPickup.GetPointType() == SpawnPoint.POINT_TYPE.EMPTY)
            {
                healthPickup.GetComponent<MeshFilter>().mesh = healthPickupMesh;
                healthPickup.GetComponent<MeshRenderer>().material = healthPickupMaterial;
                healthPickup.SetPointTo(SpawnPoint.POINT_TYPE.HEALTH);
                healthPickupsSpawned++;
            }
        }

        // Clear Unused Spawn Points
        foreach (SpawnPoint s in FindObjectsOfType<SpawnPoint>())
        {
            if (s.GetPointType() == SpawnPoint.POINT_TYPE.EMPTY)
                s.gameObject.SetActive(false);
        }
    }

    public void InitEnemyList()
    {
        foreach (Enemy s in FindObjectsOfType<Enemy>())
            enemyList.Add(s);
    }

    public void UpdateShoppingList()
    {
        string s = "";
        foreach (var ss in shoppingListText)
        {
            s += "- " + ss + "\n";
        }
        s += "Held Items: \n";
        int i = 0;
        foreach (string ss in player.heldObjectives)
        {
            i++;
            if (i >= player.heldObjectives.Count)
                s += ss;
            else
                s += ss + ", ";
        }
        if (s == "")
        {
            s = "You've completed your shopping list for this level! Head to the stairs to get the next level.";
        }
        shoppingList.text = s;
    }

    public void PrintShoppingList()
    {
        string s = "";
        foreach (SpawnPoint pt in FindObjectsOfType<SpawnPoint>())
        {
            if (pt.GetPointType() == SpawnPoint.POINT_TYPE.OBJECTIVE)
                s += (pt.GetPointName() + "\n");
        }
        if (s == "")
            s = "You've completed your shopping list for this level! Head to the stairs to get the next level.";
        Debug.Log(s);
    }

    public SpawnPoint GetClosestPoint(Vector3 position, params SpawnPoint.POINT_TYPE[] types)
    {
        if (spawnPoints.Count() <= 0)
            return null;
        SpawnPoint tempPt = spawnPoints[0];
        float distance = Mathf.Infinity;
        foreach (SpawnPoint pt in FindObjectsOfType<SpawnPoint>())
        {
            if (pt
                && types.Contains(pt.GetPointType())
                && (position - pt.transform.position).magnitude <= distance)
            {
                distance = (position - pt.transform.position).magnitude;
                tempPt = pt;
            }
        }
        if (types.Contains(tempPt.GetPointType()))
            return tempPt;
        else
            return null;
    }

    public SpawnPoint GetClosestPoint(Vector3 position)
    {
        if (spawnPoints.Count() <= 0)
            return null;
        SpawnPoint tempPt = spawnPoints[0];
        float distance = Mathf.Infinity;
        foreach (SpawnPoint pt in FindObjectsOfType<SpawnPoint>())
        {
            if (pt
                && pt.GetPointType() != SpawnPoint.POINT_TYPE.EMPTY
                && (position - pt.transform.position).magnitude < distance)
            {
                distance = (position - pt.transform.position).magnitude;
                tempPt = pt;
            }
        }
        if (tempPt.GetPointType() != SpawnPoint.POINT_TYPE.EMPTY)
            return tempPt;
        else
            return null;
    }

    public string RemovePoint(SpawnPoint spawnPoint)
    {
        spawnPoints.Remove(spawnPoint);
        return spawnPoint.OnPickedUp();
    }
}
