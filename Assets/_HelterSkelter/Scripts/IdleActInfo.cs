using UnityEngine;
using System.Collections;

public class IdleActInfo 
{
    public bool needToChangeSide = false;
    public Side neededSide = Side.RIGHT;
    public float animSpeed = 1;
    public string animName = "";
    public float minTime = 5;
    public float maxTime = 10;
    public float selectedTime = 7;
    public Vector3 pos;
}
