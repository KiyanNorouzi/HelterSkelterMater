using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TouchInRectMode
{
    OnlyStart,
    OnlyEnd,
    StartAndEnd,
}

public enum SwipeSide
{
    None,
    Up,
    Down,
    Left,
    Right,
}

public enum TouchStatus
{
    NoTouch,
    FreezeTouching,
    Tapped,
    Moving,
    TouchEnd,
}

public class TouchDownInfo
{
    public Vector2 pos;
    public Vector2 deltaPos;
    public float time;
    public float deltaTime;
}

public class TouchInfo
{
    public string ownerName = "";
    public bool isFingerDown = false;
    public bool isRecentlyFingerDown = false;
    public TouchStatus status = TouchStatus.NoTouch;
    public int fingerID;

    public float time_Start;
    public float time_End;
    public float deltaTime;

    public Vector2 pos_Start;
    public Vector2 pos_End;
    public Vector2 deltaPos;

    public Vector2 pos_Start_Px;
    public Vector2 pos_End_Px;
    public Vector2 deltaPos_Px;

    public bool isSwipingDoneForThisTouch = false;
    public bool isSwipeHappened = false;
    public bool isHorizontalSwipeHappened = false;
    public bool isVerticalSwipeHappened = false;
    public SwipeSide curHappenedNutsSwipe = SwipeSide.None;

    public bool eventHappened_SwipeUp = false;
    public bool eventHappened_SwipeDown = false;
    public bool eventHappened_SwipeRight = false;
    public bool eventHappened_SwipeLeft = false;

    public float swipeUp_DistMag = 0;
    public float swipeUp_Speed = 0;

    public float swipeDown_DistMag = 0;
    public float swipeDown_Speed = 0;

    public float swipeRight_DistMag = 0;
    public float swipeRight_Speed = 0;

    public float swipeLeft_DistMag = 0;
    public float swipeLeft_Speed = 0;

    public List<TouchDownInfo> touchDownInfos = new List<TouchDownInfo>();
    public int curTouchDownInfosIndex = -1;
    public int curTouchDownInfosMaxCount = 100;

    public bool IsFingerDown()
    {
        return isFingerDown;
    }

    public bool IsTapTimeStillValid()
    {
        return deltaTime <= MyGUIController.Instance.touchTapMaxTime;
    }

    public void SetStatus(TouchStatus _status)
    {
        status = _status;

        if (status == TouchStatus.NoTouch)
            SetOwnerName("");
    }

    public void SetOwnerName(string _ownerName)
    {
        ownerName = _ownerName;
    }
}

public class BusyTouchInfo
{
    public GUIControl control;
    public Changer changer;
    public GUIList list;

    public bool isSlider = false;
}

public class GroupOrList
{
    public GUIGroup group = null;
    public GUIList list = null;
}

public class MyGUIController : MonoBehaviour
{
    // LITTLE TOUCH ITEMS SYSTEM!!!

    #region Variables

    public static MyGUIController Instance;

    public const float LETTER_DEFAULT_HEIGHT = 1.28f;
    public const float SPRITES_Z = 100;

    public float oldSpaceToNewSpaceCoef = 1f;
    public float minScreenHeightSizeToApplyLimits = 3;

    public SpriteRenderer sourceSpriteRenderer;
    public List<GUIGroup> currentGUIGroups;
    public List<Changer> currentChangers;
    public List<GUIList> currentGUILists = new List<GUIList>();

    public TextToPersianConverter textToPersianConverter;

    public GUIGroup keyboard;
    public GUIControl keyboardBG;
    public GUIGroup keyboardTextBox;
    public GUIControl keyboardTextBox_PersianTextBox;
    public GUIControl keyboardTextBox_Cursor;
    public GUIControl keyboardBackspace;
    public GUIControl keyboardOK;
    public GUIControl keyboardShift;
    public float keyboardAnimSpeed = 3.6f;

    [HideInInspector]
    public int keyboardCursorPosIndex = 0;

    public AnimationCurve animCurve_Keybord_Y_Show;
    public AnimationCurve animCurve_Keybord_Y_Hide;
    public AnimationCurve animCurve_Dem3ButtonsWHScale_Show;
    public AnimationCurve animCurve_Dem3ButtonsWHScale_Hide;

    [HideInInspector]
    public TextPack curKeyboardTextPack = null;

    List<GUIGroup> sortedGUIGroups = new List<GUIGroup>();
    List<GUIGroup> sortedGUIGroups_ByTouchDepth = new List<GUIGroup>();

    List<GUIList> sortedGUILists = new List<GUIList>();
    List<GUIList> sortedGUILists_ByTouchDepth = new List<GUIList>();

    List<GroupOrList> sortedGroupOrLists = new List<GroupOrList>();
    List<GroupOrList> sortedGroupOrLists_ByTouchDepth = new List<GroupOrList>();

    [HideInInspector]
    public float screenScale = 1f;

    float screenDPI = 133;

    float touchTapMaxError = 0;
    float touchTapMaxError_Coef = 0.15f;

    [HideInInspector]
    public float touchTapMaxTime = 0.3f;

    public static float pixelToOldSpaceCoef = 0.01f;

    [HideInInspector]
    public float defScreen_Width = 10.8f;
    [HideInInspector]
    public float defScreen_HalfWidth = 5.4f;
    [HideInInspector]
    public float defScreen_Height = 19.2f;
    [HideInInspector]
    public float defScreen_HalfHeight = 9.6f;

    [HideInInspector]
    public float curScreen_Width = 10.8f; //Changes in wide screens
    [HideInInspector]
    public float curScreen_HalfWidth = 5.4f; //Changes in wide screens
    [HideInInspector]
    public float curScreen_Height = 19.2f;
    [HideInInspector]
    public float curScreen_HalfHeight = 9.6f;
    [HideInInspector]
    public float curScreen_Left = -5.4f; //Changes in wide screens
    [HideInInspector]
    public float curScreen_Right = 5.4f; //Changes in wide screens
    [HideInInspector]
    public float curScreen_Top = 9.6f;
    [HideInInspector]
    public float curScreen_Bot = -9.6f;

    [HideInInspector]
    public float curScreen_WidthScale = 1f;  //Changes in wide screens

    float getTouchPos_CoefX;
    float getTouchPos_CoefY;


    [HideInInspector]
    public float curScreenHeightRealSize = 3f;

    [HideInInspector]
    public bool realScreenHeightIsBiggerThanMax = false;

    [HideInInspector]
    public float bigScreenHeightSizeCoef = 1;


    //

    public float swipeHorizMinNeededDistCoef = 1;

    [HideInInspector]
    public float swipeMaxTime = 1000000f;
    [HideInInspector]
    public float swipeMinTime = 0f;
    [HideInInspector]
    public float swipeMinDist = 0.5f;
    [HideInInspector]
    public float swipeMinDist_X = 0f;
    [HideInInspector]
    public float swipeMinDist_Y = 0f;


    [HideInInspector]
    public List<TouchInfo> touches = new List<TouchInfo>();

    int maxNumOfTouches = 2;

    //




    //[HideInInspector]
    //public TouchStatus touchStatus = TouchStatus.NoTouch;
    //bool touching = false;
    //bool oldTouching = false;
    //Touch touch;
    //int touchFingerID = 0;

    bool isMenuMode = true;

    [HideInInspector]
    public TouchInfo menuMode_CurTouch = new TouchInfo();

    //[HideInInspector]
    //public Vector2 touchStartPos = Vector2.zero;
    //[HideInInspector]
    //public Vector2 touchEndPos = Vector2.zero;
    //[HideInInspector]
    //public float touchStartTime = 0;
    //[HideInInspector]
    //public float touchEndTime = 0;
    //float touchTimeCounter = 0f;





    //

    [HideInInspector]
    public ObjShowStatus keyboardShowingStatus = ObjShowStatus.FullHide;

    List<GUIControl> curTouchingControls = new List<GUIControl>();
    List<GUIList> curTouchingLists = new List<GUIList>();

    bool touchIsDisabled = false;
    List<BusyTouchInfo> curTouchInfosDatDisabledTouch = new List<BusyTouchInfo>();

    bool movingTouchIsBusy = false;
    BusyTouchInfo curBusy_MovingTouch_Info = null;

    //


    //


    //

    float keyboardBackspacePress_MaxTime = 0.311f;
    float keyboardBackspacePress_RemoveLetterMaxTime = 0.1f;
    float keyboardBackspacePress_TimeCounter = 0;
    bool isKeyboardBackspacePressedEnough = false;

    //

    #endregion

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        defScreen_Width = ConvertValToNewSpace(10.8f);

        defScreen_HalfWidth = ConvertValToNewSpace(5.4f);

        defScreen_Height = ConvertValToNewSpace(19.2f);

        defScreen_HalfHeight = ConvertValToNewSpace(9.6f);


        curScreen_Width = ConvertValToNewSpace(10.8f);

        curScreen_HalfWidth = ConvertValToNewSpace(5.4f);

        curScreen_Height = ConvertValToNewSpace(19.2f);

        curScreen_HalfHeight = ConvertValToNewSpace(9.6f);

        curScreen_Left = ConvertValToNewSpace(-5.4f);

        curScreen_Right = ConvertValToNewSpace(5.4f);

        curScreen_Top = ConvertValToNewSpace(9.6f);

        curScreen_Bot = ConvertValToNewSpace(-9.6f);


        if (Application.platform == RuntimePlatform.Android)
        {
            screenDPI = DisplayMetricsAndroid.DensityDPI;
        }

        touchTapMaxError = ConvertPixelToNewSpace(screenDPI * touchTapMaxError_Coef);

        curScreenHeightRealSize = Screen.height / screenDPI;

        if (curScreenHeightRealSize > minScreenHeightSizeToApplyLimits)
        {
            realScreenHeightIsBiggerThanMax = true;
            bigScreenHeightSizeCoef = minScreenHeightSizeToApplyLimits / curScreenHeightRealSize;
        }

        // 

        ReInitScreenSideRelatedParams();

        //

        for (int i = 0; i < maxNumOfTouches; i++)
        {
            touches.Add(new TouchInfo());
        }

        //

        swipeMinDist = ConvertValToNewSpace(swipeMinDist);
        swipeMinDist_Y = swipeMinDist / (Mathf.Sqrt(swipeHorizMinNeededDistCoef * swipeHorizMinNeededDistCoef + 1));
        swipeMinDist_X = swipeHorizMinNeededDistCoef * swipeMinDist_Y;


        // //

        for (int i = 0; i < currentChangers.Count; i++)
        {
            currentChangers[i].Init();
        }

        //

        for (int i = 0; i < currentGUIGroups.Count; i++)
        {
            currentGUIGroups[i].Init_WithoutOwner();
        }

        SortGUIGroupsByDepth();

        //

        for (int i = 0; i < currentGUILists.Count; i++)
        {
            currentGUILists[i].Init();
        }

        SortGUIListsByDepth();

        //

        SortGUIListOrGroupByDepth();

        // //
    }

    void ReInitScreenSideRelatedParams()
    {
        screenScale = ConvertPixelToNewSpace((float)(Screen.height)) / defScreen_Height;

        curScreen_WidthScale = ConvertPixelToNewSpace((float)(Screen.width)) / defScreen_Width;
        curScreen_WidthScale /= screenScale;

        float tmp = (int)(curScreen_WidthScale * 1000f);
        curScreen_WidthScale = (tmp / 1000);

        curScreen_Width *= curScreen_WidthScale;

        curScreen_HalfWidth = curScreen_Width / 2;
        curScreen_HalfHeight = curScreen_Height / 2;

        curScreen_Left = -curScreen_HalfWidth;
        curScreen_Right = curScreen_HalfWidth;

        getTouchPos_CoefX = (1 / (float)(Screen.width)) * curScreen_Width;
        getTouchPos_CoefY = (1 / (float)(Screen.height)) * curScreen_Height;
    }

    void Update()
    {
        #region Keyboard

        #region Keyboard anim status
        if (keyboardShowingStatus == ObjShowStatus.GoingToShow && keyboard.showingAnimStatus == GUIControlShowingAnimStatus.Show)
            keyboardShowingStatus = ObjShowStatus.FullShow;

        if (keyboardShowingStatus == ObjShowStatus.GoingToHide && keyboard.showingAnimStatus == GUIControlShowingAnimStatus.Hidden)
            keyboardShowingStatus = ObjShowStatus.FullHide;
        #endregion

        #region Keyboard backspace key

        if (IsKeyboardFullyShown())
        {
            if (keyboardBackspace.isPressed)
            {
                keyboardBackspacePress_TimeCounter += Time.deltaTime;

                if (!isKeyboardBackspacePressedEnough)
                {
                    if (keyboardBackspacePress_TimeCounter >= keyboardBackspacePress_MaxTime)
                    {
                        isKeyboardBackspacePressedEnough = true;
                        keyboardBackspacePress_TimeCounter = 0;
                    }
                }
                else
                {
                    if (keyboardBackspacePress_TimeCounter >= keyboardBackspacePress_RemoveLetterMaxTime)
                    {
                        keyboardBackspacePress_TimeCounter = 0;
                        DoKeyboardBackspace();
                    }
                }
            }
            else
            {
                isKeyboardBackspacePressedEnough = false;
                keyboardBackspacePress_TimeCounter = 0;
            }
        }

        #endregion

        #endregion

        #region HandleTouch

        #region Touch (General)

        //if (touchStatus == TouchStatus.Tapped)
        //{
        //    print("NoTouch");
        //    touchStatus = TouchStatus.NoTouch;
        //}

        //if (touchStatus == TouchStatus.TouchEnd)
        //{
        //    print("NoTouch");
        //    touchStatus = TouchStatus.NoTouch;
        //}

        for (int i = 0; i < touches.Count; i++)
        {
            TouchInfo curTouchInfo = touches[i];

            if (curTouchInfo.status == TouchStatus.Tapped || curTouchInfo.status == TouchStatus.TouchEnd)
            {
                curTouchInfo.SetStatus(TouchStatus.NoTouch);
            }
        }

        CheckTouch();

        //CheckTouch();
        //CheckMouse(); //<-------- TEEEEEEEEEEEEEEESSSSSSSSSSSTTTTT

        //if (touching)
        //{    //TEEEEEESSSSSSSTTTTT  vvv
        //    if (Vector2.Distance(GetMouseTouchPos(), touchStartPos) > touchTapMaxError)
        //    {
        //        print("Moving");
        //        touchStatus = TouchStatus.Moving;
        //    }

        //    if (touchStatus != TouchStatus.Moving)
        //    {
        //        print("FreezeTouching");
        //        touchStatus = TouchStatus.FreezeTouching;
        //    }
        //}
        //else
        //{
        //    if (oldTouching)
        //    {
        //        if (touchStatus == TouchStatus.FreezeTouching)
        //        {
        //            if (touchTimeCounter <= touchTapMaxTime)
        //            {
        //                print("Tapped");
        //                touchStatus = TouchStatus.Tapped;
        //            }
        //            else
        //            {
        //                print("Touch Ended");
        //                touchStatus = TouchStatus.TouchEnd;
        //            }
        //        }
        //        else
        //        {
        //            print("Touch Ended");
        //            touchStatus = TouchStatus.TouchEnd;
        //        }

        //        if (movingTouchIsBusy)
        //        {
        //            movingTouchIsBusy = false;
        //            curBusy_MovingTouch_Info = null;
        //        }
        //    }
        //}

        //oldTouching = touching;

        for (int i = 0; i < touches.Count; i++)
        {
            TouchInfo curTouchInfo = touches[i];

            if (curTouchInfo.IsFingerDown())
            {
                if (curTouchInfo.deltaPos.magnitude > touchTapMaxError)
                {
                    curTouchInfo.SetStatus(TouchStatus.Moving);
                }

                if (curTouchInfo.status != TouchStatus.Moving)
                {
                    curTouchInfo.SetStatus(TouchStatus.FreezeTouching);
                }

                if (curTouchInfo.status == TouchStatus.Moving)
                {
                    #region Check Swipe

                    if (!curTouchInfo.isSwipingDoneForThisTouch)
                    {
                        if (curTouchInfo.deltaTime >= swipeMinTime && curTouchInfo.deltaTime <= swipeMaxTime)
                        {
                            float swipe_DeltaPosMag = curTouchInfo.deltaPos.magnitude;

                            if (swipe_DeltaPosMag >= swipeMinDist)
                            {
                                float swipe_Delta_X = curTouchInfo.deltaPos.x;
                                float swipe_Delta_Y = curTouchInfo.deltaPos.y;

                                float dist_X = Mathf.Abs(swipe_Delta_X);
                                float dist_Y = Mathf.Abs(swipe_Delta_Y);

                                #region Horiz
                                if (dist_X > swipeMinDist_X)
                                {
                                    if (swipe_Delta_X > 0)
                                    {
                                        curTouchInfo.eventHappened_SwipeRight = true;
                                        curTouchInfo.swipeRight_DistMag = dist_X;
                                        curTouchInfo.swipeRight_Speed = dist_X / curTouchInfo.deltaTime;
                                    }
                                    else
                                    {
                                        curTouchInfo.eventHappened_SwipeLeft = true;
                                        curTouchInfo.swipeLeft_DistMag = dist_X;
                                        curTouchInfo.swipeLeft_Speed = dist_X / curTouchInfo.deltaTime;
                                    }

                                    curTouchInfo.isSwipeHappened = true;
                                    curTouchInfo.isHorizontalSwipeHappened = true;
                                }
                                #endregion

                                #region Vert
                                if (dist_Y > swipeMinDist_Y)
                                {
                                    if (swipe_Delta_Y > 0)
                                    {
                                        curTouchInfo.eventHappened_SwipeUp = true;
                                        curTouchInfo.swipeUp_DistMag = dist_Y;
                                        curTouchInfo.swipeUp_Speed = dist_Y / curTouchInfo.deltaTime;
                                    }
                                    else
                                    {
                                        curTouchInfo.eventHappened_SwipeDown = true;
                                        curTouchInfo.swipeDown_DistMag = dist_Y;
                                        curTouchInfo.swipeDown_Speed = dist_Y / curTouchInfo.deltaTime;
                                    }

                                    curTouchInfo.isVerticalSwipeHappened = true;
                                    curTouchInfo.isSwipeHappened = true;
                                }
                                #endregion

                                #region Nuts
                                if (curTouchInfo.isSwipeHappened)
                                {
                                    if (curTouchInfo.isHorizontalSwipeHappened && !curTouchInfo.isVerticalSwipeHappened)
                                    {
                                        if (curTouchInfo.eventHappened_SwipeRight)
                                        {
                                            curTouchInfo.curHappenedNutsSwipe = SwipeSide.Right;
                                        }
                                        else
                                        {
                                            curTouchInfo.curHappenedNutsSwipe = SwipeSide.Left;
                                        }
                                    }
                                    else
                                    {
                                        if (!curTouchInfo.isHorizontalSwipeHappened && curTouchInfo.isVerticalSwipeHappened)
                                        {
                                            if (curTouchInfo.eventHappened_SwipeUp)
                                            {
                                                curTouchInfo.curHappenedNutsSwipe = SwipeSide.Up;
                                            }
                                            else
                                            {
                                                curTouchInfo.curHappenedNutsSwipe = SwipeSide.Down;
                                            }
                                        }
                                        else
                                        {
                                            if ((dist_X - swipeMinDist_X) > (dist_Y - swipeMinDist_Y))
                                            {
                                                if (curTouchInfo.eventHappened_SwipeRight)
                                                {
                                                    curTouchInfo.curHappenedNutsSwipe = SwipeSide.Right;
                                                }
                                                else
                                                {
                                                    curTouchInfo.curHappenedNutsSwipe = SwipeSide.Left;
                                                }
                                            }
                                            else
                                            {
                                                if (curTouchInfo.eventHappened_SwipeUp)
                                                {
                                                    curTouchInfo.curHappenedNutsSwipe = SwipeSide.Up;
                                                }
                                                else
                                                {
                                                    curTouchInfo.curHappenedNutsSwipe = SwipeSide.Down;
                                                }
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    }

                    #endregion
                }
            }
            else
            {
                if (curTouchInfo.isRecentlyFingerDown)
                {
                    if (curTouchInfo.status == TouchStatus.FreezeTouching)
                    {
                        if (curTouchInfo.IsTapTimeStillValid())
                        {
                            curTouchInfo.SetStatus(TouchStatus.Tapped);
                        }
                        else
                        {
                            curTouchInfo.SetStatus(TouchStatus.TouchEnd);
                        }
                    }
                    else
                    {
                        curTouchInfo.SetStatus(TouchStatus.TouchEnd);
                    }
                }
            }
        }

        #endregion

        if (isMenuMode)
        {
            menuMode_CurTouch = touches[0];

            #region Check for freeing move touch

            if (movingTouchIsBusy)
            {
                if (!menuMode_CurTouch.IsFingerDown())
                {
                    if (menuMode_CurTouch.isRecentlyFingerDown)
                    {
                        movingTouchIsBusy = false;
                        curBusy_MovingTouch_Info = null;
                    }
                }
            }

            #endregion

            #region Touch (Set controls dem bools false)

            for (int i = 0; i < curTouchingControls.Count; i++)
            {
                if (curTouchingControls[i])
                {
                    curTouchingControls[i].isPressed = false;
                    curTouchingControls[i].ChangeStatus();
                }
            }

            curTouchingControls.RemoveRange(0, curTouchingControls.Count);

            //

            for (int i = 0; i < curTouchingLists.Count; i++)
            {
                if (curTouchingLists[i])
                {
                    curTouchingLists[i].isPressed = false;
                    curTouchingLists[i].ChangeStatus();
                    curTouchingLists[i].isFingerMoving = false;
                    curTouchingLists[i].pressingChildGroup = null;
                }
            }

            curTouchingLists.RemoveRange(0, curTouchingLists.Count);

            #endregion

            #region Touch (Controls)

            if (menuMode_CurTouch.status != TouchStatus.NoTouch)
            {
                ReSet_SortedGUIGroupsOrLists_TouchDepth();

                #region Check MovingTouchIsBusy

                if (movingTouchIsBusy)
                {
                    if (!touchIsDisabled)
                    {
                        if (curBusy_MovingTouch_Info != null && curBusy_MovingTouch_Info.changer != null)
                        {
                            curBusy_MovingTouch_Info.changer.OnMovingFinger();
                        }

                        if (curBusy_MovingTouch_Info != null && curBusy_MovingTouch_Info.control != null)
                        {
                            if (curBusy_MovingTouch_Info.isSlider)
                                curBusy_MovingTouch_Info.control.OnSlider();
                        }
                    }
                }

                #endregion

                for (int i = 0; i < sortedGroupOrLists_ByTouchDepth.Count; i++)
                {
                    if (sortedGroupOrLists_ByTouchDepth[i].group)
                    {
                        #region Group childs

                        GUIGroup gr = sortedGroupOrLists_ByTouchDepth[i].group;

                        for (int j = 0; j < gr.childs.Length; j++)
                        {
                            GUIControl datChild = gr.childs[j];

                            if (datChild.visibility_Total && datChild.isEnabled && datChild.isTouchable)
                            {
                                Rect rct = datChild.GetFinalRect();

                                if (rct.Contains(menuMode_CurTouch.pos_End))
                                {
                                    #region FreezeTouching
                                    if (menuMode_CurTouch.status == TouchStatus.FreezeTouching)
                                    {
                                        if (!touchIsDisabled)
                                        {
                                            if (datChild.sliderInfo)
                                            {
                                                if (rct.Contains(menuMode_CurTouch.pos_Start))
                                                {
                                                    movingTouchIsBusy = true;
                                                    curBusy_MovingTouch_Info = new BusyTouchInfo();
                                                    curBusy_MovingTouch_Info.control = datChild;
                                                    curBusy_MovingTouch_Info.isSlider = true;
                                                }
                                            }
                                            else
                                            {
                                                datChild.isPressed = true;
                                                datChild.ChangeStatus();
                                                curTouchingControls.Add(datChild);
                                            }
                                        }
                                    }
                                    #endregion

                                    #region Moving
                                    if (menuMode_CurTouch.status == TouchStatus.Moving)
                                    {
                                        if (!gr.changer)
                                        {
                                            #region Not Changer
                                            if (!movingTouchIsBusy)
                                            {
                                                if (!touchIsDisabled)
                                                {
                                                    if (datChild.sliderInfo)
                                                    {
                                                        if (rct.Contains(menuMode_CurTouch.pos_Start))
                                                        {
                                                            movingTouchIsBusy = true;
                                                            curBusy_MovingTouch_Info = new BusyTouchInfo();
                                                            curBusy_MovingTouch_Info.control = datChild;
                                                            curBusy_MovingTouch_Info.isSlider = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        datChild.isPressed = true;
                                                        datChild.ChangeStatus();
                                                        curTouchingControls.Add(datChild);
                                                    }
                                                }
                                            }
                                            #endregion
                                        }
                                        else
                                        {
                                            #region Changer
                                            if (!movingTouchIsBusy)
                                            {
                                                if (!touchIsDisabled)
                                                {
                                                    if (rct.Contains(menuMode_CurTouch.pos_Start))
                                                    {
                                                        movingTouchIsBusy = true;
                                                        curBusy_MovingTouch_Info = new BusyTouchInfo();
                                                        curBusy_MovingTouch_Info.changer = gr.changer;
                                                    }
                                                }
                                            }
                                            #endregion
                                        }
                                    }
                                    #endregion

                                    #region Tapped
                                    if (menuMode_CurTouch.status == TouchStatus.Tapped)
                                    {

                                        if (datChild.sliderInfo)
                                        {
                                            if (rct.Contains(menuMode_CurTouch.pos_Start))
                                            {
                                                datChild.OnSlider();
                                            }
                                        }
                                        else
                                        {
                                            if (rct.Contains(menuMode_CurTouch.pos_Start))
                                            {
                                                if (!touchIsDisabled)
                                                {
                                                    datChild.OnTap();
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                    #region TouchEnd
                                    if (menuMode_CurTouch.status == TouchStatus.TouchEnd)
                                    {
                                        if (!touchIsDisabled)
                                        {
                                            datChild.OnTouchEndedOnThis();
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }

                        #endregion
                    }
                    else
                    {
                        #region List

                        GUIList li = sortedGroupOrLists_ByTouchDepth[i].list;

                        if (li.visibility_Total && li.isTouchable)
                        {
                            Rect liRect = li.GetFinalRect();

                            if (menuMode_CurTouch.status == TouchStatus.Moving)
                            {
                                #region Moving
                                if (liRect.Contains(menuMode_CurTouch.pos_Start))
                                {
                                    if (!touchIsDisabled)
                                    {
                                        if (!movingTouchIsBusy)
                                        {
                                            movingTouchIsBusy = true;
                                            curBusy_MovingTouch_Info = new BusyTouchInfo();
                                            curBusy_MovingTouch_Info.list = li;
                                        }
                                        else
                                        {
                                            if (curBusy_MovingTouch_Info != null && li == curBusy_MovingTouch_Info.list)
                                            {
                                                li.OnMovingFinger();
                                                curTouchingLists.Add(li);
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                if (liRect.Contains(menuMode_CurTouch.pos_Start) && liRect.Contains(menuMode_CurTouch.pos_End))
                                {
                                    bool isAChildControlUnderTouch = false;

                                    GUIGroup liChildGroup = li.GetChildByPos(menuMode_CurTouch.pos_End);

                                    Rect li_DatControlRect = new Rect(0, 0, 0, 0);
                                    GUIControl li_DatControl = null;

                                    if (liChildGroup)
                                    {
                                        #region Sub control touch

                                        for (int ctrlInd = 0; ctrlInd < liChildGroup.childs.Length; ctrlInd++)
                                        {
                                            li_DatControl = liChildGroup.childs[ctrlInd];

                                            if (li_DatControl.visibility_Total && li_DatControl.isEnabled && li_DatControl.isTouchable)
                                            {
                                                li_DatControlRect = li_DatControl.GetFinalRect();

                                                if (li_DatControlRect.Contains(menuMode_CurTouch.pos_End))
                                                {
                                                    #region FreezeTouching
                                                    if (menuMode_CurTouch.status == TouchStatus.FreezeTouching)
                                                    {
                                                        if (!touchIsDisabled)
                                                        {
                                                            li_DatControl.isPressed = true;
                                                            li_DatControl.ChangeStatus();

                                                            curTouchingControls.Add(li_DatControl);

                                                            isAChildControlUnderTouch = true;
                                                        }
                                                    }
                                                    #endregion

                                                    #region Tapped
                                                    if (menuMode_CurTouch.status == TouchStatus.Tapped)
                                                    {
                                                        if (li_DatControlRect.Contains(menuMode_CurTouch.pos_Start))
                                                        {
                                                            if (!touchIsDisabled)
                                                            {
                                                                li_DatControl.OnTap();

                                                                isAChildControlUnderTouch = true;
                                                            }
                                                        }
                                                    }
                                                    #endregion

                                                    #region TouchEnd
                                                    if (menuMode_CurTouch.status == TouchStatus.TouchEnd)
                                                    {
                                                        if (!touchIsDisabled)
                                                        {
                                                            li_DatControl.OnTouchEndedOnThis();

                                                            isAChildControlUnderTouch = true;
                                                        }
                                                    }
                                                    #endregion
                                                }
                                            }
                                        }

                                        #endregion
                                    }

                                    if (!isAChildControlUnderTouch)
                                    {
                                        #region List touch

                                        if (liRect.Contains(menuMode_CurTouch.pos_End))
                                        {
                                            #region FreezeTouching
                                            if (menuMode_CurTouch.status == TouchStatus.FreezeTouching)
                                            {
                                                if (!touchIsDisabled)
                                                {
                                                    li.isPressed = true;

                                                    li.ChangeStatus();

                                                    curTouchingLists.Add(li);

                                                    li.pressingChildGroup = liChildGroup;
                                                }
                                            }
                                            #endregion

                                            #region Tapped
                                            if (menuMode_CurTouch.status == TouchStatus.Tapped)
                                            {
                                                if (liRect.Contains(menuMode_CurTouch.pos_Start))
                                                {
                                                    if (!touchIsDisabled)
                                                    {
                                                        li.OnTap(liChildGroup);
                                                    }
                                                }
                                            }
                                            #endregion

                                            #region TouchEnd
                                            if (menuMode_CurTouch.status == TouchStatus.TouchEnd)
                                            {
                                                if (!touchIsDisabled)
                                                {
                                                    li.OnTouchEndedOnThis(liChildGroup);
                                                }
                                            }
                                            #endregion
                                        }

                                        #endregion
                                    }
                                }
                            }
                        }

                        #endregion
                    }
                }
            }

            #endregion

        }
        #endregion
    }

    #region Touch (General)

    void CheckTouch()
    {
        for (int i = 0; i < touches.Count; i++)
        {
            if (touches[i].isSwipeHappened)
            {
                #region if swipe happened
                touches[i].isSwipeHappened = false;
                touches[i].isHorizontalSwipeHappened = false;
                touches[i].isVerticalSwipeHappened = false;
                touches[i].curHappenedNutsSwipe = SwipeSide.None;
                touches[i].eventHappened_SwipeDown = false;
                touches[i].eventHappened_SwipeUp = false;
                touches[i].eventHappened_SwipeLeft = false;
                touches[i].eventHappened_SwipeRight = false;

                touches[i].swipeUp_DistMag = 0;
                touches[i].swipeUp_Speed = 0;

                touches[i].swipeDown_DistMag = 0;
                touches[i].swipeDown_Speed = 0;

                touches[i].swipeRight_DistMag = 0;
                touches[i].swipeRight_Speed = 0;

                touches[i].swipeLeft_DistMag = 0;
                touches[i].swipeLeft_Speed = 0;

                touches[i].isSwipingDoneForThisTouch = true;
                #endregion
            }

            if (!touches[i].IsFingerDown())
            {
                if (touches[i].isRecentlyFingerDown)
                {
                    touches[i].isRecentlyFingerDown = false;
                }

                #region !touches[i].IsFingerDown()

                for (int j = 0; j < Input.touchCount; j++)
                {
                    Touch curTouch = Input.GetTouch(j);

                    bool isCurTouchRelated = false;

                    for (int k = 0; k < touches.Count; k++)
                    {
                        if (touches[k].IsFingerDown() && touches[k].fingerID == curTouch.fingerId)
                        {
                            isCurTouchRelated = true;
                            break;
                        }
                    }

                    if (!isCurTouchRelated)
                    {
                        touches[i].isFingerDown = true;
                        touches[i].SetStatus(TouchStatus.NoTouch);
                        touches[i].fingerID = curTouch.fingerId;

                        touches[i].pos_Start_Px = curTouch.position;
                        touches[i].pos_End_Px = touches[i].pos_Start_Px;
                        touches[i].deltaPos_Px = Vector2.zero;

                        touches[i].pos_Start = GetTouchPosInNewSpace(new Vector2(touches[i].pos_Start_Px.x, touches[i].pos_Start_Px.y));
                        touches[i].pos_End = touches[i].pos_Start;
                        touches[i].deltaPos = Vector2.zero;

                        touches[i].time_Start = Time.time;
                        touches[i].time_End = touches[i].time_Start;
                        touches[i].deltaTime = 0;

                        TouchDownInfo tdi = new TouchDownInfo();
                        tdi.pos = touches[i].pos_Start;
                        tdi.deltaPos = new Vector2(0, 0);
                        tdi.time = touches[i].time_Start;
                        tdi.deltaTime = 0;
                        touches[i].touchDownInfos.Add(tdi);
                        touches[i].curTouchDownInfosIndex = 0;

                        break;
                    }
                }

                #endregion
            }
            else
            {
                #region touches[i].IsFingerDown()

                Touch relatedTouch = new Touch();
                bool relatedTouchFound = false;

                for (int j = 0; j < Input.touchCount; j++)
                {
                    Touch curTouch = Input.GetTouch(j);

                    if (curTouch.fingerId == touches[i].fingerID)
                    {
                        relatedTouchFound = true;
                        relatedTouch = curTouch;
                        break;
                    }
                }

                if (relatedTouchFound)
                {
                    touches[i].pos_End_Px = relatedTouch.position;
                    touches[i].pos_End = GetTouchPosInNewSpace(new Vector2(touches[i].pos_End_Px.x, touches[i].pos_End_Px.y));

                    touches[i].deltaPos_Px = touches[i].pos_End_Px - touches[i].pos_Start_Px;
                    touches[i].deltaPos = touches[i].pos_End - touches[i].pos_Start;

                    touches[i].time_End = Time.time;
                    touches[i].deltaTime = touches[i].time_End - touches[i].time_Start;

                    if (touches[i].curTouchDownInfosIndex < touches[i].curTouchDownInfosMaxCount - 1)
                    {
                        touches[i].curTouchDownInfosIndex++;
                    }
                    else
                    {
                        touches[i].touchDownInfos.RemoveAt(0);
                    }

                    TouchDownInfo tdi = new TouchDownInfo();
                    tdi.pos = touches[i].pos_End;
                    tdi.deltaPos = tdi.pos - touches[i].touchDownInfos[touches[i].curTouchDownInfosIndex - 1].pos;
                    tdi.time = touches[i].time_End;
                    tdi.deltaTime = tdi.time - touches[i].touchDownInfos[touches[i].curTouchDownInfosIndex - 1].time;
                    touches[i].touchDownInfos.Add(tdi);

                }
                else
                {
                    touches[i].isFingerDown = false;
                    touches[i].isRecentlyFingerDown = true;
                    touches[i].isSwipingDoneForThisTouch = false;
                    touches[i].curTouchDownInfosIndex = -1;
                    touches[i].touchDownInfos.Clear();
                }

                #endregion
            }
        }
    }

    Vector2 GetTouchPosInNewSpace(Vector2 _touchPosInPixel)
    {
        Vector2 tpInPx = _touchPosInPixel;

        float daX = getTouchPos_CoefX * tpInPx.x - curScreen_HalfWidth;
        float daY = getTouchPos_CoefY * tpInPx.y - curScreen_HalfHeight;

        return new Vector2(daX, daY);
    }

    //void CheckTouch() //MOUSE (FOR TEST!!!!)
    //{
    //    if (touches[0].isSwipeHappened)
    //    {
    //        #region if swipe happened
    //        touches[0].isSwipeHappened = false;
    //        touches[0].isHorizontalSwipeHappened = false;
    //        touches[0].isVerticalSwipeHappened = false;
    //        touches[0].curHappenedNutsSwipe = SwipeSide.None;
    //        touches[0].eventHappened_SwipeDown = false;
    //        touches[0].eventHappened_SwipeUp = false;
    //        touches[0].eventHappened_SwipeLeft = false;
    //        touches[0].eventHappened_SwipeRight = false;

    //        touches[0].swipeUp_DistMag = 0;
    //        touches[0].swipeUp_Speed = 0;

    //        touches[0].swipeDown_DistMag = 0;
    //        touches[0].swipeDown_Speed = 0;

    //        touches[0].swipeRight_DistMag = 0;
    //        touches[0].swipeRight_Speed = 0;

    //        touches[0].swipeLeft_DistMag = 0;
    //        touches[0].swipeLeft_Speed = 0;

    //        touches[0].isSwipingDoneForThisTouch = true;
    //        #endregion
    //    }

    //    if (!touches[0].IsFingerDown())
    //    {
    //        if (touches[0].isRecentlyFingerDown)
    //        {
    //            touches[0].isRecentlyFingerDown = false;
    //        }

    //        #region !touches[i].IsFingerDown()

    //        if (Input.GetMouseButton(0))
    //        {
    //            touches[0].isFingerDown = true;
    //            touches[0].status = TouchStatus.NoTouch;
    //            touches[0].fingerID = 0;

    //            touches[0].pos_Start_Px = Input.mousePosition;
    //            touches[0].pos_End_Px = touches[0].pos_Start_Px;
    //            touches[0].deltaPos_Px = Vector2.zero;

    //            touches[0].pos_Start = GetTouchPosInNewSpace(new Vector2(touches[0].pos_Start_Px.x, touches[0].pos_Start_Px.y));
    //            touches[0].pos_End = touches[0].pos_Start;
    //            touches[0].deltaPos = Vector2.zero;

    //            touches[0].time_Start = Time.time;
    //            touches[0].time_End = touches[0].time_Start;
    //            touches[0].deltaTime = 0;

    //            TouchDownInfo tdi = new TouchDownInfo();
    //            tdi.pos = touches[0].pos_Start;
    //            tdi.deltaPos = new Vector2(0, 0);
    //            tdi.time = touches[0].time_Start;
    //            tdi.deltaTime = 0;
    //            touches[0].touchDownInfos.Add(tdi);
    //            touches[0].curTouchDownInfosIndex = 0;
    //        }

    //        #endregion
    //    }
    //    else
    //    {
    //        #region touches[i].IsFingerDown()

    //        if (Input.GetMouseButton(0))
    //        {
    //            touches[0].pos_End_Px = Input.mousePosition;
    //            touches[0].pos_End = GetTouchPosInNewSpace(new Vector2(touches[0].pos_End_Px.x, touches[0].pos_End_Px.y));

    //            touches[0].deltaPos_Px = touches[0].pos_End_Px - touches[0].pos_Start_Px;
    //            touches[0].deltaPos = touches[0].pos_End - touches[0].pos_Start;

    //            touches[0].time_End = Time.time;
    //            touches[0].deltaTime = touches[0].time_End - touches[0].time_Start;

    //            if (touches[0].curTouchDownInfosIndex < touches[0].curTouchDownInfosMaxCount - 1)
    //            {
    //                touches[0].curTouchDownInfosIndex++;
    //            }
    //            else
    //            {
    //                touches[0].touchDownInfos.RemoveAt(0);
    //            }

    //            TouchDownInfo tdi = new TouchDownInfo();
    //            tdi.pos = touches[0].pos_End;
    //            tdi.deltaPos = tdi.pos - touches[0].touchDownInfos[touches[0].curTouchDownInfosIndex - 1].pos;
    //            tdi.time = touches[0].time_End;
    //            tdi.deltaTime = tdi.time - touches[0].touchDownInfos[touches[0].curTouchDownInfosIndex - 1].time;
    //            touches[0].touchDownInfos.Add(tdi);

    //        }
    //        else
    //        {
    //            touches[0].isFingerDown = false;
    //            touches[0].isRecentlyFingerDown = true;
    //            touches[0].isSwipingDoneForThisTouch = false;
    //            touches[0].curTouchDownInfosIndex = -1;
    //            touches[0].touchDownInfos.Clear();
    //        }

    //        #endregion
    //    }
    //}

    //Vector2 GetTouchPosInNewSpace(Vector2 _touchPosInPixel)
    //{
    //    Vector2 tpInPx = _touchPosInPixel;

    //    float daX = getTouchPos_CoefX * Input.mousePosition.x - curScreen_HalfWidth;
    //    float daY = getTouchPos_CoefY * Input.mousePosition.y - curScreen_HalfHeight;

    //    return new Vector2(daX, daY);
    //}

    //JUST FOR TEST

    //void CheckMouse()
    //{
    //    if (Input.GetMouseButton(0))
    //    {
    //        if (touching)
    //        {
    //            touchTimeCounter += Time.deltaTime;

    //            touchEndPos = GetMouseTouchPos();
    //            touchEndTime = Time.time;
    //        }
    //        else
    //        {
    //            touching = true;
    //            touchFingerID = 0;
    //            touchStartPos = GetMouseTouchPos();
    //            touchEndPos = touchStartPos;
    //            touchStartTime = Time.time;
    //            touchEndTime = touchStartTime;
    //            touchTimeCounter = 0;
    //        }
    //    }
    //    else
    //        SetTouchIsEnded();

    //    //lastTimeTouchTimeCounterIncreasedInGUI = Time.time;
    //}

    //Vector2 GetMouseTouchPos()
    //{
    //    float daX = getTouchPos_CoefX * Input.mousePosition.x - curScreen_HalfWidth;
    //    float daY = getTouchPos_CoefY * Input.mousePosition.y - curScreen_HalfHeight;
    //    return new Vector2(daX, daY);
    //}

    //

    public void AddDisablingTouchInfo(BusyTouchInfo _busyTouch)
    {
        if (!curTouchInfosDatDisabledTouch.Contains(_busyTouch))
            curTouchInfosDatDisabledTouch.Add(_busyTouch);

        touchIsDisabled = (curTouchInfosDatDisabledTouch.Count > 0);
    }

    public void RemoveDisablingTouchInfo(BusyTouchInfo _busyTouch)
    {
        if (curTouchInfosDatDisabledTouch.Contains(_busyTouch))
            curTouchInfosDatDisabledTouch.Remove(_busyTouch);

        touchIsDisabled = (curTouchInfosDatDisabledTouch.Count > 0);
    }

    #endregion

    //

    public float ConvertValToNewSpace(float _val)
    {
        return _val * oldSpaceToNewSpaceCoef;
    }

    public float ConvertPixelToNewSpace(float _pixelVal)
    {
        float p = _pixelVal * pixelToOldSpaceCoef;
        return ConvertValToNewSpace(p);
    }

    //public void ChangeScreenOrientaion(ScreenOrientation _side)
    //{
    //    Screen.orientation = _side;

    //    ReInitScreenSideRelatedParams();
    //}

    public float ConvertNewSpaceValToOldSpace(float _val)
    {
        return _val / oldSpaceToNewSpaceCoef;
    }

    //

    #region Group and List

    void SortGUIGroupsByDepth()
    {
        List<GUIGroup> unsortedGUIGroups = new List<GUIGroup>();

        for (int i = 0; i < currentGUIGroups.Count; i++)
        {
            unsortedGUIGroups.Add(currentGUIGroups[i]);
        }

        while (unsortedGUIGroups.Count > 0)
        {
            int minDepth = int.MaxValue;
            int selectedIndex = -1;

            for (int i = 0; i < unsortedGUIGroups.Count; i++)
            {
                if (unsortedGUIGroups[i].depth < minDepth)
                {
                    minDepth = unsortedGUIGroups[i].depth;
                    selectedIndex = i;
                }
            }

            sortedGUIGroups.Add(unsortedGUIGroups[selectedIndex]);
            unsortedGUIGroups.RemoveAt(selectedIndex);
        }
    }

    void ReSet_SortedGUIGroups_TouchDepth()
    {
        sortedGUIGroups_ByTouchDepth = new List<GUIGroup>();

        bool isAnyVisibleGroupFound = false;
        float visGroupTouchDepth = 0;

        for (int i = sortedGUIGroups.Count - 1; i >= 0; i--)
        {
            if (!isAnyVisibleGroupFound)
            {
                if (sortedGUIGroups[i].visibility_Total)
                {
                    isAnyVisibleGroupFound = true;
                    visGroupTouchDepth = sortedGUIGroups[i].touchDepth;
                    sortedGUIGroups_ByTouchDepth.Add(sortedGUIGroups[i]);
                }
            }
            else
            {
                if (sortedGUIGroups[i].visibility_Total)
                {
                    if (sortedGUIGroups[i].touchDepth >= visGroupTouchDepth)
                        sortedGUIGroups_ByTouchDepth.Add(sortedGUIGroups[i]);
                    else
                        return;
                }
            }
        }
    }

    public GUIGroup FindChildGroupByName(string _name)
    {
        for (int i = 0; i < currentGUIGroups.Count; i++)
        {
            if (currentGUIGroups[i].myName == _name)
                return currentGUIGroups[i];
        }

        return null;
    }

    //

    void SortGUIListsByDepth()
    {
        List<GUIList> unsortedGUILists = new List<GUIList>();

        for (int i = 0; i < currentGUILists.Count; i++)
        {
            unsortedGUILists.Add(currentGUILists[i]);
        }

        while (unsortedGUILists.Count > 0)
        {
            int minDepth = int.MaxValue;
            int selectedIndex = -1;

            for (int i = 0; i < unsortedGUILists.Count; i++)
            {
                if (unsortedGUILists[i].depth < minDepth)
                {
                    minDepth = unsortedGUILists[i].depth;
                    selectedIndex = i;
                }
            }

            sortedGUILists.Add(unsortedGUILists[selectedIndex]);
            unsortedGUILists.RemoveAt(selectedIndex);
        }
    }

    void ReSet_SortedGUILists_TouchDepth()
    {
        sortedGUILists_ByTouchDepth = new List<GUIList>();

        bool isAnyVisibleListFound = false;
        float visListTouchDepth = 0;

        for (int i = sortedGUILists.Count - 1; i >= 0; i--)
        {
            if (!isAnyVisibleListFound)
            {
                if (sortedGUILists[i].visibility_Total)
                {
                    isAnyVisibleListFound = true;
                    visListTouchDepth = sortedGUILists[i].touchDepth;
                    sortedGUILists_ByTouchDepth.Add(sortedGUILists[i]);
                }
            }
            else
            {
                if (sortedGUILists[i].visibility_Total)
                {
                    if (sortedGUILists[i].touchDepth >= visListTouchDepth)
                        sortedGUILists_ByTouchDepth.Add(sortedGUILists[i]);
                    else
                        return;
                }
            }
        }
    }

    public GUIList FindChildListByName(string _name)
    {
        for (int i = 0; i < currentGUILists.Count; i++)
        {
            if (currentGUILists[i].myName == _name)
                return currentGUILists[i];
        }

        return null;
    }

    //

    void SortGUIListOrGroupByDepth()
    {
        sortedGroupOrLists = new List<GroupOrList>();

        int groupInd = 0;
        int listInd = 0;

        while (groupInd < sortedGUIGroups.Count || listInd < sortedGUILists.Count)
        {
            int minDepth = int.MaxValue;
            bool isFromGroups = true;

            if (groupInd < sortedGUIGroups.Count)
            {
                if (sortedGUIGroups[groupInd].depth < minDepth)
                {
                    minDepth = sortedGUIGroups[groupInd].depth;
                }
            }

            if (listInd < sortedGUILists.Count)
            {
                if (sortedGUILists[listInd].depth < minDepth)
                {
                    minDepth = sortedGUILists[listInd].depth;
                    isFromGroups = false;
                }
            }

            GroupOrList grOrLi = new GroupOrList();

            if (isFromGroups)
            {
                grOrLi.group = sortedGUIGroups[groupInd];

                sortedGroupOrLists.Add(grOrLi);

                groupInd++;
            }
            else
            {
                grOrLi.list = sortedGUILists[listInd];

                sortedGroupOrLists.Add(grOrLi);

                listInd++;
            }
        }
    }

    void ReSet_SortedGUIGroupsOrLists_TouchDepth()
    {
        sortedGroupOrLists_ByTouchDepth = new List<GroupOrList>();

        bool isAnyVisibleItemFound = false;
        float visItemTouchDepth = 0;

        bool noMoreGroups = false;
        bool noMoreLists = false;

        for (int i = sortedGroupOrLists.Count - 1; i >= 0; i--)
        {
            if (!isAnyVisibleItemFound)
            {
                if (sortedGroupOrLists[i].group)
                {
                    if (sortedGroupOrLists[i].group.visibility_Total)
                    {
                        isAnyVisibleItemFound = true;
                        visItemTouchDepth = sortedGroupOrLists[i].group.touchDepth;
                        sortedGroupOrLists_ByTouchDepth.Add(sortedGroupOrLists[i]);
                    }
                }
                else
                {
                    if (sortedGroupOrLists[i].list.visibility_Total)
                    {
                        isAnyVisibleItemFound = true;
                        visItemTouchDepth = sortedGroupOrLists[i].list.touchDepth;
                        sortedGroupOrLists_ByTouchDepth.Add(sortedGroupOrLists[i]);
                    }
                }
            }
            else
            {
                if (noMoreGroups && noMoreLists)
                    return;

                if (!noMoreGroups)
                {
                    if (sortedGroupOrLists[i].group)
                    {
                        if (sortedGroupOrLists[i].group.visibility_Total)
                        {
                            if (sortedGroupOrLists[i].group.touchDepth >= visItemTouchDepth)
                                sortedGroupOrLists_ByTouchDepth.Add(sortedGroupOrLists[i]);
                            else
                                noMoreGroups = true;
                        }
                    }
                }

                if (!noMoreLists)
                {
                    if (sortedGroupOrLists[i].list)
                    {
                        if (sortedGroupOrLists[i].list.visibility_Total)
                        {
                            if (sortedGroupOrLists[i].list.touchDepth >= visItemTouchDepth)
                                sortedGroupOrLists_ByTouchDepth.Add(sortedGroupOrLists[i]);
                            else
                                noMoreLists = true;
                        }
                    }
                }
            }
        }
    }

    #endregion

    #region Keyboard (English???)
    public void ShowKeyboard(TextPack _textPackToEdit)
    {
        if (keyboardShowingStatus != ObjShowStatus.FullHide)
            return;

        keyboardShowingStatus = ObjShowStatus.GoingToShow;

        keyboard.StartAnim_Y_Alpha(0, -keyboardBG.Get_Height(), 0, animCurve_Keybord_Y_Show, 0, 1, animCurve_Keybord_Y_Show, keyboardAnimSpeed, AnimMode.Show);
        keyboardTextBox.StartAnim_Alpha(0, 0, 1, animCurve_Keybord_Y_Show, keyboardAnimSpeed, AnimMode.Show);

        curKeyboardTextPack = _textPackToEdit;

        keyboardTextBox_PersianTextBox.ChangeText(curKeyboardTextPack.text);

        SetKeyboardCursorIndex(keyboardTextBox_PersianTextBox.text.Length);
    }

    public void HideKeyboard()
    {
        if (keyboardShowingStatus != ObjShowStatus.FullShow)
            return;

        keyboardShowingStatus = ObjShowStatus.GoingToHide;

        keyboard.StartAnim_Y_Alpha(0, 0, -keyboardBG.Get_Height(), animCurve_Keybord_Y_Hide, 1, 0, animCurve_Keybord_Y_Hide, keyboardAnimSpeed, AnimMode.Hide);
        keyboardTextBox.StartAnim_Alpha(0, 1, 0, animCurve_Keybord_Y_Hide, keyboardAnimSpeed, AnimMode.Hide);
    }

    public bool IsKeyboardFullyShown()
    {
        return keyboardShowingStatus == ObjShowStatus.FullShow;
    }

    public bool IsKeyboardFullyHidden()
    {
        return keyboardShowingStatus == ObjShowStatus.FullHide;
    }

    public void SetKeyboardCursorPosX_By_PosIndex()
    {
        GUIControl guiControl = keyboardTextBox_PersianTextBox;
        int posIndex = keyboardCursorPosIndex;

        List<Rect> textRects = guiControl.GetFinalTextRects();

        if (posIndex == 0)
            if (textRects != null && textRects.Count > 0)
            {
                if (guiControl.isTextPersian)
                    keyboardTextBox_Cursor.SetXByRect(new Rect(textRects[0]), GUIRectAllignX.Right_Between);
                else
                    keyboardTextBox_Cursor.SetXByRect(new Rect(textRects[0]), GUIRectAllignX.Left_Between);
            }
            else
                keyboardTextBox_Cursor.SetXByRect(new Rect(0, 0, 0, 0), GUIRectAllignX.Mid);

        if (guiControl.isTextPersian)
            keyboardTextBox_Cursor.SetXByRect(new Rect(textRects[posIndex - 1]), GUIRectAllignX.Left_Between);
        else
            keyboardTextBox_Cursor.SetXByRect(new Rect(textRects[posIndex - 1]), GUIRectAllignX.Right_Between);

        keyboardTextBox_Cursor.ChangeX();
    }

    public void SetKeyboardCursorIndex(int _val)
    {
        keyboardCursorPosIndex = _val;
        if (keyboardCursorPosIndex < 0)
            keyboardCursorPosIndex = 0;

        SetKeyboardCursorPosX_By_PosIndex();
    }

    //

    public void KeyboardKeyPressed(string _keyName)
    {
        if (IsKeyboardFullyShown())
        {
            string keyName = _keyName;

            System.String str = keyName;

            string keyFirst3Letters = str.Substring(0, 3).ToString();

            #region Function Keys

            if (keyFirst3Letters == "Fun")
            {
                if (keyName == "Fun_Backspace")
                {
                    DoKeyboardBackspace();
                }

                if (keyName == "Fun_Shift")
                {
                    keyboardShift.isActivated = !keyboardShift.isActivated;
                    keyboardShift.ChangeStatus();
                    UpdateKeyboardKeys();
                }

                if (keyName == "Fun_Ok")
                {
                    curKeyboardTextPack.ApplyResult();
                    HideKeyboard();
                }

            }

            #endregion

            #region Letter Keys

            if (keyFirst3Letters == "Key")
            {
                if (curKeyboardTextPack.text.Length < St_ExtraTextFuncs.GUIControl_Text_MaxCap)
                {

                    string newTxt = textToPersianConverter.GetEnglishLetterByKeyboardKeyName(_keyName);

                    bool isNewTextANumber = textToPersianConverter.IsCurrentEnglishLetterANumber(newTxt);

                    int an = 0;
                    for (int i = keyboardCursorPosIndex; i < curKeyboardTextPack.text.Length; i++)
                    {
                        if (textToPersianConverter.IsCurrentEnglishLetterANumber(curKeyboardTextPack.text[i].ToString()))
                            an++;
                        else
                            break;
                    }

                    int bn = 0;
                    for (int i = keyboardCursorPosIndex - 1; i >= 0; i--)
                    {
                        if (textToPersianConverter.IsCurrentEnglishLetterANumber(curKeyboardTextPack.text[i].ToString()))
                            bn++;
                        else
                            break;
                    }

                    int newCursorIndex = keyboardCursorPosIndex;
                    int textInsertIndex = keyboardCursorPosIndex;

                    if (an == 0 && bn == 0)
                    {
                        newCursorIndex += 1;
                    }

                    if (an > 0 && bn == 0)
                    {
                        if (!isNewTextANumber)
                        {
                            newCursorIndex += 1;
                        }
                        else
                        {
                            newCursorIndex += an - bn + 1;
                            textInsertIndex += an - bn;
                        }
                    }

                    if (an > 0 && bn > 0)
                    {
                        if (!isNewTextANumber)
                        {
                            newCursorIndex += an - bn + 1;
                            textInsertIndex += an - bn;
                        }
                        else
                        {
                            textInsertIndex += an - bn;
                        }
                    }

                    if (an == 0 && bn > 0)
                    {
                        if (!isNewTextANumber)
                        {
                            newCursorIndex += 1;
                        }
                        else
                        {
                            newCursorIndex += 1;
                        }
                    }

                    if (textInsertIndex == curKeyboardTextPack.text.Length)
                    {
                        curKeyboardTextPack.SetText(curKeyboardTextPack.text + newTxt);
                    }
                    else
                    {
                        string txt = curKeyboardTextPack.text;
                        txt = txt.Insert(textInsertIndex, newTxt);

                        curKeyboardTextPack.SetText(txt);
                    }

                    UpdateKeyboardTextBox();

                    SetKeyboardCursorIndex(newCursorIndex);
                }
            }

            #endregion
        }
    }

    void UpdateKeyboardKeys()
    {
        if (keyboardShift.isActivated)
        {
            for (int i = 0; i < keyboard.childs.Length; i++)
            {
                GUIControl guiCtrl = keyboard.childs[i];

                if (guiCtrl.extraTexts.Length > 0)
                {
                    if (guiCtrl.extraTexts[0] == "primary")
                    {
                        guiCtrl.isVisible = false;
                        guiCtrl.ChangeVisibility();
                    }

                    if (guiCtrl.extraTexts[0] == "secondary")
                    {
                        guiCtrl.isVisible = true;
                        guiCtrl.ChangeVisibility();
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < keyboard.childs.Length; i++)
            {
                GUIControl guiCtrl = keyboard.childs[i];

                if (guiCtrl.extraTexts.Length > 0)
                {
                    if (guiCtrl.extraTexts[0] == "primary")
                    {
                        guiCtrl.isVisible = true;
                        guiCtrl.ChangeVisibility();
                    }

                    if (guiCtrl.extraTexts[0] == "secondary")
                    {
                        guiCtrl.isVisible = false;
                        guiCtrl.ChangeVisibility();
                    }
                }
            }
        }
    }

    public void DoKeyboardBackspace()
    {
        System.String str;

        str = curKeyboardTextPack.text;

        bool isLetterRemoved = false;

        int newCursorIndex = keyboardCursorPosIndex - 1;
        int textRemoveIndex = keyboardCursorPosIndex - 1;


        if (str != null && str.Length > 0)
        {
            if (keyboardCursorPosIndex > 0)
            {
                string datLetter = curKeyboardTextPack.text[keyboardCursorPosIndex - 1].ToString();

                bool isDatLetterANumber = textToPersianConverter.IsCurrentEnglishLetterANumber(datLetter);

                int an = 0;
                for (int i = keyboardCursorPosIndex; i < curKeyboardTextPack.text.Length; i++)
                {
                    if (textToPersianConverter.IsCurrentEnglishLetterANumber(curKeyboardTextPack.text[i].ToString()))
                        an++;
                    else
                        break;
                }

                int bn = 0;
                if (!isDatLetterANumber)
                {
                    for (int i = keyboardCursorPosIndex - 2; i >= 0; i--)
                    {
                        if (textToPersianConverter.IsCurrentEnglishLetterANumber(curKeyboardTextPack.text[i].ToString()))
                            bn++;
                        else
                            break;
                    }
                }
                else
                {
                    for (int i = keyboardCursorPosIndex - 1; i >= 0; i--)
                    {
                        if (textToPersianConverter.IsCurrentEnglishLetterANumber(curKeyboardTextPack.text[i].ToString()))
                            bn++;
                        else
                            break;
                    }
                }

                if (an > 0 && bn > 0)
                {
                    if (!isDatLetterANumber)
                    {
                        newCursorIndex += an - bn;
                    }
                    else
                    {
                        newCursorIndex++;
                        textRemoveIndex += an - bn;
                    }
                }

                isLetterRemoved = true;
                str = str.Remove(textRemoveIndex, 1);
            }
        }

        if (str != null)
            curKeyboardTextPack.SetText(str.ToString());

        UpdateKeyboardTextBox();

        if (isLetterRemoved)
        {
            SetKeyboardCursorIndex(newCursorIndex);
        }
    }

    void UpdateKeyboardTextBox()
    {
        keyboardTextBox_PersianTextBox.ChangeText(curKeyboardTextPack.text);

        keyboardOK.isEnabled = true;
        keyboardOK.ChangeStatus();

        if (!curKeyboardTextPack.isEmptyTextAcceptable)
        {
            if (St_ExtraTextFuncs.IsTextNullOrEmptyOrWhiteSpace(curKeyboardTextPack.text))
            {
                keyboardOK.isEnabled = false;
                keyboardOK.ChangeStatus();
            }
        }

        if (curKeyboardTextPack.considerUnavailableTexts)
        {
            for (int i = 0; i < curKeyboardTextPack.unavailableTexts.Count; i++)
            {
                if (curKeyboardTextPack.unavailableTexts[i] == curKeyboardTextPack.text)
                {
                    keyboardOK.isEnabled = false;
                    keyboardOK.ChangeStatus();
                    break;
                }
            }
        }
    }

    public void KeyboardTextBoxCursorPressed(TouchUpInfo _touchUpInfo)
    {
        TouchUpInfo touchUpInfo = _touchUpInfo;
        List<Rect> kbTxtBox_TextRects = keyboardTextBox_PersianTextBox.GetFinalTextRects();
        Rect rect = keyboardTextBox_PersianTextBox.GetFinalRect();

        bool isPersian = keyboardTextBox_PersianTextBox.isTextPersian;

        if (kbTxtBox_TextRects.Count == 0)
        {
            SetKeyboardCursorIndex(0);
            return;
        }

        for (int i = 0; i < kbTxtBox_TextRects.Count; i++)
        {
            Rect rct = kbTxtBox_TextRects[i];
            rct = new Rect(rct.xMin, rect.yMin, rct.width, rect.height);
            Rect rctL = new Rect(rct.xMin, rct.yMin, rct.width / 2, rct.height);
            Rect rctR = new Rect(rct.xMin + rct.width / 2, rct.yMin, rct.width / 2, rct.height);

            if (rctR.Contains(touchUpInfo.pos_End))
            {
                if (isPersian)
                    SetKeyboardCursorIndex(i);
                else
                    SetKeyboardCursorIndex(i + 1);

                return;
            }

            if (rctL.Contains(touchUpInfo.pos_End))
            {
                if (isPersian)
                    SetKeyboardCursorIndex(i + 1);
                else
                    SetKeyboardCursorIndex(i);

                return;
            }
        }

        Rect rctt = kbTxtBox_TextRects[0];

        if (touchUpInfo.pos_End.x > (rctt.xMin + rctt.width / 2))
        {
            if (isPersian)
                SetKeyboardCursorIndex(0);
            else
                SetKeyboardCursorIndex(kbTxtBox_TextRects.Count);
            return;
        }

        rctt = kbTxtBox_TextRects[kbTxtBox_TextRects.Count - 1];

        if (touchUpInfo.pos_End.x < (rctt.xMin + rctt.width / 2))
        {
            if (isPersian)
                SetKeyboardCursorIndex(kbTxtBox_TextRects.Count);
            else
                SetKeyboardCursorIndex(0);
            return;
        }
    }

    #endregion

    //

    //public static float Scale(float _val)
    //{
    //    return _val * pixelToOldSpaceCoef;
    //}

    //public static float Unscale(float _val)
    //{
    //    return _val / pixelToOldSpaceCoef;
    //}

    public bool CheckTouchForRect(TouchInfo _touchInf, Rect _rect, TouchInRectMode _mode)
    {
        Rect rect = _rect;
        TouchInRectMode mode = _mode;
        TouchInfo curTouch = _touchInf;

        if (curTouch != null && curTouch.status != TouchStatus.NoTouch)
        {
            switch (mode)
            {
                case TouchInRectMode.StartAndEnd:
                    if (rect.Contains(curTouch.pos_Start) && rect.Contains(curTouch.pos_End))
                    {
                        return true;
                    }
                    break;

                case TouchInRectMode.OnlyStart:
                    if (rect.Contains(curTouch.pos_Start))
                    {
                        return true;
                    }
                    break;

                case TouchInRectMode.OnlyEnd:
                    if (rect.Contains(curTouch.pos_End))
                    {
                        return true;
                    }
                    break;
            }
        }

        return false;
    }


    public TouchInfo JustCheckTouchForRect(Rect _rect, TouchInRectMode _mode, List<TouchStatus> _validTouchStates)
    {
        Rect rect = _rect;
        TouchInRectMode mode = _mode;
        List<TouchStatus> validTouchStates = _validTouchStates;

        for (int i = 0; i < touches.Count; i++)
        {
            TouchInfo curTouch = touches[i];

            if (curTouch != null && curTouch.status != TouchStatus.NoTouch)
            {
                if (CheckTouchForRect(curTouch, rect, mode))
                {
                    for (int j = 0; j < validTouchStates.Count; j++)
                    {
                        if (curTouch.status == validTouchStates[j])
                        {
                            //print(Time.time + " " + curTouch.status);
                            return curTouch;
                        }
                    }
                }
            }
        }

        return null;
    }


    public TouchInfo CheckAndReserveTouchForRect(Rect _rect, TouchInRectMode _mode, string _ownerName)
    {
        Rect rect = _rect;
        TouchInRectMode mode = _mode;
        string ownerName = _ownerName;

        for (int i = 0; i < touches.Count; i++)
        {
            TouchInfo curTouch = touches[i];

            if (curTouch != null && curTouch.status != TouchStatus.NoTouch && curTouch.ownerName == ownerName)
            {

                //print(curTouch.ownerName);
                if (CheckTouchForRect(curTouch, rect, mode))
                    return curTouch;
                else
                    return null;
            }
        }

        for (int i = 0; i < touches.Count; i++)
        {
            TouchInfo curTouch = touches[i];

            if (curTouch != null && curTouch.status != TouchStatus.NoTouch && curTouch.ownerName == "")
            {
                if (CheckTouchForRect(curTouch, rect, mode))
                {

                    //print("New:    " + curTouch.ownerName);
                    curTouch.SetOwnerName(ownerName);
                    return curTouch;
                }
            }
        }

        return null;
    }

    public Vector3 Convert3DPointToNewSpace(Vector3 _vec)
    {
        Vector3 vec = _vec;
        Vector3 scPoint = Camera.main.WorldToScreenPoint(vec);
        scPoint = new Vector3((((scPoint.x / (float)Screen.width) - 0.5f) * curScreen_Width), (((scPoint.y / (float)Screen.height) - 0.5f) * curScreen_Height), 0);

        return scPoint;
        //return new Vector3(ConvertPixelToNewSpace(scPoint.x), ConvertPixelToNewSpace(scPoint.y), ConvertPixelToNewSpace(scPoint.z));



        //a1.transform.position = new Vector3(vecMin.x, vecMin.y, 0);
        //a2.transform.position = new Vector3(vecMin.x, vecMax.y, 0);
        //a3.transform.position = new Vector3(vecMax.x, vecMin.y, 0);
        //a4.transform.position = new Vector3(vecMax.x, vecMax.y, 0);
    }

    public Rect GetRectOfBounds(Bounds _bounds)
    {
        Bounds bnds = _bounds;

        Vector3 vecMin = Convert3DPointToNewSpace(bnds.min);
        Vector3 vecMax = Convert3DPointToNewSpace(bnds.max);

        return new Rect(vecMin.x, vecMin.y, vecMax.x - vecMin.x, vecMax.y - vecMin.y);
    }
}
