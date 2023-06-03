using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum ObjectsInfo
{
    NPC,
    TRAP,
    HIDEOUT
}

public class ObjectTrigger : MonoBehaviour 
{

    public GameObject owner;

    public ObjectsInfo objectType = ObjectsInfo.NPC;

    public IngameButton ingameButton;

}
