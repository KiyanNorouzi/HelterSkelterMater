using UnityEngine;
using System.Collections;




public class InputController : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {

    }

    public TouchInfo CheckButtonTouch(GUIControl _guiCtrl, TouchInRectMode _rectTouchMode)
    {
        TouchInfo touchInf = null;

        GUIControl guiCtrl = _guiCtrl;
        TouchInRectMode rectTouchMode = _rectTouchMode;

        if (!guiCtrl.visibility_Total)
            return touchInf;

        if (!guiCtrl.isEnabled)
            return touchInf;

        if (!guiCtrl.isTouchable)
            return touchInf;

        Rect rect = guiCtrl.GetFinalRect();
        //touchInf = MyGUIController.Instance.CheckTouchForRect(rect, rectTouchMode); <<<<<<< needs update

        if (touchInf != null)
        {
            if (!guiCtrl.isPressed)
            {
                guiCtrl.isPressed = true;
                guiCtrl.ChangeStatus();
            }
        }
        else
        {
            if (guiCtrl.isPressed)
            {
                guiCtrl.isPressed = false;
                guiCtrl.ChangeStatus();
            }
        }

        return touchInf;
    }
}
