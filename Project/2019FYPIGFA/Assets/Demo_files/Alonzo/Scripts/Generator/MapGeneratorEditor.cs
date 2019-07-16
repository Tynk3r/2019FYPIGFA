using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof (MapGenerator))]
public class MapGeneratorEditor : Editor
{
    bool generated = false;
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        MapGenerator mapGen = (MapGenerator)target;

        // DrawDefaultInspector() returns true if anything was changed
        if (DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
                mapGen.DrawMapInEditor();
        }

        if (!generated)
        {
            if (GUILayout.Button("Generate"))
            {
                mapGen.DrawMapInEditor();
                generated = true;
            }
        }
        else if (GUILayout.Button("Clear Map"))
        {
            mapGen.DrawMapInEditor();
            generated = false;
        }
    }
}
#endif
