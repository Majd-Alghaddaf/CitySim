using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VehicleCollisionAvoidance : MonoBehaviour, ICollisionAvoidanceObstacle
{
    [Header("Configuration")]
    [Range(0f, 1f)]
    [Tooltip("0.95f means that the vehicle will slow down by 5% every frame up until it reaches the min magnutitude value defined below.")]
    [SerializeField] private float slowDownFactor = 0.95f;
    [Tooltip("Once the magnitude of the velocity reaches below this value, vehicle will stop. It defines how long the vehicle will keep delecerating before it stops. Careful not to bring the value too low to a point where vehicle will never stop. And keep in mind that faster vehicles need to have a higher minimum value.")]
    [SerializeField] private float minMagnitudeValue = 2f;

    private NavMeshAgent myNavMeshAgent;
    private bool isVehicleWithinCollider;

    private void Start()
    {
        myNavMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<ICollisionAvoidanceObstacle>() != null && !other.isTrigger)
        {
            isVehicleWithinCollider = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isVehicleWithinCollider == true)
        {
            myNavMeshAgent.velocity = Vector3.Lerp(Vector3.zero, myNavMeshAgent.velocity, slowDownFactor);

            if (myNavMeshAgent.velocity.magnitude <= minMagnitudeValue) // maybe do distance instead of magnitude because it is costly
            {
                myNavMeshAgent.isStopped = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isVehicleWithinCollider == true)
        {
            if (other.GetComponentInParent<ICollisionAvoidanceObstacle>() != null && !other.isTrigger)
            {
                isVehicleWithinCollider = false;
                myNavMeshAgent.isStopped = isVehicleWithinCollider;
            }
        }
    }
}




