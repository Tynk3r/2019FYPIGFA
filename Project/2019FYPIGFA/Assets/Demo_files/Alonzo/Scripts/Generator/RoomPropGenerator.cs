using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPropGenerator
{
    public uint roomCount = 1;
    public GameObject D1_GenerateRoomDetails(Room _room, ref MapGenerator.Tile[] _floorTiles, ref MapGenerator.Tile[] _wallTiles , int offset = 0)
    {
        GameObject room = new GameObject("Room" + roomCount);
        // Generate the floor
        GameObject floor = MeshGenerator.CreatePlane(_room.m_size.x, _room.m_size.y, 1f, true);
        floor.GetComponent<Renderer>().material = _floorTiles[Random.Range(0, _floorTiles.Length)].material;
        floor.transform.Translate(_room._position.x, offset, _room._position.z);
        floor.transform.parent = room.transform;
        GameObject[] wall = new GameObject[]
        {
            MeshGenerator.CreatePlane(_room.m_size.x, _room.m_roomHeight, 1f, false), // Create top wall
            MeshGenerator.CreatePlane(_room.m_size.x, _room.m_roomHeight, 1f, false), // Create bottom wall
            MeshGenerator.CreatePlane(_room.m_roomHeight, _room.m_size.y, 1f, false), // Create left wall
            MeshGenerator.CreatePlane(_room.m_roomHeight, _room.m_size.y, 1f, false) // Create right wall
        };
        // Rotate the walls
        // Translate the walls
        wall[0].transform.Translate(new Vector3(_room._position.x, _room.m_roomHeight * 0.5f + offset, _room._position.z + _room.m_size.y * 0.5f)); // Translate top wall
        wall[1].transform.Translate(new Vector3(_room._position.x, _room.m_roomHeight * 0.5f + offset, _room._position.z - _room.m_size.y * 0.5f)); // Translate bottom wall
        wall[2].transform.Translate(new Vector3(_room._position.x - _room.m_size.x * 0.5f, _room.m_roomHeight * 0.5f + offset, _room._position.z)); // Translate left wall
        wall[3].transform.Translate(new Vector3(_room._position.x + _room.m_size.x * 0.5f, _room.m_roomHeight * 0.5f + offset, _room._position.z)); // Translate right wall

        //wall[0].transform.Translate(new Vector3(_room.m_position.x, offset, _room.m_position.y)); // Translate top wall
        //wall[1].transform.Translate(new Vector3(_room.m_position.x, offset, _room.m_position.y)); // Translate bottom wall
        //wall[2].transform.Translate(new Vector3(_room.m_position.x, offset, _room.m_position.y)); // Translate left wall
        //wall[3].transform.Translate(new Vector3(_room.m_position.x, offset, _room.m_position.y)); // Translate right wall

        wall[0].transform.Rotate(new Vector3(1f, 0f, 0f), -90f); // top wall
        wall[1].transform.Rotate(new Vector3(1f, 0f, 0f), 90f); // bottom wall
        wall[2].transform.Rotate(new Vector3(0f, 0f, 1f), -90f); // left wall
        wall[3].transform.Rotate(new Vector3(0f, 0f, 1f), 90f); // right wall

        foreach (GameObject i in wall)
        {
            i.transform.parent = room.transform;
            Renderer rend = i.GetComponent<Renderer>();
            int randomIndex = Random.Range(0, _wallTiles.Length);
            rend.material = _wallTiles[randomIndex].material;
        }
        return room;
    }
}
