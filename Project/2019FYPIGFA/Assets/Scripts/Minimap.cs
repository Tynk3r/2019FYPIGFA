using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public Transform player;
    public Mesh cube;

    private void Start()
    {
        // Display walls in Minimap
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
    }

    void LateUpdate()
    {
        Vector3 newPos = player.position;
        newPos.y = transform.position.y;
        transform.position = newPos;

        transform.rotation = Quaternion.Euler(90f, player.rotation.eulerAngles.y, 0f);
    }
}
