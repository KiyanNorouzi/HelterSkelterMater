using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuModeTouchEventController : MonoBehaviour
{
    public static MenuModeTouchEventController Instance;

    bool tapHappened = false;
    bool fingerOnCtrlUpHappened = false;

    GUIGroup lastTouch_GuiGroup = null;
    GUIControl lastTouch_GuiControl = null;
    TouchUpInfo lastTouch_TouchUpInfo;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (tapHappened)
        {
            if (lastTouch_GuiGroup.myName == "TestGroup")
            {
                if (lastTouch_GuiControl.myName == "TestControl")
                {

                }
            }
        }

        SetTouchesChecked();
    }

    public void TapHappened(GUIGroup _guiGroup, GUIControl _guiControl, TouchUpInfo _touchUpInfo)
    {
        GUIGroup guiGroup = _guiGroup;
        GUIControl guiControl = _guiControl;
        TouchUpInfo touchUpInfo = _touchUpInfo;

        lastTouch_GuiGroup = guiGroup;
        lastTouch_GuiControl = guiControl;
        lastTouch_TouchUpInfo = touchUpInfo;

        string groupName = guiGroup.myName;
        string controlName = guiControl.myName;

        tapHappened = true;
    }

    public void TouchUpHappened(GUIGroup _guiGroup, GUIControl _guiControl, TouchUpInfo _touchUpInfo)
    {
        GUIGroup guiGroup = _guiGroup;
        GUIControl guiControl = _guiControl;
        TouchUpInfo touchUpInfo = _touchUpInfo;

        lastTouch_GuiGroup = guiGroup;
        lastTouch_GuiControl = guiControl;
        lastTouch_TouchUpInfo = touchUpInfo;

        string groupName = guiGroup.myName;
        string controlName = guiControl.myName;

        fingerOnCtrlUpHappened = true;
    }

    void SetTouchesChecked()
    {
        tapHappened = false;
        fingerOnCtrlUpHappened = false;
    }
}

