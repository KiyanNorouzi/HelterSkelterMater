using UnityEngine;
using System.Collections;

public enum WaypointType
{
    CIVIL,
    POLICE,
}

public class Waypoint : MonoBehaviour {

    public WaypointType waypointType = WaypointType.CIVIL;
    public bool isPublic = true;

    [HideInInspector]
    public MapSection ownerMapSection;

    [HideInInspector]
    public bool isBusy = false;

    public string actAnimName = "";
    public float actMinTime = 5;
    public float actMaxTime = 10;
    public bool needsToForceSide = false;
    public Side forcedSide;
}
