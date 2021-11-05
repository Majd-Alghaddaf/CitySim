using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName ="ScriptableObjects/Settings", order = 1)]
public class Settings : ScriptableObject
{
    [Header("Editor Settings")]
    [SerializeField] public bool debugEndComponents;
    [SerializeField] public bool debugWaypointConnections;

    [Header("Configuration")]
    [SerializeField] public float endComponentOverlapSphereRadius;
    [SerializeField] public float gizmoSphereRadius = 0.5f;
    [SerializeField] public Vector3 endWaypointBoxColliderSize = new Vector3(5f, 2f, 5f);

    [SerializeField] public string endComponentMaskName;
    [SerializeField] public string endWaypointMaskName;

    [SerializeField] public float greenLightDuration = 15f;
    [SerializeField] public float yellowLightDuration = 2f;
    [SerializeField] public float redLightDuration = 2f;
}
