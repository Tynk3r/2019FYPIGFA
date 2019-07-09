using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public Transform player;

    [Header("Walls")]
    public Mesh cube;

    [Header("Enemies")]
    public GameObject enemyArrow;

    [Header("Spawn Points")]
    public GameObject objectiveRepresentation;

    private void Start()
    {
        // Display Walls in Minimap
        var walls = GameObject.FindGameObjectsWithTag("Walls");
        foreach (GameObject realObject in walls)
        {
            GameObject wallObject = Instantiate(realObject, realObject.transform);
            if (realObject.GetComponent<Collider>().GetType() == typeof(BoxCollider))
            {
                Destroy(wallObject.GetComponent<Collider>());
                wallObject.transform.localPosition = realObject.GetComponent<BoxCollider>().center;
                wallObject.transform.localScale = realObject.GetComponent<BoxCollider>().size;
                wallObject.transform.rotation = realObject.transform.rotation;
                wallObject.GetComponent<MeshFilter>().sharedMesh = cube;
                wallObject.layer = 9;
            }
            else
            {
                Debug.Log(realObject + " does not have a collider the game can parse as a wall. Collider Type: " + realObject.GetComponent<Collider>().GetType());
                Destroy(wallObject);
            }
        }

        // Display Enemies in Minimap
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject realObject in enemies)
        {
            GameObject arrowObject = Instantiate(enemyArrow, realObject.transform);
            arrowObject.layer = 9;
        }

        // Display Spawn Points in Minimap
        var spawnPoints = FindObjectsOfType<SpawnPoint>();
        foreach (SpawnPoint realObject in spawnPoints)
        {
            switch(realObject.GetPointType())
            {
                case SpawnPoint.POINT_TYPE.OBJECTIVE:
                    GameObject objectiveRep = Instantiate(objectiveRepresentation, realObject.transform);
                    objectiveRep.layer = 9;
                    break;
                case SpawnPoint.POINT_TYPE.WEAPON:
                    GameObject weaponRep = Instantiate(realObject.gameObject, realObject.transform);

                    bool componentsRemoved = false;
                    while(!componentsRemoved)
                    {
                        foreach (Component c in weaponRep.GetComponents(typeof(Component)))
                        {
                            if (c.GetType() == typeof(Transform)
                                || c.GetType() == typeof(MeshRenderer)
                                || c.GetType() == typeof(MeshFilter))
                            {
                                componentsRemoved = true;
                            }
                            else
                            {
                                DestroyImmediate(c);
                                componentsRemoved = false;
                            }
                        }
                    }
                    weaponRep.layer = 9;
                    weaponRep.transform.localPosition = Vector3.zero;
                    
                    break;
                default:
                    break;
            }
        }
    }

    void LateUpdate()
    {
        Vector3 newPos = player.position;
        newPos.y = transform.position.y;
        transform.position = newPos;

        transform.rotation = Quaternion.Euler(90f, player.rotation.eulerAngles.y, 0f);
    }
}
