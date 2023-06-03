using UnityEngine;
using System.Collections;

public enum WeaponStatus
{
    Idle_Start,
    Idle_Update,
    Firing_Start,
    Firing_Update,
}

public interface IWeapon
{
    bool IsActive();
    void SetActive(bool _val);
    bool IsReadyToFire();
    bool TryFire();
    void StopFire();
}

public class Weapon : PixelObj, IWeapon, IMonoBased
{
    public Character owner;

    public float betweenFireTime = 0;

    protected bool isWeaponActive = false;

    protected float betweenFireTimeCounter = 0;

    protected WeaponStatus status = WeaponStatus.Idle_Start;

    [HideInInspector]
    public string animName_Fire = "Fire";
    [HideInInspector]
    public float fireAnimSpeed = 1;

    public virtual void StartMe() 
    {
        SetActive(false);
    }

    public virtual void AwakeMe() { }

    public virtual void UpdateMe()
    { 
        if(IsActive())
        {
            SetSide(owner.side);
        }
    } //<<Testtttttt

    public bool IsActive()
    {
        return isWeaponActive;
    }

    public virtual void SetActive(bool _val)
    {
        isWeaponActive = _val;

        SetVisible(isWeaponActive);
    }

    public virtual bool IsReadyToFire()
    {
        if (!isWeaponActive)
        {
            Debug.LogError("Weapon '" + this.name + "' of '" + owner.name + "' is trying to fire before getting actived.");
            return false;
        }

        if (!IsStatus(WeaponStatus.Idle_Update))
            return false;

        return betweenFireTimeCounter == 0;
    }

    public virtual bool TryFire()
    {
        if (!IsReadyToFire())
            return false;

        SetStatus(WeaponStatus.Firing_Start);
        return true;
    }

    public virtual void StopFire()
    {

    }


    protected void SetStatus(WeaponStatus _status)
    {
        status = _status;
    }

    protected bool IsStatus(WeaponStatus _status)
    {
        return status == _status;
    }
}
