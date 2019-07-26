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

    [Header("Level")]
    public GameObject objectivePrefab;
    public GameObject cashierPrefab;

    private void Start()
    {
        foreach(GameObject go in FindObjectsOfType<GameObject>())
        {
            InitMinimapObject(go);
        }
    }

    public void DestroyMinimapElements()
    {
        GameObject[] objects = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        List<GameObject> objectsToClear = new List<GameObject>();
        foreach(GameObject obj in objects)
        {
            if (obj.layer == 9)
                objectsToClear.Add(obj);
        }
        Debug.Log("there are " + objectsToClear.Count + "GameObjects to destroy");
        foreach(GameObject obj in objectsToClear)
        {
            Destroy(obj.gameObject);
        }
    }

    public void InitMinimapObject(GameObject go)
    {
        switch (go.tag)
        {
            case "Walls":
                GameObject wallObject = Instantiate(go, go.transform);
                if (go.GetComponent<Collider>().GetType() == typeof(BoxCollider))
                {
                    Destroy(wallObject.GetComponent<Collider>());
                    wallObject.transform.localPosition = go.GetComponent<BoxCollider>().center;
                    wallObject.transform.localScale = go.GetComponent<BoxCollider>().size;
                    wallObject.transform.rotation = go.transform.rotation;
                    wallObject.GetComponent<MeshFilter>().sharedMesh = cube;
                    wallObject.tag = "Untagged";
                    wallObject.layer = 9;
                }
                else
                {
                    Debug.Log(go + " does not have a collider the game can parse as a wall. Collider Type: " + go.GetComponent<Collider>().GetType());
                    Destroy(wallObject);
                }
                break;
            case "Enemy":
                GameObject arrowObject = Instantiate(enemyArrow, go.transform);
                arrowObject.tag = "Untagged";
                arrowObject.layer = 9;
                break;
            case "Cashier":
                GameObject cashierObject = Instantiate(cashierPrefab, go.transform);
                cashierObject.tag = "Untagged";
                cashierObject.layer = 9;
                break;
            case "Objective":
                GameObject objectiveRep = Instantiate(objectivePrefab, go.transform);
                objectiveRep.layer = 9;
                break;
            case "Interactable":
                GameObject weaponRep = Instantiate(go, go.transform);
                bool componentsRemoved = false;
                while (!componentsRemoved)
                {
                    foreach (Component c in weaponRep.GetComponents(typeof(Component)))
                    {
                        if (c.GetType() == typeof(Transform)
                            || c.GetType() == typeof(MeshRenderer)
                            || c.GetType() == typeof(MeshFilter))
                            componentsRemoved = true;
                        else
                        {
                            DestroyImmediate(c);
                            componentsRemoved = false;
                        }
                    }
                }
                weaponRep.tag = "Untagged";
                weaponRep.layer = 9;
                weaponRep.transform.localPosition = Vector3.zero;
                break;
            default:
                break;
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
