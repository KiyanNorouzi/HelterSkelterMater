using UnityEngine;
using System.Collections;

public enum SliderType
{
    Horiz,Vert,
}

public class SliderInfo : MonoBehaviour {
    public GUIControl handle;
    public GUIControl curValControl;
    public float curValConrtolOffsetOnHold = 0;
    public bool curValConrtolOffsetAxisIsX = true;
    public SliderType sliderType = SliderType.Horiz;
    public float initialVal = 0;
    public float edge = 0f;
    public float minVal = 0;
    public float maxVal = 100;
    public int parts = 0;

}
