using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName ="ScriptableObjects/Settings", order = 1)]
public class Settings : ScriptableObject
{
    [Header("Editor Settings")]
    [SerializeField] public bool debugTrafficLights;
    [SerializeField] public bool debugWaypointConnections;

    [Header("Configuration")]
    [SerializeField] public float trafficLightOverlapSphereRadius;
    [SerializeField] public string trafficLightMaskName;
    [SerializeField] public string endWaypointMaskName;
}
