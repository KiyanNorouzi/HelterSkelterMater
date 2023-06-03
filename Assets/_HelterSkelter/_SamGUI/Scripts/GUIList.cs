using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ListChildType
{
    Item,
    Own_Back,
    Own_Front,
}

public enum ListType
{
    Vertical,
    Horizontal,
}

public class TimePos
{
    public float time = 0;
    public float pos = 0;
}

public class GUIList : MonoBehaviour
{
    #region Variables

    public GUIGroup sourceItem;

    public string myName;
    public ListType listType = ListType.Vertical;
    public List<GUIGroup> childs = new List<GUIGroup>();
    public Transform childsRoot;
    public GUIGroup ownGroup_Back;
    public GUIGroup ownGroup_Front;

    public GUIHorizAllign allign_Horiz = GUIHorizAllign.Mid;
    public GUIVertAllign allign_Vert = GUIVertAllign.Mid;
    public bool xIsCoef = false;
    public bool wIsCoef = false;
    public float x = 0;
    public float y = 0;
    public float w = 10.80f;
    public float h = 14.40f;
    public bool isTouchable = true;
    public bool isVisible = true;
    public bool animVis = true;
    public float alpha = 1;
    public int depth = 0;
    public float touchDepth = 0;
    public float itemsSize = 2.40f;
    public float edgeAdditionalSize = 0.60f;

    [HideInInspector]
    public bool visibility_Total = true;



    [HideInInspector]
    public float dist = 0;

    float destinationDist = 0;

    float maxDist = 0;

    [HideInInspector]
    public bool isFingerMoving = false;

    [HideInInspector]
    public bool wasFingerMovingLastUpdate = false;

    float distBeforeFingerMove = 0;

    [HideInInspector]
    public bool isPressed = false;

    [HideInInspector]
    public GUIGroup pressingChildGroup = null;

    [HideInInspector]
    TouchUpInfo touchEndedOnThis_Info;

    [HideInInspector]
    TouchUpInfo tap_Info;

    [HideInInspector]
    TouchUpInfo movingFinger_Info;

    [HideInInspector]
    TouchUpInfo oldMovingFinger_Info = null;

    float fingerDownDistSpeed = 10f;
    float distLerpError = 0.06f;

    AnimMode animMode = AnimMode.Show;

    bool anim_ShouldDoVisInit = false;
    float animDelay = 0;

    [HideInInspector]
    public GUIControlShowingAnimStatus showingAnimStatus = GUIControlShowingAnimStatus.Unused;

    float coefMode_CoefX = 1;
    float coefMode_CoefW = 1;

    //

    float minVertAccMoveStartingDif = 0.10f;
    float vertAccCheck_NeedFingerMoveTime_Max = 0.20f;
    float vertAccCheck_NeedFingerMoveTime_Min = 0.01f;
    float vertAccMove_DestDistCoef = 1;
    float vertAccMove_SpeedCoef = 0.6f;
    float vertAccMove_MaxSpeed = 120f;
    float vertAccMove_MaxEffectiveSpeed = 60f;
    float vertAccMove_SpeedDecSp = 26f;

    bool isVertAccMove = false;
    float vertAccMove_InitialDeltaTime = 0;
    float vertAccMove_InitialDeltaY = 0;
    float vertAccMove_InitialSpeed = 0;
    float vertAccMove_Speed = 0;

    float fingerDownOverlap = 0;
    float listMoveOverlapFingerStillDown_LerpError = 0.01f;
    float acceleratedOverlap = 0;
    float listMoveOverlapAccelerated_LerpError = 0.01f;
    float listMoveOverlapAccelerated_TimeCounter = 0;
    float listMoveOverlapAccelerated_MaxTime = 0.2f;
    float listMoveOverlapAccelerated_Target = 0;
    float listMoveOverlapAccelerated_TargetCoef = 0.04f;
    float listMoveOverlap_ChangeSpeed_Acc = 14f;
    bool doAcceleratedOverlap = false;

    float listMoveOverlap_LerpSpeed = 14;
    float listMoveOverlap_ChangeSpeed_FingerStillDown = 12;
    float listMoveOverlap_ChildOffsetCoef = 0.3f;
    float listMoveOverlap_ChildOffset_DecCoef = 0.8f;
    float listMoveOverlap_ChildOffset_DecCoef_AdditionalPow = 1.5f;
    float listMoveOverlap_ChildOffset_DecMakhrajCoef = 0.1f;

    List<TimePos> fingerMoveLastTimePosInfos = new List<TimePos>();

    int visItemsCap = 0;
    int capHalf = 0;

    GUIControl guiControl_FirstEdge;
    GUIControl guiControl_LastEdge;

    [HideInInspector]
    public bool isBusy_MoveDistToEnd = false;

    BusyTouchInfo busyTouchInfo = null;

    int tempY_MinIndex = 0;
    float tempY = 0;
    float tempYToZeroLerpSpeed = 14f;
    float tempYLerpError = 0.02f;


    bool moveDistToEnd = false;
    float moveDistToEnd_MinTime = 0.15f;
    float moveDistToEnd_TimeCounter = 0.15f;
    float moveDistToEndSpeed = 14f;
    float moveDistToEnd_Dest = 0;
    float moveDistToEnd_LerpError = 0.03f;
    bool moveDistToEnd_ShouldIncLastItemAlpha = false;
    float moveDistToEnd_LastItemAlphaSpeed = 2.8f;

    int moveDistToEnd_ItemIndex = 0;

    float vertAccMove_PauseFastSpeed = 200f;

    //

    float allignCoef_X = 1;
    float allignCoef_Y = 1;
    float allignCoef_W = 0;
    float allignCoef_H = 0;
    float allignOffsetX = 0;
    float allignOffsetY = 0;

    float childsOffsetX = 0;
    float childsOffsetY = 0;

    bool shouldRecalcVisibleChilds = false;
    int insideBoxChild_Index_Start = -1;
    int insideBoxChild_Index_End = -1;
    int curVisChilds_Index_Start = -1;
    int curVisChilds_Index_End = -1;
    float distWhenVisibleChildsChanged = 0;

    float firstEdgeLimitValue = 0;
    float lastEdgeLimitValue = 0;

    #endregion

    void Update()
    {
        //

        #region Check Anims
        if (showingAnimStatus == GUIControlShowingAnimStatus.GoingShow || showingAnimStatus == GUIControlShowingAnimStatus.GoingHidden)
        {
            if (animDelay > 0)
            {
                animDelay = St_ExtraMath.DecByDeltaTimeToZero(animDelay);
            }

            if (animDelay == 0)
            {
                #region anim_ShouldDoVisInit

                if (anim_ShouldDoVisInit)
                {
                    anim_ShouldDoVisInit = false;

                    animVis = true;
                    ChangeVisibility();
                }

                #endregion
            }

            if (guiControl_FirstEdge.showingAnimStatus == GUIControlShowingAnimStatus.Hidden)
            {
                showingAnimStatus = GUIControlShowingAnimStatus.Hidden;
                RemoveBusyTouch();
            }

            if(guiControl_FirstEdge.showingAnimStatus == GUIControlShowingAnimStatus.Show)
            {
                showingAnimStatus = GUIControlShowingAnimStatus.Show;
                RemoveBusyTouch();
            }

 

            //if (childs.Count > 0)
            //{
            if (guiControl_FirstEdge.showingAnimStatus == GUIControlShowingAnimStatus.Hidden)
                {
                    //#region childs[0].showingAnimStatus = GUIControlShowingAnimStatus.Hidden

                    //bool shouldSetVisFalse = true;

                    //for (int i = 0; i < childs.Count; i++)
                    //{
                    //    if (childs[i].animVis && childs[i].alpha_Total > 0)
                    //    {
                    //        shouldSetVisFalse = false;
                    //        break;
                    //    }
                    //}

                    //if (shouldSetVisFalse)
                    //{
                        animVis = false;

                        ChangeVisibility();
                    //}

                    //#endregion
                }

            if (guiControl_FirstEdge.showingAnimStatus == GUIControlShowingAnimStatus.Show)
                {
                    //#region childs[0].showingAnimStatus = GUIControlShowingAnimStatus.Show

                    //bool shouldSetVisFalse = true;

                    //for (int i = 0; i < childs.Count; i++)
                    //{
                    //    if (childs[i].animVis && childs[i].alpha_Total > 0)
                    //    {
                    //        shouldSetVisFalse = false;
                    //        break;
                    //    }
                    //}

                    //if (shouldSetVisFalse)
                    //{
                        animVis = true;

                        ChangeVisibility();
                    //}

                    //#endregion
                }
            //}
        }
        #endregion

        //

        #region FingerDown Move

        if (isFingerMoving && visibility_Total)
        {
            #region Init

            if (!wasFingerMovingLastUpdate)
            {
                distBeforeFingerMove = dist;
                destinationDist = distBeforeFingerMove;
            }

            #endregion

            #region Update

            if (listType == ListType.Vertical)
            {
                if (oldMovingFinger_Info != null)
                {
                    float fingerStartY = movingFinger_Info.pos_Start.y;

                    float fingerEndY = movingFinger_Info.pos_End.y;

                    float fingerYDif = -(fingerEndY - fingerStartY);

                    float unclampedDestinationDist = distBeforeFingerMove - fingerYDif;

                    destinationDist = Mathf.Clamp(unclampedDestinationDist, 0, maxDist);

                    fingerDownOverlap = Mathf.Lerp(fingerDownOverlap, destinationDist - unclampedDestinationDist, listMoveOverlap_LerpSpeed * Time.deltaTime);

                    if (St_ExtraMath.Dist(dist, destinationDist) > distLerpError)
                    {
                        dist = Mathf.Lerp(dist, destinationDist, fingerDownDistSpeed * Time.deltaTime);
                        ChangeDist();
                    }
                }
            }

            #endregion
        }

        #endregion

        #region FingerDown Overlap Back

        if (Mathf.Abs(fingerDownOverlap) > 0)
        {
            if (visibility_Total)
            {
                if (!isFingerMoving && !isVertAccMove)
                {
                    fingerDownOverlap = Mathf.Lerp(fingerDownOverlap, 0, listMoveOverlap_ChangeSpeed_FingerStillDown * Time.deltaTime);

                    if (Mathf.Abs(fingerDownOverlap) < listMoveOverlapFingerStillDown_LerpError)
                        fingerDownOverlap = 0;
                }
            }
            else
            {
                fingerDownOverlap = 0;
            }
        }
        #endregion

        //

        #region Accelerated Move

        #region AccMove Check and Init

        if (wasFingerMovingLastUpdate && visibility_Total && !isFingerMoving && (maxDist > 0))
        {
            if (listType == ListType.Vertical)
            {
                if (fingerMoveLastTimePosInfos != null && fingerMoveLastTimePosInfos.Count > 1)
                {
                    TimePos tpStart = fingerMoveLastTimePosInfos[fingerMoveLastTimePosInfos.Count - 2];
                    TimePos tpEnd = fingerMoveLastTimePosInfos[fingerMoveLastTimePosInfos.Count - 1];

                    float delDist = St_ExtraMath.Dist(tpStart.pos, tpEnd.pos);

                    for (int i = fingerMoveLastTimePosInfos.Count - 3; i >= 0; i--)
                    {
                        TimePos tempTpStart = fingerMoveLastTimePosInfos[i];
                        float tempDelDis = St_ExtraMath.Dist(tempTpStart.pos, tpEnd.pos);

                        if (tempDelDis >= delDist)
                        {
                            tpStart = tempTpStart;
                            delDist = tempDelDis;
                        }
                        else
                        {
                            break;
                        }
                    }

                    float delTime = tpEnd.time - tpStart.time;

                    if (delDist > minVertAccMoveStartingDif && delTime > vertAccCheck_NeedFingerMoveTime_Min)
                    {
                        isVertAccMove = true;

                        distBeforeFingerMove = dist;

                        vertAccMove_InitialDeltaTime = delTime;
                        vertAccMove_InitialDeltaY = -(tpStart.pos - tpEnd.pos);
                        vertAccMove_InitialSpeed = vertAccMove_InitialDeltaY / vertAccMove_InitialDeltaTime;

                        destinationDist = distBeforeFingerMove + vertAccMove_InitialSpeed * vertAccMove_DestDistCoef;

                        #region Init Acc Overlap Target

                        listMoveOverlapAccelerated_Target = destinationDist - dist;

                        if (listMoveOverlapAccelerated_Target > 0)
                        {
                            listMoveOverlapAccelerated_Target *= (1 - ((maxDist - dist) / maxDist));
                        }
                        else
                        {
                            listMoveOverlapAccelerated_Target *= ((maxDist - dist) / maxDist);
                        }

                        listMoveOverlapAccelerated_Target *= listMoveOverlapAccelerated_TargetCoef;

                        #endregion

                        destinationDist = Mathf.Clamp(destinationDist, 0, maxDist);

                        vertAccMove_InitialSpeed = Mathf.Abs(vertAccMove_InitialSpeed) * vertAccMove_SpeedCoef;
                        vertAccMove_InitialSpeed = Mathf.Clamp(vertAccMove_InitialSpeed, 0, vertAccMove_MaxSpeed);

                        vertAccMove_Speed = vertAccMove_InitialSpeed;
                    }
                }
            }
        }


        #endregion

        #region AccMove Update

        if (isVertAccMove)
        {
            if (visibility_Total && !isFingerMoving)
            {
                if (listType == ListType.Vertical)
                {
                    float oDist = dist;

                    float effectiveSpeed = Mathf.Clamp(vertAccMove_Speed, 0, vertAccMove_MaxEffectiveSpeed);

                    if (dist < destinationDist)
                    {
                        if (dist + effectiveSpeed * Time.deltaTime >= destinationDist)
                        {
                            dist = destinationDist;
                            ChangeDist();
                        }
                        else
                        {
                            dist += effectiveSpeed * Time.deltaTime;
                            ChangeDist();
                        }
                    }

                    if (dist > destinationDist)
                    {
                        if (dist - effectiveSpeed * Time.deltaTime <= destinationDist)
                        {
                            dist = destinationDist;
                            ChangeDist();
                        }
                        else
                        {
                            dist -= effectiveSpeed * Time.deltaTime;
                            ChangeDist();
                        }
                    }

                    vertAccMove_Speed -= Time.deltaTime * vertAccMove_SpeedDecSp;

                    if (oDist == dist)
                    {
                        isVertAccMove = false;

                        if (dist == 0 || dist == maxDist)
                        {
                            doAcceleratedOverlap = true;
                            listMoveOverlapAccelerated_TimeCounter = 0;
                        }
                    }
                }
            }
            else
            {
                isVertAccMove = false;
            }
        }

        #endregion

        #endregion

        #region Accelerated Overlap Back

        if (doAcceleratedOverlap)
        {
            #region Going

            if (visibility_Total)
            {
                listMoveOverlapAccelerated_TimeCounter += Time.deltaTime;

                if (listMoveOverlapAccelerated_TimeCounter >= listMoveOverlapAccelerated_MaxTime)
                {
                    doAcceleratedOverlap = false;
                    listMoveOverlapAccelerated_TimeCounter = 0;
                }

                acceleratedOverlap = Mathf.Lerp(acceleratedOverlap, listMoveOverlapAccelerated_Target, listMoveOverlap_ChangeSpeed_Acc * Time.deltaTime);
            }
            else
            {
                doAcceleratedOverlap = false;
                listMoveOverlapAccelerated_TimeCounter = 0;
                acceleratedOverlap = 0;
            }

            #endregion
        }
        else
        {
            #region Backing

            if (Mathf.Abs(acceleratedOverlap) > 0)
            {
                if (visibility_Total)
                {
                    acceleratedOverlap = Mathf.Lerp(acceleratedOverlap, 0, listMoveOverlap_ChangeSpeed_Acc * Time.deltaTime);

                    if (Mathf.Abs(acceleratedOverlap) < listMoveOverlapAccelerated_LerpError)
                        acceleratedOverlap = 0;
                }
                else
                {
                    acceleratedOverlap = 0;
                }
            }

            #endregion
        }

        #endregion

        // ▼ Vert/Horiz ? 

        #region Move dist to end

        if (moveDistToEnd)
        {
            moveDistToEnd_TimeCounter = St_ExtraMath.DecByDeltaTimeToZero(moveDistToEnd_TimeCounter);

            if (isVertAccMove)
            {
                vertAccMove_Speed = St_ExtraMath.DecByDeltaTimeToZero(vertAccMove_Speed, vertAccMove_PauseFastSpeed);
            }

            if (dist != moveDistToEnd_Dest)
            {
                dist = Mathf.Lerp(dist, moveDistToEnd_Dest, moveDistToEndSpeed * Time.deltaTime);
                ChangeDist();

                if (St_ExtraMath.Dist(dist, moveDistToEnd_Dest) <= moveDistToEnd_LerpError)
                {
                    dist = moveDistToEnd_Dest;
                    ChangeDist();
                }
            }

            if (tempY != 0)
            {
                tempY = Mathf.Lerp(tempY, 0, tempYToZeroLerpSpeed * Time.deltaTime);

                if (St_ExtraMath.Dist(tempY, 0) <= tempYLerpError)
                {
                    tempY = 0;
                }
            }

            if (moveDistToEnd_TimeCounter == 0 && dist == moveDistToEnd_Dest)
            {
                if (tempY == 0)
                {
                    if (moveDistToEnd_ShouldIncLastItemAlpha)
                    {
                        childs[moveDistToEnd_ItemIndex].SetAlpha(childs[moveDistToEnd_ItemIndex].alpha + moveDistToEnd_LastItemAlphaSpeed * Time.deltaTime);
                        childs[moveDistToEnd_ItemIndex].ChangeAlpha();

                        if (childs[moveDistToEnd_ItemIndex].alpha == 1)
                        {
                            moveDistToEnd_ShouldIncLastItemAlpha = false;
                        }
                    }

                    if (!moveDistToEnd_ShouldIncLastItemAlpha)
                    {
                        vertAccMove_Speed = 0;
                        isBusy_MoveDistToEnd = false;
                        RemoveBusyTouch();
                        moveDistToEnd = false;
                    }
                }
            }
        }

        #endregion

        //

        #region Set childs local Pos

        if (visibility_Total && childs.Count > 0)
        {
            if (listType == ListType.Vertical)
            {
                if (tempY != 0 || fingerDownOverlap != 0 || acceleratedOverlap != 0)
                {
                    for (int i = curVisChilds_Index_Start; i < curVisChilds_Index_End; i++)
                    {
                        ChangeItemLocalPos_Y(i);

                        shouldRecalcVisibleChilds = true;
                    }
                }
            }
        }

        #endregion

        //

        #region Recalc visible childs

        if (shouldRecalcVisibleChilds && visibility_Total)
        {
            CalcNewVisibleChildsIndexes_AndChangeVisibilityOfChilds();
        }

        #endregion

        //

        #region isFingerMoving (?) inits

        if (isFingerMoving && visibility_Total)
        {
            #region oldMovingFinger_Info

            oldMovingFinger_Info = new TouchUpInfo();

            oldMovingFinger_Info.time_Start = movingFinger_Info.time_Start;
            oldMovingFinger_Info.time_Cur = movingFinger_Info.time_Cur;
            oldMovingFinger_Info.time_End = movingFinger_Info.time_End;
            oldMovingFinger_Info.pos_Start = movingFinger_Info.pos_Start;
            oldMovingFinger_Info.pos_End = movingFinger_Info.pos_End;

            #endregion

            #region fingerMoveLastTimePosInfos

            if (fingerMoveLastTimePosInfos == null)
                fingerMoveLastTimePosInfos = new List<TimePos>();

            TimePos tp = new TimePos();
            tp.time = movingFinger_Info.time_Cur;
            tp.pos = 0;
            if (listType == ListType.Vertical)
                tp.pos = movingFinger_Info.pos_End.y;
            if (listType == ListType.Horizontal)
                tp.pos = movingFinger_Info.pos_End.x;

            fingerMoveLastTimePosInfos.Add(tp);

            if (fingerMoveLastTimePosInfos.Count > 2)
            {
                if ((fingerMoveLastTimePosInfos[fingerMoveLastTimePosInfos.Count - 1].time - fingerMoveLastTimePosInfos[1].time) > vertAccCheck_NeedFingerMoveTime_Max)
                    fingerMoveLastTimePosInfos.RemoveAt(0);
            }

            #endregion
        }
        else
        {
            oldMovingFinger_Info = null;

            fingerMoveLastTimePosInfos = null;
        }

        wasFingerMovingLastUpdate = isFingerMoving;

        #endregion
    }

    //

    public void Init()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, MyGUIController.SPRITES_Z);

        if (xIsCoef)
            coefMode_CoefX = MyGUIController.Instance.curScreen_WidthScale;

        if (wIsCoef)
            coefMode_CoefW = MyGUIController.Instance.curScreen_WidthScale;

        //

        for (int i = 0; i < childs.Count; i++)
        {
            childs[i].Init_WithOwnerList(this, ListChildType.Item);
        }

        ownGroup_Back.Init_WithOwnerList(this, ListChildType.Own_Back);
        ownGroup_Front.Init_WithOwnerList(this, ListChildType.Own_Front);

        //sourceItem.Init_WithOwnerList(this, ListChildType.Item);

        //

        guiControl_FirstEdge = ownGroup_Front.FindChildByName("FirstEdge");
        guiControl_LastEdge = ownGroup_Front.FindChildByName("LastEdge");

        //

        #region allign_Horiz
        switch (allign_Horiz)
        {
            case GUIHorizAllign.Left:
                allignCoef_W = 0.5f;
                allignOffsetX = MyGUIController.Instance.curScreen_Left;
                break;

            case GUIHorizAllign.Right:
                allignCoef_W = -0.5f;
                allignCoef_X = -1;
                allignOffsetX = MyGUIController.Instance.curScreen_Right;
                break;
        }
        #endregion

        #region allign_Vert
        switch (allign_Vert)
        {
            case GUIVertAllign.Bot:
                allignCoef_H = 0.5f;
                allignOffsetY = MyGUIController.Instance.curScreen_Bot;
                break;

            case GUIVertAllign.Top:
                allignCoef_H = -0.5f;
                allignCoef_Y = -1;
                allignOffsetY = MyGUIController.Instance.curScreen_Top;
                break;
        }
        #endregion

        //

        ChangeX();
        ChangeY();

        firstEdgeLimitValue = guiControl_FirstEdge.GetFinalRect().yMax - edgeAdditionalSize;
        lastEdgeLimitValue = guiControl_LastEdge.GetFinalRect().yMax;

        SetVisItemsCap();

        ReSetMaxDist();

        dist = 0;
        SetDist(dist);

        ChangeDist();

        if (listType == ListType.Vertical)
        {
            childsOffsetX = 0;
            childsOffsetY = Get_Height() / 2 - itemsSize / 2;
        }

        for (int i = 0; i < childs.Count; i++)
        {
            ChangeItemLocalPos_X(i);
            ChangeItemLocalPos_Y(i);

            shouldRecalcVisibleChilds = true;
        }

        CalcNewVisibleChildsIndexes_AndChangeVisibilityOfChilds();

        ChangeAlpha();
        ChangeVisibility();

        ChangeStatus();
    }

    public void AddItem(GUIGroup _itemSource, int _index, bool _shouldSetAlphaZero, bool _shouldMoveListToItem)
    {
        GUIGroup itemSource = _itemSource;

        int itemIndex = _index;

        if (itemIndex < childs.Count)
        {
            tempY = itemsSize;
            tempY_MinIndex = itemIndex + 1;
        }

        GUIGroup item = GameObject.Instantiate(itemSource) as GUIGroup;

        bool shouldSetAlphaZero = _shouldSetAlphaZero;
        bool shouldMoveListToItem = _shouldMoveListToItem;

        item.gameObject.SetActive(true);

        item.Init_WithOwnerList(this, ListChildType.Item);

        item.transform.parent = childsRoot;
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        item.isVisible = true;
        item.ChangeVisibility();

        childs.Insert(itemIndex, item);

        ReSetMaxDist();

        if (shouldSetAlphaZero)
        {
            item.SetAlpha(0);
            item.ChangeAlpha();
        }

        if (shouldMoveListToItem)
        {
            if (shouldSetAlphaZero)
                MoveDistToItemIndex_And_IncLastItemAlpha(itemIndex);
            else
                MoveDistToItemIndex(itemIndex);
        }

        for (int i = itemIndex; i < childs.Count; i++)
        {
            ChangeItemLocalPos_X(i);
            ChangeItemLocalPos_Y(i);

            shouldRecalcVisibleChilds = true;
        }

        CalcNewVisibleChildsIndexes_AndChangeVisibilityOfChilds();
    }

    //

    public void MoveDistToItemIndex(int _itemIndex)
    {
        isBusy_MoveDistToEnd = true;
        AddBusyTouch();

        moveDistToEnd = true;
        moveDistToEnd_ItemIndex = _itemIndex;

        moveDistToEnd_Dest = (moveDistToEnd_ItemIndex - capHalf) * itemsSize;
        moveDistToEnd_Dest = Mathf.Clamp(moveDistToEnd_Dest, 0, maxDist);

        moveDistToEnd_TimeCounter = moveDistToEnd_MinTime;
    }

    public void MoveDistToItemIndex_And_IncLastItemAlpha(int _itemIndex)
    {
        MoveDistToItemIndex(_itemIndex);

        moveDistToEnd_ShouldIncLastItemAlpha = true;
    }

    //

    public void OnTap(GUIGroup _childGroup)
    {
        if (IsBusy())
            return;

        tap_Info = new TouchUpInfo();
        tap_Info.time_Start = MyGUIController.Instance.menuMode_CurTouch.time_Start;
        tap_Info.time_Cur = Time.time;
        tap_Info.time_End = MyGUIController.Instance.menuMode_CurTouch.time_End;
        tap_Info.pos_Start = MyGUIController.Instance.menuMode_CurTouch.pos_Start;
        tap_Info.pos_End = MyGUIController.Instance.menuMode_CurTouch.pos_End;

        //Implement !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    }

    public void OnTouchEndedOnThis(GUIGroup _childGroup)
    {
        if (IsBusy())
            return;

        touchEndedOnThis_Info = new TouchUpInfo();
        touchEndedOnThis_Info.time_Start = MyGUIController.Instance.menuMode_CurTouch.time_Start;
        touchEndedOnThis_Info.time_Cur = Time.time;
        touchEndedOnThis_Info.time_End = MyGUIController.Instance.menuMode_CurTouch.time_End;
        touchEndedOnThis_Info.pos_Start = MyGUIController.Instance.menuMode_CurTouch.pos_Start;
        touchEndedOnThis_Info.pos_End = MyGUIController.Instance.menuMode_CurTouch.pos_End;

        //Implement !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    }

    public void OnMovingFinger()
    {
        if (IsBusy())
            return;

        movingFinger_Info = new TouchUpInfo();

        movingFinger_Info.time_Start = MyGUIController.Instance.menuMode_CurTouch.time_Start;
        movingFinger_Info.time_Cur = Time.time;
        movingFinger_Info.time_End = MyGUIController.Instance.menuMode_CurTouch.time_End;
        movingFinger_Info.pos_Start = MyGUIController.Instance.menuMode_CurTouch.pos_Start;
        movingFinger_Info.pos_End = MyGUIController.Instance.menuMode_CurTouch.pos_End;

        isFingerMoving = true;
    }

    //

    void ReSetMaxDist()
    {
        maxDist = Mathf.Clamp((childs.Count - visItemsCap) * itemsSize, 0, float.MaxValue);
    }

    void SetDist(float _val)
    {
        dist = Mathf.Clamp(_val, 0, maxDist);
    }

    void SetVisItemsCap()
    {
        if (listType == ListType.Vertical)
        {
            visItemsCap = (int)((Get_Height() / itemsSize) + 0.1f);

            capHalf = (int)(visItemsCap / 2);
        }
    }

    public void CalcPossibleVisChildsIndexes()
    {
        if (childs.Count == 0)
        {
            insideBoxChild_Index_Start = -1;
            insideBoxChild_Index_End = -1;
            curVisChilds_Index_Start = -1;
            curVisChilds_Index_End = -1;

            return;
        }

        int startInd = 0;
        int endInd = 0;

        float distDef = dist - distWhenVisibleChildsChanged;
        int indexesFromDistDef = (int)(Mathf.Abs(distDef) / itemsSize) + 1;

        if (listType == ListType.Vertical)
        {
            insideBoxChild_Index_Start = (int)(dist / itemsSize);
            insideBoxChild_Index_End = Mathf.Min(insideBoxChild_Index_Start + visItemsCap, childs.Count - 1); ;

            startInd = insideBoxChild_Index_Start - 1;
            endInd = insideBoxChild_Index_End + 1;

            if (distDef > 0)
            {
                startInd -= indexesFromDistDef;
            }

            if (distDef < 0)
            {
                endInd += indexesFromDistDef;
            }
        }

        if (startInd < 0)
            startInd = 0;

        if (endInd > childs.Count - 1)
            endInd = childs.Count - 1;

        curVisChilds_Index_Start = startInd;
        curVisChilds_Index_End = endInd;
    }

    public void CalcNewVisibleChildsIndexes_AndChangeVisibilityOfChilds()
    {
        shouldRecalcVisibleChilds = false;

        int firstInd = Mathf.Max((int)(dist / itemsSize) - 1, 0);
        int lastInd = Mathf.Min(firstInd + visItemsCap + 1, childs.Count - 1);

        for (int i = 0; i < childs.Count; i++)
        {
            childs[i].isVisible = false;
        }

        curVisChilds_Index_Start = firstInd;
        curVisChilds_Index_End = lastInd;

        int ind = 0;

        for (ind = 0; ind < childs.Count; ind++)
        {
            if (listType == ListType.Vertical)
            {
                float childY = childs[ind].Get_ObjTr_Y() + itemsSize / 2;

                //if (ind == 0)
                //{

                //    print(childY + "     " + firstEdgeLimitValue + "     " + lastEdgeLimitValue);
                //}

                if (childY < firstEdgeLimitValue)
                {
                    bool oldVis = childs[ind].isVisible;

                    childs[ind].isVisible = true;

                    if (!oldVis && childs[ind].isVisible)
                    {
                        //if (childs[ind].transform.parent != childsRoot)
                        //    childs[ind].transform.parent = childsRoot;
                        ChangeItemLocalPos_Y(ind);
                    }

                    break;
                }
            }
        }

        for (int i = ind - 1; i >= Mathf.Max(ind - 3, 0); i--)
        {
            //bool oldVis = childs[i].isVisible;

            //childs[i].isVisible = true;

            //if (!oldVis && childs[i].isVisible)
            //{
            //    //if (childs[i].transform.parent != childsRoot)
            //    //    childs[i].transform.parent = childsRoot;
                ChangeItemLocalPos_Y(i);
            //}
        }

        for (ind = ind + 1; ind < childs.Count; ind++)
        {
            if (listType == ListType.Vertical)
            {
                float childY = childs[ind].Get_ObjTr_Y() + itemsSize / 2;

                if (childY > lastEdgeLimitValue)
                {
                    bool oldVis = childs[ind].isVisible;

                    childs[ind].isVisible = true;

                    if (!oldVis && childs[ind].isVisible)
                    {
                        //if (childs[ind].transform.parent != childsRoot)
                        //    childs[ind].transform.parent = childsRoot;
                        ChangeItemLocalPos_Y(ind);
                    }
                }
                else
                    break;
            }
        }

        for (int i = ind; i <= Mathf.Min(ind + 4, childs.Count - 1); i++)
        {
            //bool oldVis = childs[i].isVisible;

            //childs[i].isVisible = true;

            //if (!oldVis && childs[i].isVisible)
            //{
                //if (childs[ind].transform.parent != childsRoot)
                //    childs[i].transform.parent = childsRoot;
                ChangeItemLocalPos_Y(i);
            //}
        }

        for (int i = 0; i < childs.Count; i++)
        {
            childs[i].ChangeVisibility();
        }

        //

        //shouldRecalcVisibleChilds = false;

        //if (childs.Count == 0)
        //{
        //    insideBoxChild_Index_Start = -1;
        //    insideBoxChild_Index_End = -1;
        //    curVisChilds_Index_Start = -1;
        //    curVisChilds_Index_End = -1;

        //    return;
        //}

        //CalcPossibleVisChildsIndexes();

        //int new_Index_Start = -1;
        //bool new_Index_Start_Found = false;

        //int new_Index_End = -1;
        //bool new_Index_End_Found = false;


        //for (int i = curVisChilds_Index_Start; i <= curVisChilds_Index_End; i++)
        //{
        //    childs[i].isVisible = true;

        //    if (listType == ListType.Vertical)
        //    {
        //        float childY = childs[i].Get_Y() + itemsSize / 2;

        //        if ((childY >= firstEdgeLimitValue))
        //        {
        //            childs[i].isVisible = false;
        //        }
        //        else
        //        {
        //            if (!new_Index_Start_Found)
        //            {
        //                new_Index_Start_Found = true;
        //                new_Index_Start = i;
        //            }
        //        }


        //        if (childY <= lastEdgeLimitValue)
        //        {
        //            childs[i].isVisible = false;

        //            if (!new_Index_End_Found)
        //            {
        //                new_Index_End_Found = true;
        //                new_Index_End = i - 1;
        //            }
        //        }
        //    }

        //    childs[i].ChangeVisibility();
        //}

        //curVisChilds_Index_Start = new_Index_Start;
        //curVisChilds_Index_End = new_Index_End;
    }

    public GUIGroup GetChildByPos(Vector2 _pos)
    {
        Vector2 pos = _pos;

        float posVal = 0;
        float localPosVal = 0;

        Rect rct = GetFinalRect();

        if (listType == ListType.Vertical)
        {
            posVal = pos.y;
            localPosVal = posVal - rct.yMin;
            localPosVal = rct.height - localPosVal;

            int ind = (int)((localPosVal + dist) / itemsSize);

            if (ind >= 0 && ind < childs.Count)
                return childs[ind];
        }

        return null;
    }

    //

    public bool GetTotalVisibility()
    {
        return (isVisible && animVis && (alpha > 0));
    }

    //

    float Get_Width()
    {
        return w * coefMode_CoefW;
    }

    float Get_Height()
    {
        return h;
    }

    float Get_FinalPosX_WCoef()
    {
        return allignCoef_W;
    }

    float Get_FinalPosY_HCoef()
    {
        return allignCoef_H;
    }

    float Get_FinalPosX()
    {
        return allignCoef_X * coefMode_CoefX * x + (Get_FinalPosX_WCoef() * Get_Width()) + allignOffsetX;
    }

    float Get_FinalPosY()
    {
        return allignCoef_Y * y + (Get_FinalPosY_HCoef() * Get_Height()) + allignOffsetY;
    }

    //

    Vector3 Get_ObjectFinalPos()
    {
        return transform.position;
    }

    //

    public Rect GetFinalRect()
    {
        Vector3 objFinPos = Get_ObjectFinalPos();

        float rW = Get_Width();
        float rH = Get_Height();
        float rX = objFinPos.x - rW / 2;
        float rY = objFinPos.y - rH / 2;

        return new Rect(rX, rY, rW, rH);
    }

    //

    public void ChangeX()
    {
        transform.position = new Vector3(Get_FinalPosX(), transform.position.y, transform.position.z);
    }

    public void ChangeY()
    {
        transform.position = new Vector3(transform.position.x, Get_FinalPosY(), transform.position.z);
    }

    //

    public void ChangeVisibility()
    {
        bool oldVis = visibility_Total;

        visibility_Total = GetTotalVisibility();

        if (oldVis != visibility_Total)
        {
            for (int i = 0; i < childs.Count; i++)
            {
                childs[i].ChangeVisibility();
            }
        }

        ownGroup_Back.ChangeVisibility();
        ownGroup_Front.ChangeVisibility();

        if (visibility_Total)
        {
            CalcNewVisibleChildsIndexes_AndChangeVisibilityOfChilds();
        }
    }

    public void ChangeAlpha()
    {
        float oldAlpha = alpha;

        for (int i = 0; i < childs.Count; i++)
        {
            childs[i].ChangeAlpha();
        }

        ownGroup_Back.ChangeAlpha();
        ownGroup_Front.ChangeAlpha();

        if ((oldAlpha == 0 && alpha > 0) || (oldAlpha > 0 && alpha == 0))
            ChangeVisibility();
    }

    public void ChangeStatus()
    {

    }

    //

    public void ChangeDist()
    {
        SetDist(dist);

        shouldRecalcVisibleChilds = true;

        if (listType == ListType.Vertical)
        {
            childsRoot.localPosition = new Vector3(0, dist, 0);
        }
    }

    public void ChangeItemLocalPos_X(int _index)
    {
        int ind = _index;
        GUIGroup gr = childs[ind];

        if (listType == ListType.Vertical)
        {
            gr.x = childsOffsetX;
            gr.ChangeX();
        }
    }

    public void ChangeItemLocalPos_Y(int _index)
    {
        int ind = _index;
        GUIGroup gr = childs[ind];

        if (listType == ListType.Vertical)
        {
            float tempYVal = 0;

            if (tempY != 0 && ind >= tempY_MinIndex)
            {
                tempYVal = tempY;
            }

            gr.y = childsOffsetY - ind * itemsSize + tempYVal;

            #region FingerDown Overlap

            if (fingerDownOverlap != 0)
            {
                float ccc = 0;
                float finalCoef = 1;

                if (fingerDownOverlap > 0)
                {
                    ccc = ind;
                    finalCoef = 1;
                }
                else
                {
                    ccc = (childs.Count - 1) - ind;
                    finalCoef = -1;
                }

                float initialCoefedOL = Mathf.Abs(fingerDownOverlap * listMoveOverlap_ChildOffsetCoef);

                float farCoef = Mathf.Pow(listMoveOverlap_ChildOffset_DecCoef, ccc + listMoveOverlap_ChildOffset_DecCoef_AdditionalPow);
                float makhraj = 1 + ccc * listMoveOverlap_ChildOffset_DecMakhrajCoef;

                float offs = (initialCoefedOL * farCoef) / makhraj;

                gr.y -= finalCoef * (initialCoefedOL - offs);
            }

            #endregion

            #region Acc Overlap

            if (acceleratedOverlap != 0)
            {
                gr.y += acceleratedOverlap;
            }

            #endregion

            gr.ChangeY();
        }
    }

    //

    void AddBusyTouch()
    {
        if (busyTouchInfo == null)
        {
            busyTouchInfo = new BusyTouchInfo();
            busyTouchInfo.list = this;

            MyGUIController.Instance.AddDisablingTouchInfo(busyTouchInfo);
        }
    }

    void RemoveBusyTouch()
    {
        if (busyTouchInfo != null)
        {
            MyGUIController.Instance.RemoveDisablingTouchInfo(busyTouchInfo);
            busyTouchInfo = null;
        }
    }

    bool IsBusy()
    {
        return isBusy_MoveDistToEnd;
    }

    //

    void AnimMutualInit(float _delay, AnimMode _animMode)
    {
        animMode = _animMode;

        anim_ShouldDoVisInit = true;

        animDelay = _delay;

        if (animMode == AnimMode.Show)
        {
            showingAnimStatus = GUIControlShowingAnimStatus.GoingShow;
        }
        else
        {
            showingAnimStatus = GUIControlShowingAnimStatus.GoingHidden;
        }

        AddBusyTouch();
    }

    public void StartAnim_Alpha(float _delay, float _startAlpha, float _endAlpha, AnimationCurve _animCurve, float _TimeCounterSpeed, AnimMode _animMode)
    {
        AnimMutualInit(_delay, _animMode);

        for (int i = 0; i < childs.Count; i++)
        {
            childs[i].StartAnim_Alpha(_delay, _startAlpha, _endAlpha, _animCurve, _TimeCounterSpeed, _animMode);
        }

        ownGroup_Back.StartAnim_Alpha(_delay, _startAlpha, _endAlpha, _animCurve, _TimeCounterSpeed, _animMode);
        ownGroup_Front.StartAnim_Alpha(_delay, _startAlpha, _endAlpha, _animCurve, _TimeCounterSpeed, _animMode);
    }

    public void StartAnim_Y_Alpha(float _delay, float _startY, float _endY, AnimationCurve _animCurve, float _startAlpha, float _endAlpha, AnimationCurve _animCurve_Alpha, float _TimeCounterSpeed, AnimMode _animMode)
    {
        AnimMutualInit(_delay, _animMode);

        for (int i = 0; i < childs.Count; i++)
        {
            childs[i].StartAnim_Y_Alpha(_delay, _startY, _endY, _animCurve, _startAlpha, _endAlpha, _animCurve_Alpha, _TimeCounterSpeed, _animMode);
        }

        ownGroup_Back.StartAnim_Y_Alpha(_delay, _startY, _endY, _animCurve, _startAlpha, _endAlpha, _animCurve_Alpha, _TimeCounterSpeed, _animMode);
        ownGroup_Front.StartAnim_Y_Alpha(_delay, _startY, _endY, _animCurve, _startAlpha, _endAlpha, _animCurve_Alpha, _TimeCounterSpeed, _animMode);
    }

    public void StartAnim_Y(float _delay, float _startY, float _endY, AnimationCurve _animCurve, float _TimeCounterSpeed, AnimMode _animMode)
    {
        AnimMutualInit(_delay, _animMode);

        for (int i = 0; i < childs.Count; i++)
        {
            childs[i].StartAnim_Y(_delay, _startY, _endY, _animCurve, _TimeCounterSpeed, _animMode);
        }

        ownGroup_Back.StartAnim_Y(_delay, _startY, _endY, _animCurve, _TimeCounterSpeed, _animMode);
        ownGroup_Front.StartAnim_Y(_delay, _startY, _endY, _animCurve, _TimeCounterSpeed, _animMode);
    }

    public void StartAnim_X_Alpha(float _delay, float _startX, float _endX, AnimationCurve _animCurve, float _startAlpha, float _endAlpha, AnimationCurve _animCurve_Alpha, float _TimeCounterSpeed, AnimMode _animMode)
    {
        AnimMutualInit(_delay, _animMode);

        for (int i = 0; i < childs.Count; i++)
        {
            childs[i].StartAnim_X_Alpha(_delay, _startX, _endX, _animCurve, _startAlpha, _endAlpha, _animCurve_Alpha, _TimeCounterSpeed, _animMode);
        }

        ownGroup_Back.StartAnim_X_Alpha(_delay, _startX, _endX, _animCurve, _startAlpha, _endAlpha, _animCurve_Alpha, _TimeCounterSpeed, _animMode);
        ownGroup_Front.StartAnim_X_Alpha(_delay, _startX, _endX, _animCurve, _startAlpha, _endAlpha, _animCurve_Alpha, _TimeCounterSpeed, _animMode);
    }

    public void StartAnim_X(float _delay, float _startX, float _endX, AnimationCurve _animCurve, float _TimeCounterSpeed, AnimMode _animMode)
    {
        AnimMutualInit(_delay, _animMode);

        for (int i = 0; i < childs.Count; i++)
        {
            childs[i].StartAnim_X(_delay, _startX, _endX, _animCurve, _TimeCounterSpeed, _animMode);
        }

        ownGroup_Back.StartAnim_X(_delay, _startX, _endX, _animCurve, _TimeCounterSpeed, _animMode);
        ownGroup_Front.StartAnim_X(_delay, _startX, _endX, _animCurve, _TimeCounterSpeed, _animMode);
    }

}
