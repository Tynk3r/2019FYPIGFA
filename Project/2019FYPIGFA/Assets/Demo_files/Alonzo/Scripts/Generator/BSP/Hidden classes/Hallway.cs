using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hallway
{
    public Room m_roomA, m_roomB; // Connecting rooms from this hallway
    public List<DHall> m_halls;

    public Hallway(Room roomA, Room roomB)
    {
        m_roomA = roomA;
        m_roomB = roomB;
        //m_halls = new List<Hall>();
    }


    public struct DHall
    {
        public Vector2 position;
        public Vector2 size;
        private Room roomA;
        private Room roomB;

        public DHall(Vector2 position, Vector2 size, Room roomA, Room roomB) : this()
        {
            this.position = position;
            this.size = size;
            this.roomA = roomA;
            this.roomB = roomB;
        }
    }
}
public class HallPoint
{
    public HallPoint next;
    public Vector3 position;
    public enum Type
    {
        END,
        TURN_LINK,
    }
}
