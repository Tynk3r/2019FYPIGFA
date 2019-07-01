﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Objectives
    public int numberOfObjectives;

    // Weapons
    public int numberOfWeapons;
    public List<ItemTemplate> weaponsToSpawn = new List<ItemTemplate>();

    // Main
    [HideInInspector]
    public bool finishedLevel = false;
    public List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

    // Start is called before the first frame update
    void Start()
    {
        InitPoints();
    }

    // Update is called once per frame
    void Update()
    {

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

}
