using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class TextPack
{
    public string text;
    public GUIControl relatedGUIControl;
    public bool isEmptyTextAcceptable = true;
    public bool considerUnavailableTexts = false;
    public List<string> unavailableTexts;

    public void Init(GUIControl _relatedGUIControl)
    {
        relatedGUIControl = _relatedGUIControl;
        SetText(relatedGUIControl.text);
    }
    public void InitFilter_EmptyText(bool _isEmptyTextAcceptable)
    {
        isEmptyTextAcceptable = _isEmptyTextAcceptable;
    }

    public void InitFilter_UnavailableTexts(List<string> _unavailableTexts)
    {
        unavailableTexts = _unavailableTexts;
        considerUnavailableTexts = true;

        if (unavailableTexts == null || (unavailableTexts != null && unavailableTexts.Count == 0))
        {
            considerUnavailableTexts = false;
        }
    }

    public void SetText(string _txt)
    {
        text = St_ExtraTextFuncs.LimitTextToMax(_txt);
    }

    public void ApplyResult()
    {
        relatedGUIControl.ChangeText(text);
    }
}
