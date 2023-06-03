using UnityEngine;
using System.Collections;

public enum TrapState
{
    WAIT,
    EXPLOSION_DELAY_START,
    EXPLOSION_DELAY_UPDATE,
    EXPLOSION_START,
    EXPLOSION_UPDATE,
    FINISHED,
}

public class Trap : PixelObj
{
    public GameObject owner;
    public TrapNPCCollisionArea killArea;
    public ObjectTrigger myObjTrigger;
    public float trapExplosionDelay = 0.5f;
    public Anim2D activeAnim;
    public SoundType activeSound = SoundType.NONE;
    public bool isActiveSoundLoop = false;
    public Anim2D explosionAnim;
    public SoundType explosionSound = SoundType.NONE;
    public ParticleSystem explosionParticle;
    public AnimationCurve scaleAnimCurve;
    public bool shouldFreezeNPC = false;
    public string freezeNPCAnimName = "";

    [HideInInspector]
    public TrapState state = TrapState.WAIT;
    float stateTimeCounter = 0;

    bool isActivated = false;
    float scaleTimeCounter = 0;

    void Awake()
    {
        PixObj_Awake();
    }

    void Update()
    {
        PixObj_Update();
    }


    public override void PixObj_Update()
    {
        base.PixObj_Update();

        if (isActivated)
        {
            //scaleTimeCounter += Time.deltaTime;
            //owner.transform.localScale = scaleAnimCurve.Evaluate(scaleTimeCounter) * Vector3.one;

            #region EXPLOSION_DELAY_START
            if (IsState(TrapState.EXPLOSION_DELAY_START))
            {
                stateTimeCounter = trapExplosionDelay;

                SetState(TrapState.EXPLOSION_DELAY_UPDATE);
            }
            #endregion

            #region EXPLOSION_DELAY_UPDATE
            if (IsState(TrapState.EXPLOSION_DELAY_UPDATE))
            {
                stateTimeCounter = MathFPlus.DecDeltaTimeToZero(stateTimeCounter);

                if (stateTimeCounter == 0)
                    SetState(TrapState.EXPLOSION_START);
            }
            #endregion

            #region EXPLOSION_START
            if (IsState(TrapState.EXPLOSION_START))
            {
                killArea.gameObject.SetActive(false);

                for (int i = 0; i < killArea.curInsideNPCs.Count; i++)
                {
                    if (killArea.curInsideNPCs[i] != null)
                        killArea.curInsideNPCs[i].KillMe(gameObject, EntityType.Trap);
                }

                if (explosionAnim != null)
                    anim2DController.PlayAnim(explosionAnim);

                AudioManager.Instance.Play(explosionSound, false);

                if (explosionParticle != null)
                {
                    explosionParticle.gameObject.SetActive(true);

                    explosionParticle.Stop();
                    explosionParticle.Play();
                }

                SetState(TrapState.EXPLOSION_UPDATE);
            }
            #endregion

            #region EXPLOSION_UPDATE
            if (IsState(TrapState.EXPLOSION_UPDATE))
            {
                SetState(TrapState.FINISHED);
            }
            #endregion
        }
    }

    public void TrapActivation()
    {
        if (isActivated)
            return;

        myObjTrigger.gameObject.SetActive(false);

        isActivated = true;

        killArea.gameObject.SetActive(true);

        if (activeAnim != null)
            anim2DController.PlayAnim(activeAnim);

        AudioManager.Instance.Play(activeSound, isActiveSoundLoop);

        PlayerTrigger.Instance.RemoveTrapObjIfExistsInList(this);
    }

    public void TrapKill(GameObject _killer, EntityType _killerType)
    {


    }

    public void SetState(TrapState _state)
    {
        state = _state;
    }

    public bool IsState(TrapState _state)
    {
        return state == _state;
    }
}
