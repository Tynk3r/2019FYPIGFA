using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    HallwayGenerator hallwayGenerator = new HallwayGenerator();
    RoomPropGenerator roomPropGenerator = new RoomPropGenerator();
    RoomGenerator roomGenerator = new RoomGenerator();
    public GenerationType generationType;
    public Material BSP_mat;
    public Material BSP_room;
    public Material BSP_hall;
    public int S_mapWidth, S_mapDepth;

    public float S_minRoomSize = 3f;
    public float S_maxLeafSize, S_minLeafSize;
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

    void CullParentLeaves(List<Leaf> leaves)
    {
        int i = 0;
        while (true)
        {
            Leaf subject = leaves[i];
            if (subject.leftChild != null || subject.rightChild != null)
            {
                leaves.Remove(subject);
                //Debug.Log("removed one. Now count is " + leaves.Count);
                if (i == leaves.Count)
                    break;
            }
            else
            {
                //Debug.Log("i is " + i + "| count is " + leaves.Count + "| yes");
                if (++i == leaves.Count)
                    break;
            }
        }
    }

    void CreateRooms()
    {
        MapTreeGenerator.MAX_LEAF_SIZE = S_maxLeafSize;
        MapTreeGenerator.MIN_LEAF_SIZE = S_minLeafSize;
        List<Leaf> map = MapTreeGenerator.GenerateLeaves(S_mapWidth, S_mapDepth);
        var rooms = roomGenerator.GenerateRooms(ref map);
        List<Hallway> hallways = hallwayGenerator.D2GenerateHallways(ref map);

        //foreach (Leaf i in map)
        //{
        //    GameObject leaf = MeshGenerator.CreatePlane(i.width, i.height, 0f, false);
        //    leaf.transform.Translate(new Vector3(i.x, i.level * 5, i.y));
        //    Renderer rend = leaf.GetComponent<Renderer>();
        //    rend.material = BSP_mat;
        //    leaf.transform.parent = demoMap.transform;
        //    offsetY += 1;
        //}

        foreach (Room i in rooms)
        {
            //GameObject room = MeshGenerator.CreatePlane(i.m_size.x, i.m_size.y, false);
            //room.transform.Translate(new Vector3(i.m_position.x, offsetY, i.m_position.y));
            //Renderer rend = room.GetComponent<Renderer>();
            //rend.material = BSP_room;
            //room.transform.parent = demoMap.transform;
            roomPropGenerator.D1_GenerateRoomDetails(i, ref tileSet, ref wallTileSet, offsetY).transform.parent = demoMap.transform;
            // Create the walls
        }
        foreach (Hallway hallway in hallways)
        {
            foreach (Hallway.Hall i in hallway.m_halls)
            {
                GameObject hall = MeshGenerator.CreatePlane(i.size.x, i.size.y, 0f, false);
                hall.transform.Translate(new Vector3(i.position.x, offsetY, i.position.z));
                Renderer rend = hall.GetComponent<Renderer>();
                rend.material = BSP_hall;
                hall.transform.parent = demoMap.transform;
            }
        }
        return;
        /*
        var originalMap = new List<Leaf>(map);
        List<Room> rooms;
        // If the map generation uses polished layout
        if (generationType > GenerationType.BSP_LAYOUT_ALL)
            CullParentLeaves(map);
        foreach (Leaf i in map)
        {
            GameObject leaf = MeshGenerator.CreatePlane(i.width, i.height, 0f, false);
            leaf.transform.Translate(new Vector3(i.x, offsetY, i.y));
            Renderer rend = leaf.GetComponent<Renderer>();
            rend.material = BSP_mat;
            leaf.transform.parent = demoMap.transform;
            offsetY += 1;
        }
        // Generate rooms and hallways
        if (generationType > GenerationType.BSP_LAYOUT_ALL)
        {
            rooms = roomGenerator.GenerateRooms(map);

            foreach (Room i in rooms)
            {
                //GameObject room = MeshGenerator.CreatePlane(i.m_size.x, i.m_size.y, false);
                //room.transform.Translate(new Vector3(i.m_position.x, offsetY, i.m_position.y));
                //Renderer rend = room.GetComponent<Renderer>();
                //rend.material = BSP_room;
                //room.transform.parent = demoMap.transform;
                roomPropGenerator.D1_GenerateRoomDetails(i, ref tileSet, ref wallTileSet, offsetY).transform.parent = demoMap.transform;
                // Create the walls
            }
            offsetY += 1;
            // Generate the hallway layout
            List<Hallway> hallways = hallwayGenerator.D1_GenerateHallways(originalMap);
            Debug.Log("hallway made with size of " + hallways.Count);
            foreach (Hallway hallway in hallways)
            {
                foreach (Hallway.Hall i in hallway.m_halls)
                {
                    GameObject hall = MeshGenerator.CreatePlane(i.size.x, i.size.y, 0f, true);
                    hall.transform.Translate(new Vector3(i.position.x, offsetY, i.position.y));
                    Renderer rend = hall.GetComponent<Renderer>();
                    rend.material = BSP_hall;
                    hall.transform.parent = demoMap.transform;
                }
            }
        }
        */
    }
/*
    void CreateLayoutDebug()
    {
        List<Leaf> map = MapTreeGenerator.GenerateLeaves(S_mapWidth, S_mapDepth);
        var originalMap = new List<Leaf>(map);
        List<Room> rooms;
        // If the map generation uses polished layout
        if (generationType > GenerationType.BSP_LAYOUT_ALL)
            CullParentLeaves(map);
        foreach(Leaf i in map)
        {
            GameObject leaf = MeshGenerator.CreatePlane(i.width, i.height, 0f, false);
            leaf.transform.Translate(new Vector3(i.x, offsetY, i.y));
            Renderer rend = leaf.GetComponent<Renderer>();
            rend.material = BSP_mat;
            leaf.transform.parent = demoMap.transform;
            offsetY += 1;
        }
        // Generate rooms and hallways
        if (generationType > GenerationType.BSP_LAYOUT_ALL)
        {
            rooms = roomGenerator.GenerateRooms(map);
            
            foreach(Room i in rooms)
            {
                GameObject room = MeshGenerator.CreatePlane(i.m_size.x, i.m_size.y, 0f, false);
                room.transform.Translate(new Vector3(i._position.x, offsetY, i._position.z));
                Renderer rend = room.GetComponent<Renderer>();
                rend.material = BSP_room;
                room.transform.parent = demoMap.transform;
            }
            offsetY += 1;
            // Generate the hallway layout
            List<Hallway> hallways = hallwayGenerator.D1_GenerateHallways(originalMap);
            Debug.Log("hallway made with size of " + hallways.Count);
            foreach(Hallway hallway in hallways)
            {
                foreach(Hallway.Hall i in hallway.m_halls)
                {
                    GameObject hall = MeshGenerator.CreatePlane(i.size.x, i.size.y, 0f, false);
                    hall.transform.Translate(new Vector3(i.position.x, offsetY, i.position.y));
                    Renderer rend = hall.GetComponent<Renderer>();
                    rend.material = BSP_hall;
                    hall.transform.parent = demoMap.transform;
                }
            }
        }
    }
    */
    public void GenerateMap()
    {
        offsetY = 0;
        CreateRooms();
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
}
