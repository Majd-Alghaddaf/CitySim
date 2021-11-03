using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] protected float gizmoSphereRadius = .5f;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, gizmoSphereRadius);
    }
}
