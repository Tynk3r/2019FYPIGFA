using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hallway
{
    public Room m_roomA, m_roomB; // Connecting rooms from this hallway
    public List<Hall> m_halls;

    public Hallway(Room roomA, Room roomB)
    {
        m_roomA = roomA;
        m_roomB = roomB;
    }
    public struct Hall
    {
        Vector2 size;
        Vector2 position;
    }
}
