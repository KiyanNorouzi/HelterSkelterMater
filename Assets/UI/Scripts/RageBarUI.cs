using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RageBarUI : MonoBehaviour
{
    public static RageBarUI Instance;

    public Image rageBar;

    float maxWidth = 100f;
    public float rage = 0;
    RectTransform rageBarParentRectTr;
    Vector2 rageStartPos;

    void Awake()
    {
        Instance = this;

        rageBarParentRectTr = rageBar.transform.parent.GetComponent<RectTransform>();
        maxWidth = rageBarParentRectTr.sizeDelta.x;
        rageStartPos = new Vector2(0, 0);
        ChangeRageVal(rage);
    }

    public void SetCurRage(float _rage)
    {
        ChangeRageVal(Mathf.Clamp(_rage, 0, 100));
    }

    void ChangeRageVal(float _rageVal)
    {
        rage = _rageVal;
        ChangeRageUI();
    }

    void ChangeRageUI()
    {
        rageBar.rectTransform.anchoredPosition = rageStartPos + new Vector2((rage / 100f) * maxWidth, rageBar.rectTransform.anchoredPosition.y);
    }
}
