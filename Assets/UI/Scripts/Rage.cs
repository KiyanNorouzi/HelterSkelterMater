using UnityEngine;
using System.Collections;

public class Rage : MonoBehaviour
{
    [HideInInspector]
    public string name;
    [HideInInspector]
    public float castTime;
    [HideInInspector]
    public Color32 rageColor;


    public Rage(string name, float castTime, Color32 rageColor)
    {

        this.name = name;
        this.castTime = castTime;
        this.rageColor = rageColor;
    }


}
