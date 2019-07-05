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
    [ReadOnly] public bool finishedLevel = false;
    public List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

    [Header("Shopping List")]
    public int numberOfObjectives;
    public TextMeshProUGUI shoppingList;

    [Header("Weapons")]
    public int numberOfWeapons;
    public List<ItemTemplate> weaponsToSpawn = new List<ItemTemplate>();

    // Start is called before the first frame update
    void Start()
    {
        InitPoints();
        UpdateShoppingList();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateShoppingList()
    {
        string s = "";
        foreach (SpawnPoint pt in FindObjectsOfType<SpawnPoint>())
        {
            if (pt.GetPointType() == SpawnPoint.POINT_TYPE.OBJECTIVE)
                s += ("- " + pt.GetPointName() + "\n");
        }
        if (s == "")
        {
            s = "You've completed your shopping list for this level! Head to the stairs to get the next level.";
            finishedLevel = true;
        }
        shoppingList.text = s;
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

        // Clear Unused Spawn Points
        foreach (SpawnPoint s in FindObjectsOfType<SpawnPoint>())
        {
            if (s.GetPointType() == SpawnPoint.POINT_TYPE.EMPTY)
                s.gameObject.SetActive(false);
        }
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
        return tempPt;
    }

    public SpawnPoint GetClosestPoint(Vector3 position)
    {
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
        return tempPt;
    }

    public string RemovePoint(SpawnPoint spawnPoint)
    {
        spawnPoints.Remove(spawnPoint);
        return spawnPoint.OnPickedUp();
    }
}
