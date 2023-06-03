using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ChangerType
{
    Vertical,
    Horizontal,
}

public enum ChangerMoveAnimSide
{
    MidBack,
    MidFr,
    Prev,
    Next,
}

public class Changer : MonoBehaviour
{
    public delegate void ChangerHandler();
    public event ChangerHandler Event_ChangeAnimStartedByHand;

    public delegate void ChangerOnChange(Changer _changer, int _curIndex);
    public event ChangerOnChange Event_OnChangeCurIndex;

    //

    public string myName;
    public ChangerType changerType = ChangerType.Vertical;
    public List<GUIGroup> childs = new List<GUIGroup>();

    [HideInInspector]
    public int currentIndex = -1;

    [HideInInspector]
    public bool isFingerMoving = false;

    [HideInInspector]
    public GUIGroup pressingChildGroup = null;

    [HideInInspector]
    TouchUpInfo tap_Info;

    [HideInInspector]
    TouchUpInfo movingFinger_Info;

    [HideInInspector]
    public bool isBusy = false;

    [HideInInspector]
    public bool wasFingerMovingLastUpdate = false;

    bool currentIndexIsChanged = false;

    float moveDistCoef_ToNext = 1;
    float moveDistCoef_ToPrev = 1;
    float sidesMoveDistCoef = 0.35f;
    float moveDist = 0;
    float moveLerpSpeed = 10f;
    float touchSpeed = 0;


    float minMoveDistToChangeIndex = 1.5f;
    float minTouchSpeedToChangeIndex = 5f;

    bool animMove = false;
    GUIGroup animMoveCurSideChild;
    float animMoveBaseLerpSpeed = 27f;
    float animMoveLerpSpeedDistMakhraj_Horiz = 40;
    float animMoveTotalLerpSpeed = 1f;
    float animMoveLerpError = 0.02f;
    float animMoveLerpAdditionalSize = 0.05f;

    float animMove_CurLerpAdditionalSize = 0;

    float animMoveCurMidChildTarget = 0;
    float animMoveSideChildTarget = 0;

    int animMoveLaterMidInd = 0;

    bool animMove_ShouldChangeCurActiveChilds = false;


    GUIGroup curChild_Mid = null;
    GUIGroup curChild_Prev = null;
    GUIGroup curChild_Nex = null;

    BusyTouchInfo busyTouchInfo = new BusyTouchInfo();

    bool isFirstIndexSet = true;

    [HideInInspector]
    public ChangerMoveAnimSide sideOfAnim;



    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (changerType == ChangerType.Horizontal)
        {
            #region IsFingerMoving

            if (isFingerMoving)
            {
                float touchDist = (movingFinger_Info.pos_End.x - movingFinger_Info.pos_Start.x);

                if (touchDist > 0)
                    moveDist = moveDistCoef_ToPrev * touchDist;
                else
                    moveDist = moveDistCoef_ToNext * touchDist;

                float moveSpeed = moveLerpSpeed * Time.deltaTime;

                #region Mid

                if (curChild_Mid != null)
                {
                    if (!curChild_Mid.isVisible)
                    {
                        curChild_Mid.isVisible = true;
                        curChild_Mid.ChangeVisibility();
                    }

                    curChild_Mid.x = Mathf.Lerp(curChild_Mid.x, moveDist, moveSpeed);
                    curChild_Mid.ChangeX();
                }

                #endregion

                #region Prev
                if (curChild_Prev != null)
                {
                    if (curChild_Mid.x > 0)
                    {

                        if (!curChild_Prev.isVisible)
                        {
                            curChild_Prev.isVisible = true;
                            curChild_Prev.ChangeVisibility();
                        }

                        curChild_Prev.x = curChild_Mid.x - MyGUIController.Instance.curScreen_Width;
                        curChild_Prev.ChangeX();
                    }
                    else
                    {
                        if (curChild_Prev.isVisible)
                        {
                            curChild_Prev.isVisible = false;
                            curChild_Prev.ChangeVisibility();

                            curChild_Prev.x = -MyGUIController.Instance.curScreen_Width;
                            curChild_Prev.ChangeX();
                        }
                    }
                }
                #endregion

                #region Nex
                if (curChild_Nex != null)
                {
                    if (curChild_Mid.x < 0)
                    {

                        if (!curChild_Nex.isVisible)
                        {
                            curChild_Nex.isVisible = true;
                            curChild_Nex.ChangeVisibility();
                        }

                        curChild_Nex.x = curChild_Mid.x + MyGUIController.Instance.curScreen_Width;
                        curChild_Nex.ChangeX();
                    }
                    else
                    {
                        if (curChild_Nex.isVisible)
                        {
                            curChild_Nex.isVisible = false;
                            curChild_Nex.ChangeVisibility();

                            curChild_Nex.x = MyGUIController.Instance.curScreen_Width;
                            curChild_Nex.ChangeX();
                        }
                    }
                }
                #endregion

            }
            else
            {
                if (Mathf.Abs(moveDist) > 0)
                {
                    ChooseAndStartAnimMoveForCurrentSituation();
                    moveDist = 0;
                }
            }

            #endregion

            #region AnimMove

            if (animMove)
            {


                if (!curChild_Mid.isVisible)
                {
                    curChild_Mid.isVisible = true;
                    curChild_Mid.ChangeVisibility();
                }

                curChild_Mid.x = Mathf.Lerp(curChild_Mid.x, animMoveCurMidChildTarget + animMove_CurLerpAdditionalSize, animMoveTotalLerpSpeed * Time.deltaTime);
                curChild_Mid.ChangeX();

                //

                if (animMoveCurSideChild)
                {
                    if (!animMoveCurSideChild.isVisible)
                    {
                        animMoveCurSideChild.isVisible = true;
                        animMoveCurSideChild.ChangeVisibility();
                    }

                    animMoveCurSideChild.x = Mathf.Lerp(animMoveCurSideChild.x, animMoveSideChildTarget + animMove_CurLerpAdditionalSize, animMoveTotalLerpSpeed * Time.deltaTime);
                    animMoveCurSideChild.ChangeX();
                }

                //

                //if (St_ExtraMath.Dist(curChild_Mid.x, animMoveCurMidChildTarget) <= animMoveLerpError)
                //{
                //    EndAnimMove();
                //}

                if (sideOfAnim == ChangerMoveAnimSide.Next || sideOfAnim == ChangerMoveAnimSide.MidBack)
                {
                    if (curChild_Mid.x <= animMoveCurMidChildTarget)
                        EndAnimMove();
                }
                else
                {
                    if (curChild_Mid.x >= animMoveCurMidChildTarget)
                        EndAnimMove();
                }
            }

            #endregion
        }

        wasFingerMovingLastUpdate = isFingerMoving;

        isFingerMoving = false;
    }

    public void Init()
    {
        for (int i = 0; i < childs.Count; i++)
        {
            MyGUIController.Instance.currentGUIGroups.Add(childs[i]);

            childs[i].SetChanger(this);
        }

        SetCurrentIndex(0);
    }

    public void OnMovingFinger()
    {
        if (isBusy)
            return;

        movingFinger_Info = new TouchUpInfo();

        movingFinger_Info.time_Start = MyGUIController.Instance.menuMode_CurTouch.time_Start;
        movingFinger_Info.time_Cur = Time.time;
        movingFinger_Info.time_End = MyGUIController.Instance.menuMode_CurTouch.time_End;
        movingFinger_Info.pos_Start = MyGUIController.Instance.menuMode_CurTouch.pos_Start;
        movingFinger_Info.pos_End = MyGUIController.Instance.menuMode_CurTouch.pos_End;

        isFingerMoving = true;
    }

    public void StartAnimMove(ChangerMoveAnimSide _side, int _laterMidIndex, bool _byHand)
    {
        if (!curChild_Mid)
            return;


        bool byHand = _byHand;
        sideOfAnim = _side;
        animMoveLaterMidInd = _laterMidIndex;

        animMove = true;

        MyGUIController.Instance.AddDisablingTouchInfo(busyTouchInfo);

        float moveDistAbs = Mathf.Abs(moveDist);

        animMoveCurSideChild = GetChildByIndex(animMoveLaterMidInd);

        animMove_ShouldChangeCurActiveChilds = false;

        if (changerType == ChangerType.Horizontal)
        {
            animMoveTotalLerpSpeed = animMoveBaseLerpSpeed * (1 - (moveDistAbs / (animMoveLerpSpeedDistMakhraj_Horiz + moveDistAbs)));

            switch (sideOfAnim)
            {
                case ChangerMoveAnimSide.MidBack:

                    animMoveCurMidChildTarget = 0;
                    animMoveSideChildTarget = -MyGUIController.Instance.curScreen_Width;
                    animMove_CurLerpAdditionalSize = -animMoveLerpAdditionalSize;

                    break;

                case ChangerMoveAnimSide.MidFr:

                    animMoveCurMidChildTarget = 0;
                    animMoveSideChildTarget = MyGUIController.Instance.curScreen_Width;
                    animMove_CurLerpAdditionalSize = +animMoveLerpAdditionalSize;

                    break;

                case ChangerMoveAnimSide.Prev:

                    if (byHand)
                        TryDoEvent_ChangeAnimStartedByHand();

                    if (animMoveCurSideChild)
                    {
                        if (!byHand)
                        {
                            animMoveCurSideChild.x = -MyGUIController.Instance.curScreen_Width;
                            animMoveCurSideChild.ChangeX();
                        }

                        animMove_CurLerpAdditionalSize = animMoveLerpAdditionalSize;
                        animMoveCurMidChildTarget = MyGUIController.Instance.curScreen_Width;
                        animMoveSideChildTarget = 0;
                        animMove_ShouldChangeCurActiveChilds = true;
                    }
                    else
                    {
                        animMove_CurLerpAdditionalSize = animMoveLerpAdditionalSize;
                        animMoveCurMidChildTarget = 0;
                    }

                    break;

                case ChangerMoveAnimSide.Next:

                    if (byHand)
                        TryDoEvent_ChangeAnimStartedByHand();

                    if (animMoveCurSideChild)
                    {
                        if (!byHand)
                        {
                            animMoveCurSideChild.x = MyGUIController.Instance.curScreen_Width;
                            animMoveCurSideChild.ChangeX();
                        }

                        animMove_CurLerpAdditionalSize = -animMoveLerpAdditionalSize;
                        animMoveCurMidChildTarget = -MyGUIController.Instance.curScreen_Width;
                        animMoveSideChildTarget = 0;
                        animMove_ShouldChangeCurActiveChilds = true;
                    }
                    else
                    {
                        animMove_CurLerpAdditionalSize = -animMoveLerpAdditionalSize;
                        animMoveCurMidChildTarget = 0;
                    }

                    break;
            }
        }
    }

    void EndAnimMove()
    {
        animMove = false;

        MyGUIController.Instance.RemoveDisablingTouchInfo(busyTouchInfo);

        if (changerType == ChangerType.Horizontal)
        {
            curChild_Mid.x = animMoveCurMidChildTarget;
            curChild_Mid.ChangeX();

            if (animMoveCurSideChild)
            {
                animMoveCurSideChild.x = animMoveSideChildTarget;
                animMoveCurSideChild.ChangeX();
            }
        }

        if (animMove_ShouldChangeCurActiveChilds)
        {
            if (curChild_Mid.isVisible)
            {
                curChild_Mid.isVisible = false;
                curChild_Mid.ChangeVisibility();
            }

            SetCurrentIndex(animMoveLaterMidInd);
        }
        else
        {
            if (animMoveCurSideChild)
            {
                if (animMoveCurSideChild.isVisible)
                {
                    animMoveCurSideChild.isVisible = false;
                    animMoveCurSideChild.ChangeVisibility();
                }
            }
        }

    }

    GUIGroup GetChildByIndex(int _ind)
    {
        int ind = _ind;

        if (childs.Count == 0)
            return null;

        if (ind > childs.Count - 1)
            return null;

        if (ind < 0)
            return null;

        return childs[ind];
    }

    void ChooseAndStartAnimMoveForCurrentSituation()
    {
        if (changerType == ChangerType.Horizontal)
        {
            touchSpeed = moveDist / (movingFinger_Info.time_End - movingFinger_Info.time_Start);

            if ((curChild_Prev != null) && ((moveDist >= minMoveDistToChangeIndex) || (touchSpeed >= minTouchSpeedToChangeIndex)))
            {
                StartAnimMove(ChangerMoveAnimSide.Prev, currentIndex - 1, true);
                return;
            }

            if ((curChild_Nex != null) && ((moveDist <= -minMoveDistToChangeIndex) || (touchSpeed <= -minTouchSpeedToChangeIndex)))
            {
                StartAnimMove(ChangerMoveAnimSide.Next, currentIndex + 1, true);
                return;
            }

            if (moveDist > 0)
            {
                StartAnimMove(ChangerMoveAnimSide.MidBack, currentIndex - 1, true);
            }
            else
            {
                StartAnimMove(ChangerMoveAnimSide.MidFr, currentIndex + 1, true);
            }
        }
    }

    void SetCurrentIndex(int _ind)
    {
        int oldIndex = currentIndex;

        currentIndex = _ind;

        if (Event_OnChangeCurIndex != null)
            Event_OnChangeCurIndex(this, currentIndex);

        //if (oldIndex != currentIndex)
        //    SetCurrentIndexIsChangedOnThisFrame();

        moveDistCoef_ToNext = 1;
        moveDistCoef_ToPrev = 1;

        for (int i = 0; i < childs.Count; i++)
        {
            if (childs[i].isVisible)
            {
                childs[i].isVisible = false;

                if (!isFirstIndexSet)
                    childs[i].ChangeVisibility();
            }

        }

        curChild_Mid = GetChildByIndex(currentIndex);

        if (curChild_Mid)
        {
            curChild_Mid.isVisible = true;

            if (!isFirstIndexSet)
                curChild_Mid.ChangeVisibility();

            if (changerType == ChangerType.Horizontal)
            {
                curChild_Mid.x = 0;

                if (!isFirstIndexSet)
                    curChild_Mid.ChangeX();
            }

            if (currentIndex == 0)
            {
                moveDistCoef_ToPrev = sidesMoveDistCoef;
            }

            if (currentIndex == childs.Count - 1)
            {
                moveDistCoef_ToNext = sidesMoveDistCoef;
            }
        }

        //

        curChild_Nex = GetChildByIndex(currentIndex + 1);

        if (curChild_Nex)
        {
            if (changerType == ChangerType.Horizontal)
            {
                curChild_Nex.x = MyGUIController.Instance.curScreen_Width;

                if (!isFirstIndexSet)
                    curChild_Nex.ChangeX();
            }
        }

        //

        curChild_Prev = GetChildByIndex(currentIndex - 1);

        if (curChild_Prev)
        {
            if (changerType == ChangerType.Horizontal)
            {
                curChild_Prev.x = -MyGUIController.Instance.curScreen_Width;

                if (!isFirstIndexSet)
                    curChild_Prev.ChangeX();
            }
        }

        isFirstIndexSet = false;
    }

    public GUIGroup GetCurChild()
    {
        return childs[currentIndex];
    }

    //

    public void Hide()
    {
        for (int i = 0; i < childs.Count; i++)
        {
            childs[i].animVis = false;
            childs[i].ChangeVisibility();
        }
    }

    public void Show()
    {
        for (int i = 0; i < childs.Count; i++)
        {
            childs[i].animVis = true;
            childs[i].ChangeVisibility();
        }
    }

    public void TryDoEvent_ChangeAnimStartedByHand()
    {
        if (Event_ChangeAnimStartedByHand != null)
        {
            Event_ChangeAnimStartedByHand();
        }
    }
}
