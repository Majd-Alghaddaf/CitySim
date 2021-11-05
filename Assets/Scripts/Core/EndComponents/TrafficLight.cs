using System.Collections;
using UnityEngine;

public class TrafficLight : EndComponent
{
    [Header("Debugging")]
    [SerializeField] private int indexOfEndWaypointThatCanPass = 0;

    private void Start()
    {
        SetInitialTrafficLightConditions();
        StartCoroutine(CycleThroughTrafficLightStates());
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, nearbyEndWaypoints[indexOfEndWaypointThatCanPass].transform.position - transform.position, Color.green);
    }

    private void SetInitialTrafficLightConditions()
    {
        indexOfEndWaypointThatCanPass = UnityEngine.Random.Range(0, nearbyEndWaypoints.Count);

        foreach (EndWaypoint endWaypoint in nearbyEndWaypoints)
        {
            endWaypoint.allowVehiclesToPass = false;
        }
    }

    IEnumerator CycleThroughTrafficLightStates()
    {
        nearbyEndWaypoints[indexOfEndWaypointThatCanPass].allowVehiclesToPass = true;
        yield return new WaitForSeconds(settings.greenLightDuration);
        // todo make light yellow - but vehicles can still pass
        yield return new WaitForSeconds(settings.yellowLightDuration);
        nearbyEndWaypoints[indexOfEndWaypointThatCanPass].allowVehiclesToPass = false;

        // because if there is only one traffic waypoint there will be a downtime which leads to issues - there should never only be one traffic waypoint at a traffic light
        if(nearbyEndWaypoints.Count > 1)
        {
            // todo make light red
            yield return new WaitForSeconds(settings.redLightDuration);
        }

        indexOfEndWaypointThatCanPass = IncrementIndexOfEndWaypointThatCanPass(indexOfEndWaypointThatCanPass);
        yield return CycleThroughTrafficLightStates();
    }

    private int IncrementIndexOfEndWaypointThatCanPass(int index)
    {
        if(index == nearbyEndWaypoints.Count - 1)
        {
            return 0;
        }
        else
        {
            return index + 1;
        }
    }
}
