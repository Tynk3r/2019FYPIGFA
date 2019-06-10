using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GenerationType generationType;
#if UNITY_EDITOR
    public bool autoUpdate;
    GameObject demoMap;
#endif
    public Tile[] tileSet = new Tile[0];
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
    public enum FloorType
    {
        INDOOR,
        OUTDOOR
    };
    [System.Serializable]
    public struct Tile
    {
        public Material material;
        public FloorType floorType;
    };
    public enum GenerationType
    {
        BSP
    }
}
