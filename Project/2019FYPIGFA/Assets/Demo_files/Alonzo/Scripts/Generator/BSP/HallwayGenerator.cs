using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallwayGenerator
{
    float m_maxDimension;
    Vector2 m_hallwayWidthRange = new Vector2(3f, 6f); // Use this to generate more hallway variations
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
                hallways.Add(CreateHallway(leaf.GetLowestLeaf(), leaf.GetLowestLeaf()));
            }
        }
        Debug.Log("count of hall iterations is " + hallways.Count + "| count of iterations is " + count);
        return hallways;
    }

    // TODO: Add improvement on variations of hallways. Curved options, if possible
    // TODO: Add a auto snapping behaviour to avoid bad meshes
    Hallway CreateHallway(Leaf _leafL, Leaf _leafR)
    {
        Hallway hallway = new Hallway(_leafL.room, _leafR.room); // Hallway to return
        Vector2 roomMinL, roomMinR;
        Vector2 roomMaxL, roomMaxR;
        bool adjacentX = false; // If rooms are adjacent to each other in X (possible straight hall)
        bool adjacentY = false; // If rooms are adjacent to each other in Y (possible straight hall)
        Vector2 roomLPoint, roomRPoint; // Random points in the room
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
        // TODO: both are true? Only if innacurate tolerance is present
        if (adjacentX || adjacentY) // If there's a possibility of a straight hallway
        {
            hallway.m_halls.Add(CreateAdjacentHall(_leafL.room, _leafR.room, adjacentX, GetIntersectingRange(adjacentY, roomMinL, roomMaxL, roomMinR, roomMaxR)));
        }
        // Else, choose which way to generate the multiple hallways
        else
        {
            // Pick random points in both rooms without restrictions
            roomLPoint = new Vector2(
                Random.Range(_leafL.room.m_position.x - _leafL.room.m_size.x * 0.5f, _leafL.room.m_position.x + _leafL.room.m_size.x * 0.5f),
                Random.Range(_leafL.room.m_position.y - _leafL.room.m_size.y * 0.5f, _leafL.room.m_position.y + _leafL.room.m_size.y * 0.5f));
            roomRPoint = new Vector2(
                Random.Range(_leafR.room.m_position.x - _leafR.room.m_size.x * 0.5f, _leafR.room.m_position.x + _leafR.room.m_size.x * 0.5f),
                Random.Range(_leafR.room.m_position.y - _leafR.room.m_size.y * 0.5f, _leafR.room.m_position.y + _leafR.room.m_size.y * 0.5f));
            // Generate the hallway
            hallway.m_halls = CreateHalls(_leafL.room, _leafR.room, roomLPoint, roomRPoint);
        }

        return hallway;
    }
    // CHECK: non axis aligned hallways
    Hallway.Hall CreateAdjacentHall(Room _roomA, Room _roomB, bool _adjacentX, Vector2 _minMaxAltPosRange) // minMaxRange is for the axis that's NOT adjacent
    {
        Vector2 pointA, pointB;
        Vector2 size;
        if (_adjacentX)
        {
            // Create the y position based on Random.Range
            float yPos = Random.Range(_minMaxAltPosRange.x, _minMaxAltPosRange.y);
            // Create 2 points at the edge of the room 
            pointA = new Vector2(_roomA.m_position.x + _roomA.m_size.x * 0.5f * (_roomA.m_position.x > _roomB.m_position.x ? -1f : 1f), yPos);
            pointB = new Vector2(_roomB.m_position.x + _roomB.m_size.x * 0.5f * (_roomB.m_position.x > _roomA.m_position.x ? -1f : 1f), yPos);
            size = new Vector2(Mathf.Abs(pointA.x - pointB.x), Random.Range(m_hallwayWidthRange.x, m_hallwayWidthRange.y));
        }
        else
        {
            // Create the x position based on Random.Range
            float xPos = Random.Range(_minMaxAltPosRange.x, _minMaxAltPosRange.y);
            // Create 2 points at the edge of the room
            pointA = new Vector2(_roomA.m_position.y + _roomA.m_size.y * 0.5f * (_roomA.m_position.y > _roomB.m_position.y ? -1f : 1f), xPos);
            pointB = new Vector2(_roomB.m_position.y + _roomB.m_size.y * 0.5f * (_roomB.m_position.y > _roomA.m_position.y ? -1f : 1f), xPos);
            size = new Vector2(Random.Range(m_hallwayWidthRange.x, m_hallwayWidthRange.y), Mathf.Abs(pointA.y - pointB.y));
        }
        Random.Range(m_hallwayWidthRange.x, m_hallwayWidthRange.y);
        return new Hallway.Hall(
            pointA + (pointB - pointA).normalized * 0.5f,  // Position of hall
            size,   // Size of the hall
            _roomA, _roomB);
    }

    // TODO: more random directions
    List<Hallway.Hall> CreateHalls(Room _roomA, Room _roomB, Vector2 _pointA, Vector2 _pointB)
    {
        var hallway = new List<Hallway.Hall>();
        bool seekX = true;
        // First, determine if going vertically or horizontally from pointA
        //seekX = Random.value > 0.5f;
        if (seekX)
        {
            // Create 1 hall stretching through x axis
            Hallway.Hall hall1 = new Hallway.Hall(new Vector2(_pointA.x + Mathf.Abs(_pointB.x - _pointA.x) * 0.5f, _pointA.y), 
                new Vector2(Mathf.Abs(_pointB.x - _pointA.x), m_hallwayWidthRange.y), _roomA, _roomB);
            // Now create another hall stretching through the y axis
            Hallway.Hall hall2 = new Hallway.Hall(new Vector2(_pointB.x, _pointA.y + Mathf.Abs(_pointB.y - _pointA.y) * 0.5f),
                new Vector2(m_hallwayWidthRange.x, Mathf.Abs(_pointB.y - _pointA.y)), _roomA, _roomB);
            hallway.Add(hall1);
            hallway.Add(hall2);
        }
        //else
        //{

        //}
        return hallway;
    }
    // Get the intersecting range based on parameters. Procedure is to get min of highest point and max of lowest point
    Vector2 GetIntersectingRange(bool _rangeAxisX, Vector2 _roomAMin, Vector2 _roomAMax, Vector2 _roomBMin, Vector2 _roomBMax)
    {
        if (_rangeAxisX)    // Find the difference in X axis
            return new Vector2(Mathf.Max(_roomAMin.x, _roomBMin.x), Mathf.Min(_roomAMax.x, _roomAMax.x));
        // Find difference in y instead
        else
            return new Vector2(Mathf.Max(_roomAMin.y, _roomBMin.y), Mathf.Min(_roomAMax.y, _roomAMax.y));
    }
}
