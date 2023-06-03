using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AnimMode
{
    Hide,
    Show,
}

public enum ToggleType
{
    Normal,
    CheckBox,
    RadioBox,
}

public enum GeneralSpeed
{
    Fast,
    Normal,
    Slow,
}

public enum AcDeacAnimType
{
    None,
    Alpha,
}

public class TouchUpInfo
{
    public float time_Cur;
    public float time_Start;
    public float time_End;
    public Vector2 pos_Start;
    public Vector2 pos_End;
}

public enum ObjShowStatus
{
    FullShow,
    GoingToShow,
    FullHide,
    GoingToHide,
}

public enum GUIControlShowingAnimStatus
{
    Unused,
    GoingShow,
    Show,
    GoingHidden,
    Hidden,
}

public enum ControlType
{
    Button,
    Picture,
    TextEdit,
}

public enum GUIVertAllign
{
    Top,
    Mid,
    Bot
}

public enum GUIHorizAllign
{
    Left,
    Mid,
    Right
}

public enum GUIRectAllignX
{
    Left_Outside,
    Left_Between,
    Left_Inside,
    Mid,
    Right_Outside,
    Right_Between,
    Right_Inside,
}

public enum GUIRectAllignY
{
    Top_Outside,
    Top_Between,
    Top_Inside,
    Mid,
    Bot_Outside,
    Bot_Between,
    Bot_Inside,
}

public enum GUIControlCurStatus
{
    Normal,
    NormPressed,
    Active,
    ActivePressed,
    Disabled_Active,
    Disabled_Deactive,
}

public class GUIControl : MonoBehaviour
{
    public delegate void SliderHandler();
    public event SliderHandler Event_OnSliderHandChange;
    public event SliderHandler Event_OnSliderHandChangeAndFingerUp;

    public delegate void TapAndTouchUpEvent(GUIControl _control, TouchUpInfo _tapInfo);
    public event TapAndTouchUpEvent Event_Tap;
    public event TapAndTouchUpEvent Event_TouchUp;

    public const int PARENT_SPRITE_DEPTH_COEF = 1000;
    public const int LIST_ITEMS_START_DEPTH_INDEX = 11;
    public const int LIST_FG_START_DEPTH_INDEX = 190;

    public const int CONTROL_SPRITE_DEPTH_COEF = 5;
    public const int MAIN_SPRITE_DEPTH = 1;
    public const int ACTIVE_SPRITE_DEPTH = 2;
    public const int ON_PRESS_EXTRA_SPRITE_DEPTH = 3;
    public const int TEXT_SPRITE_DEPTH = 4;

    #region Variables

    public string myName;
    public Color mainColor = new Color(1, 1, 1, 1);
    public ColorInfo picsColorInfo;
    public ToggleType toggleType = ToggleType.Normal;
    public RadioGroupInfo radioBoxModeGroup = null;
    public SliderInfo sliderInfo;
    public GUIHorizAllign allign_Horiz = GUIHorizAllign.Mid;
    public GUIVertAllign allign_Vert = GUIVertAllign.Mid;
    public bool xIsCoef = false;
    public bool wIsCoef = false;
    //TEST
    public bool applyBigHeightSizeLimits_X = false;
    public bool applyBigHeightSizeLimits_Y = false;
    public bool applyBigHeightSizeLimits_W = false;
    public bool applyBigHeightSizeLimits_H = false;
    //~TEST
    public float x = 0;
    public float y = 0;
    public float w = 2.7f;
    public float h = 0.96f;
    public bool isTouchable = true;
    public bool isVisible = true;
    public bool animVis = true;
    public bool isEnabled = true;
    public bool isActivated = false;
    public AcDeacAnimType activationAnim = AcDeacAnimType.Alpha;
    public float activationAnimSpeed = 1;
    public float alpha = 1;

    public string _Textures______________________________________________ = "__________________________________________________________________";
    public Sprite sprite_Normal_Main;
    public Sprite sprite_Normal_Pressed;
    public Sprite sprite_Disabled_Active;
    public Sprite sprite_Disabled_Deactive;
    public Sprite sprite_Active_Main;
    public Sprite sprite_Active_Pressed;

    public string _Text__________________________________________________ = "__________________________________________________________________";
    public bool isTextPersian = false;
    public string text = "";
    public ColorInfo textColorInfo;
    public GUITextFontInfo textFontInfo;
    public TextAnchor textAllignment = TextAnchor.MiddleCenter;
    public float textMarginLeft = 0;
    public float textMarginRight = 0;
    public float textMarginTop = 0;
    public float textMarginBot = 0;

    public string _Extras________________________________________________ = "__________________________________________________________________";
    public string[] extraTexts;
    public float[] extraNumbers;
    //public bool habelAyyah = false;

    //

    int myTotalSpriteDepth = 0;
    int main_TotalSpriteDepth = 0;
    int active_TotalSpriteDepth = 0;
    int onPressExtra_TotalSpriteDepth = 0;
    int text_TotalSpriteDepth = 0;

    //

    [HideInInspector]
    public GUIGroup ownerGroup = null;

    [HideInInspector]
    public GUIControlCurStatus currentStatus;

    [HideInInspector]
    public bool isPressed = false;

    [HideInInspector]
    public bool isBusy_ActiveAnim = false;

    [HideInInspector]
    TouchUpInfo touchEndedOnThis_Info;

    [HideInInspector]
    TouchUpInfo tap_Info;

    [HideInInspector]
    TouchUpInfo sli_Info;

    //

    [HideInInspector]
    public SpriteRenderer spriteRenderer_Main = null;

    [HideInInspector]
    public SpriteRenderer spriteRenderer_Active = null;

    [HideInInspector]
    public List<SpriteRenderer> spriteRenderer_Texts = new List<SpriteRenderer>();

    //

    //[HideInInspector]
    //public Rect rect = new Rect(0, 0, 0, 0);

    //[HideInInspector]
    //public List<Rect> textRects = new List<Rect>();

    //

    [HideInInspector]
    public bool visibility_Total = true;

    float activeAlpha = 0;

    float alpha_Total = 1;

    Color color_Total_Pics = new Color(1, 1, 1, 1);

    Color color_Total_Texts = new Color(1, 1, 1, 1);

    //

    [HideInInspector]
    public float offsetX = 0;
    [HideInInspector]
    public float offsetY = 0;
    [HideInInspector]
    public float offsetW = 0;
    [HideInInspector]
    public float offsetH = 0;

    [HideInInspector]
    public float sliderOffsetX = 0;
    [HideInInspector]
    public float sliderOffsetY = 0;

    [HideInInspector]
    public float animOffsetX = 0;
    [HideInInspector]
    public float animOffsetY = 0;
    [HideInInspector]
    public float animOffsetW = 0;
    [HideInInspector]
    public float animOffsetH = 0;
    [HideInInspector]
    public float animOffsetAlpha = 1;


    float coefMode_CoefX = 1;
    float coefMode_CoefW = 1;

    float allignCoef_X = 1;
    float allignCoef_Y = 1;
    float allignCoef_W = 0;
    float allignCoef_H = 0;
    float allignOffsetX = 0;
    float allignOffsetY = 0;

    //

    bool isItFirstTimeCreatingTextSprites = true;

    string previousText;

    float textSize = 1f;
    float textHeight = 1f;

    float textStartPosCoef_W = 0;
    float textStartPosCoef_H = 0;

    float textSumOfWidthsCoef = 0;
    float textCharsHeightCoef = 0;

    float textTotalMargin_X = 0;
    float textTotalMargin_Y = 0;

    float textSideCoef = 0;

    List<Letter> curTextLetters = new List<Letter>();

    float localLeft = 0;
    float localRight = 0;
    float localTop = 0;
    float localBot = 0;

    BusyTouchInfo busyTouchInfo = null;

    //

    bool isFingerSliding = false;
    bool isFingerSliding_OnThisFrame = false;

    float sliderLerpSpeed = 18f;
    float sliderLerpError = 0.01f;

    float sliderValOnStartOfFingerDown = -0.123456789f;
    float sliderOldVal = -0.123456789f;
    [HideInInspector]
    public float slider_CurVal = 0;
    float slider_FingerOn_HandleFinalPos = 0;
    float slider_NoFinger_TargetPos = 0;

    bool shouldMoveSliderToTarget = false;

    float sliderCurValLerpSpeed = 15f;

    bool sliderInited = false;



    [HideInInspector]
    public bool sliderChangedByHand = false;

    [HideInInspector]
    public bool sliderChangedByHand_Mem = false;
    //Anim

    bool anim_ShouldDoVisInit = false;
    float animDelay = 0;

    bool isAnim_NormToActive = false;
    bool isAnim_ActiveToNorm = false;

    [HideInInspector]
    public GUIControlShowingAnimStatus showingAnimStatus = GUIControlShowingAnimStatus.Unused;

    float animTimeCounterSpeed;
    float animTarget_Start;
    float animTarget_End;
    float animTimeCounter = 0;
    float animCoef = 1;
    AnimMode animMode = AnimMode.Show;


    float animTarget_PartB_Start;
    float animTarget_PartB_End;

    AnimationCurve animCurve = null;
    AnimationCurve animCurve_PartB = null;

    bool anim_WHScale = false;
    bool anim_YChange = false;
    bool anim_YChange_Alpha = false;
    bool anim_XChange = false;
    bool anim_XChange_Alpha = false;
    bool anim_Alpha = false;

    //~Anim

    #endregion

    // -------------------------------------            ----------------------------           ---------------------           ---------------            --------------

    void AnimMutualInit(float _delay, float _startTarg, float _endTarg, AnimationCurve _animCurve, float _TimeCounterSpeed, AnimMode _animMode)
    {
        anim_ShouldDoVisInit = true;

        animDelay = _delay;

        animTarget_Start = _startTarg;
        animTarget_End = _endTarg;

        animCurve = _animCurve;
        animMode = _animMode;

        animTimeCounter = 0;
        animTimeCounterSpeed = _TimeCounterSpeed;

        if (animMode == AnimMode.Show)
        {
            showingAnimStatus = GUIControlShowingAnimStatus.GoingShow;
        }
        else
        {
            showingAnimStatus = GUIControlShowingAnimStatus.GoingHidden;
        }

        if (animMode == AnimMode.Show)
            animCoef = 1;
        else
            animCoef = -1;

        AddBusyTouch();
    }

    void AnimPartBInit(float _startTarg_PartB, float _endTarg_PartB, AnimationCurve _animCurve_PartB)
    {
        animTarget_PartB_Start = _startTarg_PartB;
        animTarget_PartB_End = _endTarg_PartB;

        animCurve_PartB = _animCurve_PartB;
    }

    bool IsAnimTimeOver()
    {
        return (animTimeCounter == 1);
    }

    void Generic_SetAnimIsFinished(bool _curRunningAnim)
    {
        if (animMode == AnimMode.Show)
        {
            showingAnimStatus = GUIControlShowingAnimStatus.Show;
        }
        else
        {
            showingAnimStatus = GUIControlShowingAnimStatus.Hidden;
        }

        _curRunningAnim = false;

        RemoveBusyTouch();
    }

    float Anim_IncStartTarget()
    {
        return animTarget_Start + animCurve.Evaluate(animTimeCounter) * (animTarget_End - animTarget_Start);
    }

    float Anim_IncStartTarget_PartB()
    {
        return animTarget_PartB_Start + animCurve_PartB.Evaluate(animTimeCounter) * (animTarget_PartB_End - animTarget_PartB_Start);
    }

    public void StartAnim_WHScale(float _delay, float _startWHScale, float _endWHScale, AnimationCurve _animCurve, float _TimeCounterSpeed, AnimMode _animMode)
    {
        AnimMutualInit(_delay, _startWHScale, _endWHScale, _animCurve, _TimeCounterSpeed, _animMode);

        anim_WHScale = true;
    }

    public void StartAnim_Alpha(float _delay, float _startAlpha, float _endAlpha, AnimationCurve _animCurve, float _TimeCounterSpeed, AnimMode _animMode)
    {
        AnimMutualInit(_delay, _startAlpha, _endAlpha, _animCurve, _TimeCounterSpeed, _animMode);

        anim_Alpha = true;
    }

    public void StartAnim_Y(float _delay, float _startY, float _endY, AnimationCurve _animCurve, float _TimeCounterSpeed, AnimMode _animMode)
    {
        AnimMutualInit(_delay, _startY, _endY, _animCurve, _TimeCounterSpeed, _animMode);

        anim_YChange = true;
    }

    public void StartAnim_Y_Alpha(float _delay, float _startY, float _endY, AnimationCurve _animCurve, float _startAlpha, float _endAlpha, AnimationCurve _animCurve_Alpha, float _TimeCounterSpeed, AnimMode _animMode)
    {
        AnimMutualInit(_delay, _startY, _endY, _animCurve, _TimeCounterSpeed, _animMode);
        AnimPartBInit(_startAlpha, _endAlpha, _animCurve_Alpha);

        anim_YChange_Alpha = true;
    }

    public void StartAnim_X(float _delay, float _startX, float _endX, AnimationCurve _animCurve, float _TimeCounterSpeed, AnimMode _animMode)
    {
        AnimMutualInit(_delay, _startX, _endX, _animCurve, _TimeCounterSpeed, _animMode);

        anim_XChange = true;
    }

    public void StartAnim_X_Alpha(float _delay, float _startX, float _endX, AnimationCurve _animCurve, float _startAlpha, float _endAlpha, AnimationCurve _animCurve_Alpha, float _TimeCounterSpeed, AnimMode _animMode)
    {
        AnimMutualInit(_delay, _startX, _endX, _animCurve, _TimeCounterSpeed, _animMode);
        AnimPartBInit(_startAlpha, _endAlpha, _animCurve_Alpha);

        anim_XChange_Alpha = true;
    }

    public void SetAnimInitialVisFalse()
    {
        anim_ShouldDoVisInit = false;
    }

    //

    public void SetOwner(GUIGroup _owner)
    {
        ownerGroup = _owner;
    }

    public void SetAlpha(float _val)
    {
        alpha = Mathf.Clamp01(_val);
    }

    public float GetTotalAlpha()
    {
        return alpha * animOffsetAlpha * ownerGroup.alpha_Total;
    }

    public bool GetTotalVisibility()
    {
        return (isVisible && animVis && ownerGroup.visibility_Total);
    }

    public Color GetTotalColor(Color _color)
    {
        return mainColor * _color;
    }

    public Color GetTotalColor_Pics()
    {
        return GetTotalColor(picsColorInfo.color);
    }

    public Color GetTotalColor_Texts()
    {
        return GetTotalColor(textColorInfo.color);
    }

    //

    public float Get_Width()
    {
        return (w + offsetW + animOffsetW) * coefMode_CoefW;
    }

    public float Get_Height()
    {
        return (h + offsetH + animOffsetH);
    }

    float Get_FinalLocalPosX_WCoef()
    {
        return allignCoef_W;
    }

    float Get_FinalLocalPosY_HCoef()
    {
        return allignCoef_H;
    }

    float Get_FinalLocalPosX()
    {
        return (allignCoef_X * coefMode_CoefX * (x + offsetX + sliderOffsetX + animOffsetX)) + (Get_FinalLocalPosX_WCoef() * (Get_Width() - animOffsetW)) + allignOffsetX;
    }

    float Get_FinalLocalPosY()
    {
        return (allignCoef_Y * (y + offsetY + sliderOffsetY + animOffsetY)) + (Get_FinalLocalPosY_HCoef() * (Get_Height() - animOffsetH)) + allignOffsetY;
    }

    //

    Vector3 Get_ObjectFinalPos()
    {
        return transform.position;
    }

    //

    public Rect GetFinalRect()
    {
        Vector3 objFinPos = Get_ObjectFinalPos();

        float rW = Get_Width();
        float rH = Get_Height();
        float rX = objFinPos.x - rW / 2;
        float rY = objFinPos.y - rH / 2;

        return new Rect(rX, rY, rW, rH);
    }

    public List<Rect> GetFinalTextRects()
    {
        List<Rect> rects = new List<Rect>();

        for (int i = 0; i < curTextLetters.Count; i++)
        {
            rects.Add(GetFinalTextRect(i));
        }

        return rects;
    }

    public Rect GetFinalTextRect(int _index)
    {
        int ind = _index;

        Vector3 txtSprGlobalPos = spriteRenderer_Texts[ind].transform.position;

        float rW = curTextLetters[ind].width;
        float rH = textHeight;
        float rX = txtSprGlobalPos.x - rW / 2;
        float rY = txtSprGlobalPos.y - rH / 2;

        return new Rect(rX, rY, rW, rH);
    }

    public void SetXByRect(Rect _rect, GUIRectAllignX _allignX)
    {
        Rect rct = _rect;
        GUIRectAllignX allignX = _allignX;

        float w = Get_Width();

        float newX = (rct.xMin + rct.xMax) / 2;
        float rctW = rct.width;

        switch (allignX)
        {
            case GUIRectAllignX.Left_Inside:
                newX += (-rctW / 2) + (w / 2);
                break;

            case GUIRectAllignX.Left_Between:
                newX += (-rctW / 2);
                break;

            case GUIRectAllignX.Left_Outside:
                newX += (-rctW / 2) - (w / 2);
                break;

            case GUIRectAllignX.Right_Inside:
                newX += (rctW / 2) - (w / 2);
                break;

            case GUIRectAllignX.Right_Between:
                newX += (rctW / 2);
                break;

            case GUIRectAllignX.Right_Outside:
                newX += (rctW / 2) + (w / 2);
                break;
        }

        x = newX;
    }

    public void SetYByRect(Rect _rect, GUIRectAllignY _allignY)
    {
        Rect rct = _rect;
        GUIRectAllignY allignY = _allignY;

        float h = Get_Height();

        float newY = (rct.yMin + rct.yMax) / 2;
        float rctH = rct.height;

        switch (allignY)
        {
            case GUIRectAllignY.Bot_Inside:
                newY += (-rctH / 2) + (h / 2);
                break;

            case GUIRectAllignY.Bot_Between:
                newY += (-rctH / 2);
                break;

            case GUIRectAllignY.Bot_Outside:
                newY += (-rctH / 2) - (h / 2);
                break;

            case GUIRectAllignY.Top_Inside:
                newY += (rctH / 2) - (h / 2);
                break;

            case GUIRectAllignY.Top_Between:
                newY += (rctH / 2);
                break;

            case GUIRectAllignY.Top_Outside:
                newY += (rctH / 2) + (h / 2);
                break;
        }

        y = newY;
    }

    // // // // // // // // ----------------------------------------------------------------------


    public void ChangeX()
    {
        transform.localPosition = new Vector3(Get_FinalLocalPosX(), transform.localPosition.y, transform.localPosition.z);
    }

    public void ChangeY()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, Get_FinalLocalPosY(), transform.localPosition.z);
    }

    public void ChangeW()
    {
        if (Get_FinalLocalPosX_WCoef() != 0)
        {
            ChangeX();
        }

        ChangeSpriteRenderer_LocalScale_Main();
        ChangeSpriteRenderer_LocalScale_Active();

        ChangeSpriteRenderer_LocalPosition_Texts();
    }

    public void ChangeH()
    {
        if (Get_FinalLocalPosY_HCoef() != 0)
        {
            ChangeY();
        }

        ChangeSpriteRenderer_LocalScale_Main();
        ChangeSpriteRenderer_LocalScale_Active();

        ChangeSpriteRenderer_LocalPosition_Texts();
    }

    //

    public void ChangeVisibility()
    {
        bool oldVis = visibility_Total;

        visibility_Total = GetTotalVisibility();

        if (oldVis != visibility_Total)
        {
            ChangeSpriteRenderer_Visibility_Main();
            ChangeSpriteRenderer_Visibility_Active();
            ChangeSpriteRenderer_Visibility_Texts();
        }
    }

    public void ChangeAlpha()
    {
        alpha_Total = GetTotalAlpha();

        ChangeSpriteRenderer_Alpha_Main();
        ChangeSpriteRenderer_Alpha_Active();
        ChangeSpriteRenderer_Alpha_Texts();
    }

    public void ChangeColor_Main()
    {
        ChangeColor_Pics();
        ChangeColor_Texts();
    }

    public void ChangeColor_Pics()
    {
        color_Total_Pics = GetTotalColor_Pics();

        ChangeSpriteRenderer_Color_Main();
        ChangeSpriteRenderer_Color_Active();
    }

    public void ChangeColor_Texts()
    {
        color_Total_Texts = GetTotalColor_Texts();

        ChangeSpriteRenderer_Color_Texts();
    }

    //

    public void ChangeTextSize()
    {
        textSize = MyGUIController.Instance.ConvertPixelToNewSpace(textFontInfo.fontSize);

        //TEST

        if (MyGUIController.Instance.realScreenHeightIsBiggerThanMax)
        {
            float sc = MyGUIController.Instance.bigScreenHeightSizeCoef;

            if (applyBigHeightSizeLimits_W)
            {
                textSize *= sc;
            }
        }

        //~TEST

        textHeight = MyGUIController.LETTER_DEFAULT_HEIGHT * textSize;

        ChangeSpriteRenderer_LocalScale_Texts();
    }

    public void ChangeText(string _txt)
    {
        previousText = text;

        text = St_ExtraTextFuncs.LimitTextToMax(_txt);

        if (isItFirstTimeCreatingTextSprites)
        {
            isItFirstTimeCreatingTextSprites = false;

            FirstTime_CreateSpriteRenderers_Text();

            return;
        }

        CheckAndRecreateTextSpriteRenderers();
    }

    //

    public void ChangeSpriteRenderer_Sprite_Main(Sprite _sprite)
    {
        SetSprRenderer_Sprite(spriteRenderer_Main, _sprite);
    }

    public void ChangeSpriteRenderer_Sprite_Active(Sprite _sprite)
    {
        SetSprRenderer_Sprite(spriteRenderer_Active, _sprite);
    }

    //

    public void ChangeSpriteRenderer_LocalPosition_Texts()
    {
        float textStartPos_X = Get_Width() * textStartPosCoef_W + textTotalMargin_X;
        float textStartPos_Y = Get_Height() * textStartPosCoef_H + textTotalMargin_Y + textCharsHeightCoef * textHeight;

        float sumOfWidths = 0;

        for (int i = 0; i < curTextLetters.Count; i++)
        {
            Letter curLetter = curTextLetters[i];

            curLetter.localPos_X = textStartPos_X + textSideCoef * (sumOfWidths + curLetter.width / 2);
            curLetter.localPos_Y = textStartPos_Y;

            sumOfWidths += curLetter.width;

        }

        float halfSum = -textSideCoef * textSumOfWidthsCoef * sumOfWidths;

        for (int i = 0; i < spriteRenderer_Texts.Count; i++)
        {
            curTextLetters[i].localPos_X += halfSum;

            SetSprRenderer_LocalPos(spriteRenderer_Texts[i], new Vector3(curTextLetters[i].localPos_X, curTextLetters[i].localPos_Y, 0));
        }
    }

    //

    public void ChangeSpriteRenderer_LocalScale_Main()
    {
        SetSprRenderer_LocalScale(spriteRenderer_Main, CalcSpriteRenderer_LocalScale(spriteRenderer_Main, Get_Width(), Get_Height()));
    }

    public void ChangeSpriteRenderer_LocalScale_Active()
    {
        SetSprRenderer_LocalScale(spriteRenderer_Active, CalcSpriteRenderer_LocalScale(spriteRenderer_Active, Get_Width(), Get_Height()));
    }

    public void ChangeSpriteRenderer_LocalScale_Texts()
    {
        for (int i = 0; i < spriteRenderer_Texts.Count; i++)
        {
            curTextLetters[i].width = MyGUIController.Instance.ConvertPixelToNewSpace(curTextLetters[i].selectedTexture.texture.width) * textSize;

            SetSprRenderer_LocalScale(spriteRenderer_Texts[i], CalcSpriteRenderer_LocalScale(spriteRenderer_Texts[i], curTextLetters[i].width, textHeight));
        }

        ChangeSpriteRenderer_LocalPosition_Texts();
    }

    //

    public void ChangeSpriteRenderer_Color_Main()
    {
        SetSprRenderer_Color(spriteRenderer_Main, color_Total_Pics);
    }

    public void ChangeSpriteRenderer_Color_Active()
    {
        SetSprRenderer_Color(spriteRenderer_Active, color_Total_Pics);
    }

    public void ChangeSpriteRenderer_Color_Texts()
    {
        for (int i = 0; i < spriteRenderer_Texts.Count; i++)
        {
            SetSprRenderer_Color(spriteRenderer_Texts[i], color_Total_Texts);
        }
    }

    //

    public void ChangeSpriteRenderer_Alpha_Main()
    {
        float sprOldAlpha = spriteRenderer_Main.color.a;

        SetSprRenderer_Alpha(spriteRenderer_Main, alpha_Total);

        if ((sprOldAlpha == 0 && spriteRenderer_Main.color.a > 0) || (sprOldAlpha > 0 && spriteRenderer_Main.color.a == 0))
            ChangeSpriteRenderer_Visibility_Main();
    }

    public void ChangeSpriteRenderer_Alpha_Active()
    {
        float sprOldAlpha = spriteRenderer_Active.color.a;

        SetSprRenderer_Alpha(spriteRenderer_Active, activeAlpha * alpha_Total);

        if ((sprOldAlpha == 0 && spriteRenderer_Active.color.a > 0) || (sprOldAlpha > 0 && spriteRenderer_Active.color.a == 0))
            ChangeSpriteRenderer_Visibility_Active();
    }

    public void ChangeSpriteRenderer_Alpha_Texts()
    {
        float sprOldAlpha = 0;

        if (spriteRenderer_Texts.Count > 0)
        {
            sprOldAlpha = spriteRenderer_Texts[0].color.a;
        }

        for (int i = 0; i < spriteRenderer_Texts.Count; i++)
        {
            SetSprRenderer_Alpha(spriteRenderer_Texts[i], alpha_Total);
        }

        if (spriteRenderer_Texts.Count > 0)
        {
            if ((sprOldAlpha == 0 && spriteRenderer_Texts[0].color.a > 0) || (sprOldAlpha > 0 && spriteRenderer_Texts[0].color.a == 0))
                ChangeSpriteRenderer_Visibility_Texts();
        }
    }

    //

    public void ChangeSpriteRenderer_Visibility_Main()
    {
        SetSprRenderer_Visibility(spriteRenderer_Main, visibility_Total && spriteRenderer_Main.color.a > 0);
    }

    public void ChangeSpriteRenderer_Visibility_Active()
    {
        SetSprRenderer_Visibility(spriteRenderer_Active, visibility_Total && spriteRenderer_Active.color.a > 0);
    }

    public void ChangeSpriteRenderer_Visibility_Texts()
    {
        for (int i = 0; i < spriteRenderer_Texts.Count; i++)
        {
            SetSprRenderer_Visibility(spriteRenderer_Texts[i], visibility_Total && spriteRenderer_Texts[i].color.a > 0);
        }
    }

    public void ChangeStatus()
    {
        GUIControlCurStatus oldStatus = currentStatus;

        #region Change Cur Status
        if (isEnabled)
        {
            if (isActivated)
            {
                if (isPressed)
                {
                    currentStatus = GUIControlCurStatus.ActivePressed;

                    ChangeSpriteRenderer_Sprite_Active(sprite_Active_Pressed);
                    ChangeSpriteRenderer_Sprite_Main(sprite_Normal_Pressed);
                }
                else
                {
                    currentStatus = GUIControlCurStatus.Active;

                    ChangeSpriteRenderer_Sprite_Active(sprite_Active_Main);
                    ChangeSpriteRenderer_Sprite_Main(sprite_Normal_Main);
                }
            }
            else
            {
                if (isPressed)
                {
                    currentStatus = GUIControlCurStatus.NormPressed;

                    ChangeSpriteRenderer_Sprite_Active(sprite_Active_Pressed);
                    ChangeSpriteRenderer_Sprite_Main(sprite_Normal_Pressed);
                }
                else
                {
                    currentStatus = GUIControlCurStatus.Normal;

                    ChangeSpriteRenderer_Sprite_Active(sprite_Active_Main);
                    ChangeSpriteRenderer_Sprite_Main(sprite_Normal_Main);
                }

            }
        }
        else
        {
            if (isActivated)
            {
                currentStatus = GUIControlCurStatus.Disabled_Active;

                ChangeSpriteRenderer_Sprite_Active(sprite_Disabled_Active);
                ChangeSpriteRenderer_Sprite_Main(sprite_Disabled_Active);
            }
            else
            {
                currentStatus = GUIControlCurStatus.Disabled_Deactive;

                ChangeSpriteRenderer_Sprite_Active(sprite_Disabled_Deactive);
                ChangeSpriteRenderer_Sprite_Main(sprite_Disabled_Deactive);
            }

        }
        #endregion

        #region Start Activation Anim
        if (oldStatus == GUIControlCurStatus.Normal || oldStatus == GUIControlCurStatus.NormPressed || oldStatus == GUIControlCurStatus.Disabled_Deactive)
            if (currentStatus == GUIControlCurStatus.Active || currentStatus == GUIControlCurStatus.ActivePressed || currentStatus == GUIControlCurStatus.Disabled_Active)
            {
                isAnim_NormToActive = true;
                isAnim_ActiveToNorm = false;
            }

        if (oldStatus == GUIControlCurStatus.Active || oldStatus == GUIControlCurStatus.ActivePressed || oldStatus == GUIControlCurStatus.Disabled_Active)
            if (currentStatus == GUIControlCurStatus.Normal || currentStatus == GUIControlCurStatus.NormPressed || currentStatus == GUIControlCurStatus.Disabled_Deactive)
            {
                isAnim_NormToActive = false;
                isAnim_ActiveToNorm = true;
            }
        #endregion
    }


    // // // // ---------------------------------------------------------------------------------------------

    SpriteRenderer CreateSpriteRenderer(string _name, Color _color, int _depth, Sprite _sprite, float _alpha, bool _visibility)
    {
        SpriteRenderer sprRend = Instantiate(MyGUIController.Instance.sourceSpriteRenderer) as SpriteRenderer;
        sprRend.transform.parent = this.transform;
        sprRend.transform.name = _name;
        sprRend.gameObject.layer = this.gameObject.layer;
        sprRend.sortingOrder = _depth;

        SetSprRenderer_Sprite(sprRend, _sprite);
        SetSprRenderer_LocalPos(sprRend, new Vector3(0, 0, 0));
        SetSprRenderer_LocalScale(sprRend, new Vector3(1, 1, 1));
        SetSprRenderer_Color(sprRend, _color);
        SetSprRenderer_Alpha(sprRend, _alpha);
        SetSprRenderer_Visibility(sprRend, _visibility);

        return sprRend;
    }

    void CreateSpriteRenderer_Main()
    {
        spriteRenderer_Main = CreateSpriteRenderer("SpriteRenderer_Main", color_Total_Pics, main_TotalSpriteDepth, sprite_Normal_Main, alpha_Total, visibility_Total);
    }

    void CreateSpriteRenderer_Active()
    {
        spriteRenderer_Active = CreateSpriteRenderer("SpriteRenderer_Active", color_Total_Pics, active_TotalSpriteDepth, sprite_Active_Main, alpha_Total, false);
    }

    void FirstTime_CreateSpriteRenderers_Text()
    {
        int txtLength = text.Length;

        if (txtLength == 0)
            return;

        if (isTextPersian)
        {
            curTextLetters = MyGUIController.Instance.textToPersianConverter.ConvertToPersian(text);
        }
        else
        {
            curTextLetters = MyGUIController.Instance.textToPersianConverter.GetEnglishLetters(text);
        }

        spriteRenderer_Texts = new List<SpriteRenderer>();

        for (int i = 0; i < txtLength; i++)
        {
            SpriteRenderer sprRend = CreateSpriteRenderer("SpriteRenderer_Text (Index: " + i + ")", color_Total_Texts, text_TotalSpriteDepth, curTextLetters[i].selectedTexture, alpha_Total, visibility_Total);
            spriteRenderer_Texts.Add(sprRend);
        }

        ChangeSpriteRenderer_LocalScale_Texts();
    }

    void CheckAndRecreateTextSpriteRenderers()
    {
        if (text == previousText)
            return;

        if (isTextPersian)
        {
            curTextLetters = MyGUIController.Instance.textToPersianConverter.ConvertToPersian(text);
        }
        else
        {
            curTextLetters = MyGUIController.Instance.textToPersianConverter.GetEnglishLetters(text);
        }

        int txtLength = text.Length;

        int spritesCount = spriteRenderer_Texts.Count;

        for (int i = 0; i < spritesCount; i++)
        {
            Destroy(spriteRenderer_Texts[spriteRenderer_Texts.Count - 1].gameObject);
            spriteRenderer_Texts.RemoveAt(spriteRenderer_Texts.Count - 1);
        }

        for (int i = 0; i < txtLength; i++)
        {
            SpriteRenderer sprRend = CreateSpriteRenderer("SpriteRenderer_Text (Index: " + i + ")", color_Total_Texts, text_TotalSpriteDepth, curTextLetters[i].selectedTexture, alpha_Total, visibility_Total);
            spriteRenderer_Texts.Add(sprRend);
        }

        ChangeSpriteRenderer_LocalScale_Texts();

        //

        //if (text == previousText)
        //    return;

        //if (isTextPersian)
        //    curTextLetters = MyGUIController.Instance.textToPersianConverter.ConvertToPersian(text);

        //int prevTxtLength = previousText.Length;
        //int txtLength = text.Length;

        //int minLength = Mathf.Min(prevTxtLength, txtLength);

        //int afterMutuals_Index = 0;

        //for (afterMutuals_Index = 0; afterMutuals_Index < minLength; afterMutuals_Index++)
        //{
        //    if (text[afterMutuals_Index] == previousText[afterMutuals_Index])
        //        continue;
        //    else
        //        break;
        //}

        //int spritesCount = spriteRenderer_Texts.Count;

        //for (int i = afterMutuals_Index; i < spritesCount; i++)
        //{
        //    Destroy(spriteRenderer_Texts[spriteRenderer_Texts.Count - 1].gameObject);
        //    spriteRenderer_Texts.RemoveAt(spriteRenderer_Texts.Count - 1);
        //}

        //for (int i = afterMutuals_Index; i < txtLength; i++)
        //{
        //    SpriteRenderer sprRend = CreateSpriteRenderer("SpriteRenderer_Text (Index: " + i + ")", color_Total_Texts, text_TotalSpriteDepth, curTextLetters[i].selectedTexture, alpha_Total, visibility_Total);
        //    spriteRenderer_Texts.Add(sprRend);
        //}

        //ChangeSpriteRenderer_LocalScale_Texts();
    }

    //

    void SetSprRenderer_Sprite(SpriteRenderer _sprRenderer, Sprite _sprite)
    {
        SpriteRenderer sprRend = _sprRenderer;
        Sprite spr = _sprite;

        sprRend.sprite = spr;
    }

    void SetSprRenderer_LocalPos(SpriteRenderer _sprRenderer, Vector3 _localPos)
    {
        SpriteRenderer sprRend = _sprRenderer;
        Vector3 localPos = _localPos;

        sprRend.transform.localPosition = localPos;
    }

    void SetSprRenderer_LocalScale(SpriteRenderer _sprRenderer, Vector3 _localScale)
    {
        SpriteRenderer sprRend = _sprRenderer;
        Vector3 localScale = _localScale;

        sprRend.transform.localScale = localScale;
    }

    void SetSprRenderer_Color(SpriteRenderer _sprRenderer, Color _color)
    {
        SpriteRenderer sprRend = _sprRenderer;
        Color col = _color;

        sprRend.color = new Color(col.r, col.g, col.b, sprRend.color.a);
    }

    void SetSprRenderer_Alpha(SpriteRenderer _sprRenderer, float _alpha)
    {
        SpriteRenderer sprRend = _sprRenderer;
        float alph = _alpha;

        sprRend.color = new Color(sprRend.color.r, sprRend.color.g, sprRend.color.b, alph);
    }

    void SetSprRenderer_Visibility(SpriteRenderer _sprRenderer, bool _isVisible)
    {
        SpriteRenderer sprRend = _sprRenderer;
        bool isVisible = _isVisible;

        sprRend.enabled = isVisible;
    }

    Vector3 CalcSpriteRenderer_LocalScale(SpriteRenderer _spriteRenderer, float _w, float _h)
    {
        SpriteRenderer sprRenderer = _spriteRenderer;
        float daW = _w;
        float daH = _h;

        if (sprRenderer.sprite == null)
            return new Vector3(1, 1, 1);

        float spriteTextureW = MyGUIController.Instance.ConvertPixelToNewSpace(sprRenderer.sprite.texture.width);
        float spriteTextureH = MyGUIController.Instance.ConvertPixelToNewSpace(sprRenderer.sprite.texture.height);

        float scX = (daW / spriteTextureW);
        float scY = (daH / spriteTextureH);

        return new Vector3(scX, scY, 1);
    }

    //

    public void Init(GUIGroup _owner, int _parentSpriteDepth, int _mySpriteDepth)
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);

        SetOwner(_owner);

        myTotalSpriteDepth = _parentSpriteDepth * PARENT_SPRITE_DEPTH_COEF + _mySpriteDepth * CONTROL_SPRITE_DEPTH_COEF;

        main_TotalSpriteDepth = myTotalSpriteDepth + MAIN_SPRITE_DEPTH;
        active_TotalSpriteDepth = myTotalSpriteDepth + ACTIVE_SPRITE_DEPTH;
        onPressExtra_TotalSpriteDepth = myTotalSpriteDepth + ON_PRESS_EXTRA_SPRITE_DEPTH;
        text_TotalSpriteDepth = myTotalSpriteDepth + TEXT_SPRITE_DEPTH;


        //TEST
        if (MyGUIController.Instance.realScreenHeightIsBiggerThanMax)
        {
            float sc = MyGUIController.Instance.bigScreenHeightSizeCoef;

            if (applyBigHeightSizeLimits_X)
            {
                x *= sc;
            }

            if (applyBigHeightSizeLimits_Y)
            {
                y *= sc;
            }

            if (applyBigHeightSizeLimits_H)
            {
                h *= sc;
            }

            if (applyBigHeightSizeLimits_W)
            {
                w *= sc;
            }
        }
        //~TEST

        if (ownerGroup.ownerList == null)
        {
            localLeft = MyGUIController.Instance.curScreen_Left;
            localRight = MyGUIController.Instance.curScreen_Right;
            localBot = MyGUIController.Instance.curScreen_Bot;
            localTop = MyGUIController.Instance.curScreen_Top;
        }
        else
        {
            Rect rct = ownerGroup.ownerList.GetFinalRect();

            localLeft = rct.xMin;
            localRight = rct.xMax;
            localBot = ownerGroup.Get_ObjTr_Y() - ownerGroup.ownerList.itemsSize / 2;
            localTop = ownerGroup.Get_ObjTr_Y() + ownerGroup.ownerList.itemsSize / 2;
        }


        #region allign_Horiz
        switch (allign_Horiz)
        {
            case GUIHorizAllign.Left:
                allignCoef_W = 0.5f;
                allignOffsetX = localLeft;
                break;

            case GUIHorizAllign.Right:
                allignCoef_W = -0.5f;
                //allignCoef_X = -1;
                allignOffsetX = localRight;
                break;
        }
        #endregion

        #region allign_Vert
        switch (allign_Vert)
        {
            case GUIVertAllign.Bot:
                allignCoef_H = 0.5f;
                allignOffsetY = localBot;
                break;

            case GUIVertAllign.Top:
                allignCoef_H = -0.5f;
                //allignCoef_Y = -1;
                allignOffsetY = localTop;
                break;
        }
        #endregion

        if (xIsCoef)
            coefMode_CoefX = MyGUIController.Instance.curScreen_WidthScale;

        if (wIsCoef)
            coefMode_CoefW = MyGUIController.Instance.curScreen_WidthScale;

        visibility_Total = GetTotalVisibility();
        alpha_Total = GetTotalAlpha();
        color_Total_Pics = GetTotalColor_Pics();
        color_Total_Texts = GetTotalColor_Texts();

        CreateSpriteRenderer_Main();
        ChangeSpriteRenderer_LocalScale_Main();

        CreateSpriteRenderer_Active();
        ChangeSpriteRenderer_LocalScale_Active();

        #region Text Inits

        if (isTextPersian)
        {
            textSideCoef = -1f;
        }
        else
        {
            textSideCoef = 1f;
        }

        textTotalMargin_X = textMarginLeft - textMarginRight;
        textTotalMargin_Y = textMarginBot - textMarginTop;

        //TEST

        if (MyGUIController.Instance.realScreenHeightIsBiggerThanMax)
        {
            float sc = MyGUIController.Instance.bigScreenHeightSizeCoef;

            if (applyBigHeightSizeLimits_X)
            {
                textTotalMargin_X *= sc;
            }

            if (applyBigHeightSizeLimits_Y)
            {
                textTotalMargin_X *= sc;
            }
        }

        //~TEST

        #region text_Allign_Horiz

        if (textAllignment == TextAnchor.LowerLeft || textAllignment == TextAnchor.MiddleLeft || textAllignment == TextAnchor.UpperLeft)
        {
            textStartPosCoef_W = -0.5f;
            textSumOfWidthsCoef = 1f;
        }

        if (textAllignment == TextAnchor.LowerCenter || textAllignment == TextAnchor.MiddleCenter || textAllignment == TextAnchor.UpperCenter)
        {
            textStartPosCoef_W = 0;
            textSumOfWidthsCoef = 0.5f;
        }

        if (textAllignment == TextAnchor.LowerRight || textAllignment == TextAnchor.MiddleRight || textAllignment == TextAnchor.UpperRight)
        {
            textStartPosCoef_W = 0.5f;
            textSumOfWidthsCoef = 0f;
        }

        #endregion

        #region text_Allign_Vert

        if (textAllignment == TextAnchor.LowerLeft || textAllignment == TextAnchor.LowerCenter || textAllignment == TextAnchor.LowerRight)
        {
            textStartPosCoef_H = -0.5f;
            textCharsHeightCoef = 0.5f;
        }

        if (textAllignment == TextAnchor.UpperLeft || textAllignment == TextAnchor.UpperCenter || textAllignment == TextAnchor.UpperRight)
        {
            textStartPosCoef_H = 0.5f;
            textCharsHeightCoef = -0.5f;
        }

        #endregion

        if(!isTextPersian)
        {
            textStartPosCoef_W = -textStartPosCoef_W;
            textSumOfWidthsCoef = 1 - textSumOfWidthsCoef;
        }

        //spriteRenderer_Texts = new List<SpriteRenderer>();

        ChangeTextSize();

        ChangeText(text);

        #endregion

        ChangeW();
        ChangeH();
        ChangeX();
        ChangeY();

        ChangeVisibility();

        if (toggleType == ToggleType.RadioBox)
        {
            radioBoxModeGroup.AddControl(this);
        }

        ChangeStatus();
    }

    void Update()
    {


        #region Activation Anim

        #region Activation Anim - NormToActive

        if (isAnim_NormToActive)
        {
            switch (activationAnim)
            {
                case AcDeacAnimType.Alpha:

                    if (activeAlpha < 1)
                    {
                        isBusy_ActiveAnim = true;

                        float anSp = activationAnimSpeed;
                        activeAlpha = Mathf.Clamp01(activeAlpha + anSp * Time.deltaTime);

                        ChangeSpriteRenderer_Alpha_Active();
                    }

                    break;

                case AcDeacAnimType.None:

                    activeAlpha = 1;

                    ChangeSpriteRenderer_Alpha_Active();

                    break;
            }

            if (activeAlpha == 1)
            {
                isBusy_ActiveAnim = false;

                isAnim_NormToActive = false;
            }
        }

        #endregion

        #region Activation Anim - ActiveToNorm

        if (isAnim_ActiveToNorm)
        {
            switch (activationAnim)
            {
                case AcDeacAnimType.Alpha:

                    if (activeAlpha > 0)
                    {
                        isBusy_ActiveAnim = true;

                        float anSp = activationAnimSpeed;
                        activeAlpha = Mathf.Clamp01(activeAlpha - anSp * Time.deltaTime);

                        ChangeSpriteRenderer_Alpha_Active();
                    }

                    break;

                case AcDeacAnimType.None:

                    activeAlpha = 0;

                    ChangeSpriteRenderer_Alpha_Active();

                    break;
            }

            if (activeAlpha == 0)
            {
                isBusy_ActiveAnim = false;

                isAnim_ActiveToNorm = false;
            }
        }

        #endregion

        #endregion

        #region Anims

        if (showingAnimStatus == GUIControlShowingAnimStatus.GoingShow || showingAnimStatus == GUIControlShowingAnimStatus.GoingHidden)
        {
            if (animDelay > 0)
            {
                animDelay = St_ExtraMath.DecByDeltaTimeToZero(animDelay);
            }

            if (animDelay == 0)
            {
                #region anim_ShouldDoVisInit

                if (anim_ShouldDoVisInit)
                {
                    SetAnimInitialVisFalse();

                    animVis = true;
                    ChangeVisibility();
                }

                #endregion

                #region animTimeCounter

                animTimeCounter += Time.deltaTime * animTimeCounterSpeed;
                animTimeCounter = Mathf.Clamp01(animTimeCounter);

                #endregion

                //

                #region Anim_WHScale

                if (anim_WHScale)
                {
                    float scVal = Anim_IncStartTarget();

                    animOffsetW = (scVal - 1) * w;
                    animOffsetH = (scVal - 1) * h;

                    ChangeW();
                    ChangeH();

                    if (IsAnimTimeOver())
                    {
                        Generic_SetAnimIsFinished(anim_WHScale);
                    }
                }

                #endregion

                #region Anim_Alpha

                if (anim_Alpha)
                {
                    animOffsetAlpha = Anim_IncStartTarget();

                    ChangeAlpha();

                    if (IsAnimTimeOver())
                    {
                        Generic_SetAnimIsFinished(anim_Alpha);

                        if (animOffsetAlpha == 0)
                        {
                            animVis = false;
                            ChangeVisibility();
                        }
                    }
                }

                #endregion

                #region Anim_YChange

                if (anim_YChange)
                {
                    animOffsetY = Anim_IncStartTarget();
                    ChangeY();

                    if (IsAnimTimeOver())
                    {
                        Generic_SetAnimIsFinished(anim_YChange);
                    }
                }

                #endregion

                #region Anim_YChange_Alpha

                if (anim_YChange_Alpha)
                {
                    animOffsetY = Anim_IncStartTarget();
                    animOffsetAlpha = Anim_IncStartTarget_PartB();

                    ChangeY();
                    ChangeAlpha();

                    if (IsAnimTimeOver())
                    {
                        Generic_SetAnimIsFinished(anim_YChange_Alpha);

                        if (animOffsetAlpha == 0)
                        {
                            animVis = false;
                            ChangeVisibility();
                        }
                    }
                }

                #endregion

                #region Anim_XChange

                if (anim_XChange)
                {
                    animOffsetX = Anim_IncStartTarget();

                    ChangeX();

                    if (IsAnimTimeOver())
                    {
                        Generic_SetAnimIsFinished(anim_XChange);
                    }
                }

                #endregion

                #region Anim_XChange_Alpha

                if (anim_XChange_Alpha)
                {
                    animOffsetX = Anim_IncStartTarget();
                    animOffsetAlpha = Anim_IncStartTarget_PartB();

                    ChangeX();
                    ChangeAlpha();

                    if (IsAnimTimeOver())
                    {
                        Generic_SetAnimIsFinished(anim_XChange_Alpha);

                        if (animOffsetAlpha == 0)
                        {
                            animVis = false;
                            ChangeVisibility();
                        }
                    }
                }

                #endregion
            }
        }

        #endregion

        #region Slider

        if (sliderInfo != null)
        {
            #region Init slider
            if (!sliderInited)
            {
                sliderInited = true;

                SetSliderCurVal(sliderInfo.initialVal);
                slider_NoFinger_TargetPos = GetSliderTargetPosBySliderVal(slider_CurVal);

                if (sliderInfo.sliderType == SliderType.Horiz)
                {
                    sliderInfo.handle.sliderOffsetX = slider_NoFinger_TargetPos;
                    sliderInfo.handle.ChangeX();
                }
            }
            #endregion

            if (isFingerSliding_OnThisFrame)
            {
                #region Slider finger down actions

                SetSliderVars();

                if (sliderInfo.sliderType == SliderType.Horiz)
                {
                    sliderInfo.handle.sliderOffsetX = Mathf.Lerp(sliderInfo.handle.sliderOffsetX, slider_FingerOn_HandleFinalPos, sliderLerpSpeed * Time.deltaTime);
                    sliderInfo.handle.ChangeX();
                }

                #region Move slider curVal control to target pos
                if (sliderInfo.curValControl && sliderInfo.curValConrtolOffsetOnHold != 0)
                {
                    if (sliderInfo.curValConrtolOffsetAxisIsX)
                    {
                        sliderInfo.curValControl.sliderOffsetX = Mathf.Lerp(sliderInfo.curValControl.sliderOffsetX, sliderInfo.curValConrtolOffsetOnHold, sliderCurValLerpSpeed * Time.deltaTime);
                        sliderInfo.curValControl.ChangeX();
                    }
                    else
                    {
                        sliderInfo.curValControl.sliderOffsetY = Mathf.Lerp(sliderInfo.curValControl.sliderOffsetY, sliderInfo.curValConrtolOffsetOnHold, sliderCurValLerpSpeed * Time.deltaTime);
                        sliderInfo.curValControl.ChangeY();
                    }
                }
                #endregion

                shouldMoveSliderToTarget = true;

                #endregion
            }
            else
            {
                #region Slider finger up actions

                #region Move Slider to target pos
                if (shouldMoveSliderToTarget)
                {
                    if (sliderInfo.sliderType == SliderType.Horiz)
                    {
                        slider_NoFinger_TargetPos = GetSliderTargetPosBySliderVal(slider_CurVal);

                        sliderInfo.handle.sliderOffsetX = Mathf.Lerp(sliderInfo.handle.sliderOffsetX, slider_NoFinger_TargetPos, sliderLerpSpeed * Time.deltaTime);

                        if (St_ExtraMath.Dist(sliderInfo.handle.sliderOffsetX, slider_NoFinger_TargetPos) <= sliderLerpError)
                        {
                            sliderInfo.handle.sliderOffsetX = slider_NoFinger_TargetPos;
                            shouldMoveSliderToTarget = false;
                        }

                        sliderInfo.handle.ChangeX();
                    }
                }
                #endregion

                #region Back slider curVal control to zero pos
                if (sliderInfo.curValControl && sliderInfo.curValConrtolOffsetOnHold != 0)
                {
                    if (sliderInfo.curValConrtolOffsetAxisIsX && sliderInfo.curValControl.sliderOffsetX != 0)
                    {
                        sliderInfo.curValControl.sliderOffsetX = Mathf.Lerp(sliderInfo.curValControl.sliderOffsetX, 0, sliderCurValLerpSpeed * Time.deltaTime);
                        sliderInfo.curValControl.ChangeX();
                    }

                    if (!sliderInfo.curValConrtolOffsetAxisIsX && sliderInfo.curValControl.sliderOffsetY != 0)
                    {
                        sliderInfo.curValControl.sliderOffsetY = Mathf.Lerp(sliderInfo.curValControl.sliderOffsetY, 0, sliderCurValLerpSpeed * Time.deltaTime);
                        sliderInfo.curValControl.ChangeY();
                    }
                }
                #endregion

                #endregion
            }

            //

            if (!isFingerSliding_OnThisFrame)
                isFingerSliding = false;

            //

            if (sliderChangedByHand)
            {
                TryDoEvent_OnSliderHandChange();
            }

            if (sliderChangedByHand_Mem && !isFingerSliding)
            {
                TryDoEvent_OnSliderHandChangeAndFingerUp();
            }

            //

            isFingerSliding_OnThisFrame = false;
            sliderChangedByHand_Mem = sliderChangedByHand;

            sliderChangedByHand = false;
        }

        #endregion
    }

    public void OnTap()
    {
        if (IsBusy())
            return;

        //if (toggleType == ToggleType.RadioBox)
        //{
        //    for (int i = 0; i < radioBoxModeGroup.curControls.Count; i++)
        //        if (radioBoxModeGroup.curControls[i].isBusy)
        //            return;
        //}

        tap_Info = new TouchUpInfo();
        tap_Info.time_Start = MyGUIController.Instance.menuMode_CurTouch.time_Start;
        tap_Info.time_Cur = Time.time;
        tap_Info.time_End = MyGUIController.Instance.menuMode_CurTouch.time_End;
        tap_Info.pos_Start = MyGUIController.Instance.menuMode_CurTouch.pos_Start;
        tap_Info.pos_End = MyGUIController.Instance.menuMode_CurTouch.pos_End;

        if (toggleType == ToggleType.RadioBox)
        {
            radioBoxModeGroup.ReSetControlsActivationStatus(this);
        }

        if (toggleType == ToggleType.CheckBox)
        {
            isActivated = !isActivated;
            ChangeStatus();
        }

        //

        bool done = false;

        switch (ownerGroup.myName)
        {
            case "Keyboard":
                MyGUIController.Instance.KeyboardKeyPressed(myName);
                done = true;
                break;

            case "KeyboardTextBox":
                if (myName == "TextBox")
                {
                    MyGUIController.Instance.KeyboardTextBoxCursorPressed(tap_Info);
                    done = true;
                }
                break;
        }

        if (!done)
            MenuModeTouchEventController.Instance.TapHappened(ownerGroup, this, tap_Info);

        if (Event_Tap != null)
            Event_Tap(this, tap_Info);
    }

    public void OnTouchEndedOnThis()
    {
        if (IsBusy())
            return;

        touchEndedOnThis_Info = new TouchUpInfo();
        touchEndedOnThis_Info.time_Start = MyGUIController.Instance.menuMode_CurTouch.time_Start;
        touchEndedOnThis_Info.time_Cur = Time.time;
        touchEndedOnThis_Info.time_End = MyGUIController.Instance.menuMode_CurTouch.time_End;
        touchEndedOnThis_Info.pos_Start = MyGUIController.Instance.menuMode_CurTouch.pos_Start;
        touchEndedOnThis_Info.pos_End = MyGUIController.Instance.menuMode_CurTouch.pos_End;

        //

        bool done = false;

        switch (ownerGroup.myName)
        {
            case "Keyboard":
                MyGUIController.Instance.KeyboardKeyPressed(myName);
                done = true;
                break;

            case "KeyboardTextBox":
                if (myName == "TextBox")
                {
                    MyGUIController.Instance.KeyboardTextBoxCursorPressed(touchEndedOnThis_Info);
                    done = true;
                }
                break;
        }

        if (!done)
            MenuModeTouchEventController.Instance.TouchUpHappened(ownerGroup, this, touchEndedOnThis_Info);

        if (Event_TouchUp != null)
            Event_TouchUp(this, tap_Info);
    }

    public void OnSlider()
    {
        if (IsBusy())
            return;

        sli_Info = new TouchUpInfo();

        sli_Info.time_Start = MyGUIController.Instance.menuMode_CurTouch.time_Start;
        sli_Info.time_Cur = Time.time;
        sli_Info.time_End = MyGUIController.Instance.menuMode_CurTouch.time_End;
        sli_Info.pos_Start = MyGUIController.Instance.menuMode_CurTouch.pos_Start;
        sli_Info.pos_End = MyGUIController.Instance.menuMode_CurTouch.pos_End;

        if (!isFingerSliding)
            sliderValOnStartOfFingerDown = slider_CurVal;

        isFingerSliding = true;
        isFingerSliding_OnThisFrame = true;
    }

    void AddBusyTouch()
    {
        if (busyTouchInfo == null)
        {
            busyTouchInfo = new BusyTouchInfo();
            busyTouchInfo.control = this;

            MyGUIController.Instance.AddDisablingTouchInfo(busyTouchInfo);
        }
    }

    void RemoveBusyTouch()
    {
        if (busyTouchInfo != null)
        {
            MyGUIController.Instance.RemoveDisablingTouchInfo(busyTouchInfo);
            busyTouchInfo = null;
        }
    }

    bool IsBusy()
    {
        return isBusy_ActiveAnim;
    }

    void SetSliderVars()
    {
        Rect finalRct = GetFinalRect();

        if (sliderInfo.sliderType == SliderType.Horiz)
        {
            float halfRectW = finalRct.width / 2;
            float edge = sliderInfo.edge;
            float halfHandleRange = halfRectW - edge;
            float handleRange = 2 * halfHandleRange;

            float rectMid = (finalRct.xMin + finalRct.xMax) / 2;
            float xMin = rectMid - halfHandleRange;
            float xMax = rectMid + halfHandleRange;
            float wdth = xMax - xMin;
            float touchVal = sli_Info.pos_End.x;

            touchVal = Mathf.Clamp(touchVal, xMin, xMax);

            float slider_FingerOn_PercentageVal = (touchVal - xMin) / wdth;

            float sliderMinVal = sliderInfo.minVal;

            float sliderValRange = (sliderInfo.maxVal - sliderInfo.minVal);

            slider_CurVal = (sliderMinVal + slider_FingerOn_PercentageVal * sliderValRange);

            if (sliderInfo.parts != 0)
            {
                float partRange = sliderValRange / (sliderInfo.parts - 1);
                float halfPartRange = partRange / 2;
                int partIndex = (int)(((sliderInfo.parts - 1) * (slider_CurVal - sliderMinVal + halfPartRange) / sliderValRange));
                partIndex = Mathf.Clamp(partIndex, 0, sliderInfo.parts - 1);

                slider_CurVal = (sliderMinVal + partIndex * partRange);
            }

            sliderChangedByHand = true;

            SetSliderCurVal(slider_CurVal);

            slider_FingerOn_HandleFinalPos = (-0.5f + slider_FingerOn_PercentageVal) * handleRange;

            return;
        }
    }

    float GetSliderTargetPosBySliderVal(float _sliderVal)
    {
        float sliderVal = _sliderVal;

        Rect finalRct = GetFinalRect();

        if (sliderInfo.sliderType == SliderType.Horiz)
        {
            float halfRectW = finalRct.width / 2;
            float edge = sliderInfo.edge;
            float halfHandleRange = halfRectW - edge;
            float handleRange = 2 * halfHandleRange;

            float rectMid = (finalRct.xMin + finalRct.xMax) / 2;
            float xMin = rectMid - halfHandleRange;
            float xMax = rectMid + halfHandleRange;
            float wdth = xMax - xMin;

            float sliderMinVal = sliderInfo.minVal;
            float sliderValRange = sliderInfo.maxVal - sliderMinVal;

            float percentage = (sliderVal - sliderMinVal) / sliderValRange;

            slider_CurVal = (sliderVal);

            if (sliderInfo.parts != 0)
            {
                float partRange = sliderValRange / (sliderInfo.parts - 1);
                float halfPartRange = partRange / 2;
                int partIndex = (int)(((sliderInfo.parts - 1) * (slider_CurVal - sliderMinVal + halfPartRange) / sliderValRange));
                partIndex = Mathf.Clamp(partIndex, 0, sliderInfo.parts - 1);

                slider_CurVal = (sliderMinVal + partIndex * partRange);
                percentage = (slider_CurVal - sliderMinVal) / sliderValRange;
            }

            SetSliderCurVal(slider_CurVal);

            return (-0.5f + percentage) * handleRange;
        }

        return 0;
    }

    void SetSliderCurVal(float _val)
    {
        slider_CurVal = _val;

        if (sliderInfo.curValControl && sliderOldVal != slider_CurVal)
        {
            sliderInfo.curValControl.ChangeText(slider_CurVal.ToString());
        }

        sliderOldVal = slider_CurVal;
    }

    public void ChangeSliderToVal(float _val)
    {
        shouldMoveSliderToTarget = true;

        slider_CurVal = _val;
    }

    //

    public void TryDoEvent_OnSliderHandChange()
    {
        if (Event_OnSliderHandChange != null)
        {
            Event_OnSliderHandChange();
        }
    }

    public void TryDoEvent_OnSliderHandChangeAndFingerUp()
    {
        if (Event_OnSliderHandChangeAndFingerUp != null)
        {
            Event_OnSliderHandChangeAndFingerUp();
        }
    }
}