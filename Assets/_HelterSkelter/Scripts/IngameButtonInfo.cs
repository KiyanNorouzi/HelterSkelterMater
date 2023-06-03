using UnityEngine;
using System.Collections;

public enum IngameButtonType
{
    Kill,
    Trap,
    Hideout,
}

public enum IngameButtonPicType
{
    NORMAL,
    PRESSED,
    ACTIVATE,
}

public class IngameButtonInfo : MonoBehaviour {

    public IngameButtonType buttonType = IngameButtonType.Kill;
    public Sprite sprite_Normal;
    public Sprite sprite_Pressed;
    public Sprite sprite_Activate;
}
