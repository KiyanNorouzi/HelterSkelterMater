using UnityEngine;
using System.Collections;

public static class St_ExtraTextFuncs
{
    public static int GUIControl_Text_MaxCap = 100;

    public static bool IsTextNullOrEmptyOrWhiteSpace(string _text)
    {
        string text = _text;

        if (string.IsNullOrEmpty(text))
            return true;

        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] != ' ')
                return false;
        }

        return true;
    }

    public static string LimitTextToMax(string _text)
    {
        string text = _text;

        if (string.IsNullOrEmpty(text))
            return text;

        int maxInd = Mathf.Min(text.Length, GUIControl_Text_MaxCap);

        return text.Substring(0, maxInd);
    }
}
