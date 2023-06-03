using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum NPCActState
{
    WAIT_START,
    WAIT_UPDATE,
    MOVE_START,
    MOVE_UPDATE,
    MOVE_FINISH,
    IDLE_START,
    IDLE_UPDATE,
    IDLE_FINISH,
}

public class NPC : Character
{
    public List<Transform> raycastPosesTr;

    protected IdleActInfo curIdleActInfo;
    protected MoveActInfo curMoveActInfo;
    protected float actionTimeCounter = 0;
    protected bool isMovingDestReached = false;
    protected NPCActState npcActState = NPCActState.WAIT_START;
    protected bool actStateNeedsToBeFinished = false;
    protected float normalMoveSpeed = 37f;
    protected float fastMoveSpeedCoef = 2f;

    float moveDestMaxError = 3f;

    protected Waypoint selectedWaypoint = null;
    protected List<Waypoint> okWaypoints = new List<Waypoint>();

    PolyNavAgent navAgent;

    Side oldSide;

    protected float maxViewHalfAngle = 80;
    protected Vector3 viewVector = new Vector3(0, 0, 0);
    protected float maxViewRange = 100f;

    protected RaycastHit2D[] hitResults = new RaycastHit2D[10];

    protected List<Corpse> knownCorpses = new List<Corpse>();

    protected bool amIGettingDiedByPlayer = false;

    protected bool isTrapFreezed = false;

    protected Trap freezingTrap = null;

    protected bool canCheckSeeNewCorpses = true;

    protected bool canCheckSeePlayer = true;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void PixObj_Update()
    {
        base.PixObj_Update();

        #region Act FSM

        #region WAIT_START
        if (IsNPCActState(NPCActState.WAIT_START))
        {
            SetNPCActStateNeedsToBeFinished(false);
            SetNPCActState(NPCActState.WAIT_UPDATE);
        }
        #endregion

        #region WAIT_UPDATE
        if (IsNPCActState(NPCActState.WAIT_UPDATE))
        {

        }
        #endregion

        #region MOVE_START
        if (IsNPCActState(NPCActState.MOVE_START))
        {
            oldSide = side;
            PlayAnimForCurSide(curMoveActInfo.animName, curMoveActInfo.animSpeed);
            navAgent.maxSpeed = curMoveActInfo.moveSpeed;
            navAgent.SetDestination(curMoveActInfo.pos);
            isMovingDestReached = false;

            SetNPCActState(NPCActState.MOVE_UPDATE);
        }
        #endregion

        #region MOVE_UPDATE
        if (IsNPCActState(NPCActState.MOVE_UPDATE))
        {
            //TESTTTTTTT

            if (Vector3.Magnitude(transform.position - curMoveActInfo.pos) < moveDestMaxError)
                OnDestinationReached();

            //~TESTTTTTTT

            if (isMovingDestReached || actStateNeedsToBeFinished)
            {
                SetNPCActState(NPCActState.MOVE_FINISH);
            }
            else
            {
                SetSideByVec(new Vector3(navAgent.nextPoint.x, navAgent.nextPoint.y, 0) - transform.position);

                if (oldSide != side)
                {
                    PlayAnimForCurSide(curMoveActInfo.animName, curMoveActInfo.animSpeed);
                }

                oldSide = side;
            }
        }
        #endregion

        #region MOVE_FINISH
        if (IsNPCActState(NPCActState.MOVE_FINISH))
        {
            navAgent.Stop();
            anim2DController.Stop();

            if (actStateNeedsToBeFinished)
                SetNPCActState(NPCActState.WAIT_START);
            else
                SetNPCActState(NPCActState.IDLE_START);
        }
        #endregion

        #region IDLE_START
        if (IsNPCActState(NPCActState.IDLE_START))
        {
            //transform.position = curIdleActInfo.pos;

            if (curIdleActInfo.needToChangeSide)
            {
                SetSide(curIdleActInfo.neededSide);
            }

            PlayAnimForCurSide(curIdleActInfo.animName, curIdleActInfo.animSpeed);

            actionTimeCounter = curIdleActInfo.selectedTime;

            SetNPCActState(NPCActState.IDLE_UPDATE);
        }
        #endregion

        #region IDLE_UPDATE
        if (IsNPCActState(NPCActState.IDLE_UPDATE))
        {
            actionTimeCounter = MathFPlus.DecDeltaTimeToZero(actionTimeCounter);

            if (actionTimeCounter == 0 || actStateNeedsToBeFinished)
            {
                SetNPCActState(NPCActState.IDLE_FINISH);
            }
        }
        #endregion

        #region IDLE_FINISH
        if (IsNPCActState(NPCActState.IDLE_FINISH))
        {
            anim2DController.Stop();
            actionTimeCounter = 0;
            SetNPCActState(NPCActState.WAIT_START);
        }
        #endregion

        #endregion
    }

    public override void PixObj_Awake()
    {
        base.PixObj_Awake();

        navAgent = GetComponent<PolyNavAgent>();
        navAgent.SetMessageReceiver(this.gameObject);
    }

    public void SetNPCActState(NPCActState _state)
    {
        npcActState = _state;
    }

    public bool IsNPCActState(NPCActState _state)
    {
        return npcActState == _state;
    }

    public void OnDestinationReached()
    {
        isMovingDestReached = true;
    }

    public void SetNPCActStateNeedsToBeFinished(bool _val)
    {
        actStateNeedsToBeFinished = _val;
    }

    public override void SetSide(Side _side) //<<< For Test : Make it right.
    {
        base.SetSide(_side);

        side = _side;

        switch (side)
        {
            case Side.DOWN:
                viewVector = Vector3.down;
                break;

            case Side.LEFT:
            case Side.LEFT_DOWN:
            case Side.LEFT_UP:
                viewVector = Vector3.left;
                break;

            case Side.RIGHT:
            case Side.RIGHT_DOWN:
            case Side.RIGHT_UP:
                viewVector = Vector3.right;
                break;

            case Side.UP:
                viewVector = Vector3.up;
                break;

            //case Side.RIGHT_UP:
            //    viewVector = new Vector3(1,1,0);
            //    break;

            //case Side.RIGHT_DOWN:
            //    viewVector = new Vector3(1, -1, 0);
            //    break;

            //case Side.LEFT_UP:
            //    viewVector = new Vector3(-1, 1, 0);
            //    break;

            //case Side.LEFT_DOWN:
            //    viewVector = new Vector3(-1, -1, 0);
            //    break;
        }
    }

    public void SetSelectedWaypoint(Waypoint _waypoint)
    {
        if (selectedWaypoint != null)
            selectedWaypoint.isBusy = false;

        selectedWaypoint = _waypoint;

        SetWaypointIsBusy(selectedWaypoint, true);
    }

    public void SetWaypointIsBusy(Waypoint _wayPoint, bool _isBusy)
    {
        if (_wayPoint == null)
            return;

        bool isBusy = _isBusy;
        Waypoint waypoint = _wayPoint;

        if (isBusy)
        {
            waypoint.isBusy = true;
        }
        else
        {
            waypoint.isBusy = false;
        }
    }

    public override void KillMe(GameObject _Killer, EntityType _killerType)
    {
        if (selectedWaypoint != null)
        {
            SetWaypointIsBusy(selectedWaypoint, false);
        }

        PlayerTrigger.Instance.RemoveNPCNearObjIfExistsInNearList(this);

        if(!PlayerRageBar.Instance.IsInRageMode)
        {
            PlayerRageBar.Instance.AddRageOfKill();
        }

        base.KillMe(_Killer, _killerType);
    }

    public bool IsAnyNewCorpseSeen(out Corpse corpse)
    {
        if (!CanCheckSeeNewCorpses())
        {
            corpse = null;
            return false;
        }

        for (int i = 0; i < MapManager.Instance.allCorpses.Count; i++)
        {
            if (MapManager.Instance.allCorpses[i] == null)
                continue;

            if (MapManager.Instance.allCorpses[i].isIdentified)
                continue;

            for (int r = 0; r < raycastPosesTr.Count; r++)
            {
                if (MathFPlus.IsPointGenerallyInView(raycastPosesTr[r].position, MapManager.Instance.allCorpses[i].transform.position, maxViewRange, viewVector, maxViewHalfAngle))
                {
                    for (int j = 0; j < MapManager.Instance.allCorpses[i].raycastTargetPoses.Count; j++)
                    {
                        if (IsRaycastHitToTarget(raycastPosesTr[r].position, MapManager.Instance.allCorpses[i].raycastTargetPoses[j].position, maxViewRange, MapManager.Instance.rayCastLayer_Corpse, MapManager.corpseRayColTag))
                        {
                            corpse = MapManager.Instance.allCorpses[i];

                            return true;
                        }
                    }
                }
            }

        }

        corpse = null;
        return false;
    }

    public bool IsRaycastHitToTarget(Vector3 _startPoint, Vector3 _endPoint, float _maxDist, LayerMask _hitLayer, string _targetTag)
    {
        float maxDist = _maxDist;
        Vector2 origin = new Vector2(_startPoint.x, _startPoint.y);
        Vector2 dir = new Vector2(_endPoint.x - _startPoint.x, _endPoint.y - _startPoint.y);
        LayerMask hitLayer = _hitLayer;
        string targetTag = _targetTag;

        int hitCount = Physics2D.RaycastNonAlloc(origin, dir, hitResults, maxDist, hitLayer);

        for (int i = 0; i < hitCount; i++)
        {
            if (hitResults[i].collider == null)
                continue;

            if (hitResults[i].collider.gameObject == null)
                continue;

            if (hitResults[i].collider.gameObject == gameObject)
                continue;

            return hitResults[i].collider.gameObject.tag == targetTag;
        }

        return false;
    }

    public void AddCorpseToKnownCorpsesList(Corpse _corpse)
    {
        //Corpse corpse = _corpse;

        //if (corpse == null)
        //    return;

        //if (!knownCorpses.Contains(corpse))
        //    knownCorpses.Add(corpse);
    }

    public void TryRemoveCorpseFromKnownCorpsesList(Corpse _corpse)
    {
        Corpse corpse = _corpse;

        if (corpse == null)
            return;

        if (knownCorpses.Contains(corpse))
        {
            knownCorpses.Remove(corpse);
        }
    }

    public void SetItsGettingDiedByPlayer()
    {
        amIGettingDiedByPlayer = true;
    }

    public bool IsPlayerSeen()
    {
        if (!CanCheckSeePlayer())
        {
            return false;
        }

        if (!Player.Instance.isRaycastable)
            return false;

        for (int r = 0; r < raycastPosesTr.Count; r++)
        {
            if (MathFPlus.IsPointGenerallyInView(raycastPosesTr[r].position, Player.Instance.transform.position, maxViewRange, viewVector, maxViewHalfAngle))
            {
                for (int j = 0; j < Player.Instance.raycastTargetPoses.Count; j++)
                {
                    if (IsRaycastHitToTarget(raycastPosesTr[r].position, Player.Instance.raycastTargetPoses[j].position, maxViewRange, MapManager.Instance.rayCastLayer_Player, MapManager.playerRayColTag))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public virtual void SetFreezedForTrap(Trap _trap, float _killingDelay)
    {
        if (isTrapFreezed)
            return;

        isTrapFreezed = true;

        freezingTrap = _trap;

        canCheckSeePlayer = false;
        canCheckSeeNewCorpses = false;

        KillMe(_trap.gameObject, EntityType.Trap, _killingDelay);
    }

    bool CanCheckSeeNewCorpses()
    {
        return canCheckSeeNewCorpses;
    }

    bool CanCheckSeePlayer()
    {
        return canCheckSeePlayer;
    }
}


