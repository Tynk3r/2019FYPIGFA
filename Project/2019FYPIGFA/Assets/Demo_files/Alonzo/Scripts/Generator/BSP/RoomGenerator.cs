﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator
{
    public int m_minRoomSize = 3;
    const float ROOM_LEAF_OFFSET = 1f;
    // TODO: add different variables and map conditions for guranteed essential rooms

    // Create a room for each leaf. This function assumes there are no rooms.
    public List<Room> GenerateRooms(List<Leaf> _leaves)
    {
        var rooms = new List<Room>();
        foreach(Leaf leaf in _leaves)
        {
            // Creating a room
            Room room = new Room(
                Random.Range(m_minRoomSize, leaf.width - ROOM_LEAF_OFFSET),
                Random.Range(m_minRoomSize, leaf.height - ROOM_LEAF_OFFSET));
            // Now find the min and max points of the leaf to avoid colliding into other leaves
            Vector2 leafMin = new Vector2(leaf.x - leaf.width * 0.5f + ROOM_LEAF_OFFSET
                , leaf.y - leaf.height * 0.5f + ROOM_LEAF_OFFSET);
            Vector2 leafMax = new Vector2(leaf.x + leaf.width * 0.5f - ROOM_LEAF_OFFSET
                , leaf.y + leaf.height * 0.5f - ROOM_LEAF_OFFSET);
            // Now set the coordinates based on the size and boundaries of the leaf
            room.m_position = new Vector2(
                Random.Range(leafMin.x + room.m_size.x * 0.5f, leafMax.x - room.m_size.x * 0.5f),
                Random.Range(leafMin.y + room.m_size.y * 0.5f, leafMax.y - room.m_size.y * 0.5f));
            // Add the room to the list of rooms
            rooms.Add(room);
        }
        return rooms;
    }
}
