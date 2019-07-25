using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    /// <summary>
    /// Creates a plane mesh. UV can be automatically calculated based on size.
    /// The plane normal is always facing upwards and there is no depth as it's a plane
    /// </summary>
    /// <param name="_width">The width of the plane</param>
    /// <param name="_height">The height of the plane</param>
    /// <param name="_uvScale">UV scale for texturing based on current dimensions</param>
    /// <param name="_collider">Places a collider on the object. Default true</param>
    /// <returns></returns>
    public static GameObject CreatePlane(float _width, float _height, float _uvScale = 1f, bool _collider = true)
    {
        GameObject go = new GameObject("Plane");
        MeshFilter mf = go.AddComponent(typeof(MeshFilter)) as MeshFilter;
        MeshRenderer mr = go.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        Mesh m = new Mesh();
        m.vertices = new Vector3[]
        {
            new Vector3(-_width * 0.5f, 0, -_height * 0.5f), // Bottom left
            new Vector3(_width * 0.5f , 0, -_height * 0.5f), // Bottom right
            new Vector3(_width * 0.5f , 0, _height * 0.5f), // Top right
            new Vector3(-_width * 0.5f, 0, _height * 0.5f) // Top left
        };

        float maxUV = Mathf.Max(_width, _height);
        m.uv = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(0, _width * _uvScale),
            new Vector2(_height * _uvScale, _width * _uvScale),
            new Vector2(_height * _uvScale, 0)
        };
        m.triangles = new int[] { 2, 1, 0, 3, 2, 0 }; // 2 triangles, diagonal bottom left to top right

        mf.mesh = m;
        if (_collider)
        {
            (go.AddComponent(typeof(MeshCollider)) as MeshCollider).sharedMesh = m;
        }
        m.RecalculateBounds();
        m.RecalculateNormals();
        return go;
    }
    /// <summary>
    /// Creates a wall with a space for a doorway.
    /// UV is also recalculated accordingly for seamless tiling effect
    /// The wall normal is always facing the -Z axis
    /// </summary>
    /// <param name="_width">The overall width of the wall</param>
    /// <param name="_height">The overall height of the door</param>
    /// <param name="_doorWidth">The width of the doorway</param>
    /// <param name="_doorHeight">The height of the doorway</param>
    /// <param name="_doorX">The X position of the door on the wall (minimum 0)</param>
    /// <param name="_uvScale">The UV scale of the wall.</param>
    /// <returns></returns>
    public static GameObject CreateDoorWall(float _width, float _height, float _doorWidth, float _doorHeight, float _doorX, float _uvScale = 1f)
    {
        GameObject go = new GameObject("DoorWall");
        MeshFilter mf = go.AddComponent(typeof(MeshFilter)) as MeshFilter;
        MeshRenderer mr = go.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        Mesh m = new Mesh();

        m.vertices = new Vector3[]
        {
            new Vector3(-_width * 0.5f, 0f, 0f), // 0 - First bottom
            new Vector3(-_width * 0.5f + _doorX - _doorWidth * 0.5f, 0f, 0f), // 1 - Second bottom
            new Vector3(-_width * 0.5f + _doorX + _doorWidth * 0.5f, 0f, 0f), // 2 - Third bottom
            new Vector3(_width * 0.5f, 0f, 0f), // 3 - Fourth bottom
            
            new Vector3(-_width * 0.5f + _doorX - _doorWidth * 0.5f, _doorHeight, 0f), // 4 - First middle
            new Vector3(-_width * 0.5f + _doorX + _doorWidth * 0.5f, _doorHeight, 0f), // 5 - Second middle

            new Vector3(-_width * 0.5f, _height, 0f), // 6 - First top
            new Vector3(-_width * 0.5f + _doorX - _doorWidth * 0.5f, _height, 0f), // 7 - Second top
            new Vector3(-_width * 0.5f + _doorX + _doorWidth * 0.5f, _height, 0f), // 8 - Third top
            new Vector3(_width * 0.5f, _height, 0f) // 9 - Fourth top
        };

        float maxUV = Mathf.Max(_width, _height);
        m.uv = new Vector2[]    // HHAHAhahAHAhahAHhahAHAhaha
        {
            new Vector2(-_width * 0.5f, 0f), // 0 - First bottom
            new Vector2(-_width * 0.5f + _doorX - _doorWidth * 0.5f, 0f), // 1 - Second bottom
            new Vector2(-_width * 0.5f + _doorX + _doorWidth * 0.5f, 0f), // 2 - Third bottom
            new Vector2(_width * 0.5f, 0f), // 3 - Fourth bottom
            
            new Vector2(-_width * 0.5f + _doorX - _doorWidth * 0.5f, _doorHeight), // 4 - First middle
            new Vector2(-_width * 0.5f + _doorX + _doorWidth * 0.5f, _doorHeight), // 5 - Second middle

            new Vector2(-_width * 0.5f, _height), // 6 - First top
            new Vector2(-_width * 0.5f + _doorX - _doorWidth * 0.5f, _height), // 7 - Second top
            new Vector2(-_width * 0.5f + _doorX + _doorWidth * 0.5f, _height), // 8 - Third top
            new Vector2(_width * 0.5f, _height) // 9 - Fourth top
        };
        m.triangles = new int[] {   // 6 triangles, starting with left panel top left to right
            0, 7, 6,    // First triangle
            0, 1, 7,    // Second triangle
            4, 8, 7,    // Third triangle
            4, 5, 8,    // Fourth triangle
            2, 9, 8,    // Fifth triangle
            2, 3, 9     // Sixth triangle
        };
        mf.mesh = m;

        (go.AddComponent(typeof(MeshCollider)) as MeshCollider).sharedMesh = m;
        m.RecalculateBounds();
        m.RecalculateNormals();
        return go;
    }

    public static GameObject CreateStairs(float _stairWidth = 1f, float _stairDepth = 1f, float _stairHeight = 0.5f)
    {
        // Make a plane facing -z
        GameObject go = new GameObject("Plane");
        MeshFilter mf = go.AddComponent(typeof(MeshFilter)) as MeshFilter;
        MeshRenderer mr = go.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        Mesh m = new Mesh();

        m.vertices = new Vector3[]
        {
            new Vector3(-_stairWidth * 0.5f, 0, -_stairDepth * 0.5f), // Bottom left            |0|
            new Vector3(_stairWidth * 0.5f , 0, -_stairDepth * 0.5f), // Bottom right           |1|
            new Vector3(_stairWidth * 0.5f , 0, _stairDepth * 0.5f), // Top right               |2|
            new Vector3(-_stairWidth * 0.5f, 0, _stairDepth * 0.5f), // Top left                |3|
            // bottom of steps
            new Vector3(-_stairWidth * 0.5f, -_stairHeight, -_stairDepth * 0.5f), // Low left    |4|
            new Vector3(_stairWidth * 0.5f , -_stairHeight, -_stairDepth * 0.5f), // Low right   |5|
        };
        float _uvScale = 1f;
        if (0f == _uvScale)
        {
            m.uv = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(0, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 0),
                new Vector2(1f, 1f),
                new Vector2(0f, 0f)
            };
        }
        else
        {
            float maxUV = Mathf.Max(_stairWidth, _stairDepth);
            m.uv = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(0, _stairWidth * _uvScale),
                new Vector2(_stairDepth * _uvScale, _stairWidth * _uvScale),
                new Vector2(_stairDepth * _uvScale, 0),
                new Vector2(_stairHeight * _uvScale, 0f),
                new Vector2(_stairHeight * _uvScale, _stairWidth),
            };
        }
        m.triangles = new int[] { 2, 1, 0, 3, 2, 0, 1, 5, 4, 0, 1, 4}; // 2 triangles, diagonal bottom left to top right, CLOCKWISE

        mf.mesh = m;
        if (true)
        {
            (go.AddComponent(typeof(MeshCollider)) as MeshCollider).sharedMesh = m;
        }
        m.RecalculateBounds();
        m.RecalculateNormals();
        return go;
    }
}