using UnityEngine;
using System.Collections;

public class Hideout : PixelObj 
{
    public ObjectTrigger myObjTrigger;
    public Sprite sprite_GetOutBtn;
    public Sprite sprite_GetOutBtn_Pressed;
    public SpriteRenderer outButton;
    public bool shouldForceSide = false;
    public Side forcedSide = Side.RIGHT;
    public SoundType hideoutSound;
    [HideInInspector]
    public bool isHidden = false;

    [HideInInspector]
    public Collider2D hidingArea;

    float timeCounter = 0;
    float delayTimeToShowGetOutButton = 0.5f;

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

        if (isHidden)
        {
            if (!outButton.gameObject.activeSelf)
            {
                timeCounter = MathFPlus.DecDeltaTimeToZero(timeCounter);

                if (timeCounter == 0)
                {
                    outButton.gameObject.SetActive(true);

                    MultiTouchManager.Instance.curHideout_OutButton = outButton;
                    MultiTouchManager.Instance.curRelatedHideout = this;
                }
            }
        }
    }

    public void StartHide()
    {
        isHidden = true;

        timeCounter = delayTimeToShowGetOutButton;

        AudioManager.Instance.Play(hideoutSound, false);
    }

    public void GetOut()
    {
        isHidden = false;
        outButton.sprite = sprite_GetOutBtn;
        outButton.gameObject.SetActive(false);
        Player.Instance.SetGetOutButtonPressed(true);

        MultiTouchManager.Instance.curHideout_OutButton = null;
        MultiTouchManager.Instance.curRelatedHideout = null;

        AudioManager.Instance.Play(hideoutSound, false);
    }
}
