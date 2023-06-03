using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeleeVictimInfo
{
    public Character victim;
    public float timeCounter;
}


public class MeleeWeapon : Weapon
{
    public bool isPassive = false;
    public float damageVal = 100f;
    public MeleeWeaponEffector meleeEffector;

    //


    //

    int victimIndex = 0;
    List<MeleeVictimInfo> curAffectedVictims = new List<MeleeVictimInfo>();


    void Awake()
    {
        PixObj_Awake();
        AwakeMe();
    }

    void Start()
    {
        StartMe();
    }

    void Update()
    {
        PixObj_Update();
        UpdateMe();
    }

    public override void UpdateMe()
    {
        base.UpdateMe();

        CheckVictims();

        #region States

        #region Idle_Start
        if (IsStatus(WeaponStatus.Idle_Start))
        {
            SetStatus(WeaponStatus.Idle_Update);
        }
        #endregion

        #region Idle_Update
        if (IsStatus(WeaponStatus.Idle_Update))
        {

        }
        #endregion

        #region Firing_Start
        if (IsStatus(WeaponStatus.Firing_Start))
        {
            PlayAnimForCurSide(animName_Fire, fireAnimSpeed);

            if (isPassive)
            {
                meleeEffector.isEffective = true;
            }
            else
            {
                //???
            }

            SetStatus(WeaponStatus.Firing_Update);
        }
        #endregion

        #region Firing_Update
        if (IsStatus(WeaponStatus.Firing_Update))
        {
            PlayAnimForCurSide(animName_Fire, fireAnimSpeed);

            if(!isPassive)
            {
                //???
            }
        }
        #endregion 

        #endregion
    }

    public bool TryAddNewVictim(Character _newVictim)
    {
        for (int i = 0; i < curAffectedVictims.Count; i++)
        {
            if (curAffectedVictims[i] != null && curAffectedVictims[i].victim != null && curAffectedVictims[i].victim == _newVictim)
                return false;
        }

        MeleeVictimInfo vic = new MeleeVictimInfo();

        vic.victim = _newVictim;
        vic.timeCounter = betweenFireTime;

        curAffectedVictims.Add(vic);

        DamageInfo damageInfo = new DamageInfo(owner.gameObject, vic.victim.entityType, DamageType.Melee, damageVal); 

        vic.victim.SendMessage("ApplyDamage", damageInfo);

        return true;
    }

    void CheckVictims()
    {
        victimIndex = 0;

        while (victimIndex < curAffectedVictims.Count)
        {
            if (curAffectedVictims[victimIndex] == null)
            {
                curAffectedVictims.RemoveAt(victimIndex);
                continue;
            }

            if (curAffectedVictims[victimIndex].victim == null)
            {
                curAffectedVictims.RemoveAt(victimIndex);
                continue;
            }

            curAffectedVictims[victimIndex].timeCounter = MathFPlus.DecDeltaTimeToZero(curAffectedVictims[victimIndex].timeCounter);

            if (curAffectedVictims[victimIndex].timeCounter == 0)
            {
                curAffectedVictims.RemoveAt(victimIndex);
                continue;
            }

            victimIndex++;
        }
    }

    public override void StopFire()
    {
        base.StopFire();

        anim2DController.Stop();
        meleeEffector.isEffective = false;

        if (isPassive)
        {
            SetStatus(WeaponStatus.Idle_Start);
        }
    }
}
