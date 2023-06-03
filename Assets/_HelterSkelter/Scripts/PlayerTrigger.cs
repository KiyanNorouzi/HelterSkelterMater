using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerTrigger : MonoBehaviour
{
    public static PlayerTrigger Instance;

    List<ObjectTrigger> nearObjects = new List<ObjectTrigger>();

    void Awake()
    {
        Instance = this;
        MultiTouchManager.Instance.Event_TapButtonsOrJoystickEnabilityChanged += ButtonsEnabilityChanged;
    }

    void Update()
    {

    }

    void ButtonsEnabilityChanged()
    {
        RefreshCurIngameButton();

        //if (!MultiTouchManager.Instance.AreTapButtonsSikiminallyEnabled())
        //    DisabledCurIngameButton();
        //else
        //    RefreshCurIngameButton();
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "NPCSenseTrigger" || col.gameObject.tag == "TrapSenseTrigger" || col.gameObject.tag == "HideoutSenseTrigger")
        {
            AddToList(col.gameObject.GetComponent<ObjectTrigger>());
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "NPCSenseTrigger" || col.gameObject.tag == "TrapSenseTrigger" || col.gameObject.tag == "HideoutSenseTrigger")
        {
            RemoveFromList(col.gameObject.GetComponent<ObjectTrigger>());
        }
    }


    public void AddToList(ObjectTrigger _nearObjects)
    {
        if (!nearObjects.Contains(_nearObjects))
        {
            nearObjects.Add(_nearObjects);
            RefreshCurIngameButton();
        }
    }


    void RemoveFromList(ObjectTrigger _nearObjects)
    {
        if (nearObjects.Contains(_nearObjects))
        {
            //            print("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");

            nearObjects.Remove(_nearObjects);
            RefreshCurIngameButton();
        }
    }


    void RefreshCurIngameButton()
    {
        if (!MultiTouchManager.Instance.AreTapButtonsSikiminallyEnabled())
            return;

        ObjectTrigger bestNearObjTrig = ChooseBestNearGameObj();

        if (bestNearObjTrig == null)
        {
            DisabledCurIngameButton();
        }
        else
        {
            GameObject go = bestNearObjTrig.owner;

            IngameButton igb = bestNearObjTrig.ingameButton;

            if (MultiTouchManager.Instance.curIngameButton != null)
            {
                if (igb != MultiTouchManager.Instance.curIngameButton)
                {
                    MultiTouchManager.Instance.curIngameButton.SetButtonEnabled(false);

                    MultiTouchManager.Instance.curIngameButton = igb;
                    MultiTouchManager.Instance.curIngameButton.SetButtonEnabled(true);
                }
            }
            else
            {
                MultiTouchManager.Instance.curIngameButton = igb;
                MultiTouchManager.Instance.curIngameButton.SetButtonEnabled(true);
            }
        }
    }

    void DisabledCurIngameButton()
    {
        if (MultiTouchManager.Instance.curIngameButton != null)
            MultiTouchManager.Instance.curIngameButton.SetButtonEnabled(false);

        MultiTouchManager.Instance.curIngameButton = null;
    }

    ObjectTrigger ChooseBestNearGameObj()
    {
        if (nearObjects.Count == 0)
            return null;

        for (int i = 0; i < nearObjects.Count; i++)
        {
            if (nearObjects[i].objectType == ObjectsInfo.HIDEOUT)
            {
                return nearObjects[i];
            }
        }

        for (int i = 0; i < nearObjects.Count; i++)
        {
            if (nearObjects[i].objectType == ObjectsInfo.TRAP)
            {
                return nearObjects[i];
            }
        }

        float minDist = float.MaxValue;
        int index = -1;

        for (int i = 0; i < nearObjects.Count; i++)
        {
            if (nearObjects[i].objectType == ObjectsInfo.NPC)
            {
                float dist = Vector3.Distance(nearObjects[i].owner.transform.position, Player.Instance.transform.position);

                if (dist < minDist)
                {
                    minDist = dist;
                    index = i;
                }
            }
        }

        if (index >= 0)
        {
            return nearObjects[index];
        }

        return nearObjects[0];
    }

    public void RemoveNPCNearObjIfExistsInNearList(NPC _npc)
    {
        NPC npc = _npc;

        for (int i = 0; i < nearObjects.Count; i++)
        {
            if (nearObjects[i].objectType == ObjectsInfo.NPC)
            {
                if (nearObjects[i].owner == npc.gameObject)
                {
                    RemoveFromList(nearObjects[i]);
                }
            }
        }
    }

    /// <summary>
    /// Remove every traps that use one times
    /// </summary>
    /// <param name="_trap"></param>
    public void RemoveTrapObjIfExistsInList(Trap _trap)
    {
        Trap trap = _trap;

        for (int i = 0; i < nearObjects.Count; i++)
        {
            if (nearObjects[i].objectType == ObjectsInfo.TRAP)
            {
                if (nearObjects[i].owner == trap.gameObject)
                {
                    RemoveFromList(nearObjects[i]);
                }
            }
        }

    }

    public void RemoveHideoutNearObjIfExistsInNearList(Hideout _hideout)
    {
        Hideout hideout = _hideout;

        for (int i = 0; i < nearObjects.Count; i++)
        {
            if (nearObjects[i].objectType == ObjectsInfo.HIDEOUT)
            {
                if (nearObjects[i].owner == hideout.gameObject)
                {
                    RemoveFromList(nearObjects[i]);
                }
            }
        }
    }
}
