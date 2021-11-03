using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(TrafficLight))]
public class TrafficLightEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TrafficLight centralTrafficSystem = (TrafficLight)target;

        if (GUILayout.Button("Detect Nearby Waypoints"))
        {
            centralTrafficSystem.DetectNearbyWaypoints();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
