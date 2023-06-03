using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SenseObjType
{
    Trap,
    NPC,
    Hideout,
}

public class SenseObj
{
    public GameObject obj;
    public SenseObjType myType;
}

public class PlayerSenseTrigger : MonoBehaviour
{
    public List<SenseObj> nearSenseObjs = new List<SenseObj>();

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "NPCSenseTrigger")
        {
            SenseObj senseObj = new SenseObj();

            senseObj.myType = SenseObjType.NPC;
            senseObj.obj = other.gameObject.GetComponent<ObjectSenseTrigger>().owner;

            AddSenseObjToNearList(senseObj);
        }

    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "NPCSenseTrigger")
        {
            GameObject owner = other.gameObject.GetComponent<ObjectSenseTrigger>().owner;

            for (int i = 0; i < nearSenseObjs.Count; i++)
            {
                if (nearSenseObjs[i] != null && nearSenseObjs[i].obj == owner)
                    RemoveSenseObjFromNearList(nearSenseObjs[i]);
            }
        }
    }

    void AddSenseObjToNearList(SenseObj _senseObj)
    {
        SenseObj senseObj = _senseObj;

        if (!nearSenseObjs.Contains(senseObj))
        {
            nearSenseObjs.Add(senseObj);
        }

        RefreshList();
    }

    void RemoveSenseObjFromNearList(SenseObj _senseObj)
    {
        SenseObj senseObj = _senseObj;

        if (nearSenseObjs.Contains(senseObj))
        {
            nearSenseObjs.Remove(senseObj);
        }

        RefreshList();
    }

    void RefreshList()
    {
        for (int i = 0; i < nearSenseObjs.Count; i++)
        {
            if (nearSenseObjs[i] == null)
            {
                nearSenseObjs.RemoveAt(i);
                i--;
            }
        }

        GameObject go = ChooseBestNearGameObj();

        if (go == null)
        {
            if (MultiTouchManager.Instance.curIngameButton != null)
                MultiTouchManager.Instance.curIngameButton.SetButtonEnabled(false);

            MultiTouchManager.Instance.curIngameButton = null;
        }
        else
        {
            IngameButton igb = go.GetComponentInChildren<IngameButton>();

            if (MultiTouchManager.Instance.curIngameButton != null)
            {
                if (igb != MultiTouchManager.Instance.curIngameButton)
                {
                    MultiTouchManager.Instance.curIngameButton.SetButtonEnabled(false);

                    MultiTouchManager.Instance.curIngameButton = go.GetComponentInChildren<IngameButton>();
                    MultiTouchManager.Instance.curIngameButton.SetButtonEnabled(true);
                }
            }
            else
            {
                MultiTouchManager.Instance.curIngameButton = go.GetComponentInChildren<IngameButton>();
                MultiTouchManager.Instance.curIngameButton.SetButtonEnabled(true);
            }
        }
    }

    GameObject ChooseBestNearGameObj()
    {
        if (nearSenseObjs.Count == 0)
            return null;

        return nearSenseObjs[0].obj;
    }
}
