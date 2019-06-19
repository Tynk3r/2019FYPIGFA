using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    // Plane creation
    public static GameObject CreatePlane(float _width, float _height, float _uvScale = 0f, bool _collider = true)
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

        if (0f == _uvScale)
        {
            m.uv = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(0, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 0)
            };
        }
        else
        {
            float maxUV = Mathf.Max(_width, _height);
            m.uv = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(0, _width * _uvScale),
                new Vector2(_height * _uvScale, _width * _uvScale),
                new Vector2(_height * _uvScale, 0)
            };
        }
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