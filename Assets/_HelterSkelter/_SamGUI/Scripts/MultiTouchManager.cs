using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum JoypadArrow
{
    none,
    right,
    rightUp,
    up,
    leftUp,
    left,
    leftDown,
    down,
    rightDown,

}

public class MultiTouchManager : MonoBehaviour
{
    public static MultiTouchManager Instance;

    public delegate void ButtonsAndJoystickEnability();
    public event ButtonsAndJoystickEnability Event_TapButtonsOrJoystickEnabilityChanged;

    public GUIControl guiCTRL_PadBG;
    public GUIControl guiCTRL_PadStick;
    public GUIControl guiCTRL_Screen;

    [HideInInspector]
    public IngameButton curIngameButton = null;

    [HideInInspector]
    public bool IsJoystickEnabled = true;

    [HideInInspector]
    public bool AreTapButtonsEnabled = true;

    [HideInInspector]
    public SpriteRenderer curHideout_OutButton = null;

    [HideInInspector]
    public Hideout curRelatedHideout = null;

    //



    Rect padBGRect;
    TouchInfo touchInf_Pad;
    float padMaxAlpha = 0.35f;
    float padAlphaShowSpeed = 3.6f;
    float padAlphaHideSpeed = 3.6f;
    string padOwnerName = "Pad";

    float maxRadius = 0;

    float stickLerpSpeed_Go = 30f;
    float stickLerpSpeed_Back = 22f;

    TouchInfo touchInfo_IngameButton;
    TouchInfo touchInfo_Hideout_OutButton;
    string buttonOwnerName = "Button";

    [HideInInspector]
    public Vector3 joypadAxis = Vector3.zero;

    float minNeedAccForDescreteArrows = 0.3f;

    float arrowsToghserCoef = 0.28f;

    bool isInited = false;

    Rect screenRect;
    TouchInfo touchInf_Screen_Pad;
    TouchInfo touchInf_Screen_Button;
    bool shouldCheckIngameButton = true;

    [HideInInspector]
    public bool areInGameButtonsSikiminEnabled = true;

    List<int> tocuhBusyFingerIDs = new List<int>();
    List<TouchStatus> padValidTouchStates = new List<TouchStatus>();
    List<TouchStatus> buttonValidTouchStates = new List<TouchStatus>();

    void Awake()
    {
        Instance = this;

        padValidTouchStates.Add(TouchStatus.FreezeTouching);
        padValidTouchStates.Add(TouchStatus.Moving);

        buttonValidTouchStates.Add(TouchStatus.Tapped);
    }

    void Start()
    {
        screenRect = guiCTRL_Screen.GetFinalRect();
    }

    void Update()
    {
        if (!isInited)
        {
            isInited = true;
        }

        tocuhBusyFingerIDs.Clear();


        if (IsJoystickSikiminallyEnabled())
        {
            touchInf_Screen_Pad = MyGUIController.Instance.JustCheckTouchForRect(screenRect, TouchInRectMode.OnlyStart, padValidTouchStates);
        }

        if(AreTapButtonsSikiminallyEnabled())
        {
            touchInf_Screen_Button = MyGUIController.Instance.JustCheckTouchForRect(screenRect, TouchInRectMode.StartAndEnd, buttonValidTouchStates);
        }

        #region Joystick


        if (IsJoystickSikiminallyEnabled() && touchInf_Screen_Pad != null)
        {
            if (((touchInf_Screen_Pad.status == TouchStatus.FreezeTouching) && !(touchInf_Screen_Pad.IsTapTimeStillValid())) || touchInf_Screen_Pad.status == TouchStatus.Moving)
            {
                #region Inc Pad alphas
                if (guiCTRL_PadBG.alpha < padMaxAlpha)
                {
                    guiCTRL_PadBG.SetAlpha(guiCTRL_PadBG.alpha + padAlphaShowSpeed * Time.deltaTime);

                    if (guiCTRL_PadBG.alpha > padMaxAlpha)
                        guiCTRL_PadBG.SetAlpha(padMaxAlpha);

                    guiCTRL_PadBG.ChangeAlpha();
                }

                if (guiCTRL_PadStick.alpha < padMaxAlpha)
                {
                    guiCTRL_PadStick.SetAlpha(guiCTRL_PadStick.alpha + padAlphaShowSpeed * Time.deltaTime);

                    if (guiCTRL_PadStick.alpha > padMaxAlpha)
                        guiCTRL_PadStick.SetAlpha(padMaxAlpha);

                    guiCTRL_PadStick.ChangeAlpha();
                }
                #endregion

                #region SetPadInitialPos
                if (guiCTRL_PadBG.x != touchInf_Screen_Pad.pos_Start.x)
                {
                    guiCTRL_PadBG.x = touchInf_Screen_Pad.pos_Start.x;
                    guiCTRL_PadBG.ChangeX();
                }

                if (guiCTRL_PadBG.y != touchInf_Screen_Pad.pos_Start.y)
                {
                    guiCTRL_PadBG.y = touchInf_Screen_Pad.pos_Start.y;
                    guiCTRL_PadBG.ChangeY();
                }
                #endregion

                #region Stick
                padBGRect = guiCTRL_PadBG.GetFinalRect();
                maxRadius = padBGRect.width / guiCTRL_PadBG.extraNumbers[0];

                touchInf_Pad = MyGUIController.Instance.CheckAndReserveTouchForRect(padBGRect, TouchInRectMode.OnlyStart, padOwnerName);

                if (touchInf_Pad != null)
                {
                    float stickDistX = touchInf_Pad.pos_End.x - padBGRect.center.x;
                    float stickDistY = touchInf_Pad.pos_End.y - padBGRect.center.y;

                    float ang = Mathf.Atan(stickDistY / stickDistX);
                    float cosAng = Mathf.Cos(ang);
                    float AbsSinAng = Mathf.Abs(Mathf.Sin(ang));

                    guiCTRL_PadStick.x = Mathf.Lerp(guiCTRL_PadStick.x, Mathf.Clamp(stickDistX, cosAng * -maxRadius, cosAng * maxRadius), stickLerpSpeed_Go * Time.deltaTime);
                    guiCTRL_PadStick.ChangeX();
                    guiCTRL_PadStick.y = Mathf.Lerp(guiCTRL_PadStick.y, Mathf.Clamp(stickDistY, AbsSinAng * -maxRadius, AbsSinAng * maxRadius), stickLerpSpeed_Go * Time.deltaTime);
                    guiCTRL_PadStick.ChangeY();
                }
                else
                {
                    guiCTRL_PadStick.x = Mathf.Lerp(guiCTRL_PadStick.x, 0, stickLerpSpeed_Back * Time.deltaTime);
                    guiCTRL_PadStick.ChangeX();
                    guiCTRL_PadStick.y = Mathf.Lerp(guiCTRL_PadStick.y, 0, stickLerpSpeed_Back * Time.deltaTime);
                    guiCTRL_PadStick.ChangeY();
                }


                joypadAxis = new Vector3(guiCTRL_PadStick.x / maxRadius, guiCTRL_PadStick.y / maxRadius);
                #endregion
            }

        }
        else
        {
            #region Dec Pad alphas
            if (guiCTRL_PadBG.alpha > 0)
            {
                guiCTRL_PadBG.SetAlpha(guiCTRL_PadBG.alpha - padAlphaHideSpeed * Time.deltaTime);

                guiCTRL_PadBG.ChangeAlpha();
            }

            if (guiCTRL_PadStick.alpha > 0)
            {
                guiCTRL_PadStick.SetAlpha(guiCTRL_PadStick.alpha - padAlphaShowSpeed * Time.deltaTime);

                guiCTRL_PadStick.ChangeAlpha();
            }
            #endregion

            joypadAxis = Vector3.zero;
        }

        #endregion

        #region Ingame Button

        if (AreTapButtonsSikiminallyEnabled() && curIngameButton != null && curIngameButton.isButtonEnabled)
        {
            if (touchInf_Screen_Button != null && touchInf_Screen_Button.status == TouchStatus.Tapped)
            {
                touchInfo_IngameButton = MyGUIController.Instance.CheckAndReserveTouchForRect(screenRect, TouchInRectMode.OnlyStart, buttonOwnerName);

                shouldCheckIngameButton = true;
            }
        }
        else
        {
            shouldCheckIngameButton = false;
        }

        if (shouldCheckIngameButton && touchInfo_IngameButton != null)
        {
            if (touchInfo_IngameButton.status == TouchStatus.Tapped)
            {
                curIngameButton.Activate();
            }
            //else
            //{
                //if ((touchInfo_IngameButton.status == TouchStatus.FreezeTouching && touchInfo_IngameButton.IsTapTimeStillValid()))
                //{
                //    curIngameButton.SetSpritePic(IngameButtonPicType.PRESSED);
                //}

                //if (touchInfo_IngameButton.status == TouchStatus.Moving || (touchInfo_IngameButton.status == TouchStatus.FreezeTouching && !touchInfo_IngameButton.IsTapTimeStillValid()))
                //{
                //    curIngameButton.SetAlpha(0);
                //}
            //}
        }
        else
        {
            if (curIngameButton != null && curIngameButton.curSprPic != IngameButtonPicType.NORMAL)
            {
                curIngameButton.ChangePicToNormalIfNotActivated();
            }
        } 
        #endregion

        #region Hideout_Out Button

        if (curHideout_OutButton != null)
        {
           if (touchInf_Screen_Button != null && touchInf_Screen_Button.status == TouchStatus.Tapped)
            {


                //touchInfo_IngameButton = MyGUIController.Instance.CheckAndReserveTouchForRect(screenRect, TouchInRectMode.OnlyStart, buttonOwnerName);

                //touchInfo_Hideout_OutButton = MyGUIController.Instance.CheckAndReserveTouchForRect(screenRect, TouchInRectMode.StartAndEnd, buttonOwnerName);

                //if (touchInfo_Hideout_OutButton != null)
                //{
                    ////if (touchInfo_Hideout_OutButton.status == TouchStatus.FreezeTouching || touchInfo_Hideout_OutButton.status == TouchStatus.Moving)
                    ////{
                    ////    curHideout_OutButton.sprite = curRelatedHideout.sprite_GetOutBtn_Pressed;
                    ////}
                    ////else
                    ////{
                    ////    curHideout_OutButton.sprite = curRelatedHideout.sprite_GetOutBtn;
                    ////}


                    //if (touchInfo_Hideout_OutButton.status == TouchStatus.Tapped)
                    //{
                        curRelatedHideout.GetOut();
                    //}
                //}
            }
        }
        #endregion
    }

    public JoypadArrow GetJoypadActiveArrow()
    {
        if (joypadAxis.magnitude < minNeedAccForDescreteArrows)
            return JoypadArrow.none;

        float angle = Mathf.Atan(joypadAxis.y / joypadAxis.x);

        if (joypadAxis.x >= 0)
        {
            if (angle == float.NaN)
                return JoypadArrow.right;

            if ((angle >= -(Mathf.PI / 2) * arrowsToghserCoef) && (angle <= (Mathf.PI / 2) * arrowsToghserCoef))
            {
                return JoypadArrow.right;
            }

            if ((angle > (Mathf.PI / 2) * arrowsToghserCoef) && (angle < (Mathf.PI / 2) * (1 - arrowsToghserCoef)))
            {
                return JoypadArrow.rightUp;
            }

            if ((angle < -(Mathf.PI / 2) * arrowsToghserCoef) && (angle > -(Mathf.PI / 2) * (1 - arrowsToghserCoef)))
            {
                return JoypadArrow.rightDown;
            }

            if ((angle >= (Mathf.PI / 2) * (1 - arrowsToghserCoef)))
            {
                return JoypadArrow.up;
            }

            if ((angle <= -(Mathf.PI / 2) * (1 - arrowsToghserCoef)))
            {
                return JoypadArrow.down;
            }
        }
        else
        {
            angle = -angle;

            if (angle == float.NaN)
                return JoypadArrow.left;

            if ((angle >= -(Mathf.PI / 2) * arrowsToghserCoef) && (angle <= (Mathf.PI / 2) * arrowsToghserCoef))
            {
                return JoypadArrow.left;
            }

            if ((angle > (Mathf.PI / 2) * arrowsToghserCoef) && (angle < (Mathf.PI / 2) * (1 - arrowsToghserCoef)))
            {
                return JoypadArrow.leftUp;
            }

            if ((angle < -(Mathf.PI / 2) * arrowsToghserCoef) && (angle > -(Mathf.PI / 2) * (1 - arrowsToghserCoef)))
            {
                return JoypadArrow.leftDown;
            }

            if ((angle >= (Mathf.PI / 2) * (1 - arrowsToghserCoef)))
            {
                return JoypadArrow.up;
            }

            if ((angle <= -(Mathf.PI / 2) * (1 - arrowsToghserCoef)))
            {
                return JoypadArrow.down;
            }
        }

        return JoypadArrow.none;
    }

    //public TouchInfo CheckButtonTouch(GUIControl _guiCtrl, TouchInRectMode _rectTouchMode)
    //{
    //    TouchInfo touchInf = null;

    //    GUIControl guiCtrl = _guiCtrl;
    //    TouchInRectMode rectTouchMode = _rectTouchMode;

    //    if (!guiCtrl.visibility_Total)
    //        return touchInf;

    //    if (!guiCtrl.isEnabled)
    //        return touchInf;

    //    if (!guiCtrl.isTouchable)
    //        return touchInf;

    //    Rect rect = guiCtrl.GetFinalRect();
    //    touchInf = MyGUIController.Instance.CheckTouchForRect(rect, rectTouchMode);

    //    if (touchInf != null)
    //    {
    //        if (!guiCtrl.isPressed)
    //        {
    //            guiCtrl.isPressed = true;
    //            guiCtrl.ChangeStatus();
    //        }
    //    }
    //    else
    //    {
    //        if (guiCtrl.isPressed)
    //        {
    //            guiCtrl.isPressed = false;
    //            guiCtrl.ChangeStatus();
    //        }
    //    }

    //    return touchInf;
    //}

    public void SetJoystickEnablility(bool _isEnabled)
    {
        bool oldVal = IsJoystickEnabled;

        IsJoystickEnabled = _isEnabled;

        if (oldVal != IsJoystickEnabled)
            if (null != Event_TapButtonsOrJoystickEnabilityChanged)
                Event_TapButtonsOrJoystickEnabilityChanged();
    }

    public void SetTapButtonsEnablility(bool _isEnabled)
    {
        bool oldVal = AreTapButtonsEnabled;

        AreTapButtonsEnabled = _isEnabled;

        if (oldVal != AreTapButtonsEnabled)
            if (null != Event_TapButtonsOrJoystickEnabilityChanged)
                Event_TapButtonsOrJoystickEnabilityChanged();
    }

    public void SetInGameButtonsSikiminEnabled(bool _areEnabled)
    {
        bool oldVal = areInGameButtonsSikiminEnabled;

        areInGameButtonsSikiminEnabled = _areEnabled;

        if (oldVal != areInGameButtonsSikiminEnabled)
            if (null != Event_TapButtonsOrJoystickEnabilityChanged)
                Event_TapButtonsOrJoystickEnabilityChanged();
    }

    public bool AreTapButtonsSikiminallyEnabled()
    {
        return areInGameButtonsSikiminEnabled && AreTapButtonsEnabled;
    }

    public bool IsJoystickSikiminallyEnabled()
    {
        return areInGameButtonsSikiminEnabled && IsJoystickEnabled;
    }


}
