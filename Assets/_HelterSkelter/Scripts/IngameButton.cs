using UnityEngine;
using System.Collections;

public enum IngameButtonState
{
    NORMAL_START,
    NORMAL_UPDATE,
    ACTIVATE_START,
    ACTIVATE_UPDATE,
}

public class IngameButton : MonoBehaviour
{
    public GameObject owner;
    public SpriteRenderer sprRend;
    public IngameButtonInfo btnInfo;

    [HideInInspector]
    public bool isButtonEnabled = false;


    IngameButtonState state = IngameButtonState.NORMAL_START;

    float spriteFadeSpeed = 5f;

    float activatePicTimeCounter = 0;
    float activatePicMaxTime = 0.2f;

    [HideInInspector]
    public IngameButtonPicType curSprPic = IngameButtonPicType.NORMAL;

    void Start()
    {

    }

    void Update()
    {
        //print(Camera.main.WorldToScreenPoint(go.transform.position));

        if (isButtonEnabled)
        {
            #region NORMAL_START
            if (state == IngameButtonState.NORMAL_START)
            {
                state = IngameButtonState.NORMAL_UPDATE;
            }
            #endregion

            #region NORMAL_UPDATE
            if (state == IngameButtonState.NORMAL_UPDATE)
            {

            }
            #endregion

            #region ACTIVATE_START
            if (state == IngameButtonState.ACTIVATE_START)
            {
                SetSpritePic(IngameButtonPicType.ACTIVATE);

                activatePicTimeCounter = activatePicMaxTime;


                state = IngameButtonState.ACTIVATE_UPDATE;
            }
            #endregion

            #region ACTIVATE_UPDATE
            if (state == IngameButtonState.ACTIVATE_UPDATE)
            {
                activatePicTimeCounter = MathFPlus.DecDeltaTimeToZero(activatePicTimeCounter);

                if (activatePicTimeCounter == 0)
                {
                    sprRend.color = new Color(sprRend.color.r, sprRend.color.g, sprRend.color.b, Mathf.Clamp01(sprRend.color.a - spriteFadeSpeed * Time.deltaTime));

                    if (sprRend.color.a == 0)
                    {
                        SetButtonEnabled(false);
                        state = IngameButtonState.NORMAL_START;
                    }
                }
            }
            #endregion
        }
    }

    public void SetButtonEnabled(bool _val)
    {
        isButtonEnabled = _val;

        sprRend.gameObject.SetActive(isButtonEnabled);

        if (isButtonEnabled)
        {
            SetAlpha(1);
            state = IngameButtonState.NORMAL_START;
        }
        else
        {
            SetSpritePic(IngameButtonPicType.NORMAL);
            SetAlpha(0);
        }
    }

    public void Activate()
    {
        if (state == IngameButtonState.NORMAL_UPDATE)
        {
            state = IngameButtonState.ACTIVATE_START;

            switch (btnInfo.buttonType)
            {
                case IngameButtonType.Kill:
                    Player.Instance.SetSelectedVictim(owner.GetComponent<NPC>());
                    Player.Instance.SetKillButtonPressed(true);
                    break;

                case IngameButtonType.Trap:
                    Player.Instance.SetSelectedTrap(owner.GetComponent<Trap>());
                    Player.Instance.SetTrapButtonPressed(true);
                    break;

                case IngameButtonType.Hideout:
                    Player.Instance.SetSelectedHideout(owner.GetComponent<Hideout>());
                    Player.Instance.SetHideoutButtonPressed(true);
                    break;
            }
        }
    }

    public void SetSpritePic(IngameButtonPicType _picType)
    {
        curSprPic = _picType;

        switch (_picType)
        {
            case IngameButtonPicType.NORMAL:
                sprRend.sprite = btnInfo.sprite_Normal;
                break;

            case IngameButtonPicType.PRESSED:
                sprRend.sprite = btnInfo.sprite_Pressed;
                break;

            case IngameButtonPicType.ACTIVATE:
                sprRend.sprite = btnInfo.sprite_Activate;
                break;
        }
    }

    public Rect GetRect()
    {
        return MyGUIController.Instance.GetRectOfBounds(sprRend.bounds);
    }

    public void ChangePicToNormalIfNotActivated()
    {
        if (state == IngameButtonState.NORMAL_UPDATE)
        {
            SetSpritePic(IngameButtonPicType.NORMAL);
        }
    }

    public void SetAlpha(float _val)
    {
        if (sprRend.color.a == _val)
            return;

        sprRend.color = new Color(sprRend.color.r, sprRend.color.g, sprRend.color.b, _val);
    }
}
