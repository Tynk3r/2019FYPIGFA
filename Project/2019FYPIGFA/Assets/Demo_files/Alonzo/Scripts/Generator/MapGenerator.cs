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
    
    public Vector2 S_roomSizeLimit;
    public Vector2 S_roomHeightLimit;
    public float S_maxLeafSize, S_minLeafSize;

    public Vector2 liftSize;

    private int m_levelSeed;
    #if UNITY_EDITOR
    int offsetY = 0;
    public bool autoUpdate;
    GameObject demoMap;
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

    void CalculatePropBounds()
    {
        int index = 0;
        while (index < props.Length)
        {
            GameObject item = Instantiate(props[index].prefab);
            Collider col = item.GetComponent<BoxCollider>();
            props[index].minBounds = col.bounds.min;
            props[index].maxBounds = col.bounds.max;
            DestroyImmediate(item);
            ++index;
        }
    }

    public void GenerateMap()
    {

        CalculatePropBounds();
        if (null != demoMap)
            DestroyImmediate(demoMap);
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
            roomPropGenerator.GenerateLayout(new Vector2(0f, roomSize.y * 0.5f), roomSize, ref props)
                .transform.parent = demoMap.transform;
        }

        foreach (NavMeshSurface surface in navSurface)
        {
            surface.BuildNavMesh();
        }
        return;
        offsetY = 0;
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
    [System.Serializable]
    public struct Prop
    {
        public GameObject prefab;
        public Vector3 minBounds;
        public Vector3 maxBounds;
    }
}
