using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class MapGenerator : MonoBehaviour
{
    public NavMeshSurface[] navSurface;
    public Prop[] props;

    RoomPropGenerator roomPropGenerator = new RoomPropGenerator();
    public GenerationType generationType;
    public Material BSP_mat;
    public Material BSP_room;
    public Material BSP_hall;
    public int S_mapWidth, S_mapDepth;

    public Vector2 S_forwardSpaceBoundary;
    public Vector2 S_shelfSpacingBoundary;
    public Vector2 S_roomSizeLimit;
    public Vector2 S_roomHeightLimit;
    public float S_maxLeafSize, S_minLeafSize;

    public Vector2 liftSize;

    private int m_levelSeed;
    GameObject demoMap;
    public Minimap minimap;
    #if UNITY_EDITOR
    int offsetY = 0;
    public bool autoUpdate;
    public bool TEMPDELMAP;
#endif
    public Tile[] tileSet = new Tile[0];
    public Tile[] wallTileSet = new Tile[0];
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Rigidbody test = gameObject.GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Spawns the props, calculate the boundaries and destroys them.
    /// This is because bounds are incorrect if prefabs aren't spawned at least once
    /// </summary>
    void CalculatePropBounds()
    {
        int index = 0;
        while (index < props.Length)
        {
            GameObject item = Instantiate(props[index].prefab);
            Collider col = item.GetComponent<BoxCollider>();
            props[index].minBounds = col.bounds.min;
            props[index].maxBounds = col.bounds.max;
#if UNITY_EDITOR
            DestroyImmediate(item);
#endif
            ++index;
        }
    }

    public void CleanupMap()
    {
        if (null != demoMap)
        {
#if UNITY_EDITOR
            DestroyImmediate(demoMap);
#else
            Destroy(demoMap);
#endif
        }
    }

    /// <summary>
    /// Generates the map on current floor.
    /// Uses member variables to help guide level sizes and generation
    /// </summary>
    public void GenerateMap()
    {
        //minimap.DestroyMinimapElements();
        CalculatePropBounds();
        CleanupMap();
        demoMap = new GameObject("MAP");
        m_levelSeed = Random.Range(int.MinValue, int.MaxValue);
        Random.InitState(m_levelSeed);

        var roomSize = new Vector2(Random.Range(S_roomSizeLimit.x, S_roomSizeLimit.y), Random.Range(S_roomSizeLimit.x, S_roomSizeLimit.y));
        float roomHeight = Random.Range(S_roomHeightLimit.x, S_roomHeightLimit.y);
        Tile wall = wallTileSet[Random.Range(0, wallTileSet.Length)];
        Tile floor = tileSet[Random.Range(0, tileSet.Length)];
        float uvScale = 0.5f;
        // Generate the base floor of the room
        {
            GameObject baseFloor = MeshGenerator.CreatePlane(roomSize.x, roomSize.y, uvScale);
            baseFloor.transform.Translate(0f, 0f, roomSize.y * 0.5f);
            baseFloor.transform.parent = demoMap.transform;
            var renderer = baseFloor.GetComponent<Renderer>();
            renderer.material = floor.material;
        }
        // Generate the lift entrance
        {
            GameObject liftDoor = MeshGenerator.CreateDoorWall(roomSize.x, roomHeight, liftSize.x, liftSize.y, roomSize.x * 0.5f, uvScale);
            liftDoor.transform.parent = demoMap.transform;
            var renderer = liftDoor.GetComponent<Renderer>();
            renderer.material = wall.material;
        }
        // Generate the walls now that do not include the lift wall
        {
            GameObject leftWall = MeshGenerator.CreatePlane(roomHeight, roomSize.y, uvScale);
            GameObject rightWall = MeshGenerator.CreatePlane(roomHeight, roomSize.y, uvScale);
            GameObject backWall = MeshGenerator.CreatePlane(roomSize.x, roomHeight, uvScale);
            // Translate to right position
            leftWall.transform.Translate(roomSize.x * -0.5f, roomHeight * 0.5f, roomSize.y * 0.5f);
            rightWall.transform.Translate(roomSize.x * 0.5f, roomHeight * 0.5f, roomSize.y * 0.5f);
            backWall.transform.Translate(0f, roomHeight * 0.5f, roomSize.y);
            // Rotate to correct direction
            leftWall.transform.Rotate(new Vector3(0f, 0f, -90f));
            rightWall.transform.Rotate(new Vector3(0f, 0f, 90f));
            backWall.transform.Rotate(new Vector3(-90f, 0f, 0f));
            // Texture the walls
            Renderer renderer;
            renderer = leftWall.GetComponent<Renderer>();
            renderer.material = wall.material;
            renderer = rightWall.GetComponent<Renderer>();
            renderer.material = wall.material;
            renderer = backWall.GetComponent<Renderer>();
            renderer.material = wall.material;
            // Set the walls parent
            leftWall.transform.parent = demoMap.transform;
            rightWall.transform.parent = demoMap.transform;
            backWall.transform.parent = demoMap.transform;
        }

        // Make the props spawn
        {
            if (!roomPropGenerator.SetShelfSpacingBoundaries(S_shelfSpacingBoundary))
                Debug.LogError("ShelfSpacingBoundaries is invalid! use this format: x = min val, y = max val");
            if (!roomPropGenerator.SetForwardSpaceBoundary(S_forwardSpaceBoundary))
                Debug.LogError("ForwardSpaceBoundary is invalid! use this format: x = min val, y = max val");
            roomPropGenerator.GenerateLayout(new Vector2(0f, roomSize.y * 0.5f), roomSize, ref props)
                .transform.parent = demoMap.transform;
        }

        foreach (NavMeshSurface surface in navSurface)
        {
            surface.BuildNavMesh();
        }
        //if (!TEMPDELMAP)
        //{
        //    minimap.InitializeMiniMap();
        //}
        return;
        GameObject plane = MeshGenerator.CreateStairs();// (10, 10, 1f, false);
        Renderer rend = plane.GetComponent<Renderer>();
        rend.material = tileSet[0].material;
        plane.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        plane.transform.parent = demoMap.transform;

#if UNITY_EDITOR
        plane.transform.parent = demoMap.transform;
#endif
    }
#if UNITY_EDITOR
    /// <summary>
    /// Generates the map in editor. This will not be compiled into builds
    /// </summary>
    public void DrawMapInEditor()
    {
        if (null == demoMap)
        {
            demoMap = new GameObject("MAP");
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
    /// <summary>
    /// Tile that contains materials for ingame level floor
    /// </summary>
    [System.Serializable]
    public struct Tile
    {
        public Material material;
        public FloorType floorType;
    };
    public enum GenerationType
    {
        BSP_LAYOUT_ALL,
        BSP_LAYOUT_TOP_LAYER,
        HIDDEN
    }
    /// <summary>
    /// Prop list which contains the boundaries
    /// Boundaries are only calculated when generation begins
    /// Objects are assumed to be facing -Z direction, especially if one sided
    /// </summary>
    [System.Serializable]
    public struct Prop
    {
        public GameObject prefab;
        public Vector3 minBounds;
        public Vector3 maxBounds;
    }
}
