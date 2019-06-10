using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    // Plane creation
    public static GameObject CreatePlane(float _width, float _height, bool _collider = true)
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

        m.uv = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(0, _width),
            new Vector2(_width, _width),
            new Vector2(_width, 0)
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
}
