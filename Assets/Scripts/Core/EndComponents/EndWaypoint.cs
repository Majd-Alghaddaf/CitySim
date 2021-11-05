using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EndWaypoint : Waypoint
{
    public bool allowVehiclesToPass = true;

    private int numOfVehiclesWithinCollider = 0;
    private List<NavMeshAgent> agentsWithinCollider = new List<NavMeshAgent>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<VehicleMovement>() != null && !other.isTrigger)
        {
            numOfVehiclesWithinCollider++;
            agentsWithinCollider.Add(other.GetComponentInParent<NavMeshAgent>());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (numOfVehiclesWithinCollider > 0)
        {
            if (allowVehiclesToPass)
            {
                foreach (NavMeshAgent agent in agentsWithinCollider)
                {
                    agent.isStopped = false;
                }
            }
            else
            {
                foreach (NavMeshAgent agent in agentsWithinCollider)
                {
                    agent.velocity = Vector3.zero;
                    agent.isStopped = true;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<VehicleMovement>() != null && !other.isTrigger)
        {
            numOfVehiclesWithinCollider--;
            agentsWithinCollider.Remove(other.GetComponentInParent<NavMeshAgent>());
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
