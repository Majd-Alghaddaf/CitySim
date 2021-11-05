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
        TrafficLight trafficLight = (TrafficLight)target;

        if (GUILayout.Button("Detect Nearby End Waypoints"))
        {
            trafficLight.DetectNearbyEndWaypoints();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}

[CanEditMultipleObjects]
[CustomEditor(typeof(StopSign))]
public class StopSignEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        StopSign stopSign = (StopSign)target;

        if (GUILayout.Button("Detect Nearby Waypoints"))
        {
            stopSign.DetectNearbyEndWaypoints();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
