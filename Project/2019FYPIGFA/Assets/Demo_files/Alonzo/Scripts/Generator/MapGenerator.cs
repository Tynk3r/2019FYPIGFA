using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
#if UNITY_EDITOR
    public bool autoUpdate;
    GameObject demoMap;
#endif
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateMap()
    {
        GameObject plane = MeshGenerator.CreatePlane(50, 50);
#if UNITY_EDITOR
        plane.transform.parent = demoMap.transform;
#endif
    }
#if UNITY_EDITOR
    public void DrawMapInEditor()
    {
        if (null == demoMap)
        {
            demoMap = new GameObject("DEMO MAP");
        }
        else
        {
            DestroyImmediate(demoMap);
            //DrawMapInEditor();
            return;
        }
        GenerateMap();
    }
#endif
}
