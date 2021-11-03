using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightResponse
{
    public Path pathResponse;
    public int waypointIndexResponse;

    public TrafficLightResponse(Path pathResponse, int waypointIndexResponse)
    {
        this.pathResponse = pathResponse;
        this.waypointIndexResponse = waypointIndexResponse;
    }
}
