using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TrafficWaypoint : Waypoint
{
    public bool allowVehiclesToPass = false;

    private int numOfVehiclesWithinCollider = 0;
    private bool vehicleExistsWithinCollider = false;
    private NavMeshAgent vehicleAgentWithinCollider;

    private void OnTriggerEnter(Collider other)
    {
        VehicleMovement vehicleMovementComponent = other.GetComponentInParent<VehicleMovement>();
        if (vehicleMovementComponent != null && !other.isTrigger)
        {
            numOfVehiclesWithinCollider++;
            vehicleAgentWithinCollider = other.GetComponentInParent<NavMeshAgent>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (vehicleExistsWithinCollider)
        {
            if (allowVehiclesToPass)
            {
                vehicleAgentWithinCollider.isStopped = false;
            }
            else
            {
                vehicleAgentWithinCollider.velocity = Vector3.zero;
                vehicleAgentWithinCollider.isStopped = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<VehicleMovement>() != null)
        {
            vehicleExistsWithinCollider = false;
            vehicleAgentWithinCollider = null;
        }
    }

    #region EDITOR SCRIPTS
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, settings.gizmoSphereRadius);
    } 
    #endregion
}
