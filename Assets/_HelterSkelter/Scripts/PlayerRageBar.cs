using UnityEngine;
using System.Collections;

public class PlayerRageBar : RageBar
{
    public static PlayerRageBar Instance;

    public float ragePerKill = 20;
    public float rageModeMovementSpeedCoef = 1.3f;


    float timeToStopCamAnimBeforeRageFinish = .8f;

    bool isCamEffectsStartedToFinish = false;

    void Awake()
    {
        AwakeMe();
    }

    void Start()
    {
        StartMe();
    }

    void Update()
    {
        UpdateMe();
    }

    public override void AwakeMe()
    {
        base.AwakeMe();

        Instance = this;
    }

    public override void ChangeRage(float _val)
    {
        base.ChangeRage(_val);

        ChangeUI();
    }

    void ChangeUI()
    {
        RageBarUI.Instance.SetCurRage(CurRage);
    }

    public void AddRageOfKill()
    {
        AddRage(ragePerKill);
    }

    public override void StartRageMode()
    {
        if (MapManager.Instance.IsPlayerDetected())
            return;

        base.StartRageMode();

        RageModeCamera.Instance.StartRageMode();
        isCamEffectsStartedToFinish = false;

        Player.Instance.SetRageModeMoveSpeedCoef(rageModeMovementSpeedCoef);
    }

    public override void UpdateMe()
    {
        base.UpdateMe();

        if (IsInRageMode)
        {
            if (!isCamEffectsStartedToFinish && (rageModeTimeCounter < timeToStopCamAnimBeforeRageFinish))
            {
                RageModeCamera.Instance.ShouldStopRageMode();
                isCamEffectsStartedToFinish = true;
            }
        }
    }

    public override void StopRageMode()
    {
        base.StopRageMode();
        isCamEffectsStartedToFinish = false;
        Player.Instance.SetRageModeMoveSpeedCoef(1);
    }
}
