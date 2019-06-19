using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public Vector2 m_size;
    public Vector3 _position;
    public List<HallPoint> m_entrances = new List<HallPoint>();
    public float m_roomHeight = 1;
    public Room(float width, float height)
    {
        m_size = new Vector2(width, height);
    }
}
