using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EndComponent : MonoBehaviour
{
    [Header("End of Path Component Configuration")]
    [SerializeField] protected Settings settings;
    [SerializeField] protected List<Waypoint> nearbyNormalWaypoints = new List<Waypoint>();
    [SerializeField] protected List<EndWaypoint> nearbyEndWaypoints = new List<EndWaypoint>();
    [SerializeField] protected Dictionary<EndWaypoint, List<Waypoint>> availableWaypointsFromEndWaypoint = new Dictionary<EndWaypoint, List<Waypoint>>();

    [Header("Debugging")]
    [SerializeField] protected bool debugEndComponent = false;

    private void Awake()
    {
        PopulateAvailableWaypointsFromEndWaypointDictionary();
    }

    private void PopulateAvailableWaypointsFromEndWaypointDictionary(bool debug = false)
    {
        foreach (EndWaypoint endWaypoint in nearbyEndWaypoints)
        {
            availableWaypointsFromEndWaypoint.Add(endWaypoint, nearbyNormalWaypoints);
        }

        if (debug)
        {
            string debugText = "";
            foreach (KeyValuePair<EndWaypoint, List<Waypoint>> keyValuePair in availableWaypointsFromEndWaypoint)
            {
                debugText = $"End Waypoint {keyValuePair.Key} has {keyValuePair.Value.Count} available waypoints to go to: ";
                foreach (Waypoint wp in keyValuePair.Value)
                {
                    debugText += wp.name + " / ";
                }
                Debug.Log(debugText);
            }
        }
    }

    public GeneratedPathResponse GenerateResponseForNewPathAtEndComponent(EndWaypoint endWaypoint)
    {
        List<Waypoint> viableWaypointsToReturn = availableWaypointsFromEndWaypoint[endWaypoint];

        if (viableWaypointsToReturn.Count == 0)
        {
            Debug.LogError("End Component Available Waypoints From End Waypoint Dictionary was not populated. Please make sure to correctly configure the End Component!");
        }

        Waypoint waypointResponse = viableWaypointsToReturn[UnityEngine.Random.Range(0, viableWaypointsToReturn.Count)];
        Path pathResponse = waypointResponse.GetComponentInParent<Path>();
        int waypointIndexResponse = pathResponse.waypoints.IndexOf(waypointResponse);

        if (waypointIndexResponse != 0)
        {
            Debug.LogError("The response waypoint of a new path should always be the first waypoint. Check that everything is set up correctly in the scene");
        }

        GeneratedPathResponse generatedPathResponse = new GeneratedPathResponse(pathResponse, waypointIndexResponse);
        return generatedPathResponse;
    }

#if UNITY_EDITOR
    #region EDITOR SCRIPTS
    public void DetectNearbyEndWaypoints()
    {
        EditorUtility.SetDirty(this);

        nearbyNormalWaypoints.Clear();
        nearbyEndWaypoints.Clear();
        availableWaypointsFromEndWaypoint.Clear();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, settings.endComponentOverlapSphereRadius, LayerMask.GetMask(settings.endWaypointMaskName));
        PopulateWaypointsLists(hitColliders);
        PopulateAvailableWaypointsFromEndWaypointDictionary(true);
    }

    private void PopulateWaypointsLists(Collider[] hitColliders)
    {
        foreach (Collider hitCollider in hitColliders)
        {
            Waypoint nearbyWaypointComponent = hitCollider.GetComponent<Waypoint>();
            if (nearbyWaypointComponent != null)
            {
                EndWaypoint nearbyEndWaypointComponent = hitCollider.GetComponent<EndWaypoint>();
                if (nearbyEndWaypointComponent != null)
                {
                    nearbyEndWaypoints.Add(nearbyEndWaypointComponent);
                }
                else
                {
                    nearbyNormalWaypoints.Add(nearbyWaypointComponent);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (debugEndComponent && settings.debugEndComponents)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, settings.endComponentOverlapSphereRadius);

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, settings.endComponentOverlapSphereRadius, LayerMask.GetMask(settings.endWaypointMaskName));

            if (hitColliders.Length == 0)
            {
                Debug.LogError("Check if the Layer Mask and Box Collider are correctly set up on the end waypoints");
            }
        }
    }
    #endregion
#endif
}
