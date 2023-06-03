using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PageName
{
    Nothing,
    Today,
}

public enum SubPageName
{
    Nothing,
    Food,
    Exercise,
    Info,
}

public enum AppStep
{
    Nothing,
    Today_Init,
    Today_Update,
}

public class AppLogic : MonoBehaviour
{
    public GUIGroup Today;

    public int TodayListsMaxCountToNotHidePageBG = 3;

    public GUIList Today_Food_List;
    public GUIControl Today_Food_Pic_PageBG;
    public GUIControl Today_Food_Btn_Top;
    public GUIControl Today_Food_Pic_Icon;
    public GUIGroup Today_Food_TitleGroup;
    public GUIControl Today_Food_Pic_TitleGroupBG;
    public GUIControl Today_Food_Btn_Add;

    public GUIList Today_Exercises_List;
    public GUIControl Today_Exercise_Pic_PageBG;
    public GUIControl Today_Exercise_Btn_Top;
    public GUIControl Today_Exercise_Pic_Icon;
    public GUIGroup Today_Exercise_TitleGroup;
    public GUIControl Today_Exercise_Pic_TitleGroupBG;

    public GUIControl Today_Info_Pic_PageBG;
    public GUIControl Today_Info_Btn_Top;
    public GUIControl Today_Info_Pic_Icon;
    public GUIGroup Today_Info_TitleGroup;
    public GUIControl Today_Info_Pic_TitleGroupBG;

    public GUIControl[] Today_Food_Page_Controls;
    public GUIList[] Today_Food_Page_Lists;
    public GUIControl[] Today_Exercise_Page_Controls;
    public GUIList[] Today_Exercise_Page_Lists;
    public GUIControl[] Today_Info_Page_Controls;
    public GUIList[] Today_Info_Page_Lists;

    public float Today_SubpageChange_Anim_Speed_Button = 2;
    public float Today_SubpageChange_Anim_Speed_ButtonLabel = 4;
    public float Today_SubpageChange_Anim_MinScale = 1;
    public float Today_SubpageChange_Anim_MaxScale = 2;

    //

    float hidingBackBGSpeed = 2f;

    //

    public static AppLogic Instance;

    //

    AppStep step = AppStep.Nothing;

    PageName curPage = PageName.Nothing;

    SubPageName curSubPage = SubPageName.Nothing;

    //

    bool tapHappened = false;
    bool fingerOnCtrlUpHappened = false;

    GUIGroup lastTouch_GuiGroup = null;
    GUIControl lastTouch_GuiControl = null;
    TouchUpInfo lastTouch_TouchUpInfo;

    bool isSubPageChanging = false;
    SubPageName newNeededSubpageName = SubPageName.Nothing;

    //

    GUIControl Today_CurBtn = null;
    GUIGroup Today_CurBtn_TitleGroup = null;
    GUIControl Today_CurBtn_TitleGroup_BG = null;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        #region Nothing
        if (IsStep(AppStep.Nothing))
        {
            SetStep(AppStep.Today_Init);
            SetCurPage(PageName.Today);

            TodayPage_StartSubPage(SubPageName.Food);
        }
        #endregion

        #region Today_Init
        if (IsStep(AppStep.Today_Init))
        {
            SetStep(AppStep.Today_Update);
        }
        #endregion

        #region Today_Update
        if (IsStep(AppStep.Today_Update))
        {
            if (Today_Food_List.childs.Count > TodayListsMaxCountToNotHidePageBG)
            {
                if (Today_Food_Pic_PageBG.alpha > 0)
                {
                    Today_Food_Pic_PageBG.SetAlpha(Today_Food_Pic_PageBG.alpha - hidingBackBGSpeed * Time.deltaTime);
                    Today_Food_Pic_PageBG.ChangeAlpha();
                }
            }
            else
            {
                if (Today_Food_Pic_PageBG.alpha < 1)
                {
                    Today_Food_Pic_PageBG.SetAlpha(Today_Food_Pic_PageBG.alpha + hidingBackBGSpeed * Time.deltaTime);
                    Today_Food_Pic_PageBG.ChangeAlpha();
                }
            }

            if (isSubPageChanging)
            {
                if (Today_CurBtn.showingAnimStatus == GUIControlShowingAnimStatus.Show)
                {
                    isSubPageChanging = false;

                    SetCurSubPage(newNeededSubpageName);
                }
            }
            else
            {
                if (tapHappened)
                {
                    if (lastTouch_GuiControl == Today_Food_Btn_Top)
                    {
                        #region MyRegion
                        if (curSubPage != SubPageName.Food)
                        {
                            if (curSubPage == SubPageName.Exercise)
                            {
                                for (int i = 0; i < Today_Exercise_Page_Controls.Length; i++)
                                {
                                    Today_Exercise_Page_Controls[i].StartAnim_X_Alpha(0, 0, 1.5f * MyGUIController.Instance.defScreen_Width, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Hide, 1, 0, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Hide, Today_SubpageChange_Anim_Speed_Button, AnimMode.Hide);
                                }

                                for (int i = 0; i < Today_Exercise_Page_Lists.Length; i++)
                                {
                                    Today_Exercise_Page_Lists[i].StartAnim_X_Alpha(0, 0, 1.5f * MyGUIController.Instance.defScreen_Width, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Hide, 1, 0, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Hide, Today_SubpageChange_Anim_Speed_Button, AnimMode.Hide);
                                }
                            }

                            if (curSubPage == SubPageName.Info)
                            {
                                for (int i = 0; i < Today_Info_Page_Controls.Length; i++)
                                {
                                    Today_Info_Page_Controls[i].StartAnim_X_Alpha(0, 0, 1.5f * MyGUIController.Instance.defScreen_Width, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Hide, 1, 0, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Hide, Today_SubpageChange_Anim_Speed_Button, AnimMode.Hide);
                                }

                                for (int i = 0; i < Today_Info_Page_Lists.Length; i++)
                                {
                                    Today_Info_Page_Lists[i].StartAnim_X_Alpha(0, 0, 1.5f * MyGUIController.Instance.defScreen_Width, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Hide, 1, 0, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Hide, Today_SubpageChange_Anim_Speed_Button, AnimMode.Hide);
                                }
                            }

                            //
                            //

                            for (int i = 0; i < Today_Food_Page_Controls.Length; i++)
                            {
                                Today_Food_Page_Controls[i].StartAnim_X_Alpha(0, -1.5f * MyGUIController.Instance.defScreen_Width, 0, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Show, 0, 1, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Show, Today_SubpageChange_Anim_Speed_Button, AnimMode.Show);
                            }

                            for (int i = 0; i < Today_Food_Page_Lists.Length; i++)
                            {
                                Today_Food_Page_Lists[i].StartAnim_X_Alpha(0, -1.5f * MyGUIController.Instance.defScreen_Width, 0, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Show, 0, 1, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Show, Today_SubpageChange_Anim_Speed_Button, AnimMode.Show);
                            }

                            //
                            //

                            TodayPage_StartSubPage(SubPageName.Food);
                            SetTouchesChecked();
                        }
                    }

                    if (lastTouch_GuiControl == Today_Exercise_Btn_Top)
                    {
                        if (curSubPage != SubPageName.Exercise)
                        {
                            if (curSubPage == SubPageName.Food)
                            {
                                for (int i = 0; i < Today_Food_Page_Controls.Length; i++)
                                {
                                    Today_Food_Page_Controls[i].StartAnim_X_Alpha(0, 0, -1.5f * MyGUIController.Instance.defScreen_Width, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Hide, 1, 0, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Hide, Today_SubpageChange_Anim_Speed_Button, AnimMode.Hide);
                                }

                                for (int i = 0; i < Today_Food_Page_Lists.Length; i++)
                                {
                                    Today_Food_Page_Lists[i].StartAnim_X_Alpha(0, 0, -1.5f * MyGUIController.Instance.defScreen_Width, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Hide, 1, 0, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Hide, Today_SubpageChange_Anim_Speed_Button, AnimMode.Hide);
                                }
                            }

                            if (curSubPage == SubPageName.Info)
                            {
                                for (int i = 0; i < Today_Info_Page_Controls.Length; i++)
                                {
                                    Today_Info_Page_Controls[i].StartAnim_X_Alpha(0, 0, 1.5f * MyGUIController.Instance.defScreen_Width, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Hide, 1, 0, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Hide, Today_SubpageChange_Anim_Speed_Button, AnimMode.Hide);
                                }

                                for (int i = 0; i < Today_Info_Page_Lists.Length; i++)
                                {
                                    Today_Info_Page_Lists[i].StartAnim_X_Alpha(0, 0, 1.5f * MyGUIController.Instance.defScreen_Width, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Hide, 1, 0, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Hide, Today_SubpageChange_Anim_Speed_Button, AnimMode.Hide);
                                }
                            }

                            //
                            //

                            if (curSubPage == SubPageName.Food)
                            {
                                for (int i = 0; i < Today_Exercise_Page_Controls.Length; i++)
                                {
                                    Today_Exercise_Page_Controls[i].StartAnim_X_Alpha(0, 1.5f * MyGUIController.Instance.defScreen_Width, 0, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Show, 0, 1, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Show, Today_SubpageChange_Anim_Speed_Button, AnimMode.Show);
                                }

                                for (int i = 0; i < Today_Exercise_Page_Lists.Length; i++)
                                {
                                    Today_Exercise_Page_Lists[i].StartAnim_X_Alpha(0, 1.5f * MyGUIController.Instance.defScreen_Width, 0, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Show, 0, 1, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Show, Today_SubpageChange_Anim_Speed_Button, AnimMode.Show);
                                }
                            }

                            if (curSubPage == SubPageName.Info)
                            {
                                for (int i = 0; i < Today_Exercise_Page_Controls.Length; i++)
                                {
                                    Today_Exercise_Page_Controls[i].StartAnim_X_Alpha(0, -1.5f * MyGUIController.Instance.defScreen_Width, 0, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Show, 0, 1, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Show, Today_SubpageChange_Anim_Speed_Button, AnimMode.Show);
                                }

                                for (int i = 0; i < Today_Exercise_Page_Lists.Length; i++)
                                {
                                    Today_Exercise_Page_Lists[i].StartAnim_X_Alpha(0, -1.5f * MyGUIController.Instance.defScreen_Width, 0, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Show, 0, 1, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Show, Today_SubpageChange_Anim_Speed_Button, AnimMode.Show);
                                }
                            }

                            //
                            //

                            TodayPage_StartSubPage(SubPageName.Exercise);
                            SetTouchesChecked();
                        }
                    }

                    if (lastTouch_GuiControl == Today_Info_Btn_Top)
                    {
                        if (curSubPage != SubPageName.Info)
                        {
                            if (curSubPage == SubPageName.Exercise)
                            {
                                for (int i = 0; i < Today_Exercise_Page_Controls.Length; i++)
                                {
                                    Today_Exercise_Page_Controls[i].StartAnim_X_Alpha(0, 0, -1.5f * MyGUIController.Instance.defScreen_Width, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Hide, 1, 0, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Hide, Today_SubpageChange_Anim_Speed_Button, AnimMode.Hide);
                                }

                                for (int i = 0; i < Today_Exercise_Page_Lists.Length; i++)
                                {
                                    Today_Exercise_Page_Lists[i].StartAnim_X_Alpha(0, 0, -1.5f * MyGUIController.Instance.defScreen_Width, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Hide, 1, 0, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Hide, Today_SubpageChange_Anim_Speed_Button, AnimMode.Hide);
                                }
                            }

                            if (curSubPage == SubPageName.Food)
                            {
                                for (int i = 0; i < Today_Food_Page_Controls.Length; i++)
                                {
                                    Today_Food_Page_Controls[i].StartAnim_X_Alpha(0, 0, -1.5f * MyGUIController.Instance.defScreen_Width, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Hide, 1, 0, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Hide, Today_SubpageChange_Anim_Speed_Button, AnimMode.Hide);
                                }

                                for (int i = 0; i < Today_Food_Page_Lists.Length; i++)
                                {
                                    Today_Food_Page_Lists[i].StartAnim_X_Alpha(0, 0, -1.5f * MyGUIController.Instance.defScreen_Width, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Hide, 1, 0, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Hide, Today_SubpageChange_Anim_Speed_Button, AnimMode.Hide);
                                }
                            }

                            //
                            //

                            for (int i = 0; i < Today_Info_Page_Controls.Length; i++)
                            {
                                Today_Info_Page_Controls[i].StartAnim_X_Alpha(0, 1.5f * MyGUIController.Instance.defScreen_Width, 0, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Show, 0, 1, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Show, Today_SubpageChange_Anim_Speed_Button, AnimMode.Show);
                            }

                            for (int i = 0; i < Today_Info_Page_Lists.Length; i++)
                            {
                                Today_Info_Page_Lists[i].StartAnim_X_Alpha(0, 1.5f * MyGUIController.Instance.defScreen_Width, 0, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Show, 0, 1, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Show, Today_SubpageChange_Anim_Speed_Button, AnimMode.Show);
                            }

                            //
                            //

                            TodayPage_StartSubPage(SubPageName.Info);
                            SetTouchesChecked();
                        }
                        #endregion
                    }

                    if (lastTouch_GuiControl == Today_Food_Btn_Add)
                    {
                        Today_Food_List.AddItem(Today_Food_List.sourceItem, Today_Food_List.childs.Count, true, true);
                    }
                }
            }
        }
        #endregion

        SetTouchesChecked();
    }

    #region Touch and Tap Events

    public void TapHappened(GUIGroup _guiGroup, GUIControl _guiControl, TouchUpInfo _touchUpInfo)
    {
        GUIGroup guiGroup = _guiGroup;
        GUIControl guiControl = _guiControl;
        TouchUpInfo touchUpInfo = _touchUpInfo;

        lastTouch_GuiGroup = guiGroup;
        lastTouch_GuiControl = guiControl;
        lastTouch_TouchUpInfo = touchUpInfo;

        string groupName = guiGroup.myName;
        string controlName = guiControl.myName;

        tapHappened = true;
    }

    public void TouchUpHappened(GUIGroup _guiGroup, GUIControl _guiControl, TouchUpInfo _touchUpInfo)
    {
        GUIGroup guiGroup = _guiGroup;
        GUIControl guiControl = _guiControl;
        TouchUpInfo touchUpInfo = _touchUpInfo;

        lastTouch_GuiGroup = guiGroup;
        lastTouch_GuiControl = guiControl;
        lastTouch_TouchUpInfo = touchUpInfo;

        string groupName = guiGroup.myName;
        string controlName = guiControl.myName;

        fingerOnCtrlUpHappened = true;
    }

    #endregion

    #region App Logic

    void SetStep(AppStep _val)
    {
        step = _val;
    }

    bool IsStep(AppStep _val)
    {
        return step == _val;
    }

    void SetCurPage(PageName _val)
    {
        curPage = _val;
    }

    bool IsCurPage(PageName _val)
    {
        return curPage == _val;
    }

    void SetCurSubPage(SubPageName _val)
    {
        curSubPage = _val;
    }

    bool IsCurSubPage(SubPageName _val)
    {
        return curSubPage == _val;
    }

    void SetTouchesChecked()
    {
        tapHappened = false;
        fingerOnCtrlUpHappened = false;
    }

    //

    void TodayPage_StartSubPage(SubPageName _subpage)
    {
        SubPageName spName = _subpage;

        GUIControl oldBtn = Today_CurBtn;
        GUIGroup oldGroup = Today_CurBtn_TitleGroup;
        GUIControl oldGroup_Ctrl_BG = Today_CurBtn_TitleGroup_BG;

        GUIControl newBtn = null;
        GUIGroup newGroup = null;
        GUIControl newGroup_Ctrl_BG = null;

        switch (spName)
        {
            case SubPageName.Food:
                newBtn = Today_Food_Pic_Icon;
                newGroup = Today_Food_TitleGroup;
                newGroup_Ctrl_BG = Today_Food_Pic_TitleGroupBG;
                break;

            case SubPageName.Exercise:
                newBtn = Today_Exercise_Pic_Icon;
                newGroup = Today_Exercise_TitleGroup;
                newGroup_Ctrl_BG = Today_Exercise_Pic_TitleGroupBG;
                break;

            case SubPageName.Info:
                newBtn = Today_Info_Pic_Icon;
                newGroup = Today_Info_TitleGroup;
                newGroup_Ctrl_BG = Today_Info_Pic_TitleGroupBG;
                break;
        }

        if (oldBtn)
        {
            oldBtn.StartAnim_WHScale(0, Today_SubpageChange_Anim_MaxScale, Today_SubpageChange_Anim_MinScale, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Hide, Today_SubpageChange_Anim_Speed_Button, AnimMode.Hide);
            oldGroup.StartAnim_Y_Alpha(0, -oldGroup_Ctrl_BG.h, 0.1f * oldGroup_Ctrl_BG.h, MyGUIController.Instance.animCurve_Keybord_Y_Hide, 1, 0, MyGUIController.Instance.animCurve_Keybord_Y_Hide, Today_SubpageChange_Anim_Speed_ButtonLabel, AnimMode.Hide);
        }

        newBtn.StartAnim_WHScale(0, Today_SubpageChange_Anim_MinScale, Today_SubpageChange_Anim_MaxScale, MyGUIController.Instance.animCurve_Dem3ButtonsWHScale_Show, Today_SubpageChange_Anim_Speed_Button, AnimMode.Show);
        newGroup.StartAnim_Y_Alpha(1 / Today_SubpageChange_Anim_Speed_ButtonLabel, 0.1f * newGroup_Ctrl_BG.h, -newGroup_Ctrl_BG.h, MyGUIController.Instance.animCurve_Keybord_Y_Show, 0, 1, MyGUIController.Instance.animCurve_Keybord_Y_Show, Today_SubpageChange_Anim_Speed_ButtonLabel, AnimMode.Show);

        Today_CurBtn = newBtn;
        Today_CurBtn_TitleGroup = newGroup;
        Today_CurBtn_TitleGroup_BG = newGroup_Ctrl_BG;

        isSubPageChanging = true;
        newNeededSubpageName = spName;
    }

    #endregion
}

