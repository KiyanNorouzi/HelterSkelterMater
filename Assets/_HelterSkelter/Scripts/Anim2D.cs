using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AnimType
{
    OneTime,
    Loop,
    Backward,
    Backward_Loop,
    PingPong,
    PingPong_Loop,
}

public class Anim2D : MonoBehaviour {

    public string animName;
    public AnimType animType = AnimType.OneTime;
    public float frameTime = 0.1f;
    public List<Sprite> sprites = new List<Sprite>();

    [HideInInspector]
    public float TotalTime
    {
        get { return TotalNumOfFrames * frameTime; }
    }

    [HideInInspector]
    public int TotalNumOfFrames
    {
        get { return sprites.Count; }
    }
}
