using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CentralTrafficSystem))]
public class CentralTrafficSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CentralTrafficSystem centralTrafficSystem = (CentralTrafficSystem)target;
        if (GUILayout.Button("Populate Paths List"))
        {
            centralTrafficSystem.PopulatePathsList();
        }
        else if (GUILayout.Button("Populate Vehicles List"))
        {
            centralTrafficSystem.PopulateVehiclesList();
        }
        else if (GUILayout.Button("Populate Traffic Lights List"))
        {
            centralTrafficSystem.PopulateTrafficLightsList();
        }
        else if (GUILayout.Button("Rename & Setup Paths"))
        {
            centralTrafficSystem.RenameAndSetupPaths();
        }
        else if (GUILayout.Button("Rename & Setup Traffic Lights"))
        {
            centralTrafficSystem.RenameAndSetupTrafficLights();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
