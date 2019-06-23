using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallwayGenerator
{
    float m_maxDimension = 1f;
    Vector2 m_hallwayWidthRange = new Vector2(10f, 6f); // Use this to generate more hallway variations

    // From the second highest level leaf, unify the 2 children together, if any and continue upwards to the heavens
    public List<Hallway> D2GenerateHallways(ref List<Leaf> _leaves)
    {
        int currentIndex = _leaves.Count - 1;
        int currentLevel = _leaves[currentIndex].level; // This is the last level in the holy lands
        var hallPointList = new List<List<HallPoint>>();
        --currentLevel; // Second last leaf required as last leaves do not have children for the blood sacrifice
        while (0 <= currentLevel)
        {
            var leavesList = new List<Leaf>(); // The chosen ones for this iteration
            // Retrieve the chosen ones in the current layer, and update the currentIndex where the lord demands it
            while (currentLevel <= _leaves[currentIndex].level)
            {
                if (_leaves[currentIndex].level == currentLevel)
                    leavesList.Add(_leaves[currentIndex--]);
                else
                    --currentIndex;
                if (currentIndex < 0)
                    break;
            }
            // Now connect the rooms in left and right children (if any) to unify them
            foreach (Leaf leaf in leavesList)
            {
                if (null == leaf.leftChild && null == leaf.rightChild) // Means that this leaf has a room and no children
                    continue;
                hallPointList.Add(GenerateHallPoints(leaf));
            }
            --currentLevel;
        }
        // Generate hall points first
        return GenerateHalls(hallPointList);
    }
    /// <summary>
    /// Creates hallpoints between the 2 sets of room(s) that are both children of given leaf
    /// if child has more than 1 room, simply pick 1 of the rooms or their halls to connect to
    /// </summary>
    /// <param name="_leaf">Specified leaf that has 2 children</param>
    /// <returns></returns>
    List<HallPoint> GenerateHallPoints(Leaf _leaf)
    {
        // We need to get every room and hallway under this leaf
        Leaf leftChild = _leaf.leftChild;
        Leaf rightChild = _leaf.rightChild;
        var leftHalls = new List<HallPoint>();
        var rightHalls = new List<HallPoint>();
        List<Room> leftRooms;
        List<Room> rightRooms;
        List<HallPoint> hallPoints = new List<HallPoint>();
        bool onXAxis;
        // Find which direction were the leaves split
        onXAxis = leftChild.x != rightChild.x;
        // Get the rooms in this leaf
        leftRooms = R_GetRooms(_leaf.leftChild);
        rightRooms = R_GetRooms(_leaf.rightChild);
        // Determine min and max boundaries of both rooms
        Vector2 leftBoundaries = GetBoundaries(leftRooms, onXAxis);
        Vector2 rightBoundaries = GetBoundaries(rightRooms, onXAxis);
        // TODO: Check if boundaries don't meet at all (RARE CASE)
        if (Mathf.Min(leftBoundaries.y, rightBoundaries.y) - Mathf.Max(leftBoundaries.x, rightBoundaries.x) < m_maxDimension)
        {
            Debug.LogError("THE BOUNDARIES ARE OPEN");
        }
        // Pick a random point that lies in both boundaries
        float randomPoint = Random.Range(Mathf.Max(leftBoundaries.x, rightBoundaries.x) + m_maxDimension * 0.5f,
            Mathf.Min(leftBoundaries.y, rightBoundaries.y) - m_maxDimension * 0.5f);
        // Determine the possible rooms hit, pick the closest room 
        
        // Pick the closest hallway instead if no rooms
        // Generate a straight hallway from leaf to leaf
        hallPoints.Add(pointA);
        hallPoints.Add(pointB);
        return hallPoints;
    }
    // Recursive function gets a list of all the rooms in leaf
    List<Room> R_GetRooms(Leaf _leaf)
    {
        List<Room> finalList;
        finalList = new List<Room>();
        if (null != _leaf.room)
        {
            finalList.Add(_leaf.room);
            return finalList;
        }
        List<Room> listL, listR;
        listL = listR = null;
        if (null != _leaf.leftChild)
            listL = R_GetRooms(_leaf.leftChild);
        if (null != _leaf.rightChild)
            listR = R_GetRooms(_leaf.rightChild);
        if (null != listL)
            finalList.AddRange(listL);
        if (null != listR)
            finalList.AddRange(listR);
        return finalList;
    }

    Vector2 GetBoundaries(List<Room> _rooms, bool _onXAxis)
    {
        Vector2 boundaries;
        if (_onXAxis)
            boundaries = new Vector2(_rooms[0]._position.x, _rooms[0]._position.x);
        else
            boundaries = new Vector2(_rooms[0]._position.z, _rooms[0]._position.z);
        foreach (Room room in _rooms)
        {
            if (_onXAxis)
            {
                if (room._position.x - room.m_size.x * 0.5f < boundaries.x)
                    boundaries.x = room._position.x - room.m_size.x * 0.5f;
                if (room._position.x + room.m_size.x * 0.5f > boundaries.y)
                    boundaries.y = room._position.x + room.m_size.x * 0.5f;
            }
            else
            {
                if (room._position.z - room.m_size.y * 0.5f < boundaries.x)
                    boundaries.x = room._position.z - room.m_size.y * 0.5f;
                if (room._position.z + room.m_size.y * 0.5f > boundaries.y)
                    boundaries.y = room._position.z + room.m_size.y * 0.5f;
            }
        }
        return boundaries;
    }

    List<Room> GetRoomsInBoundaries(Vector2 _boundaries, List<Room> _rooms, bool _onXAxis)
    {
        // Rooms in boundaries have to collide and provide enough space for hall
        foreach(Room room in _rooms)
        {

        }
    }

    List<Hallway> GenerateHalls(List<List<HallPoint>> _hallPointList)
    {
        var hallways = new List<Hallway>();
        foreach (List<HallPoint> hallPoints in _hallPointList)
        {
            var hallway = new Hallway(null, null);
            var hall = new Hallway.Hall(new Vector3(hallPoints[0].position.x, 0f, hallPoints[0].position.z),
                new Vector2(1f, 1f), null, null);
            hallway.m_halls.Add(hall);
            hallways.Add(hallway);
        }
        return hallways;
    }
    public List<Hallway> D1_GenerateHallways(List<Leaf> _leaves)
    {
        var hallways = new List<Hallway>();
        var rooms = new List<Room>();
        // Get all the rooms
        foreach (Leaf leaf in _leaves)
        {
            if (null == leaf.room)
                continue;
            rooms.Add(leaf.room);
        }
        // For every leaf that has children, create hallways connecting the 2 children
        int count = 0;
        //foreach (Leaf leaf in _leaves)
        //{
        //    ++count;
        //    if (leaf.leftChild != null && leaf.rightChild != null)
        //    {
        //        Leaf leafA = leaf.leftChild;
        //        Leaf leafB = leaf.rightChild;
        //        if (leafA != leafB)
        //        {
        //            List<Hallway> newHallways = new List<Hallway>();
        //            foreach (Hallway hallway in newHallways)
        //            {
        //                hallways.Add(hallway);
        //            }
        //        }
        //        else
        //            Debug.Log("2 leaves are the same!");
        //    }
        //}
        
        Debug.Log("count of hall iterations is " + hallways.Count + "| count of iterations is " + count);
        return hallways;
    }

    bool GetLeavesByLayer(int layer, ref List<Leaf> _fetchedLeaves)
    {
        return true;
    }
}

//    public List<Hallway> D2_GenerateHallways(List<Leaf> _leaves, List<Room> _rooms)
//    {
//        var hallways = new List<Hallway>();
//        // For every leaf that has children, create hallways connecting the 2 children
//        int count = 0;
//        foreach (Leaf leaf in _leaves)
//        {
//            if (leaf.leftChild != null && leaf.rightChild != null)
//            {
//                ++count;
//                Leaf leafA = leaf.leftChild;
//                Leaf leafB = leaf.rightChild;
//                if (leafA != leafB)
//                {
//                    hallways.Add(DCreateHallway2(leafA.GetLowestLeaf(), leafB.GetLowestLeaf()));

//                }
//                else
//                    Debug.Log("2 leaves are the same!");
//            }
//        }
//        Debug.Log("count of hall iterations is " + hallways.Count + "| count of iterations is " + count);
//        return hallways;
//    }

//    // TODO: Add improvement on variations of hallways. Curved options, if possible
//    Hallway DCreateHallway1(Leaf _leafA, Leaf _leafB)
//    {
//        Hallway hallway = new Hallway(_leafA.room, _leafB.room); // Hallway to return
//        Room roomA = _leafA.room;
//        Room roomB = _leafB.room;
//        Vector2 roomMinL, roomMinR;
//        Vector2 roomMaxL, roomMaxR;

//        // Pick random points within the 2 rooms
//        Vector2 pointA = new Vector2(
//            roomA._position.x + Random.Range(-0.5f * roomA.m_size.x, 0.5f * roomA.m_size.x),
//            roomA._position.z + Random.Range(-0.5f * roomA.m_size.y, 0.5f * roomA.m_size.y));
//        Vector2 pointB = new Vector2(
//            roomB._position.x + Random.Range(-0.5f * roomB.m_size.x, 0.5f * roomB.m_size.x),
//            roomB._position.z + Random.Range(-0.5f * roomB.m_size.y, 0.5f * roomB.m_size.y));
//        // This allows us to get RAW hallways that are not culled and has no intersection or y axis checks
//        hallway.m_halls = CreateHalls(roomA, roomB, pointA, pointB);
//        // Cull and split halls that collides with other rooms

//        return hallway;
//    }
//    // Create hallway using points instead of layout with related room culling
//    Hallway DCreateHallway2(Leaf _leafA, Leaf _leafB)
//    {
//        Hallway hallway = new Hallway(_leafA.room, _leafB.room); // Hallway to return
//        List<HallPoint> hallPoints;
//        Room roomA = _leafA.room;
//        Room roomB = _leafB.room;
//        Vector2 roomMinL, roomMinR;
//        Vector2 roomMaxL, roomMaxR;

//        hallPoints = GenerateHallPoints(roomA, roomB); // Generate hall points
//        hallPoints = CullPointExcess(hallPoints); // Cull and split halls that collides with other rooms
//        // Now Settle for collision with other rooms and Y axis generation
//        hallway.m_halls = CreateHallsFromPoints(hallPoints); // Generate the halls

//        return hallway;
//    }
//    // Create hallway using points instead of layout with both related and unrelated room culling
//    List<Hallway> DCreateHallway3(Leaf _leafA, Leaf _leafB, List<Room> _rooms)
//    {
//        var hallwayList = new List<Hallway>();
//        List<HallPoint> hallPoints;
//        Room roomA = _leafA.room;
//        Room roomB = _leafB.room;
//        Vector2 roomMinL, roomMinR;
//        Vector2 roomMaxL, roomMaxR;

//        hallPoints = GenerateHallPoints(roomA, roomB); // Generate hall points
//        hallPoints = CullPointExcess(hallPoints); // Cull and split halls that collides with other rooms
//        // Now Settle for collision with other rooms and Y axis generation
//        List<List<HallPoint>> newHallPoints = PolishHallPoints(hallPoints, _rooms);
//        foreach (List<HallPoint> hpIndex in newHallPoints)
//        {
//            var newHallway = new Hallway(hpIndex[0].roomA, hpIndex[0].roomB); // Set the room info for hall
//            newHallway.m_halls = CreateHallsFromPoints(hpIndex); // Generate the halls
//            hallwayList.Add(newHallway); // Add hall to the list to return
//        }
//        //var newHallway = new Hallway(newHallPoints[0][0].roomA, newHallPoints[0][0].roomB); // Set the room info for hall
//        //newHallway.m_halls = CreateHallsFromPoints(hallPoints); // Generate the halls
//        //hallwayList.Add(newHallway); // Add hall to the list to return
//        return hallwayList;
//    }
//    List<HallPoint> GenerateHallPoints(Room _roomA, Room _roomB)
//    {
//        var hallPoints = new List<HallPoint>();

//        // Pick random points within the 2 rooms
//        Vector2 pointA = new Vector2(
//            _roomA._position.x + Random.Range(-0.5f * _roomA.m_size.x, 0.5f * _roomA.m_size.x),
//            _roomA._position.z + Random.Range(-0.5f * _roomA.m_size.y, 0.5f * _roomA.m_size.y));
//        Vector2 pointB = new Vector2(
//            _roomB._position.x + Random.Range(-0.5f * _roomB.m_size.x, 0.5f * _roomB.m_size.x),
//            _roomB._position.z + Random.Range(-0.5f * _roomB.m_size.y, 0.5f * _roomB.m_size.y));
//        // Find the linking point between point A and B (this point is the corner of the 90 degree turning point)
//        Vector2 pointC = new Vector2(pointB.x, pointA.y);
//        // Create 2 hall points for the random points
//        HallPoint hallPointA = new HallPoint(new Vector3(pointA.x, 0f, pointA.y), HallPoint.Type.END_A, _roomA, _roomB);
//        HallPoint hallPointB = new HallPoint(new Vector3(pointB.x, 0f, pointB.y), HallPoint.Type.END_B, _roomA, _roomB);
//        // Now create the corner node and determine if it's within a room
//        HallPoint hallPointC = new HallPoint(new Vector3(pointC.x, 0f, pointC.y), HallPoint.Type.TURN_LINK, _roomA, _roomB);
//        // Add the points
//        hallPoints.Add(hallPointA);
//        hallPoints.Add(hallPointB);
//        hallPoints.Add(hallPointC);
//        return hallPoints;
//    }
//    // The first and second elements of hall points MUST be END_A and END_B respectively
//    List<HallPoint> CullPointExcess(List<HallPoint> _hallPoints)
//    {
//        // Identify the 2 rooms, endpoint A and B
//        Room roomA = _hallPoints[0].roomA;
//        Room roomB = _hallPoints[0].roomB;
//        HallPoint pointA = null;
//        HallPoint pointB = null;
//        HallPoint pointC = null;
//        foreach (HallPoint point in _hallPoints)
//        {
//            switch (point.type)
//            {
//                case HallPoint.Type.END_A:
//                    pointA = point;
//                    break;
//                case HallPoint.Type.END_B:
//                    pointB = point;
//                    break;
//                case HallPoint.Type.TURN_LINK:
//                    pointC = point;
//                    break;
//                default:
//                    Debug.LogError("could not find any suitable type");
//                    break;
//            }
//        }
//        //Debug.Log("the state of points is not " + (HallPoint.Type.END_A != pointA.type || HallPoint.Type.END_B != pointB.type));
//        // Check if the link is within any of the known rooms. If it is, replace that room's end point with the link
//        if (Mathf.Abs(pointC.position.x - roomA._position.x) <= roomA.m_size.x * 0.5f &&
//            Mathf.Abs(pointC.position.z - roomA._position.z) <= roomA.m_size.y * 0.5f)
//        {
//            _hallPoints.Remove(pointA);
//            pointC.type = HallPoint.Type.END_A; // Replacing room A point with link
//            pointA = pointC;    // Update the new pointA
//            pointC = null;
//        }
//        else if (Mathf.Abs(pointC.position.x - roomB._position.x) <= roomB.m_size.x * 0.5f &&
//                    Mathf.Abs(pointC.position.z - roomB._position.z) <= roomB.m_size.y * 0.5f)
//        {
//            _hallPoints.Remove(pointB);
//            pointC.type = HallPoint.Type.END_B; // Replacing room B point with link
//            pointB = pointC;    // Update the new pointB
//            pointC = null;
//        }
//        // Now cull the points that are within the room but not in the boundary
//        // Case 1: a link is betweeen point A and B
//        if (pointC != null)
//        {
//            // POINT A
//            if (pointC.position.x != pointA.position.x) // If the path follows only the X axis
//                pointA.position.x = pointA.roomA._position.x
//                    + pointA.roomA.m_size.x * (pointC.position.x < pointA.position.x ? -0.5f : 0.5f);
//            else if (pointC.position.z != pointA.position.z) // If the path follows only the Z axis
//                pointA.position.z = pointA.roomA._position.z
//                    + pointA.roomA.m_size.y * (pointC.position.z < pointA.position.z ? -0.5f : 0.5f);
//            else
//                Debug.LogError("Points A and C are not aligned");
//            // POINT B
//            if (pointC.position.x != pointB.position.x) // If the path follows only the X axis
//                pointB.position.x = pointB.roomB._position.x
//                    + pointB.roomB.m_size.x * (pointC.position.x < pointB.position.x ? -0.5f : 0.5f);
//            else if (pointC.position.z != pointB.position.z) // If the path follows only the Z axis
//                pointB.position.z = pointB.roomB._position.z
//                    + pointB.roomB.m_size.y * (pointC.position.z < pointB.position.z ? -0.5f : 0.5f);
//            else
//                Debug.LogError("Points A and C are not aligned");
//        }
//        // Case 2: no link between point A and B
//        else
//        {
//            // POINT A
//            if (pointB.position.x != pointA.position.x) // If the path follows only the X axis
//                pointA.position.x = pointA.roomA._position.x
//                    + pointA.roomA.m_size.x * (pointB.position.x < pointA.position.x ? -0.5f : 0.5f);
//            else if (pointB.position.z != pointA.position.z) // If the path follows only the Z axis
//                pointA.position.z = pointA.roomA._position.z
//                    + pointA.roomA.m_size.y * (pointB.position.z < pointA.position.z ? -0.5f : 0.5f);
//            else
//                Debug.LogError("Points A and B are not aligned");
//            // POINT B
//            if (pointA.position.x != pointB.position.x) // If the path follows only the X axis
//                pointB.position.x = pointB.roomB._position.x
//                    + pointB.roomB.m_size.x * (pointA.position.x < pointB.position.x ? -0.5f : 0.5f);
//            else if (pointA.position.z != pointB.position.z) // If the path follows only the Z axis
//                pointB.position.z = pointB.roomB._position.z
//                    + pointB.roomB.m_size.y * (pointA.position.z < pointB.position.z ? -0.5f : 0.5f);
//            else
//                Debug.LogError("Points A and B are not aligned");
//        }
//        return _hallPoints;
//    }

//    List<Hallway.Hall> CreateHallsFromPoints(List<HallPoint> _points)
//    {
//        HallPoint pointA = null;
//        HallPoint pointB = null;
//        HallPoint pointC = null;
//        var hallways = new List<Hallway.Hall>();
//        // Assign the points to the correct points on the list
//        foreach(HallPoint point in _points)
//        {
//            switch(point.type)
//            {
//                case HallPoint.Type.END_A:
//                    pointA = point;
//                    break;
//                case HallPoint.Type.END_B:
//                    pointB = point;
//                    break;
//                case HallPoint.Type.TURN_LINK:
//                    pointC = point;
//                    break;
//            }
//        }
//        // Straight halls
//        if (pointC == null)
//        {
//            if (pointA.position.x != pointB.position.x && pointA.position.z != pointB.position.z)
//                Debug.Log("What the fuck");
//            hallways.Add(new Hallway.Hall(new Vector2(
//                pointA.position.x + (pointB.position.x - pointA.position.x) * 0.5f,
//                pointA.position.z + (pointB.position.z - pointA.position.z) * 0.5f),
//                //new Vector2(0f, 0f),
//                new Vector2(Mathf.Max(Mathf.Abs(pointA.position.x - pointB.position.x), 1f), Mathf.Max(Mathf.Abs(pointA.position.z - pointB.position.z), 1f)),
//                pointA.roomA, pointA.roomB)); // Add a hall from pointA to pointB only
//            hallways.Add(new Hallway.Hall(new Vector2(
//                pointA.position.x,
//                pointA.position.z),
//                new Vector2(0.5f, 0.5f),
//                //new Vector2(Mathf.Max(Mathf.Abs(pointA.position.x - pointB.position.x), 1f), Mathf.Max(Mathf.Abs(pointA.position.z - pointB.position.z), 1f)),
//                pointA.roomA, pointA.roomB)); // Add a hall from pointA to pointB only
//            hallways.Add(new Hallway.Hall(new Vector2(
//                pointB.position.x,
//                pointB.position.z),
//                new Vector2(0.5f, 0.5f),
//                //new Vector2(Mathf.Max(Mathf.Abs(pointA.position.x - pointB.position.x), 1f), Mathf.Max(Mathf.Abs(pointA.position.z - pointB.position.z), 1f)),
//                pointA.roomA, pointA.roomB)); // Add a hall from pointA to pointB only
//        }
//        // Bent halls require 2 halls instead
//        else
//        {
//            hallways.Add(new Hallway.Hall(new Vector2(
//                pointA.position.x + (pointC.position.x - pointA.position.x) * 0.5f,
//                pointA.position.z + (pointC.position.z - pointA.position.z) * 0.5f),
//                new Vector2(Mathf.Max(Mathf.Abs(pointC.position.x - pointA.position.x), 1f), Mathf.Max(Mathf.Abs(pointC.position.z - pointA.position.z), 1f)),
//                //new Vector2(0f, 0f),
//                pointA.roomA, pointA.roomB)); // Add a hall from pointA to point C
//            hallways.Add(new Hallway.Hall(new Vector2(
//                pointB.position.x + (pointC.position.x - pointB.position.x) * 0.5f,
//                pointB.position.z + (pointC.position.z - pointB.position.z) * 0.5f),
//                new Vector2(Mathf.Max(Mathf.Abs(pointC.position.x - pointB.position.x), 1f), Mathf.Max(Mathf.Abs(pointC.position.z - pointB.position.z), 1f)),
//                //new Vector2(0f, 0f),
//                pointB.roomA, pointB.roomB)); // Add a hall from pointB to point C
//            //hallways.Add(new Hallway.Hall(new Vector2(
//            //    5000f,
//            //    5000f),
//            //    new Vector2(1f, 1f),
//            //    pointA.roomA, pointA.roomB)); // Add a hall from pointA to point C
//        }
//        return hallways;
//    }
//    // Takes the list of hallways and deals with the second last biggest problem: collision of halls into unrelated rooms
//    List<List<HallPoint>> PolishHallPoints(List<HallPoint> _hallPoints, List<Room> _rooms)
//    {
//        //var test = new List<List<HallPoint>>();
//        //test.Add(_hallPoints);
//        //return test;
//        var hallPointsList = new List<List<HallPoint>>(); // The polished hallways to return
//        HallPoint pointA, pointB, pointC;
//        pointA = pointB = pointC = null;
//        foreach(HallPoint hallPoint in _hallPoints)
//        {
//            switch(hallPoint.type)
//            {
//                case HallPoint.Type.END_A:
//                    pointA = hallPoint;
//                    break;
//                case HallPoint.Type.END_B:
//                    pointB = hallPoint;
//                    break;
//                case HallPoint.Type.TURN_LINK:
//                    pointC = hallPoint;
//                    break;
//            }
//        }
//        // Case 1: Check rooms between point A and point B and split the hallways
//        if (null == pointC)
//        {
//            // Get ALL the rooms that this hallway collides with (AXIS ALIGNED)
//            var collidingRooms = new List<Room>();
//            bool xAxis = pointA.position.z == pointB.position.z; // Direction of hallway
//            foreach(Room room in _rooms)
//            {
//                if (xAxis)
//                {
//                    if (room._position.z - room.m_size.y * 0.5f < pointA.position.z &&
//                        room._position.z + room.m_size.y * 0.5f > pointA.position.z &&
//                        room._position.x > Mathf.Min(pointA.position.x, pointB.position.x) &&
//                        room._position.x < Mathf.Max(pointA.position.x, pointB.position.x))
//                        collidingRooms.Add(room);
//                }
//                else if(room._position.x - room.m_size.x * 0.5f < pointA.position.x &&
//                        room._position.x + room.m_size.x * 0.5f > pointA.position.x &&
//                        room._position.z > Mathf.Min(pointA.position.z, pointB.position.z) &&
//                        room._position.z < Mathf.Max(pointA.position.z, pointB.position.z))
//                        collidingRooms.Add(room);
//            }
//            // Now split the hallways that collide with the rooms, starting with the closest room to A
//            while(0 != collidingRooms.Count)
//            {
//                Room closest = null;
//                foreach (Room room in collidingRooms)   // Getting the closest room
//                {
//                    if (null == closest)
//                    {
//                        closest = room;
//                        continue;
//                    }
//                    else if (xAxis)
//                    {
//                        if (Mathf.Abs(room._position.x - pointA.position.x) < Mathf.Abs(closest._position.x - pointA.position.x))
//                            closest = room;
//                    }
//                    else
//                    {
//                        if (Mathf.Abs(room._position.z - pointA.position.z) < Mathf.Abs(closest._position.z - pointA.position.z))
//                            closest = room;
//                    }
//                }
//                // Add a shortened hallway from pointA to closest room
//                pointA.roomB = closest;
//                HallPoint newPointB = new HallPoint(new Vector3(0f, 0f, 0f), HallPoint.Type.END_B, pointA.roomA, closest);
//                newPointB.position.x = xAxis ? pointA.position.x + 
//                    ((closest._position.x + closest.m_size.x * (closest._position.x < pointA.position.x ? 0.5f : -0.5f)) - pointA.position.x) : pointA.position.x;    // X axis here
//                newPointB.position.z = xAxis ? pointA.position.z : pointA.position.z + 
//                    (closest._position.z + closest.m_size.y * (closest._position.z < pointA.position.z ? 0.5f : -0.5f) - pointA.position.z);
//                // Add the points into the hallpoint list
//                var newHallPoints = new List<HallPoint>();
//                newHallPoints.Add(pointA);
//                newHallPoints.Add(newPointB);
//                hallPointsList.Add(newHallPoints);
//                // Update the new pointA past the closest room
//                HallPoint newPointA = new HallPoint(new Vector3(0f, 0f, 0f), HallPoint.Type.END_A, closest, null);
//                newPointA.position.x = xAxis ? closest._position.x + closest.m_size.x * (closest._position.x < pointA.position.x ? -0.5f : 0.5f) : pointA.position.x;
//                newPointA.position.z = xAxis ? pointA.position.z : closest._position.z + closest.m_size.y * (closest._position.z < pointA.position.z ? -0.5f : 0.5f);
//                pointA = null;
//                pointA = newPointA; // Update the new pointA
//                if (1 == collidingRooms.Count)
//                {
//                    newHallPoints = new List<HallPoint>();
//                    pointA.roomB = pointB.roomB; // Update roomB for pointA
//                    newHallPoints.Add(pointA); // Settle the linking to the final room
//                    newHallPoints.Add(pointB);
//                    hallPointsList.Add(newHallPoints);
//                }
//                collidingRooms.Remove(closest);
//            }
//        }
//        // Case 2: Check from point A to point C and point B to point C
//        else
//        {
//            hallPointsList.Add(_hallPoints);
//            return hallPointsList;
//            var collidingRooms = new List<Room>();
//            bool xAxis = pointA.position.z == pointC.position.z;
//            bool finishedCorner = false;
//            bool polished = false;
//            List<HallPoint> newHallPoints = null;
//            while (!polished)
//            {
//                // Get colliding rooms from point A to point C
//                foreach (Room room in _rooms)
//                {
//                    if (xAxis)
//                    {
//                        if (room._position.z - room.m_size.y * 0.5f < pointA.position.z &&
//                            room._position.z + room.m_size.y * 0.5f > pointA.position.z &&
//                            room._position.x > Mathf.Min(pointA.position.x, pointC.position.x) &&
//                            room._position.x < Mathf.Max(pointA.position.x, pointC.position.x))
//                            collidingRooms.Add(room);
//                    }
//                    else if (room._position.x - room.m_size.x * 0.5f < pointA.position.x &&
//                            room._position.x + room.m_size.x * 0.5f > pointA.position.x &&
//                            room._position.z > Mathf.Min(pointA.position.z, pointC.position.z) &&
//                            room._position.z < Mathf.Max(pointA.position.z, pointC.position.z))
//                        collidingRooms.Add(room);
//                    if (0 == collidingRooms.Count)
//                    {
//                    // Case 1: rooms empty and finishedCorner is false, make straight path from pointA to pointC PASS
//                        if (!finishedCorner)
//                        {
//                            newHallPoints = new List<HallPoint>();
//                            newHallPoints.Add(pointA);
//                            newHallPoints.Add(pointC);
//                            finishedCorner = true;
//                        }
//                    // Case 2: rooms empty and finishedCorner is true
//                        else
//                        {
//                            if (null == newHallPoints)
//                            {
//                                newHallPoints = new List<HallPoint>();
//                                newHallPoints.Add(pointA);
//                            }
//                            newHallPoints.Add(pointB);
//                            hallPointsList.Add(newHallPoints);
//                            polished = true;
//                        }
//                    }
//                }
//                while (0 != collidingRooms.Count)
//                {
//                    HallPoint newPointB;
//                    Room closest = null;
//                    if (null == newHallPoints)
//                        newHallPoints = new List<HallPoint>();
//                    // Get closest room
//                    foreach (Room room in collidingRooms)
//                    {
//                        if (null == closest)
//                        {
//                            closest = room;
//                            continue;
//                        }
//                        else if (xAxis)
//                        {
//                            if (Mathf.Abs(room._position.x - pointA.position.x) < Mathf.Abs(closest._position.x - pointA.position.x))
//                                closest = room;
//                        }
//                        else
//                        {
//                            if (Mathf.Abs(room._position.z - pointA.position.z) < Mathf.Abs(closest._position.z - pointA.position.z))
//                                closest = room;
//                        }
//                    }
//                    // Case 1: Corner is within closest room (only for last one)
//                    if (1 == collidingRooms.Count && !finishedCorner &&
//                        xAxis ? (Mathf.Abs(pointC.position.x - closest._position.x) < closest.m_size.x * 0.5f) : (Mathf.Abs(pointC.position.z - closest._position.z) < closest.m_size.y * 0.5f))
//                    {
//                        // Create a pointB, add halls and set a new point A
//                        newPointB = new HallPoint(new Vector3(0f, 0f, 0f), HallPoint.Type.END_B, pointA.roomA, closest);
//                        newHallPoints.Add(pointA);
//                        newHallPoints.Add(newPointB);
//                        hallPointsList.Add(newHallPoints);
//                        newHallPoints = null;
//                        pointA = new HallPoint(new Vector3(0f, 0f, 0f), HallPoint.Type.END_A, closest, pointB.roomB);
//                        pointA.position.Set(pointC.position.x + closest.m_size.x * (pointB.position.x == pointC.position.x ? 0f : pointB.position.x < pointC.position.x ? -0.5f : 0.5f)
//                            , pointC.position.y, pointC.position.z + closest.m_size.y * (pointB.position.z == pointC.position.z ? 0f : pointB.position.z < pointC.position.z? -0.5f : 0.5f));
//                        collidingRooms.Remove(closest);
//                        finishedCorner = true;
//                        xAxis = !xAxis; // THE HOLY TRINITY
//                        continue;
//                    }

//                    // Create new pointB, add A and B to newHallPoints and set a new pointA
//                    pointA.roomB = closest;
//                    newPointB = new HallPoint(new Vector3(0f, 0f, 0f), HallPoint.Type.END_B, pointA.roomA);
//                    newPointB.position.Set(xAxis ? pointA.position.x +
//                        ((closest._position.x + closest.m_size.x * (closest._position.x < pointA.position.x ? 0.5f : -0.5f)) - pointA.position.x) : pointA.position.x, 0f,
//                        xAxis ? pointA.position.z : closest._position.z + closest.m_size.y * (closest._position.z < pointA.position.z ? -0.5f : 0.5f));
//                    if (newHallPoints.Count == 0)
//                    {
//                        newHallPoints.Add(pointA);
//                    }
//                    else
//                    {
//                        foreach(HallPoint pt in newHallPoints)
//                        {
//                            pt.roomB = closest;
//                        }
//                    }
//                    newHallPoints.Add(newPointB);
//                    hallPointsList.Add(newHallPoints);
//                    newHallPoints = null;
//                    var newPointA = new HallPoint(new Vector3(0f, 0f, 0f), HallPoint.Type.END_A, closest, pointB.roomB);
//                    newPointA.position.Set(xAxis ? closest._position.x + closest.m_size.x * (closest._position.x < pointA.position.x ? -0.5f : 0.5f) : pointA.position.x,
//                        0f, xAxis ? pointA.position.z : closest._position.z + closest.m_size.y * (closest._position.z < pointA.position.z ? -0.5f : 0.5f));
//                    pointA = null;
//                    pointA = newPointA;
//                    if (collidingRooms.Count == 1)
//                    {
//                    // Case 3: approaching pointC 
//                        if (!finishedCorner)
//                        {
//                            // Create one last hallway, set the new pointA
//                            newHallPoints = new List<HallPoint>();
//                            newHallPoints.Add(pointA);
//                            newHallPoints.Add(pointC);
//                            pointA = new HallPoint(new Vector3(0f, 0f, 0f), HallPoint.Type.TURN_LINK, closest, pointC.roomB);
//                            pointA.position = pointC.position;
//                            finishedCorner = true;
//                            xAxis = !xAxis; // THE HOLY TRINITY
//                        }
//                    // Case 4: approaching targetPoint and at last room (last one)
//                    // Case 2: between pointA and targetPoint
//                        else
//                        {
//                            // Link new point A with pointB
//                            newHallPoints = new List<HallPoint>();
//                            newHallPoints.Add(pointA);
//                            newHallPoints.Add(pointB);
//                            hallPointsList.Add(newHallPoints);
//                            polished = true;
//                        }
//                    }
//                    collidingRooms.Remove(closest);
//                }
//            }
//        }
//        if (hallPointsList.Count == 0)
//            hallPointsList.Add(_hallPoints);
//        return hallPointsList;
//    }

//    // !Deprecated prototyping function to create hallways using rectangles. Has limitations and ineffective in determining collisions early
//    List<Hallway.Hall> CreateHalls(Room _roomA, Room _roomB, Vector2 _pointA, Vector2 _pointB)
//    {
//        var halls = new List<Hallway.Hall>();
//        // TODO: customizable width
//        Hallway.Hall hallX = new Hallway.Hall(
//            new Vector2(_pointA.x + (_pointB.x - _pointA.x) * 0.5f, _pointA.y),
//            new Vector2(Mathf.Abs(_pointB.x - _pointA.x), 1f), _roomA, _roomB);
//        Hallway.Hall hallY = new Hallway.Hall(
//            new Vector2(_pointB.x, _pointB.y + (_pointA.y - _pointB.y) * 0.5f),
//            new Vector2(1f, Mathf.Abs(_pointA.y - _pointB.y)), _roomA, _roomB);
//        halls.Add(hallX);
//        halls.Add(hallY);
//        return halls;
//    }
//}
