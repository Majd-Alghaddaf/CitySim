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
        Object[] paths = targets;
        if (GUILayout.Button("Populate Waypoints List"))
        {
            foreach(Path path in paths)
            {
                path.PopulateWaypointsList();
            }
        }
        else if (GUILayout.Button("Rename Waypoints"))
        {
            foreach (Path path in paths)
            {
                path.RenameWaypoints();
            }
        }
        else if(GUILayout.Button("Setup First & End Waypoint"))
        {
            foreach (Path path in paths)
            {
                path.SetupFirstAndEndWaypoint();
            }
        }
        else if (GUILayout.Button("Generate Intermediate Waypoints"))
        {
            foreach (Path path in paths)
            {
                path.GenerateIntermediatePoints();
            }
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
