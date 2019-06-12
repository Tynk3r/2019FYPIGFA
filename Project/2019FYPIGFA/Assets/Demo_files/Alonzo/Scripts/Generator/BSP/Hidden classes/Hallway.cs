using System;
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
        public Vector2 position;
        public Vector2 size;
        private Room roomA;
        private Room roomB;

        public Hall(Vector2 position, Vector2 size, Room roomA, Room roomB) : this()
        {
            this.position = position;
            this.size = size;
            this.roomA = roomA;
            this.roomB = roomB;
        }
    }
}
