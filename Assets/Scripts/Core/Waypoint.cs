using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [SerializeField] protected Settings settings;

    public void SetSettings(Settings settings)
    {
        this.settings = settings;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, settings.gizmoSphereRadius);
    }
}
