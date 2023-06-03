using UnityEngine;
using System.Collections;

public class GUIGroup : MonoBehaviour
{
    #region Variables

    public string myName = "";
    public GUIControl[] childs;
    public bool isVisible = true;
    public bool animVis = true;
    public float alpha = 1;
    public int depth = 0;
    public float touchDepth = 0;
    public float x;
    public float y;
    public string _Extras________________________________________________ = "__________________________________________________________________";
    public string[] extraTexts;
    public float[] extraNumbers;

    [HideInInspector]
    public GUIList ownerList = null;

    [HideInInspector]
    public Changer changer = null;

    [HideInInspector]
    public bool visibility_Total = true;

    [HideInInspector]
    public float alpha_Total = 1;

    //

    AnimMode animMode = AnimMode.Show;

    bool anim_ShouldDoVisInit = false;
    float animDelay = 0;

    [HideInInspector]
    public GUIControlShowingAnimStatus showingAnimStatus = GUIControlShowingAnimStatus.Unused;

    #endregion

    //

    public void Init_WithoutOwner()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, MyGUIController.SPRITES_Z);

        for (int i = 0; i < childs.Length; i++)
        {
            childs[i].Init(this, depth, i);
        }

        ChangeVisibility();
        ChangeAlpha();
        ChangeX();
        ChangeY();
    }

    public void Init_WithOwnerList(GUIList _owner, ListChildType _type)
    {
        SetOwner(_owner);

        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);

        ListChildType lcType = _type;

        if (lcType == ListChildType.Own_Back)
        {
            for (int i = 0; i < childs.Length; i++)
            {
                childs[i].Init(this, ownerList.depth, i);
            }
        }

        if (lcType == ListChildType.Item)
        {
            for (int i = 0; i < childs.Length; i++)
            {
                childs[i].Init(this, ownerList.depth, i + GUIControl.LIST_ITEMS_START_DEPTH_INDEX);
            }
        }

        if (lcType == ListChildType.Own_Front)
        {
            for (int i = 0; i < childs.Length; i++)
            {
                childs[i].Init(this, ownerList.depth, i + GUIControl.LIST_FG_START_DEPTH_INDEX);
            }
        }

        ChangeVisibility();
        ChangeAlpha();
        ChangeX();
        ChangeY();
    }

    void Update()
    {
        #region Check Anims

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
                    anim_ShouldDoVisInit = false;

                    animVis = true;
                    ChangeVisibility();
                }

                #endregion
            }

            if (childs.Length > 0)
            {
                if (childs[0].showingAnimStatus == GUIControlShowingAnimStatus.Hidden)
                {
                    #region showingAnimStatus = GUIControlShowingAnimStatus.Hidden

                    showingAnimStatus = GUIControlShowingAnimStatus.Hidden;

                    bool shouldSetVisFalse = true;

                    for (int i = 0; i < childs.Length; i++)
                    {
                        if (childs[i].animVis && childs[i].GetTotalAlpha() > 0)
                        {
                            shouldSetVisFalse = false;
                            break;
                        }
                    }

                    if (shouldSetVisFalse)
                    {
                        animVis = false;

                        ChangeVisibility();
                    }

                    #endregion
                }

                if (childs[0].showingAnimStatus == GUIControlShowingAnimStatus.Show)
                {
                    #region showingAnimStatus = GUIControlShowingAnimStatus.Show

                    showingAnimStatus = GUIControlShowingAnimStatus.Show;

                    bool shouldSetVisFalse = true;

                    for (int i = 0; i < childs.Length; i++)
                    {
                        if (childs[i].animVis && childs[i].GetTotalAlpha() > 0)
                        {
                            shouldSetVisFalse = false;
                            break;
                        }
                    }

                    if (shouldSetVisFalse)
                    {
                        animVis = false;

                        ChangeVisibility();
                    }

                    #endregion
                }
            }
            else
            {
                if (showingAnimStatus == GUIControlShowingAnimStatus.GoingShow)
                    showingAnimStatus = GUIControlShowingAnimStatus.Show;

                if (showingAnimStatus == GUIControlShowingAnimStatus.GoingHidden)
                    showingAnimStatus = GUIControlShowingAnimStatus.Hidden;
            }
        }

        #endregion
    }

    //

    public void SetOwner(GUIList _owner)
    {
        ownerList = _owner;
    }

    public void SetChanger(Changer _changer)
    {
        changer = _changer;
    }

    public GUIControl FindChildByName(string _name)
    {
        for (int i = 0; i < childs.Length; i++)
        {
            if (childs[i].myName == _name)
                return childs[i];
        }

        return null;
    }

    public void SetAlpha(float _val)
    {
        alpha = Mathf.Clamp01(_val);
    }

    //

    public float Get_X()
    {
        return x;
    }

    public float Get_Y()
    {
        return y;
    }

    public float Get_ObjTr_X()
    {
        return transform.position.x;
    }

    public float Get_ObjTr_Y()
    {
        return transform.position.y;
    }

    public bool GetTotalVisibility()
    {
        if (!ownerList)
            return (isVisible && animVis &&(alpha_Total > 0));
        else
            return (isVisible && animVis && ownerList.visibility_Total && (alpha_Total > 0));
    }

    public float GetTotalAlpha()
    {
        float ownerAlpha = 1;

        if (ownerList)
            ownerAlpha = ownerList.alpha;

        return alpha * ownerAlpha;
    }

    //

    public void ChangeX()
    {
        if (ownerList)
            transform.localPosition = new Vector3(Get_X(), transform.localPosition.y, transform.localPosition.z);
        else
            transform.position = new Vector3(Get_X(), transform.position.y, transform.position.z);
    }

    public void ChangeY()
    {
        if (ownerList)
            transform.localPosition = new Vector3(transform.localPosition.x, Get_Y(), transform.localPosition.z);
        else
            transform.position = new Vector3(transform.position.x, Get_Y(), transform.position.z);
    }

    public void ChangeVisibility()
    {
        bool oldVis = visibility_Total;

        visibility_Total = GetTotalVisibility();

        if (oldVis != visibility_Total)
        {
            for (int i = 0; i < childs.Length; i++)
            {
                childs[i].ChangeVisibility();
            }
        }
    }

    public void ChangeAlpha()
    {
        float oldAlpha = alpha_Total;

        alpha_Total = GetTotalAlpha();

        for (int i = 0; i < childs.Length; i++)
        {
            childs[i].ChangeAlpha();
        }

        if ((oldAlpha == 0 && alpha_Total > 0) || (oldAlpha > 0 && alpha_Total == 0))
            ChangeVisibility();
    }

    //

    void AnimMutualInit(float _delay, AnimMode _animMode)
    {
        animMode = _animMode;

        anim_ShouldDoVisInit = true;

        animDelay = _delay;

        if (animMode == AnimMode.Show)
        {
            showingAnimStatus = GUIControlShowingAnimStatus.GoingShow;
        }
        else
        {
            showingAnimStatus = GUIControlShowingAnimStatus.GoingHidden;
        }
    }

    public void StartAnim_Alpha(float _delay, float _startAlpha, float _endAlpha, AnimationCurve _animCurve, float _TimeCounterSpeed, AnimMode _animMode)
    {
        AnimMutualInit(_delay, _animMode);

        for (int i = 0; i < childs.Length; i++)
        {
            childs[i].StartAnim_Alpha(_delay, _startAlpha, _endAlpha, _animCurve, _TimeCounterSpeed, _animMode);
        }
    }

    public void StartAnim_Y_Alpha(float _delay, float _startY, float _endY, AnimationCurve _animCurve, float _startAlpha, float _endAlpha, AnimationCurve _animCurve_Alpha, float _TimeCounterSpeed, AnimMode _animMode)
    {
        AnimMutualInit(_delay, _animMode);

        for (int i = 0; i < childs.Length; i++)
        {
            childs[i].StartAnim_Y_Alpha(_delay, _startY, _endY, _animCurve, _startAlpha, _endAlpha, _animCurve_Alpha, _TimeCounterSpeed, _animMode);
        }
    }

    public void StartAnim_Y(float _delay, float _startY, float _endY, AnimationCurve _animCurve, float _TimeCounterSpeed, AnimMode _animMode)
    {
        AnimMutualInit(_delay, _animMode);

        for (int i = 0; i < childs.Length; i++)
        {
            childs[i].StartAnim_Y(_delay, _startY, _endY, _animCurve, _TimeCounterSpeed, _animMode);
        }
    }

    public void StartAnim_X_Alpha(float _delay, float _startX, float _endX, AnimationCurve _animCurve, float _startAlpha, float _endAlpha, AnimationCurve _animCurve_Alpha, float _TimeCounterSpeed, AnimMode _animMode)
    {
        AnimMutualInit(_delay, _animMode);

        for (int i = 0; i < childs.Length; i++)
        {
            childs[i].StartAnim_X_Alpha(_delay, _startX, _endX, _animCurve, _startAlpha, _endAlpha, _animCurve_Alpha, _TimeCounterSpeed, _animMode);
        }
    }

    public void StartAnim_X(float _delay, float _startX, float _endX, AnimationCurve _animCurve, float _TimeCounterSpeed, AnimMode _animMode)
    {
        AnimMutualInit(_delay, _animMode);

        for (int i = 0; i < childs.Length; i++)
        {
            childs[i].StartAnim_X(_delay, _startX, _endX, _animCurve, _TimeCounterSpeed, _animMode);
        }
    }

    //
}
