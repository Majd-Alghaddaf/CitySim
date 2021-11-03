using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VehicleCollisionAvoidance : MonoBehaviour, ICollisionAvoidanceObstacle
{
    [Header("Debugging")]
    [SerializeField] private bool vehicleStopped;
    [SerializeField] private GameObject vehicleWithinStoppingDistance;

    private NavMeshAgent myNavMeshAgent;

    private void Start()
    {
        myNavMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponentInParent<ICollisionAvoidanceObstacle>() != null && !other.isTrigger)
        {
            vehicleStopped = true;
            vehicleWithinStoppingDistance = other.gameObject;

            myNavMeshAgent.velocity = Vector3.zero;
            myNavMeshAgent.isStopped = vehicleStopped;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<ICollisionAvoidanceObstacle>() != null && !other.isTrigger)
        {
            vehicleStopped = false;
            myNavMeshAgent.isStopped = vehicleStopped;
        }
    }
}
