using UnityEngine;
using System.Collections;

public enum Side
{
    LEFT,
    RIGHT,
    UP,
    DOWN,
    LEFT_UP,
    RIGHT_UP,
    LEFT_DOWN,
    RIGHT_DOWN,
}


public enum EntityType
{
    Player,
    Trap,
    Police,
    Civil,
}

public enum DamageType
{
    Melee,
    Bullet,
    Explosion,
}

public class DamageInfo
{
    public GameObject damageSource;
    public EntityType sourceType;
    public DamageType damageType;
    public float damageVal;

    public DamageInfo()
    {

    }

    public DamageInfo(GameObject _damageSource, EntityType _sourceType, DamageType _damageType, float _damageVal)
    {
        damageSource = _damageSource;
        sourceType = _sourceType;
        damageType = _damageType;
        damageVal = _damageVal;
    }
}

public class Character : PixelObj
{
    public float maxHP = 100f;
    public Corpse sourceCorpseObj;
    public SoundType dieSound;
    public EntityType entityType;
    public Weapon currentWeapon;

    [HideInInspector]
    public string animName_Walk = "Walk";

    [HideInInspector]
    public string animName_Idle = "Idle";

    [HideInInspector]
    public bool isKilled = false;

    [HideInInspector]
    public float killDelayTimeCounter = 0;

    [HideInInspector]
    public bool isDelayedKillingStarted = false;

    [HideInInspector]
    public GameObject myKiller = null;

    [HideInInspector]
    public EntityType myKillerEntityType;

    [HideInInspector]
    public float curHP;

    // Use this for initialization
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void PixObj_Awake()
    {
        base.PixObj_Awake();

        curHP = maxHP;
    }

    public override void PixObj_Update()
    {
        base.PixObj_Update();

        if (isDelayedKillingStarted)
        {
            killDelayTimeCounter = MathFPlus.DecDeltaTimeToZero(killDelayTimeCounter);

            if (killDelayTimeCounter == 0)
            {
                KillMe(myKiller, myKillerEntityType);
            }
        }
    }

    public virtual void KillMe(GameObject _Killer, EntityType _killerType)
    {
        if (isKilled)
            return;

        isKilled = true;

        myKiller = _Killer;
        myKillerEntityType = _killerType;

        Corpse corpse = Instantiate(sourceCorpseObj, transform.position, transform.rotation) as Corpse;
        corpse.Init(dieSound);


        corpse.SetSide(side);

        Destroy(this.gameObject);
    }

    public virtual void KillMe(GameObject _Killer, EntityType _killerType, float _killDelay)
    {
        isDelayedKillingStarted = true;
        myKiller = _Killer;
        myKillerEntityType = _killerType;
        killDelayTimeCounter = _killDelay;
    }

    public void ApplyDamage(DamageInfo _damageInfo)
    {
        SetCurHP(curHP - _damageInfo.damageVal);

        if(curHP == 0)
        {
            KillMe(_damageInfo.damageSource, _damageInfo.sourceType);
        }
    }

    public void SetCurHP(float _val)
    {
        curHP = Mathf.Clamp(_val, 0, maxHP);
    }
}
