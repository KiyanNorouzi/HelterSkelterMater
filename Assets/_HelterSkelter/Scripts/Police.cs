using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#region Police state
public enum PoliceState
{
    START,
    WAIT_FOR_ALARM,
    COMING_START,
    SEARCHING_START,
    SEARCHING_UPDATE,
    SEARCHING_FINISH,
    CHECK_FOR_MORE_CORPSE,
    PATROLLING_INIT,
    PATROLLING_UPDATE,
    PATROLLING_END,
    EXIT_INIT,
    EXIT_UPDATE,
    EXIT_END,
    PLAYER_DETECTED_INIT,
    PLAYER_DETECTED_UPDATE,
    PLAYER_DETECTED_END,
    STOP_ACT_AND_WAIT__TO__PLAYER_DETECTED__INIT,
    STOP_ACT_AND_WAIT__TO__PLAYER_DETECTED__UPDATE,
    STOP_ACT_AND_WAIT__TO__SEARCHING__INIT,
    STOP_ACT_AND_WAIT__TO__SEARCHING__UPDATE,
    KEYFIN,
    TRAP_FREEZE_START,
    TRAP_FREEZE_UPDATE,
    TRAP_FREEZE_FINISH,
}
#endregion

#region Police sense state
public enum PoliceSenseState
{
    SENSING_START,
    SENSING_UPDATE,
    SENSING_END,
    PLAYER_DETECTED_START,
    PLAYER_DETECTED_UPDATE,
    PLAYER_DETECTED_END,
    WAIT_START,
    WAIT_UPDATE,
    WAIT_END,
    KEYFIN,
}
#endregion


class SectionRate
{
    public MapSection section;
    public float rating = 1;
    public float totalDistToNewCorpses = 0;
    public float distToPlayer = 0;
}

public class Police : NPC
{
    List<Waypoint> selectedWaypointsForPatroll = new List<Waypoint>();
    Vector3 lastCourpsePos;
    int curPatrolWPIndex;
    bool isNewCorpseDetected = false;

    [HideInInspector]
    public PoliceState policeState = PoliceState.WAIT_FOR_ALARM;
    [HideInInspector]
    public PoliceSenseState policeSenseState = PoliceSenseState.WAIT_START;

    Corpse newCorpse;
    Corpse curBestCorpseToReport;

    string animName_ReportCorpse = "Report";
    float maxReportTime = 1f;

    string animName_BustPlayer = "Bust";
    float maxBustTime = 0.5f;

    public int numOfWaypointsForPatrol_Min = 1;
    public int numOfWaypointsForPatrol_Max = 3;

    List<SectionRate> t_SectionRate = new List<SectionRate>();
    List<SectionRate> t_SortedSectionRate = new List<SectionRate>();

    List<Vector3> t_NewCorpsePosesInThisSession = new List<Vector3>();



    float sectionRatingMaxDecreasementCoef_DistToNewCorpses = 0.4f;
    float sectionRatingMaxDecreasementCoef_ContainsPlayer = 0.12f;
    float sectionRatingMaxDecreasementCoef_DistantToPlayer = 0.18f;
    float sectionRatingRandomCoef = 0.12f;

    void Awake()
    {
        PixObj_Awake();
    }

    public override void PixObj_Awake()
    {
        base.PixObj_Awake();

        MapManager.Instance.Event_CorpseDetected += CorpseDetected;
    }

    void Update()
    {
        PixObj_Update();
    }

    public override void PixObj_Update()
    {
        base.PixObj_Update();

    StartSteps:

        #region Police Main States

        #region START
        if (PoliceIsState(PoliceState.START))
        {
            //print(SelectWaypointsForPatrol().Count);

            PoliceSetState(PoliceState.WAIT_FOR_ALARM);
        }
        #endregion

        #region WAIT_FOR_ALARM
        if (PoliceIsState(PoliceState.WAIT_FOR_ALARM))
        {
            //SelectWaypointsForPatrol();

            if (MapManager.Instance.IsPolicePresent())
            {
                MapManager.Instance.SetAlarmIsBusy(false);

                PoliceSetState(PoliceState.COMING_START);
            }
        }
        #endregion

        #region COMING_START
        if (PoliceIsState(PoliceState.COMING_START))
        {
            t_NewCorpsePosesInThisSession.Clear();

            for (int i = 0; i < MapManager.Instance.NotReportedIdentifiedCorpses.Count; i++)
            {
                t_NewCorpsePosesInThisSession.Add(MapManager.Instance.NotReportedIdentifiedCorpses[i].transform.position);
            }

            PoliceSetState(PoliceState.SEARCHING_START);
        }
        #endregion


        #region SEARCHING_START
        if (PoliceIsState(PoliceState.SEARCHING_START))
        {
            if (MapManager.Instance.IsPlayerDetected())
            {
                PoliceSetState(PoliceState.PLAYER_DETECTED_INIT);
                goto StartSteps;
            }

            curBestCorpseToReport = SelectBestCorpseToReport();

            curIdleActInfo = new IdleActInfo();

            curIdleActInfo.animName = animName_ReportCorpse;
            curIdleActInfo.animSpeed = 1;
            curIdleActInfo.selectedTime = maxReportTime;
            curIdleActInfo.pos = curBestCorpseToReport.transform.position;
            curIdleActInfo.needToChangeSide = false;

            curMoveActInfo = new MoveActInfo();

            curMoveActInfo.pos = curBestCorpseToReport.transform.position;
            curMoveActInfo.moveSpeed = normalMoveSpeed;
            curMoveActInfo.animSpeed = 1;
            curMoveActInfo.animName = animName_Walk;

            SetNPCActState(NPCActState.MOVE_START);

            PoliceSetState(PoliceState.SEARCHING_UPDATE);
        }
        #endregion

        #region SEARCHING_UPDATE
        if (PoliceIsState(PoliceState.SEARCHING_UPDATE))
        {
            if (MapManager.Instance.IsPlayerDetected())
            {
                PoliceSetState(PoliceState.STOP_ACT_AND_WAIT__TO__PLAYER_DETECTED__INIT);
                goto StartSteps;
            }

            if (IsNPCActState(NPCActState.WAIT_UPDATE))
            {
                PoliceSetState(PoliceState.SEARCHING_FINISH);
            }
        }
        #endregion

        #region SEARCHING_FINISH
        if (PoliceIsState(PoliceState.SEARCHING_FINISH))
        {
            isNewCorpseDetected = false;

            CleanCourpse(curBestCorpseToReport);

            PoliceSetState(PoliceState.CHECK_FOR_MORE_CORPSE);
        }
        #endregion


        #region CHECK_FOR_MORE_CORPSE
        if (PoliceIsState(PoliceState.CHECK_FOR_MORE_CORPSE))
        {
            if (MapManager.Instance.NotReportedIdentifiedCorpses.Count != 0)
            {
                PoliceSetState(PoliceState.SEARCHING_START);
                goto StartSteps;
            }
            else
            {
                PoliceSetState(PoliceState.PATROLLING_INIT);
            }
        }
        #endregion


        #region STOP_ACT_AND_WAIT__TO__PLAYER_DETECTED__INIT
        if (PoliceIsState(PoliceState.STOP_ACT_AND_WAIT__TO__PLAYER_DETECTED__INIT))
        {
            SetNPCActStateNeedsToBeFinished(true);
            PoliceSetState(PoliceState.STOP_ACT_AND_WAIT__TO__PLAYER_DETECTED__UPDATE);
        }
        #endregion

        #region STOP_ACT_AND_WAIT__TO__PLAYER_DETECTED__UPDATE
        if (PoliceIsState(PoliceState.STOP_ACT_AND_WAIT__TO__PLAYER_DETECTED__UPDATE))
        {
            if (IsNPCActState(NPCActState.WAIT_UPDATE))
            {
                PoliceSetState(PoliceState.PLAYER_DETECTED_INIT);
            }
        }
        #endregion


        #region PATROLLING_INIT
        if (PoliceIsState(PoliceState.PATROLLING_INIT))
        {
            curPatrolWPIndex = -1;
            selectedWaypointsForPatroll = SelectWaypointsForPatrol();

            //t_NewCorpsesInThisSession.Clear();

            PoliceSetState(PoliceState.PATROLLING_UPDATE);
        }
        #endregion

        #region PATROLLING_UPDATE
        if (PoliceIsState(PoliceState.PATROLLING_UPDATE))
        {
            if (MapManager.Instance.IsPlayerDetected())
            {
                PoliceSetState(PoliceState.STOP_ACT_AND_WAIT__TO__PLAYER_DETECTED__INIT);
                goto StartSteps;
            }

            if (isNewCorpseDetected)
            {
                PoliceSetState(PoliceState.STOP_ACT_AND_WAIT__TO__SEARCHING__INIT);
                goto StartSteps;
            }

            if (IsNPCActState(NPCActState.WAIT_UPDATE))
            {
                curPatrolWPIndex++;

                if (curPatrolWPIndex < selectedWaypointsForPatroll.Count)
                {
                    Waypoint wp = selectedWaypointsForPatroll[curPatrolWPIndex];
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
                }
                else
                {
                    PoliceSetState(PoliceState.PATROLLING_END);
                    goto StartSteps;
                }
            }
        }
        #endregion

        #region PATROLLING_END
        if (PoliceIsState(PoliceState.PATROLLING_END))
        {
            PoliceSetState(PoliceState.EXIT_INIT);
        }
        #endregion


        #region STOP_ACT_AND_WAIT__TO__SEARCHING__INIT
        if (PoliceIsState(PoliceState.STOP_ACT_AND_WAIT__TO__SEARCHING__INIT))
        {
            SetNPCActStateNeedsToBeFinished(true);
            PoliceSetState(PoliceState.STOP_ACT_AND_WAIT__TO__SEARCHING__UPDATE);
        }
        #endregion

        #region STOP_ACT_AND_WAIT__TO__SEARCHING__UPDATE
        if (PoliceIsState(PoliceState.STOP_ACT_AND_WAIT__TO__SEARCHING__UPDATE))
        {
            if (IsNPCActState(NPCActState.WAIT_UPDATE))
            {
                PoliceSetState(PoliceState.SEARCHING_START);
            }
        }
        #endregion


        #region EXIT_INIT
        if (PoliceIsState(PoliceState.EXIT_INIT))
        {
            Waypoint wp = MapManager.Instance.policeStartWaypoint;
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

            PoliceSetState(PoliceState.EXIT_UPDATE);
        }
        #endregion

        #region EXIT_UPDATE
        if (PoliceIsState(PoliceState.EXIT_UPDATE))
        {
            if (MapManager.Instance.IsPlayerDetected())
            {
                PoliceSetState(PoliceState.STOP_ACT_AND_WAIT__TO__PLAYER_DETECTED__INIT);
                goto StartSteps;
            }

            if (isNewCorpseDetected)
            {
                PoliceSetState(PoliceState.STOP_ACT_AND_WAIT__TO__SEARCHING__INIT);
                goto StartSteps;
            }

            if (IsNPCActState(NPCActState.WAIT_UPDATE))
            {
                PoliceSetState(PoliceState.EXIT_END);
            }
        }
        #endregion

        #region EXIT_END
        if (PoliceIsState(PoliceState.EXIT_END))
        {
            MapManager.Instance.SetPoliceIsPresent(false);
            PoliceSetState(PoliceState.WAIT_FOR_ALARM);
        }
        #endregion


        #region PLAYER_DETECTED_INIT
        if (PoliceIsState(PoliceState.PLAYER_DETECTED_INIT))
        {
            curIdleActInfo = new IdleActInfo();

            curIdleActInfo.animName = animName_BustPlayer;
            curIdleActInfo.animSpeed = 1;
            curIdleActInfo.selectedTime = maxBustTime;
            curIdleActInfo.pos = GetPosToKillPlayer();
            curIdleActInfo.needToChangeSide = false;

            curMoveActInfo = new MoveActInfo();

            curMoveActInfo.pos = curIdleActInfo.pos;
            curMoveActInfo.moveSpeed = fastMoveSpeedCoef * normalMoveSpeed;
            curMoveActInfo.animSpeed = fastMoveSpeedCoef;
            curMoveActInfo.animName = animName_Walk;

            AudioManager.Instance.Play(SoundType.Game_PoliceFreeze, false);

            SetNPCActState(NPCActState.MOVE_START);

            PoliceSetState(PoliceState.PLAYER_DETECTED_UPDATE);
        }
        #endregion

        #region PLAYER_DETECTED_UPDATE
        if (PoliceIsState(PoliceState.PLAYER_DETECTED_UPDATE))
        {
            if (IsNPCActState(NPCActState.WAIT_UPDATE))
            {
                PoliceSetState(PoliceState.PLAYER_DETECTED_END);
            }
        }
        #endregion

        #region PLAYER_DETECTED_END
        if (PoliceIsState(PoliceState.PLAYER_DETECTED_END))
        {
            MapManager.Instance.SetLevelIsFailed();
            PoliceSetState(PoliceState.KEYFIN);
        }
        #endregion

        #region TRAP_FREEZE_START
        if (PoliceIsState(PoliceState.TRAP_FREEZE_START))
        {
            SetSelectedWaypoint(null);

            SetNPCActStateNeedsToBeFinished(true);

            PoliceSetState(PoliceState.TRAP_FREEZE_UPDATE);
        }
        #endregion

        #region TRAP_FREEZE_UPDATE
        if (PoliceIsState(PoliceState.TRAP_FREEZE_UPDATE))
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

                PoliceSetState(PoliceState.TRAP_FREEZE_FINISH);
            }
        }
        #endregion

        #endregion

        #region Sensing States

        #region WAIT_START
        if (PoliceIsSenseState(PoliceSenseState.WAIT_START))
        {
            PoliceSetSenseState(PoliceSenseState.WAIT_UPDATE);
        }
        #endregion

        #region WAIT_UPDATE
        if (PoliceIsSenseState(PoliceSenseState.WAIT_UPDATE))
        {
            if (MapManager.Instance.IsPolicePresent())
                PoliceSetSenseState(PoliceSenseState.WAIT_END);
        }
        #endregion

        #region WAIT_END
        if (PoliceIsSenseState(PoliceSenseState.WAIT_END))
        {
            PoliceSetSenseState(PoliceSenseState.SENSING_START);
        }
        #endregion

        #region SENSING_START
        if (PoliceIsSenseState(PoliceSenseState.SENSING_START))
        {
            PoliceSetSenseState(PoliceSenseState.SENSING_UPDATE);
        }
        #endregion

        #region SENSING_UPDATE
        if (PoliceIsSenseState(PoliceSenseState.SENSING_UPDATE))
        {
            CheckSeePlayer();

            if (IsAnyNewCorpseSeen(out newCorpse))
            {
                if (!knownCorpses.Contains(newCorpse))
                {
                    AddCorpseToKnownCorpsesList(newCorpse);
                    newCorpse.SetIdentified();
                }
            }

            if (MapManager.Instance.IsPlayerDetected() || !MapManager.Instance.IsPolicePresent())
            {
                PoliceSetSenseState(PoliceSenseState.SENSING_END);
            }
        }
        #endregion

        #region SENSING_END
        if (PoliceIsSenseState(PoliceSenseState.SENSING_END))
        {
            if (MapManager.Instance.IsPlayerDetected())
            {
                PoliceSetSenseState(PoliceSenseState.PLAYER_DETECTED_START);
            }
            else
            {
                PoliceSetSenseState(PoliceSenseState.WAIT_START);
            }
        }
        #endregion

        #region PLAYER_DETECTED_START
        if (PoliceIsSenseState(PoliceSenseState.PLAYER_DETECTED_START))
        {

            PoliceSetSenseState(PoliceSenseState.PLAYER_DETECTED_UPDATE);
        }
        #endregion

        #endregion
    }

    void CheckSeePlayer()
    {
        if (MapManager.Instance.IsPlayerDetected())
            return;

        if (Player.Instance.IsInRageMode())
            return;

        if (IsPlayerSeen())
            Player.Instance.SetDetected();
    }

    public void CleanCourpse(Corpse _corpse)
    {
        Corpse corpse = _corpse;

        corpse.SetShouldBeRemoved();
        MapManager.Instance.NotReportedIdentifiedCorpses.Remove(corpse);
    }

    public void CorpseDetected(Corpse _corpse)
    {
        AddCorpseToKnownCorpsesList(newCorpse);

        t_NewCorpsePosesInThisSession.Add(_corpse.transform.position);

        isNewCorpseDetected = true;
    }

    public Vector3 GetPosToKillPlayer()
    {
        return Player.Instance.transform.position;
    }

    List<Waypoint> SelectWaypointsForPatrol()
    {
        List<int> indices = new List<int>();

        List<Waypoint> finalWaypoints = new List<Waypoint>();

        int numOfWaypointsForPatrol = Random.Range(numOfWaypointsForPatrol_Min, numOfWaypointsForPatrol_Max + 1);

        //

        for (int i = 0; i < MapManager.Instance.policeWaypoints.Count; i++)
        {
            if (!MapManager.Instance.policeWaypoints[i].isBusy && MapManager.Instance.policeWaypoints[i].isPublic)
            {
                indices.Add(i);
            }
        }

        //

        t_SectionRate.Clear();
        t_SortedSectionRate.Clear();

        float minSectionTotalDistToNewCorpses = float.PositiveInfinity;
        float maxSectionTotalDistToNewCorpses = 0;

        float minSectionDistToPlayer = float.PositiveInfinity;
        float maxSectionDistToPlayer = 0;

        for (int i = 0; i < indices.Count; i++)
        {
            if (MapManager.Instance.policeWaypoints[indices[i]].ownerMapSection == null)
            {
                continue;
            }

            bool shouldSkip = false;

            for (int j = 0; j < t_SectionRate.Count; j++)
            {
                if (MapManager.Instance.policeWaypoints[indices[i]].ownerMapSection == t_SectionRate[j].section)
                {
                    shouldSkip = true;
                    continue;
                }
            }

            if (shouldSkip)
                continue;


            SectionRate sr = new SectionRate();

            sr.section = MapManager.Instance.policeWaypoints[indices[i]].ownerMapSection;

            //Calculate dist to new corpses

            float totalDistToCorpses = 0;

            for (int j = 0; j < t_NewCorpsePosesInThisSession.Count; j++)
            {
                totalDistToCorpses += Vector3.Distance(sr.section.transform.position, t_NewCorpsePosesInThisSession[j]);
            }

            sr.totalDistToNewCorpses = totalDistToCorpses;

            if (sr.totalDistToNewCorpses > maxSectionTotalDistToNewCorpses)
                maxSectionTotalDistToNewCorpses = sr.totalDistToNewCorpses;

            if (sr.totalDistToNewCorpses < minSectionTotalDistToNewCorpses)
                minSectionTotalDistToNewCorpses = sr.totalDistToNewCorpses;

            //Calculate dist to player

            float distToPlayer = Vector3.Distance(sr.section.transform.position, Player.Instance.transform.position);

            sr.distToPlayer = distToPlayer;

            if (sr.distToPlayer > maxSectionDistToPlayer)
                maxSectionDistToPlayer = sr.distToPlayer;

            if (sr.distToPlayer < minSectionDistToPlayer)
                minSectionDistToPlayer = sr.distToPlayer;

            //

            t_SectionRate.Add(sr);
        }

        #region Apply ratings

        for (int i = 0; i < t_SectionRate.Count; i++)
        {
            //Dist to new corpses
            if (maxSectionTotalDistToNewCorpses != 0)
                t_SectionRate[i].rating *= ((1 - sectionRatingMaxDecreasementCoef_DistToNewCorpses) + (sectionRatingMaxDecreasementCoef_DistToNewCorpses * (1 - ((t_SectionRate[i].totalDistToNewCorpses - minSectionTotalDistToNewCorpses) / (maxSectionTotalDistToNewCorpses - minSectionTotalDistToNewCorpses)))));

            //print(t_SectionRate[i].section.ToString() + " rating (After dist to corpses): " + t_SectionRate[i].rating);


            //Dist to player
            t_SectionRate[i].rating *= ((1 - sectionRatingMaxDecreasementCoef_DistantToPlayer) + (sectionRatingMaxDecreasementCoef_DistantToPlayer * (1 - ((t_SectionRate[i].distToPlayer - minSectionDistToPlayer) / (maxSectionDistToPlayer - minSectionDistToPlayer)))));

            //print(t_SectionRate[i].section.ToString() + " rating (After dist to player): " + t_SectionRate[i].rating);

            //Containing player
            if (!t_SectionRate[i].section.ContainsPos(Player.Instance.transform.position))
            {
                t_SectionRate[i].rating *= (1 - sectionRatingMaxDecreasementCoef_ContainsPlayer);
            }

            //print(t_SectionRate[i].section.ToString() + " rating (After containing player): " + t_SectionRate[i].rating);

            //Random
            t_SectionRate[i].rating *= (1 + Random.Range(-sectionRatingRandomCoef, sectionRatingRandomCoef));

            //print(t_SectionRate[i].section.ToString() + " rating (After random): " + t_SectionRate[i].rating);
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

        for (int i = 0; i < numOfWaypointsForPatrol; i++)
        {
            finalWaypoints.Add(t_SortedSectionRate[i].section.GetARandomFree_Police_Waypoint());
        }

        return finalWaypoints;
    }

    Corpse SelectBestCorpseToReport()
    {
        if (MapManager.Instance.NotReportedIdentifiedCorpses.Count == 0)
            return null;

        int selectedIndex = -1;
        float distanceToPolice = float.PositiveInfinity;

        for (int i = 0; i < MapManager.Instance.NotReportedIdentifiedCorpses.Count; i++)
        {
            float dist = Vector3.Distance(MapManager.Instance.NotReportedIdentifiedCorpses[i].transform.position, transform.position);

            if (dist < distanceToPolice)
            {
                selectedIndex = i;
                distanceToPolice = dist;
            }
        }

        return MapManager.Instance.NotReportedIdentifiedCorpses[selectedIndex];
    }

    public override void SetFreezedForTrap(Trap _trap, float _killingDelay)
    {
        if (isTrapFreezed)
            return;

        base.SetFreezedForTrap(_trap, _killingDelay);

        PoliceSetState(PoliceState.TRAP_FREEZE_START);

        PoliceSetSenseState(PoliceSenseState.KEYFIN);
    }

    public override void KillMe(GameObject _Killer, EntityType _killerType)
    {
        if (!MapManager.Instance.IsPlayerDetected())
            MapManager.Instance.SetPoliceIsPresent(false);

        MapManager.Instance.ShouldRecreatePolice();

        base.KillMe(_Killer, _killerType);
    }

    #region Police state functions

    void PoliceSetState(PoliceState _state)
    {
        policeState = _state;
    }

    bool PoliceIsState(PoliceState _state)
    {
        return policeState == _state;
    }

    void PoliceSetSenseState(PoliceSenseState _state)
    {
        policeSenseState = _state;
    }

    bool PoliceIsSenseState(PoliceSenseState _state)
    {
        return policeSenseState == _state;
    }

    #endregion
}