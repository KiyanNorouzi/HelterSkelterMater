using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PlayerState
{
    IDLE_START,
    IDLE_UPDATE,
    IDLE_FINISH,
    MOVEMENT_START,
    MOVEMENT_UPDATE,
    MOVEMENT_FINISH,
    Knife_START,
    Knife_UPDATE,
    Knife_FINISH,
    TRAP_START,
    TRAP_UPDATE,
    TRAP_FINISH,
    HIDDEN_START,
    HIDDEN_UPDATE,
    HIDDEN_FINISH,
    HIDEOUT_START,
    HIDEOUT_UPDATE,
    HIDEOUT_FINISH,
    RAGE_IDLE_START,
    RAGE_IDLE_UPDATE,
    RAGE_IDLE_FINISH,
    RAGE_MOVEMENT_START,
    RAGE_MOVEMENT_UPDATE,
    RAGE_MOVEMENT_FINISH,
    RAGE_FINISH,
}
public enum IdentificationState
{
    FREEZE_START,
    FREEZE_UPDATE
}
public enum Sikiminhierarchical
{
    PLAYER_NOT_DETECTED_START,
    PLAYER_NOT_DETECTED_UPDATE,
    WAIT_FOR_CHILD_FINISH,
    PLAYER_NOT_DETECTED_END,
    PLAYER_DETECTED_START,
    PLAYER_DETECTED_UPDATE,
}
public enum PlayerSenseState
{
    NORNAL_START,
    NORMAL_UPDATE
}

public class Player : Character
{
    public static Player Instance;

    public List<Transform> raycastTargetPoses = new List<Transform>();

    NPC selectedVictim;
    Trap selectedTrap;
    Hideout selectedHideout;

    float moveSpeedMax = 65f;
    float minMoveSpeedCoef = 0.1f;

    float curRageModeMovementSpeedCoef = 1;

    Vector3 curMoveSpeedCoef = Vector3.zero;
    Vector3 curMoveSpeed = Vector3.zero;
    Vector3 curMoveDist = Vector3.zero;

    [HideInInspector]
    public bool needsToBeFinished = false;

    PlayerState state = PlayerState.IDLE_START;
    IdentificationState identification = IdentificationState.FREEZE_START;
    Sikiminhierarchical sikim = Sikiminhierarchical.PLAYER_NOT_DETECTED_START;
    PlayerSenseState senseState = PlayerSenseState.NORNAL_START;

    bool isKillButtonPressed = false;
    bool isTrapButtonPressed = false;
    bool isHideoutButtonPressed = false;
    bool isGetOutButtonPressed = false;

    [HideInInspector]
    public bool isMurdering = false;

    [HideInInspector]
    public bool isRaycastable = true;

    float playerStateTimeCounter = 0;
    float killingDelayMaxTime = 0.25f;

    [HideInInspector]
    public string animName_Kill = "Kill";

    [HideInInspector]
    public string animName_Hideout = "Hideout";

    [HideInInspector]
    public string animName_Trap = "Trap";

    [HideInInspector]
    public string animName_RageModeIdle = "Idle";

    [HideInInspector]
    public string animName_RageModeMove = "Walk";


    Rigidbody2D myRigid2D;

    bool mustMove = false;
    Vector3 moveSpeed;

    bool isInRageMode = false;
    bool shouldFinishRageMode = false;

    MeleeWeapon curMeleeWeapon;


    void Awake()
    {
        PixObj_Awake();

        PlayerRageBar.Instance.Event_RageModeStarted += CheckEvent_StartRageMode;
        PlayerRageBar.Instance.Event_RageModeStopped += CheckEvent_FinishRageMode;
    }

    void Update()
    {
        PixObj_Update();
    }

    public override void PixObj_Awake()
    {
        base.PixObj_Awake();

        Instance = this;

        myRigid2D = GetComponent<Rigidbody2D>();
        //print("");
    }

    public override void PixObj_Update()
    {
        base.PixObj_Update();

        #region States

    BeginStates:

        #region IDLE_START
        if (PlayerIsState(PlayerState.IDLE_START))
        {
            if (isInRageMode)
            {
                PlayerSetState(PlayerState.RAGE_IDLE_START);
                goto BeginStates;
            }

            PlayAnimForCurSide(animName_Idle);
            PlayerSetState(PlayerState.IDLE_UPDATE);
        }
        #endregion

        #region IDLE_UPDATE
        if (PlayerIsState(PlayerState.IDLE_UPDATE))
        {
            if(isInRageMode)
            {
                PlayerSetState(PlayerState.RAGE_IDLE_START);
                goto BeginStates;
            }

            if (isKillButtonPressed)
            {
                PlayerSetState(PlayerState.Knife_START);
                goto BeginStates;
            }

            if (isTrapButtonPressed)
            {
                PlayerSetState(PlayerState.TRAP_START);
                goto BeginStates;
            }

            if (isHideoutButtonPressed)
            {
                PlayerSetState(PlayerState.HIDEOUT_START);
                goto BeginStates;
            }

            if (IsJoypad_Pressed())
            {
                PlayerSetState(PlayerState.IDLE_FINISH);
            }

        }
        #endregion

        #region IDLE_FINISH
        if (PlayerIsState(PlayerState.IDLE_FINISH))
        {
            if (IsJoypad_Pressed())
            {
                PlayerSetState(PlayerState.MOVEMENT_START);
            }
        }
        #endregion


        #region MOVEMENT_START
        if (PlayerIsState(PlayerState.MOVEMENT_START))
        {
            if (isInRageMode)
            {
                PlayerSetState(PlayerState.RAGE_IDLE_START);
                goto BeginStates;
            }

            PlayAnimForCurSide(animName_Walk);
            PlayerSetState(PlayerState.MOVEMENT_UPDATE);
        }
        #endregion

        #region MOVEMENT_UPDATE
        if (PlayerIsState(PlayerState.MOVEMENT_UPDATE))
        {
            if (isInRageMode)
            {
                PlayerSetState(PlayerState.RAGE_IDLE_START);
                goto BeginStates;
            }

            if (isKillButtonPressed)
            {
                PlayerSetState(PlayerState.Knife_START);
                goto BeginStates;
            }

            if (isTrapButtonPressed)
            {
                PlayerSetState(PlayerState.TRAP_START);
                goto BeginStates;
            }

            if (isHideoutButtonPressed)
            {
                PlayerSetState(PlayerState.HIDEOUT_START);
                goto BeginStates;
            }

            if (IsJoypad_Pressed())
            {
                curMoveSpeedCoef = MultiTouchManager.Instance.joypadAxis;
                curMoveSpeed = curMoveSpeedCoef * curRageModeMovementSpeedCoef * moveSpeedMax;
                curMoveDist = curMoveSpeed * Time.deltaTime;

                //print(curMoveDist);

                PlayAnimForCurSide(animName_Walk);

                Movement(curMoveSpeed);
            }
            else
            {
                PlayerSetState(PlayerState.MOVEMENT_FINISH);
            }
        }
        #endregion

        #region MOVEMENT_FINISH
        if (PlayerIsState(PlayerState.MOVEMENT_FINISH))
        {
            curMoveSpeedCoef = Vector3.zero;
            curMoveSpeed = Vector3.zero;
            curMoveDist = Vector3.zero;

            PlayerSetState(PlayerState.IDLE_START);
        }
        #endregion


        #region Knife_START
        if (PlayerIsState(PlayerState.Knife_START))
        {
            SetKillButtonPressed(false);

            if (selectedVictim != null)
            {
                selectedVictim.SetItsGettingDiedByPlayer();
            }

            MultiTouchManager.Instance.SetTapButtonsEnablility(false);
            MultiTouchManager.Instance.SetJoystickEnablility(false);

            PlayAnimForCurSide(animName_Kill);

            playerStateTimeCounter = killingDelayMaxTime;

            AudioManager.Instance.Play(SoundType.Game_Knife, false);

            PlayerSetState(PlayerState.Knife_UPDATE);
        }
        #endregion

        #region Knife_UPDATE
        if (PlayerIsState(PlayerState.Knife_UPDATE))
        {
            playerStateTimeCounter = MathFPlus.DecDeltaTimeToZero(playerStateTimeCounter);

            if (playerStateTimeCounter == 0)
            {
                playerStateTimeCounter = 10000000f;

                if (selectedVictim != null)
                {
                    selectedVictim.KillMe(gameObject, EntityType.Player);
                    isMurdering = true;
                }
            }

            if (anim2DController.status == AnimControllerStatus.finished)
            {
                PlayerSetState(PlayerState.Knife_FINISH);
                goto EndStates;
            }
        }
        #endregion

        #region Knife_FINISH
        if (PlayerIsState(PlayerState.Knife_FINISH))
        {
            isMurdering = false;

            MultiTouchManager.Instance.SetTapButtonsEnablility(true);
            MultiTouchManager.Instance.SetJoystickEnablility(true);
            isMurdering = false;
            PlayerSetState(PlayerState.IDLE_START);
        }
        #endregion


        #region TRAP_START
        if (PlayerIsState(PlayerState.TRAP_START))
        {
            SetTrapButtonPressed(false);

            MultiTouchManager.Instance.SetTapButtonsEnablility(false);
            MultiTouchManager.Instance.SetJoystickEnablility(false);

            PlayAnimForCurSide(animName_Trap);
            PlayerSetState(PlayerState.TRAP_UPDATE);
        }
        #endregion

        #region TRAP_UPDATE
        if (PlayerIsState(PlayerState.TRAP_UPDATE))
        {
            if (anim2DController.status == AnimControllerStatus.finished)
                PlayerSetState(PlayerState.TRAP_FINISH);
        }
        #endregion

        #region TRAP_FINISH
        if (PlayerIsState(PlayerState.TRAP_FINISH))
        {
            MultiTouchManager.Instance.SetTapButtonsEnablility(true);
            MultiTouchManager.Instance.SetJoystickEnablility(true);

            selectedTrap.TrapActivation();

            PlayerSetState(PlayerState.IDLE_START);
        }
        #endregion


        #region HIDEOUT_START
        if (PlayerIsState(PlayerState.HIDEOUT_START))
        {
            SetHideoutButtonPressed(false);

            MultiTouchManager.Instance.SetTapButtonsEnablility(false);
            MultiTouchManager.Instance.SetJoystickEnablility(false);

            if (selectedHideout.shouldForceSide)
                SetSide(selectedHideout.forcedSide);

            PlayAnimForCurSide(animName_Hideout);

            PlayerSetState(PlayerState.HIDEOUT_UPDATE);
        }
        #endregion

        #region HIDEOUT_UPDATE
        if (PlayerIsState(PlayerState.HIDEOUT_UPDATE))
        {
            if (MapManager.Instance.IsPlayerDetected()) // <<<<<<<<<< Fot Test
            {
                PlayerSetState(PlayerState.IDLE_START);
                goto BeginStates;
            }

            if (anim2DController.status == AnimControllerStatus.finished)
                PlayerSetState(PlayerState.HIDEOUT_FINISH);
        }
        #endregion

        #region HIDEOUT_FINISH
        if (PlayerIsState(PlayerState.HIDEOUT_FINISH))
        {
            PlayerSetState(PlayerState.HIDDEN_START);
        }
        #endregion


        #region HIDDEN_START
        if (PlayerIsState(PlayerState.HIDDEN_START))
        {
            selectedHideout.StartHide();

            SetIsRaycastable(false);
            SetVisible(false);

            PlayerSetState(PlayerState.HIDDEN_UPDATE);
        }
        #endregion

        #region HIDDEN_UPDATE
        if (PlayerIsState(PlayerState.HIDDEN_UPDATE))
        {
            if (isGetOutButtonPressed)
                PlayerSetState(PlayerState.HIDDEN_FINISH);
        }
        #endregion

        #region HIDDEN_FINISH
        if (PlayerIsState(PlayerState.HIDDEN_FINISH))
        {
            SetGetOutButtonPressed(false);

            SetIsRaycastable(true);
            SetVisible(true);

            MultiTouchManager.Instance.SetTapButtonsEnablility(true);
            MultiTouchManager.Instance.SetJoystickEnablility(true);

            PlayerTrigger.Instance.RemoveHideoutNearObjIfExistsInNearList(selectedHideout);
            PlayerTrigger.Instance.AddToList(selectedHideout.myObjTrigger);

            PlayerSetState(PlayerState.IDLE_START);
        }
        #endregion


        #region RAGE_IDLE_START
        if (PlayerIsState(PlayerState.RAGE_IDLE_START))
        {
            MultiTouchManager.Instance.SetTapButtonsEnablility(false);
            SetIsRaycastable(false);

            PlayAnimForCurSide(animName_RageModeIdle);

            curMeleeWeapon = currentWeapon as MeleeWeapon;

            curMeleeWeapon.SetActive(true);
            curMeleeWeapon.TryFire();

            PlayerSetState(PlayerState.RAGE_IDLE_UPDATE);
        }
        #endregion

        #region RAGE_IDLE_UPDATE
        if (PlayerIsState(PlayerState.RAGE_IDLE_UPDATE))
        {
            if (shouldFinishRageMode || IsJoypad_Pressed())
            {
                PlayerSetState(PlayerState.RAGE_IDLE_FINISH);
            }
        }
        #endregion

        #region RAGE_IDLE_FINISH
        if (PlayerIsState(PlayerState.RAGE_IDLE_FINISH))
        {
            if (shouldFinishRageMode)
            {
                PlayerSetState(PlayerState.RAGE_FINISH);
                goto BeginStates;
            }

            if (IsJoypad_Pressed())
            {
                PlayerSetState(PlayerState.RAGE_MOVEMENT_START);
            }
        }
        #endregion


        #region RAGE_MOVEMENT_START
        if (PlayerIsState(PlayerState.RAGE_MOVEMENT_START))
        {
            PlayAnimForCurSide(animName_RageModeIdle);
            PlayerSetState(PlayerState.RAGE_MOVEMENT_UPDATE);
        }
        #endregion

        #region RAGE_MOVEMENT_UPDATE
        if (PlayerIsState(PlayerState.RAGE_MOVEMENT_UPDATE))
        {
            if(shouldFinishRageMode)
            {
                PlayerSetState(PlayerState.RAGE_MOVEMENT_FINISH);
                goto BeginStates;
            }

            if (IsJoypad_Pressed())
            {
                curMoveSpeedCoef = MultiTouchManager.Instance.joypadAxis;
                curMoveSpeed = curMoveSpeedCoef * curRageModeMovementSpeedCoef * moveSpeedMax;
                curMoveDist = curMoveSpeed * Time.deltaTime;

                //print(curMoveDist);

                PlayAnimForCurSide(animName_RageModeMove);

                Movement(curMoveSpeed);
            }
            else
            {
                PlayerSetState(PlayerState.RAGE_MOVEMENT_FINISH);
            }
        }
        #endregion

        #region RAGE_MOVEMENT_FINISH
        if (PlayerIsState(PlayerState.RAGE_MOVEMENT_FINISH))
        {
            curMoveSpeedCoef = Vector3.zero;
            curMoveSpeed = Vector3.zero;
            curMoveDist = Vector3.zero;

            if (shouldFinishRageMode)
            {
                PlayerSetState(PlayerState.RAGE_FINISH);
                goto BeginStates;
            }

            PlayerSetState(PlayerState.RAGE_IDLE_START);
        }
        #endregion


        #region RAGE_FINISH
        if (PlayerIsState(PlayerState.RAGE_FINISH))
        {
            curMoveSpeedCoef = Vector3.zero;
            curMoveSpeed = Vector3.zero;
            curMoveDist = Vector3.zero;

            shouldFinishRageMode = false;
            isInRageMode = false;

            curMeleeWeapon.StopFire();
            curMeleeWeapon.SetActive(false);

            MultiTouchManager.Instance.SetTapButtonsEnablility(true);

            SetIsRaycastable(true);

            PlayerSetState(PlayerState.IDLE_START);
        }
        #endregion


    EndStates: ;

        #endregion
    }


    void Movement(Vector3 _moveSpeed)
    {
        mustMove = true;

        //MoveMotion = Vector3.Lerp(myRigid2D.position, (myRigid2D.position + new Vector2(_Motion.x, _Motion.y)), Time.deltaTime * 50f);

        moveSpeed = _moveSpeed;
    }

    void FixedUpdate()
    {
        if (mustMove)
        {
            myRigid2D.MovePosition(myRigid2D.position + new Vector2(moveSpeed.x, moveSpeed.y) * Time.fixedDeltaTime);

            SetSideByVec(moveSpeed);

            mustMove = false;
        }
    }

    public void SetIdentified()
    {

    }

    void SetIsRaycastable(bool _val)
    {
        isRaycastable = _val;
    }

    #region Player states function
    void PlayerSetState(PlayerState _state)
    {
        state = _state;
    }
    bool PlayerIsState(PlayerState _state)
    {
        return state == _state;
    }
    #endregion

    #region KillMode:
    bool KillMode(PlayerState _State)
    {
        return state == _State;
    }
    #endregion

    #region Identification states function
    void Identification_SetState(IdentificationState _state)
    {
        identification = _state;
    }
    bool Identification_IsState(IdentificationState _state)
    {
        return identification == _state;
    }
    #endregion

    #region Sikiminhierarchical
    void Sikiminhierarchical_SetState(Sikiminhierarchical _state)
    {
        sikim = _state;
    }
    bool Sikiminhierarchical_IsState(Sikiminhierarchical _state)
    {
        return sikim == _state;
    }
    #endregion

    #region Player sense states function
    void PlayerSense_SetState(PlayerSenseState _state)
    {
        senseState = _state;
    }
    bool PlayerSense_IsState(PlayerSenseState _state)
    {
        return senseState == _state;
    }
    #endregion

    bool IsJoypad_Pressed()
    {
        return MultiTouchManager.Instance.joypadAxis.magnitude > minMoveSpeedCoef;
    }

    public void SetKillButtonPressed(bool _val)
    {
        isKillButtonPressed = _val;
    }

    public void SetTrapButtonPressed(bool _val)
    {
        isTrapButtonPressed = _val;
    }

    public void SetHideoutButtonPressed(bool _val)
    {
        isHideoutButtonPressed = _val;
    }

    public void SetGetOutButtonPressed(bool _val)
    {
        isGetOutButtonPressed = _val;
    }

    public void SetSelectedVictim(NPC _victim)
    {
        selectedVictim = _victim;
    }

    public void SetSelectedTrap(Trap _trap)
    {
        selectedTrap = _trap;
    }

    public void SetSelectedHideout(Hideout _hideout)
    {
        selectedHideout = _hideout;
    }

    public void SetDetected()
    {
        MapManager.Instance.SetPlayerIsDetected();
    }

    void CheckEvent_StartRageMode()
    {
        isInRageMode = true;
    }

    void CheckEvent_FinishRageMode()
    {
        shouldFinishRageMode = true;
    }

    public bool IsInRageMode()
    {
        return isInRageMode;
    }

    public void SetRageModeMoveSpeedCoef(float _val)
    {
        curRageModeMovementSpeedCoef = _val;
    }
}
