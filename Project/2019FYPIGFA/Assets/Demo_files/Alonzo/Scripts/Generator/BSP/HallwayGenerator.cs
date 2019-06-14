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
                {
                    hallways.Add(DCreateHallway2(leafA.GetLowestLeaf(), leafB.GetLowestLeaf()));

                }
                else
                    Debug.Log("2 leaves are the same!");
            }
        }
        Debug.Log("count of hall iterations is " + hallways.Count + "| count of iterations is " + count);
        return hallways;
    }

    // TODO: Add improvement on variations of hallways. Curved options, if possible
    // TODO: Add a auto snapping behaviour to avoid bad meshes
    Hallway DCreateHallway1(Leaf _leafA, Leaf _leafB)
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
    // Create hallway using points instead of layout without culling
    Hallway DCreateHallway2(Leaf _leafA, Leaf _leafB)
    {
        Hallway hallway = new Hallway(_leafA.room, _leafB.room); // Hallway to return
        List<HallPoint> hallPoints;
        Room roomA = _leafA.room;
        Room roomB = _leafB.room;
        Vector2 roomMinL, roomMinR;
        Vector2 roomMaxL, roomMaxR;

        hallPoints = GenerateHallPoints(roomA, roomB); // Generate hall points
        hallPoints = CullPointExcess(hallPoints); // Cull and split halls that collides with other rooms
        // Now Settle for collision with other rooms and Y axis generation
        hallway.m_halls = CreateHallsFromPoints(hallPoints); // Generate the halls

        return hallway;
    }

    List<HallPoint> GenerateHallPoints(Room _roomA, Room _roomB)
    {
        var hallPoints = new List<HallPoint>();

        // Pick random points within the 2 rooms
        Vector2 pointA = new Vector2(
            _roomA.m_position.x + Random.Range(-0.5f * _roomA.m_size.x, 0.5f * _roomA.m_size.x),
            _roomA.m_position.y + Random.Range(-0.5f * _roomA.m_size.y, 0.5f * _roomA.m_size.y));
        Vector2 pointB = new Vector2(
            _roomB.m_position.x + Random.Range(-0.5f * _roomB.m_size.x, 0.5f * _roomB.m_size.x),
            _roomB.m_position.y + Random.Range(-0.5f * _roomB.m_size.y, 0.5f * _roomB.m_size.y));
        // Find the linking point between point A and B (this point is the corner of the 90 degree turning point)
        Vector2 pointC = new Vector2(pointB.x, pointA.y);
        // Create 2 hall points for the random points
        HallPoint hallPointA = new HallPoint(new Vector3(pointA.x, 0f, pointA.y), HallPoint.Type.END_A, _roomA, _roomB);
        HallPoint hallPointB = new HallPoint(new Vector3(pointB.x, 0f, pointB.y), HallPoint.Type.END_B, _roomA, _roomB);
        // Now create the corner node and determine if it's within a room
        HallPoint hallPointC = new HallPoint(new Vector3(pointC.x, 0f, pointC.y), HallPoint.Type.TURN_LINK, _roomA, _roomB);
        // Add the points
        hallPoints.Add(hallPointA);
        hallPoints.Add(hallPointB);
        hallPoints.Add(hallPointC);
        return hallPoints;
    }
    // The first and second elements of hall points MUST be END_A and END_B respectively
    List<HallPoint> CullPointExcess(List<HallPoint> _hallPoints)
    {
        // Identify the 2 rooms, endpoint A and B
        Room roomA = _hallPoints[0].roomA;
        Room roomB = _hallPoints[0].roomB;
        HallPoint pointA = null;
        HallPoint pointB = null;
        HallPoint pointC = null;
        foreach (HallPoint point in _hallPoints)
        {
            switch (point.type)
            {
                case HallPoint.Type.END_A:
                    pointA = point;
                    break;
                case HallPoint.Type.END_B:
                    pointB = point;
                    break;
                case HallPoint.Type.TURN_LINK:
                    pointC = point;
                    break;
                default:
                    Debug.LogError("could not find any suitable type");
                    break;
            }
        }
        //Debug.Log("the state of points is not " + (HallPoint.Type.END_A != pointA.type || HallPoint.Type.END_B != pointB.type));
        // Check if the link is within any of the known rooms. If it is, replace that room's end point with the link
        if (Mathf.Abs(pointC.position.x - roomA.m_position.x) <= roomA.m_size.x * 0.5f &&
            Mathf.Abs(pointC.position.z - roomA.m_position.y) <= roomA.m_size.y * 0.5f)
        {
            _hallPoints.Remove(pointA);
            pointC.type = HallPoint.Type.END_A; // Replacing room A point with link
            pointA = pointC;    // Update the new pointA
            pointC = null;
        }
        else if (Mathf.Abs(pointC.position.x - roomB.m_position.x) <= roomB.m_size.x * 0.5f &&
                    Mathf.Abs(pointC.position.z - roomB.m_position.y) <= roomB.m_size.y * 0.5f)
        {
            _hallPoints.Remove(pointB);
            pointC.type = HallPoint.Type.END_B; // Replacing room B point with link
            pointB = pointC;    // Update the new pointB
            pointC = null;
        }
        // Now cull the points that are within the room but not in the boundary
        // Case 1: a link is betweeen point A and B
        if (pointC != null)
        {
            // POINT A
            if (pointC.position.x != pointA.position.x) // If the path follows only the X axis
                pointA.position.x = pointA.roomA.m_position.x
                    + pointA.roomA.m_size.x * (pointC.position.x < pointA.position.x ? -0.5f : 0.5f);
            else if (pointC.position.z != pointA.position.z) // If the path follows only the Z axis
                pointA.position.z = pointA.roomA.m_position.y
                    + pointA.roomA.m_size.y * (pointC.position.z < pointA.position.z ? -0.5f : 0.5f);
            else
                Debug.LogError("Points A and C are not aligned");
            // POINT B
            if (pointC.position.x != pointB.position.x) // If the path follows only the X axis
                pointB.position.x = pointB.roomB.m_position.x
                    + pointB.roomB.m_size.x * (pointC.position.x < pointB.position.x ? -0.5f : 0.5f);
            else if (pointC.position.z != pointB.position.z) // If the path follows only the Z axis
                pointB.position.z = pointB.roomB.m_position.y
                    + pointB.roomB.m_size.y * (pointC.position.z < pointB.position.z ? -0.5f : 0.5f);
            else
                Debug.LogError("Points A and C are not aligned");
        }
        // Case 2: no link between point A and B
        else
        {
            // POINT A
            if (pointB.position.x != pointA.position.x) // If the path follows only the X axis
                pointA.position.x = pointA.roomA.m_position.x
                    + pointA.roomA.m_size.x * (pointB.position.x < pointA.position.x ? -0.5f : 0.5f);
            else if (pointB.position.z != pointA.position.z) // If the path follows only the Z axis
                pointA.position.z = pointA.roomA.m_position.y
                    + pointA.roomA.m_size.y * (pointB.position.z < pointA.position.z ? -0.5f : 0.5f);
            else
                Debug.LogError("Points A and B are not aligned");
            // POINT B
            if (pointA.position.x != pointB.position.x) // If the path follows only the X axis
                pointB.position.x = pointB.roomB.m_position.x
                    + pointB.roomB.m_size.x * (pointA.position.x < pointB.position.x ? -0.5f : 0.5f);
            else if (pointA.position.z != pointB.position.z) // If the path follows only the Z axis
                pointB.position.z = pointB.roomB.m_position.y
                    + pointB.roomB.m_size.y * (pointA.position.z < pointB.position.z ? -0.5f : 0.5f);
            else
                Debug.LogError("Points A and B are not aligned");
        }
        return _hallPoints;
    }

    List<Hallway.Hall> CreateHallsFromPoints(List<HallPoint> _points)
    {
        HallPoint pointA = null;
        HallPoint pointB = null;
        HallPoint pointC = null;
        var hallways = new List<Hallway.Hall>();
        // Assign the points to the correct points on the list
        foreach(HallPoint point in _points)
        {
            switch(point.type)
            {
                case HallPoint.Type.END_A:
                    pointA = point;
                    break;
                case HallPoint.Type.END_B:
                    pointB = point;
                    break;
                case HallPoint.Type.TURN_LINK:
                    pointC = point;
                    break;
            }
        }
        // Straight halls
        if (pointC == null)
        {
            if (pointA.position.x != pointB.position.x && pointA.position.z != pointB.position.z)
                Debug.Log("What the fuck");
            hallways.Add(new Hallway.Hall(new Vector2(
                pointA.position.x + (pointB.position.x - pointA.position.x) * 0.5f,
                pointA.position.z + (pointB.position.z - pointA.position.z) * 0.5f),
                //new Vector2(0f, 0f),
                new Vector2(Mathf.Max(Mathf.Abs(pointA.position.x - pointB.position.x), 1f), Mathf.Max(Mathf.Abs(pointA.position.z - pointB.position.z), 1f)),
                pointA.roomA, pointA.roomB)); // Add a hall from pointA to pointB only
            hallways.Add(new Hallway.Hall(new Vector2(
                pointA.position.x,
                pointA.position.z),
                new Vector2(0.5f, 0.5f),
                //new Vector2(Mathf.Max(Mathf.Abs(pointA.position.x - pointB.position.x), 1f), Mathf.Max(Mathf.Abs(pointA.position.z - pointB.position.z), 1f)),
                pointA.roomA, pointA.roomB)); // Add a hall from pointA to pointB only
            hallways.Add(new Hallway.Hall(new Vector2(
                pointB.position.x,
                pointB.position.z),
                new Vector2(0.5f, 0.5f),
                //new Vector2(Mathf.Max(Mathf.Abs(pointA.position.x - pointB.position.x), 1f), Mathf.Max(Mathf.Abs(pointA.position.z - pointB.position.z), 1f)),
                pointA.roomA, pointA.roomB)); // Add a hall from pointA to pointB only
        }
        // Bent halls require 2 halls instead
        else
        {
            hallways.Add(new Hallway.Hall(new Vector2(
                pointA.position.x + (pointC.position.x - pointA.position.x) * 0.5f,
                pointA.position.z + (pointC.position.z - pointA.position.z) * 0.5f),
                new Vector2(Mathf.Max(Mathf.Abs(pointC.position.x - pointA.position.x), 1f), Mathf.Max(Mathf.Abs(pointC.position.z - pointA.position.z), 1f)),
                //new Vector2(0f, 0f),
                pointA.roomA, pointA.roomB)); // Add a hall from pointA to point C
            hallways.Add(new Hallway.Hall(new Vector2(
                pointB.position.x + (pointC.position.x - pointB.position.x) * 0.5f,
                pointB.position.z + (pointC.position.z - pointB.position.z) * 0.5f),
                new Vector2(Mathf.Max(Mathf.Abs(pointC.position.x - pointB.position.x), 1f), Mathf.Max(Mathf.Abs(pointC.position.z - pointB.position.z), 1f)),
                //new Vector2(0f, 0f),
                pointB.roomA, pointB.roomB)); // Add a hall from pointB to point C
            //hallways.Add(new Hallway.Hall(new Vector2(
            //    5000f,
            //    5000f),
            //    new Vector2(1f, 1f),
            //    pointA.roomA, pointA.roomB)); // Add a hall from pointA to point C
        }
        return hallways;
    }
    // !Deprecated function to create hallways using rectangles. Has limitations and ineffective in determining collisions early
    List<Hallway.Hall> CreateHalls(Room _roomA, Room _roomB, Vector2 _pointA, Vector2 _pointB)
    {
        var halls = new List<Hallway.Hall>();
        // TODO: customizable width
        Hallway.Hall hallX = new Hallway.Hall(
            new Vector2(_pointA.x + (_pointB.x - _pointA.x) * 0.5f, _pointA.y),
            new Vector2(Mathf.Abs(_pointB.x - _pointA.x), 1f), _roomA, _roomB);
        Hallway.Hall hallY = new Hallway.Hall(
            new Vector2(_pointB.x, _pointB.y + (_pointA.y - _pointB.y) * 0.5f),
            new Vector2(1f, Mathf.Abs(_pointA.y - _pointB.y)), _roomA, _roomB);
        halls.Add(hallX);
        halls.Add(hallY);
        return halls;
    }
}
