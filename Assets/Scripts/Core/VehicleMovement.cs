using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VehicleMovement : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Stopping Distance must be less than this value")]
    [SerializeField] private float minDistanceToCurrentDestination = 3;

    [Header("Debugging")]
    [SerializeField] private Path currentPath;
    [SerializeField] private Waypoint currentWaypoint;
    [SerializeField] private int currentWaypointIndex = 0;

    private NavMeshAgent myNavMeshAgent;
    private Vector3 currentDestination;

    void Start()
    {
        myNavMeshAgent = GetComponent<NavMeshAgent>();
        SetInitialDestination();
    }

    private void Update()
    {
        if (ReachedMinDistance())
        {
            EvaluateNextDestination();
        }
    }

    private void SetInitialDestination()
    {
        currentWaypointIndex = currentPath.GetNextWaypointIndex(currentWaypointIndex - 1);
        currentWaypoint = currentPath.GetWaypointByIndex(currentWaypointIndex);
        currentDestination = currentWaypoint.transform.position;
        myNavMeshAgent.destination = currentDestination;
    }

    private void EvaluateNextDestination()
    {
        TrafficWaypoint trafficWaypointComponent = currentWaypoint.GetComponent<TrafficWaypoint>();
        if (trafficWaypointComponent != null)
        {
            TrafficLightResponse trafficLightResponse = CentralTrafficSystem.Instance.RequestNewPath(trafficWaypointComponent);
            SetPathAndCurrentWaypointIndex(trafficLightResponse.pathResponse, trafficLightResponse.waypointIndexResponse);
            currentWaypoint = currentPath.GetWaypointByIndex(currentWaypointIndex);
        }
        else
        {
            currentWaypointIndex = currentPath.GetNextWaypointIndex(currentWaypointIndex);
            currentWaypoint = currentPath.GetWaypointByIndex(currentWaypointIndex);
        }
        currentDestination = currentWaypoint.transform.position;
        myNavMeshAgent.destination = currentDestination;
    }

    private bool ReachedMinDistance()
    {
        return Vector3.Distance(transform.position, currentDestination) < minDistanceToCurrentDestination;
    }

    public void SetPathAndCurrentWaypointIndex(Path path, int waypointIndex)
    {
        currentPath = path;
        currentWaypointIndex = waypointIndex;
    }
}
