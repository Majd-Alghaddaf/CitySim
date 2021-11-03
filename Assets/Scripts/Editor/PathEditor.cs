using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(Path))]
public class PathEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Path path = (Path)target;
        if (GUILayout.Button("Populate Waypoints List"))
        {
            path.PopulateWaypointsList();
        }
        else if (GUILayout.Button("Rename Waypoints"))
        {
            path.RenameWaypoints();
        }
        else if(GUILayout.Button("Setup End Waypoints"))
        {
            path.SetupEndWaypoints();
        }
        else if (GUILayout.Button("Generate Intermediate Waypoints"))
        {
            path.GenerateIntermediatePoints();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
