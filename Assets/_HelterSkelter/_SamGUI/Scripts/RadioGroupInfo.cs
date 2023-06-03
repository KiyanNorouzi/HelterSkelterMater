using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RadioGroupInfo : MonoBehaviour
{

    [HideInInspector]
    public List<GUIControl> curControls = new List<GUIControl>();
    GUIControl curActiveControl = null;

    public void AddControl(GUIControl _ctrl)
    {
        GUIControl ctrl = _ctrl;

        if (!curControls.Contains(ctrl))
            curControls.Add(ctrl);

        if (ctrl.isActivated)
            ReSetControlsActivationStatus(ctrl);
    }

    public void RemoveControl(GUIControl _ctrl)
    {
        GUIControl ctrl = _ctrl;

        if (curControls.Contains(ctrl))
            curControls.Remove(ctrl);

        if (curActiveControl == ctrl)
            ReSetControlsActivationStatus(null);
    }

    public void ReSetControlsActivationStatus(GUIControl _curActiveControl)
    {
        curActiveControl = _curActiveControl;

        if (curActiveControl != null)
        {
            curActiveControl.isActivated = true;
            curActiveControl.ChangeStatus();
        }

        for (int i = 0; i < curControls.Count; i++)
        {
            if ((curActiveControl == null) || ((curActiveControl != null) && (curControls[i] != curActiveControl)))
            {
                curControls[i].isActivated = false;
                curControls[i].ChangeStatus();
            }
        }
    }
}
