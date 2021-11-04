using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private Settings settings;

    [Header("Debugging")]
    [SerializeField] private bool debugTrafficLight = false;
    [SerializeField] private int indexOfTrafficWaypointThatCanPass = 0;

    [SerializeField] private List<Waypoint> nearbyNormalWaypoints = new List<Waypoint>();
    [SerializeField] private List<TrafficWaypoint> nearbyTrafficWaypoints = new List<TrafficWaypoint>();
    [SerializeField] private Dictionary<TrafficWaypoint, List<Waypoint>> availableWaypointsFromTrafficWaypoint = new Dictionary<TrafficWaypoint, List<Waypoint>>();

    private void Awake()
    {
        PopulateAvailableWaypointsFromTrafficWaypointDictionary();
    }

    private void Start()
    {
        SetInitialTrafficLightConditions();
        StartCoroutine(CycleThroughTrafficLightStates());
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, nearbyTrafficWaypoints[indexOfTrafficWaypointThatCanPass].transform.position - transform.position, Color.green);
    }

    private void PopulateAvailableWaypointsFromTrafficWaypointDictionary(bool debug = false)
    {
        foreach (TrafficWaypoint trafficWaypoint in nearbyTrafficWaypoints)
        {
            availableWaypointsFromTrafficWaypoint.Add(trafficWaypoint, nearbyNormalWaypoints);
        }

        if (debug)
        {
            string debugText = "";
            foreach (KeyValuePair<TrafficWaypoint, List<Waypoint>> keyValuePair in availableWaypointsFromTrafficWaypoint)
            {
                debugText = $"Traffic Waypoint {keyValuePair.Key} has {keyValuePair.Value.Count} available waypoints to go to: ";
                foreach (Waypoint wp in keyValuePair.Value)
                {
                    debugText += wp.name + " / ";
                }
                Debug.Log(debugText);
            }
        }
    }

    public TrafficLightResponse GenerateResponseForNewPathAtTrafficLight(TrafficWaypoint currentTrafficWaypoint)
    {
        List<Waypoint> viableWaypointsToReturn = availableWaypointsFromTrafficWaypoint[currentTrafficWaypoint];

        if(viableWaypointsToReturn.Count == 0)
        {
            Debug.LogError("Traffic Light Available Waypoints From Traffic Waypoint Dictionary was not populated. Please make sure to correctly configure the Traffic Light!");
        }

        Waypoint waypointResponse = viableWaypointsToReturn[UnityEngine.Random.Range(0, viableWaypointsToReturn.Count)];
        Path pathResponse = waypointResponse.GetComponentInParent<Path>();
        int waypointIndexResponse = pathResponse.waypoints.IndexOf(waypointResponse);

        if(waypointIndexResponse != 0)
        {
            Debug.LogError("The response waypoint of a new path should always be the first waypoint. Check that everything is set up correctly in the scene");
        }

        TrafficLightResponse trafficLightResponse = new TrafficLightResponse(pathResponse, waypointIndexResponse);
        return trafficLightResponse;
    }

    private void SetInitialTrafficLightConditions()
    {
        indexOfTrafficWaypointThatCanPass = UnityEngine.Random.Range(0, nearbyTrafficWaypoints.Count);

        foreach (TrafficWaypoint trafficWaypoint in nearbyTrafficWaypoints)
        {
            trafficWaypoint.allowVehiclesToPass = false;
        }
    }

    IEnumerator CycleThroughTrafficLightStates()
    {
        nearbyTrafficWaypoints[indexOfTrafficWaypointThatCanPass].allowVehiclesToPass = true;
        yield return new WaitForSeconds(settings.greenLightDuration);
        // todo make light yellow - but vehicles can still pass
        yield return new WaitForSeconds(settings.yellowLightDuration);
        nearbyTrafficWaypoints[indexOfTrafficWaypointThatCanPass].allowVehiclesToPass = false;

        // because if there is only one traffic waypoint there will be a downtime which leads to issues - there should never only be one traffic waypoint at a traffic light
        if(nearbyTrafficWaypoints.Count > 1)
        {
            // todo make light red
            yield return new WaitForSeconds(settings.redLightDuration);
        }

        indexOfTrafficWaypointThatCanPass = IncrementIndexOfTrafficWaypointThatCanPass(indexOfTrafficWaypointThatCanPass);
        yield return CycleThroughTrafficLightStates();
    }

    private int IncrementIndexOfTrafficWaypointThatCanPass(int index)
    {
        if(index == nearbyTrafficWaypoints.Count - 1)
        {
            return 0;
        }
        else
        {
            return index + 1;
        }
    }

#if UNITY_EDITOR
    #region EDITOR SCRIPTS
    public void DetectNearbyWaypoints()
    {
        EditorUtility.SetDirty(this);

        nearbyNormalWaypoints.Clear();
        nearbyTrafficWaypoints.Clear();
        availableWaypointsFromTrafficWaypoint.Clear();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, settings.trafficLightOverlapSphereRadius, LayerMask.GetMask(settings.endWaypointMaskName));
        PopulateWaypointsLists(hitColliders);
        PopulateAvailableWaypointsFromTrafficWaypointDictionary(true);
    }

    private void PopulateWaypointsLists(Collider[] hitColliders)
    {
        foreach (Collider hitCollider in hitColliders)
        {
            Waypoint nearbyWaypointComponent = hitCollider.GetComponent<Waypoint>();
            if (nearbyWaypointComponent != null)
            {
                TrafficWaypoint nearbyTrafficWaypointComponent = hitCollider.GetComponent<TrafficWaypoint>();
                if (nearbyTrafficWaypointComponent != null)
                {
                    nearbyTrafficWaypoints.Add(nearbyTrafficWaypointComponent);
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
        if (debugTrafficLight && settings.debugTrafficLights)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, settings.trafficLightOverlapSphereRadius);

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, settings.trafficLightOverlapSphereRadius, LayerMask.GetMask(settings.endWaypointMaskName));

            if(hitColliders.Length == 0)
            {
                Debug.LogError("Check if the Layer Mask and Box Collider are correctly set up on the end waypoints");
            }
        }
    }
    #endregion
#endif
}
