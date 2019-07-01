using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public int numberOfObjectives;
    public int numberOfWeapons;
    public List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
    public List<ItemTemplate> weaponsToSpawn = new List<ItemTemplate>();

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

        // Spawn objectives
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

        // Spawn
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
