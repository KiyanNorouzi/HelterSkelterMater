using UnityEngine;
using System.Collections;


public class PlayerForTest : Character
{
    float minMoveSpeedCoef = 0.2f;

    Vector3 curMoveSpeedCoef = Vector3.zero;

    Vector3 curMoveSpeed = Vector3.zero;
    Vector3 curMoveDist = Vector3.zero;
    float maxMoveSpeed = 50f;

    CharacterController characterController;

    [HideInInspector]
    public bool needsToBeFinished = false;

    PlayerState state = PlayerState.IDLE_START;
    IdentificationState identification = IdentificationState.FREEZE_START;
    Sikiminhierarchical sikim = Sikiminhierarchical.PLAYER_NOT_DETECTED_START;
    PlayerSenseState senseState = PlayerSenseState.NORNAL_START;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Start()
    {

    }

    void Update()
    {
        if (Mathf.Abs(MultiTouchManager.Instance.joypadAxis.x) > minMoveSpeedCoef || Mathf.Abs(MultiTouchManager.Instance.joypadAxis.y) > minMoveSpeedCoef)
        {
            curMoveSpeedCoef = new Vector3(MultiTouchManager.Instance.joypadAxis.x, MultiTouchManager.Instance.joypadAxis.y, 0);
            curMoveSpeed = curMoveSpeedCoef * maxMoveSpeed;
            curMoveDist = curMoveSpeed * Time.deltaTime;

            Move(curMoveDist);

            anim2DController.PlayAnim(GetAnimNameForCurSide(animName_Walk));
        }
        else
        {
            curMoveSpeedCoef = Vector3.zero;
            curMoveSpeed = Vector3.zero;
            curMoveDist = Vector3.zero;

            anim2DController.PlayAnim(GetAnimNameForCurSide(animName_Idle));
        }
    }

    public void Move(Vector3 _moveDist)
    {
        Vector3 moveDist = _moveDist;

        characterController.Move(new Vector3(moveDist.x, moveDist.y));
        SetSideByVec(moveDist);

    }



    void PlayerMove()
    {

    }


    void SetIdentified()
    {

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
}
