using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallwayGenerator
{
    float m_maxDimension;
    Vector2 m_hallwayWidthRange; // Use this to generate more hallway variations

    public List<Hallway> GenerateHallways(List<Leaf> _leaves)
    {
        var hallways = new List<Hallway>();
        // For every leaf that has children, create hallways connecting the 2 children
        foreach (Leaf leaf in _leaves)
        {
            if (leaf.leftChild != null && leaf.rightChild != null)
            {
                
            }
        }
        //return hallways;
    }

    // TODO: Add improvement on variations of hallways. Curved options, if possible
    // TODO: Add a auto snapping behaviour to avoid bad meshes
    Hallway CreateHallway(Leaf _leafL, Leaf _leafR)
    {
        Hallway hallway; // Hallway to return
        Vector2 roomMinL, roomMinR;
        Vector2 roomMaxL, roomMaxR;
        bool adjacentX = false; // If rooms are adjacent to each other in X (possible straight hall)
        bool adjacentY = false; // If rooms are adjacent to each other in Y (possible straight hall)
        Vector2 leafLPoint, leafRPoint; // Random points in the room
        adjacentX = _leafL.room.m_size.x == 2f; //DELETE
        // Find the min and max boundaries of both rooms
        roomMinL = new Vector2(_leafL.room.m_position.x - _leafL.room.m_size.x * 0.5f, _leafL.room.m_position.y - _leafL.room.m_size.y * 0.5f);
        roomMaxL = new Vector2(_leafL.room.m_position.x + _leafL.room.m_size.x * 0.5f, _leafL.room.m_position.y + _leafL.room.m_size.y * 0.5f);

        roomMinR = new Vector2(_leafR.room.m_position.x - _leafR.room.m_size.x * 0.5f, _leafR.room.m_position.y - _leafR.room.m_size.y * 0.5f);
        roomMaxR = new Vector2(_leafR.room.m_position.x + _leafR.room.m_size.x * 0.5f, _leafR.room.m_position.y + _leafR.room.m_size.y * 0.5f);
        // CHECK: should a width check be present?
        // First compare and see if rooms are next to each other
        if (roomMinL.x > roomMinR.x && roomMinL.x < roomMaxR.x || roomMaxL.x > roomMinR.x && roomMaxL.x < roomMaxR.x)
            adjacentX = true;
        if (roomMinL.y > roomMinR.y && roomMinL.y < roomMaxR.y || roomMaxL.y > roomMinR.y && roomMaxL.y < roomMaxR.y)
            adjacentY = true;
        // By chance, determine if hallway should be straight or have more than 1 hall
        if (adjacentX || adjacentY) // If there's a possibility of a straight hallway
        {
            if (adjacentX)
                ;// Generate horizontall hallway
            else
                ;// Generate vertical hallway
        }
        // Else, choose which way to generate the multiple hallways
        else
        {
            
        }
        // Pick random points in both rooms without restrictions
        leafLPoint = new Vector2(
            Random.Range(_leafL.room.m_position.x - _leafL.room.m_size.x * 0.5f, _leafL.room.m_position.x + _leafL.room.m_size.x * 0.5f),
            Random.Range(_leafL.room.m_position.y - _leafL.room.m_size.y * 0.5f, _leafL.room.m_position.y + _leafL.room.m_size.y * 0.5f));
        leafRPoint = new Vector2(
            Random.Range(_leafR.room.m_position.x - _leafR.room.m_size.x * 0.5f, _leafR.room.m_position.x + _leafR.room.m_size.x * 0.5f),
            Random.Range(_leafR.room.m_position.y - _leafR.room.m_size.y * 0.5f, _leafR.room.m_position.y + _leafR.room.m_size.y * 0.5f));

        //return hallway;
    }
    
    //Hallway.Hall[] CreateHalls()
    //{
        
    //}
}
