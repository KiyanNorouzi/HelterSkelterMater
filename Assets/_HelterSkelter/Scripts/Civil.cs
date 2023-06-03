using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CivilDecisionState
{
    START,
    SELECT_NEW_WP,
    NORMAL_SENSE_START,
    NORMAL_SENSE_UPDATE,
    NORMAL_SENSE_WAIT_FOR_FINISH_TO_SCREAM__START,
    NORMAL_SENSE_WAIT_FOR_FINISH_TO_SCREAM__UPDATE,
    SCREAM_START,
    SCREAM_UPDATE,
    FLEE_MODE_START,
    GO_TO_ALARM_START,
    GO_TO_ALARM_UPDATE,
    ALARM_START,
    TRAP_FREEZE_START,
    TRAP_FREEZE_UPDATE,
    TRAP_FREEZE_FINISH,
}

public class Civil : NPC
{
    public SoundType screamSound;
    string animName_Scream = "Scream";

    CivilDecisionState civilDecisionState = CivilDecisionState.START;

    float decisionFSM_TimeCounter = 0;

    float screamMaxTime = 0.4f;

    Corpse newCorpse;

    Corpse selectedCorpseToReport = null;

    List<SectionRate> t_SectionRate = new List<SectionRate>();
    List<SectionRate> t_SortedSectionRate = new List<SectionRate>();
    List<Waypoint> t_WaypointsList = new List<Waypoint>();

    float sectionRatingMaxDecreasementCoef_DistToNewCorpse = 0.15f;
    float sectionRatingMaxDecreasementCoef_ContainsCorpse = 0.1f;
    float sectionRatingRandomCoef = 0.25f;

    // Use this for initialization
    void Awake()
    {
        PixObj_Awake();
    }

    // Update is called once per frame
    void Update()
    {
        PixObj_Update();
    }

    public override void PixObj_Awake()
    {
        base.PixObj_Awake();

        MapManager.Instance.Event_CorpseRemoved += TryRemoveCorpseFromKnownCorpsesList;
    }

    public override void PixObj_Update()
    {
        base.PixObj_Update();

    Begin:

        #region START
        if (IsCivilDecisionState(CivilDecisionState.START))
        {
            SetCivilDecisionState(CivilDecisionState.SELECT_NEW_WP);
        }
        #endregion

        #region SELECT_NEW_WP
        if (IsCivilDecisionState(CivilDecisionState.SELECT_NEW_WP))
        {
            Waypoint wp = SelectNewRandomWaypoint();
            SetSelectedWaypoint(wp);

            curIdleActInfo = new IdleActInfo();

            curIdleActInfo.animName = wp.actAnimName;
            curIdleActInfo.animSpeed = 1;
            curIdleActInfo.minTime = wp.actMinTime;
            curIdleActInfo.maxTime = wp.actMaxTime;
            curIdleActInfo.selectedTime = Random.Range(curIdleActInfo.minTime, curIdleActInfo.maxTime);
            curIdleActInfo.pos = wp.transform.position;
            curIdleActInfo.needToChangeSide = wp.needsToForceSide;
            curIdleActInfo.neededSide = wp.forcedSide;

            curMoveActInfo = new MoveActInfo();

            curMoveActInfo.pos = wp.transform.position;
            curMoveActInfo.moveSpeed = normalMoveSpeed;
            curMoveActInfo.animSpeed = 1;
            curMoveActInfo.animName = animName_Walk;

            SetNPCActState(NPCActState.MOVE_START);

            SetCivilDecisionState(CivilDecisionState.NORMAL_SENSE_START);
        }
        #endregion

        #region NORMAL_SENSE_START
        if (IsCivilDecisionState(CivilDecisionState.NORMAL_SENSE_START))
        {
            SetCivilDecisionState(CivilDecisionState.NORMAL_SENSE_UPDATE);
        }
        #endregion

        #region NORMAL_SENSE_UPDATE
        if (IsCivilDecisionState(CivilDecisionState.NORMAL_SENSE_UPDATE))
        {
            if (IsAnyNewCorpseSeen(out newCorpse))
            {
                if (!knownCorpses.Contains(newCorpse))
                {
                    AddCorpseToKnownCorpsesList(newCorpse);

                    if (MapManager.Instance.IsPolicePresent())
                    {
                        newCorpse.SetIdentified();
                    }

                    selectedCorpseToReport = newCorpse;
                    SetCivilDecisionState(CivilDecisionState.NORMAL_SENSE_WAIT_FOR_FINISH_TO_SCREAM__START);
                    goto Begin;
                }
            }

            if (IsMurderingSeen())
            {
                Player.Instance.SetDetected();
                SetCivilDecisionState(CivilDecisionState.NORMAL_SENSE_WAIT_FOR_FINISH_TO_SCREAM__START);
                goto Begin;
            }

            if (IsNPCActState(NPCActState.WAIT_UPDATE))
                SetCivilDecisionState(CivilDecisionState.SELECT_NEW_WP);
        }
        #endregion

        #region NORMAL_SENSE_WAIT_FOR_FINISH_TO_SCREAM__START
        if (IsCivilDecisionState(CivilDecisionState.NORMAL_SENSE_WAIT_FOR_FINISH_TO_SCREAM__START))
        {
            SetNPCActStateNeedsToBeFinished(true);
            SetCivilDecisionState(CivilDecisionState.NORMAL_SENSE_WAIT_FOR_FINISH_TO_SCREAM__UPDATE);
        }
        #endregion

        #region NORMAL_SENSE_WAIT_FOR_FINISH_TO_SCREAM__UPDATE
        if (IsCivilDecisionState(CivilDecisionState.NORMAL_SENSE_WAIT_FOR_FINISH_TO_SCREAM__UPDATE))
        {
            if (IsMurderingSeen())
            {
                Player.Instance.SetDetected();
            }

            if (IsNPCActState(NPCActState.WAIT_UPDATE))
                SetCivilDecisionState(CivilDecisionState.SCREAM_START);
        }
        #endregion

        #region SCREAM_START
        if (IsCivilDecisionState(CivilDecisionState.SCREAM_START))
        {
            SetSelectedWaypoint(null);

            curIdleActInfo = new IdleActInfo();

            curIdleActInfo.animName = animName_Scream;
            curIdleActInfo.animSpeed = 1;
            curIdleActInfo.selectedTime = screamMaxTime;
            //print(screamMaxTime);
            curIdleActInfo.pos = transform.position;
            curIdleActInfo.needToChangeSide = false;

            AudioManager.Instance.Play(screamSound, false);

            SetNPCActState(NPCActState.IDLE_START);

            SetCivilDecisionState(CivilDecisionState.SCREAM_UPDATE);
        }
        #endregion

        #region SCREAM_UPDATE
        if (IsCivilDecisionState(CivilDecisionState.SCREAM_UPDATE))
        {
            if (IsMurderingSeen())
            {
                Player.Instance.SetDetected();
            }

            if (IsNPCActState(NPCActState.WAIT_UPDATE))
            {
                if (MapManager.Instance.IsAlarmBusy() || MapManager.Instance.IsPolicePresent())
                {
                    SetCivilDecisionState(CivilDecisionState.FLEE_MODE_START);
                }
                else
                {
                    SetCivilDecisionState(CivilDecisionState.GO_TO_ALARM_START);
                }
            }
        }
        #endregion

        #region FLEE_MODE_START
        if (IsCivilDecisionState(CivilDecisionState.FLEE_MODE_START))
        {
            Waypoint wp = SelectWaypointForFlee();
            SetSelectedWaypoint(wp);

            curIdleActInfo = new IdleActInfo();

            curIdleActInfo.animName = wp.actAnimName;
            curIdleActInfo.animSpeed = 1;
            curIdleActInfo.minTime = wp.actMinTime;
            curIdleActInfo.maxTime = wp.actMaxTime;
            curIdleActInfo.selectedTime = Random.Range(curIdleActInfo.minTime, curIdleActInfo.maxTime);
            curIdleActInfo.pos = wp.transform.position;
            curIdleActInfo.needToChangeSide = wp.needsToForceSide;
            curIdleActInfo.neededSide = wp.forcedSide;

            curMoveActInfo = new MoveActInfo();

            curMoveActInfo.pos = wp.transform.position;
            curMoveActInfo.moveSpeed = fastMoveSpeedCoef * normalMoveSpeed;
            curMoveActInfo.animSpeed = fastMoveSpeedCoef;
            curMoveActInfo.animName = animName_Walk;

            SetNPCActState(NPCActState.MOVE_START);

            SetCivilDecisionState(CivilDecisionState.NORMAL_SENSE_START);
        }
        #endregion

        #region GO_TO_ALARM_START
        if (IsCivilDecisionState(CivilDecisionState.GO_TO_ALARM_START))
        {
            MapManager.Instance.SetAlarmIsBusy(true);

            Waypoint wp = MapManager.Instance.alarmWaypoint;
            SetSelectedWaypoint(wp);

            curIdleActInfo = new IdleActInfo();

            curIdleActInfo.animName = wp.actAnimName;
            curIdleActInfo.animSpeed = 1;
            curIdleActInfo.minTime = wp.actMinTime;
            curIdleActInfo.maxTime = wp.actMaxTime;
            curIdleActInfo.selectedTime = Random.Range(curIdleActInfo.minTime, curIdleActInfo.maxTime);
            curIdleActInfo.pos = wp.transform.position;
            curIdleActInfo.needToChangeSide = wp.needsToForceSide;
            curIdleActInfo.neededSide = wp.forcedSide;

            curMoveActInfo = new MoveActInfo();

            curMoveActInfo.pos = wp.transform.position;
            curMoveActInfo.moveSpeed = fastMoveSpeedCoef * normalMoveSpeed;
            curMoveActInfo.animSpeed = fastMoveSpeedCoef;
            curMoveActInfo.animName = animName_Walk;

            SetNPCActState(NPCActState.MOVE_START);

            SetCivilDecisionState(CivilDecisionState.GO_TO_ALARM_UPDATE);
        }
        #endregion

        #region GO_TO_ALARM_UPDATE
        if (IsCivilDecisionState(CivilDecisionState.GO_TO_ALARM_UPDATE))
        {
            if (IsMurderingSeen())
            {
                Player.Instance.SetDetected();
            }

            if (IsNPCActState(NPCActState.WAIT_UPDATE))
            {
                SetCivilDecisionState(CivilDecisionState.ALARM_START);
            }
        }
        #endregion

        #region ALARM_START
        if (IsCivilDecisionState(CivilDecisionState.ALARM_START))
        {
            MapManager.Instance.SetAlarmIsBusy(false);

            if (selectedCorpseToReport != null)
            {
                selectedCorpseToReport.SetIdentified();
                selectedCorpseToReport = null;
            }

            CallPolice();

            SetCivilDecisionState(CivilDecisionState.SELECT_NEW_WP);
        }
        #endregion

        #region TRAP_FREEZE_START
        if (IsCivilDecisionState(CivilDecisionState.TRAP_FREEZE_START))
        {
            SetSelectedWaypoint(null);

            SetNPCActStateNeedsToBeFinished(true);

            SetCivilDecisionState(CivilDecisionState.TRAP_FREEZE_UPDATE);
        }
        #endregion

        #region TRAP_FREEZE_UPDATE
        if (IsCivilDecisionState(CivilDecisionState.TRAP_FREEZE_UPDATE))
        {
            if (IsNPCActState(NPCActState.WAIT_UPDATE))
            {
                curIdleActInfo = new IdleActInfo();

                curIdleActInfo.animName = freezingTrap.freezeNPCAnimName;
                curIdleActInfo.animSpeed = 1;
                curIdleActInfo.selectedTime = 1000;
                curIdleActInfo.pos = transform.position;
                curIdleActInfo.needToChangeSide = false;

                SetNPCActState(NPCActState.IDLE_START);

                SetCivilDecisionState(CivilDecisionState.TRAP_FREEZE_FINISH);
            }
        }
        #endregion
    }

    public void SetCivilDecisionState(CivilDecisionState _state)
    {
        civilDecisionState = _state;
    }

    public bool IsCivilDecisionState(CivilDecisionState _state)
    {
        return civilDecisionState == _state;
    }

    public Waypoint SelectNewRandomWaypoint()
    {
        okWaypoints.Clear();

        for (int i = 0; i < MapManager.Instance.civilWaypoints.Count; i++)
        {
            if (!MapManager.Instance.civilWaypoints[i].isBusy && MapManager.Instance.civilWaypoints[i].isPublic)
            {
                okWaypoints.Add(MapManager.Instance.civilWaypoints[i]);
            }
        }

        return okWaypoints[Random.Range(0, okWaypoints.Count)];
    }

    public bool IsMurderingSeen()
    {
        if (MapManager.Instance.IsPlayerDetected())
            return false;

        if (amIGettingDiedByPlayer)
            return false;

        if (Player.Instance.isMurdering && !Player.Instance.IsInRageMode())
        {
            return IsPlayerSeen();
        }

        return false;
    }

    public Waypoint SelectWaypointForFlee()
    {
        List<int> indices = new List<int>();

        for (int i = 0; i < MapManager.Instance.civilWaypoints.Count; i++)
        {
            if (!MapManager.Instance.civilWaypoints[i].isBusy && MapManager.Instance.civilWaypoints[i].isPublic)
            {
                indices.Add(i);
            }
        }

        //

        t_SectionRate.Clear();
        t_SortedSectionRate.Clear();

        float minSectionTotalDistToCorpse = float.PositiveInfinity;
        float maxSectionTotalDistToCorpse = 0;

        for (int i = 0; i < indices.Count; i++)
        {
            if (MapManager.Instance.civilWaypoints[indices[i]].ownerMapSection == null)
            {
                continue;
            }

            bool shouldSkip = false;

            for (int j = 0; j < t_SectionRate.Count; j++)
            {
                if (MapManager.Instance.civilWaypoints[indices[i]].ownerMapSection == t_SectionRate[j].section)
                {
                    shouldSkip = true;
                    continue;
                }
            }

            if (shouldSkip)
                continue;


            SectionRate sr = new SectionRate();

            sr.section = MapManager.Instance.civilWaypoints[indices[i]].ownerMapSection;

            //Calculate dist to corpse

            float distToCorpse = 1;

            if (null != newCorpse)
                distToCorpse = Vector3.Distance(sr.section.transform.position, newCorpse.transform.position);

            sr.totalDistToNewCorpses = distToCorpse;

            if (sr.totalDistToNewCorpses > maxSectionTotalDistToCorpse)
                maxSectionTotalDistToCorpse = sr.totalDistToNewCorpses;

            if (sr.totalDistToNewCorpses < minSectionTotalDistToCorpse)
                minSectionTotalDistToCorpse = sr.totalDistToNewCorpses;

            t_SectionRate.Add(sr);
        }

        #region Apply ratings

        for (int i = 0; i < t_SectionRate.Count; i++)
        {
            //Dist to new corpses
            if (maxSectionTotalDistToCorpse != 0)
                t_SectionRate[i].rating *= ((1 - sectionRatingMaxDecreasementCoef_DistToNewCorpse) + (sectionRatingMaxDecreasementCoef_DistToNewCorpse * (((t_SectionRate[i].totalDistToNewCorpses - minSectionTotalDistToCorpse) / (maxSectionTotalDistToCorpse - minSectionTotalDistToCorpse)))));

            //Containing corpses
            for (int j = 0; j < MapManager.Instance.allCorpses.Count; j++)
            {
                if ((null != MapManager.Instance.allCorpses[j]) && (t_SectionRate[i].section.ContainsPos(MapManager.Instance.allCorpses[j].transform.position)))
                {
                    t_SectionRate[i].rating *= (1 - sectionRatingMaxDecreasementCoef_ContainsCorpse);
                    break;
                }
            }

            //Random
            t_SectionRate[i].rating *= (1 + Random.Range(-sectionRatingRandomCoef, sectionRatingRandomCoef));
        }

        #endregion

        while (t_SectionRate.Count > 0)
        {
            float maxRating = -1;
            int selectedInd = -1;


            for (int i = 0; i < t_SectionRate.Count; i++)
            {
                if (t_SectionRate[i].rating > maxRating)
                {
                    maxRating = t_SectionRate[i].rating;
                    selectedInd = i;
                }
            }

            t_SortedSectionRate.Add(t_SectionRate[selectedInd]);
            t_SectionRate.RemoveAt(selectedInd);
        }

        //for (int i = 0; i < numOfWaypointsForPatrol; i++)
        //{
        //    int keyfin = Random.Range(0, indices.Count);
        //    int index = indices[keyfin];
        //    indices.RemoveAt(keyfin);

        //    finalWaypoints.Add(MapManager.Instance.policeWaypoints[index]);
        //}

        for (int i = 0; i < t_SortedSectionRate.Count; i++)
        {
            t_WaypointsList.Clear();
            t_WaypointsList = t_SortedSectionRate[i].section.GetFree_Civil_Waypoints();

            if (t_WaypointsList.Count > 0)
            {
                return t_SortedSectionRate[i].section.GetARandomFree_Civil_Waypoint();
            }
        }

        return null;
    }

    public void CallPolice()
    {
        MapManager.Instance.SetPoliceIsPresent(true);
        AudioManager.Instance.Play(SoundType.Game_PoliceEnter, false);
    }

    public override void KillMe(GameObject _Killer, EntityType _killerType)
    {
        if (selectedCorpseToReport != null)
        {
            MapManager.Instance.SetAlarmIsBusy(false);
            selectedCorpseToReport = null;
        }

        base.KillMe(_Killer, _killerType);
    }

    public override void SetFreezedForTrap(Trap _trap, float _killingDelay)
    {
        if (isTrapFreezed)
            return;

        base.SetFreezedForTrap(_trap, _killingDelay);

        SetCivilDecisionState(CivilDecisionState.TRAP_FREEZE_START);
    }
}
