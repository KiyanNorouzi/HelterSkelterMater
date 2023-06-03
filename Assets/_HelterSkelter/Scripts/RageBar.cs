using UnityEngine;
using System.Collections;

public class RageBar : MonoBehaviour
{
    public float maxRage = 100;
    public float rageDecreasementSpeed = 5f;
    public float rageModeDuration = 10f;
    public bool autoStartRageMode = true;

    public delegate void RageModeDelegate();
    public RageModeDelegate Event_RageModeStarted;
    public RageModeDelegate Event_RageModeStopped;

    private float curRage = 0;
    public float CurRage
    {
        get { return curRage; }
    }

    private bool isInRageMode = false;
    public bool IsInRageMode
    {
        get { return isInRageMode; }
    }

    protected float rageModeTimeCounter;

    public virtual void AwakeMe()
    {

    }

    public virtual void StartMe()
    {
        ChangeRage(0);
    }

    public virtual void UpdateMe()
    {
        if (!isInRageMode)
        {
            if (curRage > 0)
                ChangeRage(MathFPlus.DecDeltaTimeToZero(curRage, rageDecreasementSpeed));
        }
        else
        {
            rageModeTimeCounter = MathFPlus.DecDeltaTimeToZero(rageModeTimeCounter);

            if (curRage > 0)
                ChangeRage((rageModeTimeCounter / rageModeDuration)*maxRage);

            if (rageModeTimeCounter == 0)
                StopRageMode();
        }
    }

    public void AddRage(float _val)
    {
        ChangeRage(curRage + _val);
    }

    public virtual void ChangeRage(float _val)
    {
        curRage = Mathf.Clamp(_val, 0, maxRage);

        if (autoStartRageMode)
            CheckStartRageMode();
    }

    public void CheckStartRageMode()
    {
        if (isInRageMode)
            return;

        if (curRage >= maxRage)
        {
            StartRageMode();
        }
    }

    public virtual void StartRageMode()
    {
        isInRageMode = true;
        rageModeTimeCounter = rageModeDuration;
        if (null != Event_RageModeStarted) Event_RageModeStarted();
    }

    public virtual void StopRageMode()
    {
        isInRageMode = false;
        if (null != Event_RageModeStopped) Event_RageModeStopped();
    }
}
