using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapSection : MonoBehaviour
{
    public List<BoxCollider2D> areas;

    [HideInInspector]
    public List<Waypoint> civilWaypoints;

    [HideInInspector]
    public List<Waypoint> policeWaypoints;

    List<Waypoint> t_freeCivilWaypoints;
    List<Waypoint> t_freePoliceWaypoints;

    void Awake()
    {
        InitWaypoints();
    }

    void Start()
    {
        MapManager.Instance.sections.Add(this);
    }

    void InitWaypoints()
    {
        for (int i = 0; i < areas.Count; i++)
        {
            for (int j = 0; j < MapManager.Instance.civilWaypoints.Count; j++)
            {
                if (MapManager.Instance.civilWaypoints[j].isPublic)
                    if (AreaContainsPos(i, MapManager.Instance.civilWaypoints[j].transform.position))
                        if (!civilWaypoints.Contains(MapManager.Instance.civilWaypoints[j]))
                        {
                            MapManager.Instance.civilWaypoints[j].ownerMapSection = this;
                            civilWaypoints.Add(MapManager.Instance.civilWaypoints[j]);
                        }
            }

            //

            for (int j = 0; j < MapManager.Instance.policeWaypoints.Count; j++)
            {
                if (MapManager.Instance.policeWaypoints[j].isPublic)
                    if (AreaContainsPos(i, MapManager.Instance.policeWaypoints[j].transform.position))
                        if (!policeWaypoints.Contains(MapManager.Instance.policeWaypoints[j]))
                        {
                            MapManager.Instance.policeWaypoints[j].ownerMapSection = this;
                            policeWaypoints.Add(MapManager.Instance.policeWaypoints[j]);
                        }
            }
        }
    }

    public List<Waypoint> GetFree_Civil_Waypoints()
    {
        if (null == t_freeCivilWaypoints) t_freeCivilWaypoints = new List<Waypoint>();

        t_freeCivilWaypoints.Clear();

        for (int i = 0; i < civilWaypoints.Count; i++)
        {
            if (!civilWaypoints[i].isBusy)
            {
                t_freeCivilWaypoints.Add(civilWaypoints[i]);
            }
        }

        return t_freeCivilWaypoints;
    }

    public Waypoint GetARandomFree_Civil_Waypoint()
    {
        GetFree_Civil_Waypoints();

        return t_freeCivilWaypoints[Random.Range(0, t_freeCivilWaypoints.Count)];
    }

    public List<Waypoint> GetFree_Police_Waypoints()
    {
        if (null == t_freePoliceWaypoints) t_freePoliceWaypoints = new List<Waypoint>();

        t_freePoliceWaypoints.Clear();

        for (int i = 0; i < policeWaypoints.Count; i++)
        {
            if (!policeWaypoints[i].isBusy)
            {
                t_freePoliceWaypoints.Add(policeWaypoints[i]);
            }
        }

        return t_freePoliceWaypoints;
    }

    public Waypoint GetARandomFree_Police_Waypoint()
    {
        GetFree_Police_Waypoints();

        return t_freePoliceWaypoints[Random.Range(0, t_freePoliceWaypoints.Count)];
    }

    public bool ContainsPos(Vector3 _pos)
    {
        for (int i = 0; i < areas.Count; i++)
        {
            if(AreaContainsPos(i, _pos))
                return true;
        }

        return false;
    }

    bool AreaContainsPos(int _areaIndex, Vector3 _pos)
    {
        return (areas[_areaIndex].bounds.Contains(_pos));
    }
}
