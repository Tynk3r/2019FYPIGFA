using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public Vector2 m_size;
    public Vector2 m_position;
    
    public Room(float width, float height)
    {
        m_size = new Vector2(width, height);
    }
}
