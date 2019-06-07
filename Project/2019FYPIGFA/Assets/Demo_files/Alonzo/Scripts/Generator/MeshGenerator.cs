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
            new Vector3(0, 0, 0), // Bottom left
            new Vector3(_width, 0, 0), // Bottom right
            new Vector3(_width, _height, 0), // Top right
            new Vector3(0, _height, 0) // Top left
        };

        m.uv = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0)
        };
        m.triangles = new int[] { 0, 1, 2, 0, 2, 3 }; // 2 triangles, diagonal bottom left to top right

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
