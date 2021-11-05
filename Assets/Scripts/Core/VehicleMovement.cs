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
        EndWaypoint endWaypointComponent = currentWaypoint.GetComponent<EndWaypoint>();
        if (endWaypointComponent != null)
        {
            GeneratedPathResponse generatedPathResponse = CentralTrafficSystem.Instance.RequestNewPath(endWaypointComponent);
            SetPathAndCurrentWaypointIndex(generatedPathResponse.pathResponse, generatedPathResponse.waypointIndexResponse);
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
