using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Path : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private GameObject waypointPrefab;
    [SerializeField] private bool debugWaypointConnections = true;
    [SerializeField] private Settings settings;

    [Header("Debugging")]
    [SerializeField] public List<Waypoint> waypoints = new List<Waypoint>();

    public int GetNextWaypointIndex(int currentWaypointIndex)
    {
        return currentWaypointIndex + 1;
    }

    public Waypoint GetWaypointByIndex(int waypointIndex)
    {
        return waypoints[waypointIndex];
    }

#if UNITY_EDITOR
    #region EDITOR SCRIPTS
    public void PopulateWaypointsList()
    {
        EditorUtility.SetDirty(this);

        waypoints.Clear();
        foreach (Waypoint waypoint in transform.GetComponentsInChildren<Waypoint>())
        {
            waypoint.SetSettings(settings);
            waypoints.Add(waypoint);
        }
    }

    public void GenerateIntermediatePoints()
    {
        int siblingIndex = 1;
        int j;

        for (int i = 0; i <= waypoints.Count - 2; i++)
        {
            j = i + 1;

            Vector3 newWaypointPosition = CalculateNewIntermediateWaypointPosition(i, j);
            InstantiateNewIntermediateWaypoint(siblingIndex, i, newWaypointPosition);
            siblingIndex++;
        }

        PopulateWaypointsList();
    }
    private Vector3 CalculateNewIntermediateWaypointPosition(int i, int j)
    {
        float newX = (waypoints[j].transform.position.x + waypoints[i].transform.position.x) / 2;
        float newZ = (waypoints[j].transform.position.z + waypoints[i].transform.position.z) / 2;
        return new Vector3(newX, 1, newZ);
    }

    private void InstantiateNewIntermediateWaypoint(int siblingIndex, int i, Vector3 newWaypointPosition)
    {
        GameObject newIntermediateWaypoint = PrefabUtility.InstantiatePrefab(waypointPrefab, transform) as GameObject;
        newIntermediateWaypoint.transform.position = newWaypointPosition;
        newIntermediateWaypoint.transform.SetSiblingIndex(i + siblingIndex);
    }

    public void RenameWaypoints()
    {
        for (int i = 0; i <= waypoints.Count - 1; i++)
        {
            EditorUtility.SetDirty(waypoints[i].gameObject);
            TrafficWaypoint trafficWaypointComponent = waypoints[i].GetComponent<TrafficWaypoint>();
            if (trafficWaypointComponent != null)
            {
                waypoints[i].gameObject.name = $"{name}-TW{i}";
            }
            else
            {
                waypoints[i].gameObject.name = $"{name}-W{i}";
            }
        }
    }

    public void SetupEndWaypoints()
    {
        SetupTrafficWaypoint();

        GameObject firstWaypointGameObject = waypoints[0].gameObject;
        SetBoxColliderAndLayerForWaypoint(firstWaypointGameObject);

        PopulateWaypointsList();
    }

    private void SetupTrafficWaypoint()
    {
        GameObject trafficWaypointGameObject = waypoints[waypoints.Count - 1].gameObject;

        TrafficWaypoint trafficWaypoint = trafficWaypointGameObject.AddComponent<TrafficWaypoint>();
        DestroyImmediate(trafficWaypointGameObject.GetComponent<Waypoint>());

        SetBoxColliderAndLayerForWaypoint(trafficWaypointGameObject);
    }

    private void SetBoxColliderAndLayerForWaypoint(GameObject waypointGameObject)
    {
        BoxCollider waypointBoxCollider = waypointGameObject.GetComponent<BoxCollider>();
        if (waypointBoxCollider == null)
        {
            BoxCollider trafficWaypointCollider = waypointGameObject.AddComponent<BoxCollider>();
            trafficWaypointCollider.size = settings.trafficWaypointBoxColliderSize;
            trafficWaypointCollider.isTrigger = true;
        }
        else
        {
            if (waypointBoxCollider.size != settings.trafficWaypointBoxColliderSize)
            {
                waypointBoxCollider.size = settings.trafficWaypointBoxColliderSize;
            }
        }
        waypointGameObject.layer = LayerMask.NameToLayer("End Waypoint");
    }

    private void OnDrawGizmos()
    {
        if (debugWaypointConnections && settings.debugWaypointConnections)
        {
            for (int i = 0; i < waypoints.Count - 1; i++)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(waypoints[i].transform.position, waypoints[i + 1].transform.position);
            }
        }
    }
    #endregion  
#endif
}
