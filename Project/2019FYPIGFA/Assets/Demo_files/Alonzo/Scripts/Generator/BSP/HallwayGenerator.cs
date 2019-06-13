using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallwayGenerator
{
    float m_maxDimension;
    Vector2 m_hallwayWidthRange = new Vector2(10f, 6f); // Use this to generate more hallway variations
    public List<Hallway> GenerateHallways(List<Leaf> _leaves)
    {
        var hallways = new List<Hallway>();
        // For every leaf that has children, create hallways connecting the 2 children
        int count = 0;
        foreach (Leaf leaf in _leaves)
        {
            if (leaf.leftChild != null && leaf.rightChild != null)
            {
                ++count;
                Leaf leafA = leaf.leftChild;
                Leaf leafB = leaf.rightChild;
                if (leafA != leafB)
                    hallways.Add(CreateHallway(leafA.GetLowestLeaf(), leafB.GetLowestLeaf()));
                else
                    Debug.Log("2 leaves are the same!");
            }
        }
        Debug.Log("count of hall iterations is " + hallways.Count + "| count of iterations is " + count);
        return hallways;
    }

    // TODO: Add improvement on variations of hallways. Curved options, if possible
    // TODO: Add a auto snapping behaviour to avoid bad meshes
    Hallway CreateHallway(Leaf _leafA, Leaf _leafB)
    {
        Hallway hallway = new Hallway(_leafA.room, _leafB.room); // Hallway to return
        Room roomA = _leafA.room;
        Room roomB = _leafB.room;
        Vector2 roomMinL, roomMinR;
        Vector2 roomMaxL, roomMaxR;

        // Pick random points within the 2 rooms
        Vector2 pointA = new Vector2(
            roomA.m_position.x + Random.Range(-0.5f * roomA.m_size.x, 0.5f * roomA.m_size.x),
            roomA.m_position.y + Random.Range(-0.5f * roomA.m_size.y, 0.5f * roomA.m_size.y));
        Vector2 pointB = new Vector2(
            roomB.m_position.x + Random.Range(-0.5f * roomB.m_size.x, 0.5f * roomB.m_size.x),
            roomB.m_position.y + Random.Range(-0.5f * roomB.m_size.y, 0.5f * roomB.m_size.y));
        // This allows us to get RAW hallways that are not culled and has no intersection or y axis checks
        hallway.m_halls = CreateHalls(roomA, roomB, pointA, pointB);
        // Cull and split halls that collides with other rooms

        return hallway;
    }

    List<Hallway> CullHalls(List<Hallway> _hallways)
    {
        // Take every hallway, cull off areas colliding with rooms, split hallways into 2 or more if more rooms are within halls
        foreach (Hallway hallway in _hallways)
        {
            // For each hall in hallway, check if it collides into a room

        }
        return _hallways;
    }

    // !Deprecated function to create hallways using rectangles. Has limitations and ineffective in determining collisions early
    List<Hallway.DHall> CreateHalls(Room _roomA, Room _roomB, Vector2 _pointA, Vector2 _pointB)
    {
        var halls = new List<Hallway.DHall>();
        // TODO: customizable width
        Hallway.DHall hallX = new Hallway.DHall(
            new Vector2(_pointA.x + (_pointB.x - _pointA.x) * 0.5f, _pointA.y),
            new Vector2(Mathf.Abs(_pointB.x - _pointA.x), 1f), _roomA, _roomB);
        Hallway.DHall hallY = new Hallway.DHall(
            new Vector2(_pointB.x, _pointB.y + (_pointA.y - _pointB.y) * 0.5f),
            new Vector2(1f, Mathf.Abs(_pointA.y - _pointB.y)), _roomA, _roomB);
        halls.Add(hallX);
        halls.Add(hallY);
        return halls;
    }
}
