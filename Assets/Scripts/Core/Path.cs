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
            EndWaypoint endWaypointComponent = waypoints[i].GetComponent<EndWaypoint>();
            if (endWaypointComponent != null)
            {
                waypoints[i].gameObject.name = $"{name}-EW{i}";
            }
            else
            {
                waypoints[i].gameObject.name = $"{name}-W{i}";
            }
        }
    }

    public void SetupFirstAndEndWaypoint()
    {
        SetupEndWaypoint();

        GameObject firstWaypointGameObject = waypoints[0].gameObject;
        SetBoxColliderAndLayerForWaypoint(firstWaypointGameObject);

        PopulateWaypointsList();
    }

    private void SetupEndWaypoint()
    {
        GameObject endWaypointGameObject = waypoints[waypoints.Count - 1].gameObject;

        endWaypointGameObject.AddComponent<EndWaypoint>();
        DestroyImmediate(endWaypointGameObject.GetComponent<Waypoint>());

        SetBoxColliderAndLayerForWaypoint(endWaypointGameObject);
    }

    private void SetBoxColliderAndLayerForWaypoint(GameObject waypointGameObject)
    {
        BoxCollider existingEndWaypointCollider = waypointGameObject.GetComponent<BoxCollider>();
        if (existingEndWaypointCollider == null)
        {
            BoxCollider endWaypointCollider = waypointGameObject.AddComponent<BoxCollider>();
            endWaypointCollider.size = settings.endWaypointBoxColliderSize;
            endWaypointCollider.isTrigger = true;
        }
        else
        {
            if (existingEndWaypointCollider.size != settings.endWaypointBoxColliderSize)
            {
                existingEndWaypointCollider.size = settings.endWaypointBoxColliderSize;
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
