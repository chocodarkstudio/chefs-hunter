using DG.Tweening;
using ScreenTransition;
using UIAnimShortcuts;
using UnityEngine;
using UnityEngine.UI;

public class UIGameTimer : MonoBehaviour
{
    [SerializeField] GameObject mainPanel;

    [SerializeField] Image timerIcon;
    [SerializeField] RectTransform timerArrow;

    [SerializeField] Sprite[] timerIconVariants;

    public bool ShowState { get; private set; }
    Tween panelScaleTween;


    /// <summary>
    /// Instant apply show state </summary>
    public void SetShow(bool show)
    {
        ShowState = show;
        mainPanel.SetActive(show);

        // on show
        if (show)
        {

        }
        // on hide
        else
        {

        }
    }

    /// <summary>
    /// Show state with open/close animation </summary>
    public void Show(bool show)
    {
        // prevent run multiple animations
        if (panelScaleTween != null)
            panelScaleTween.Kill();

        // open anim
        if (show)
        {
            panelScaleTween = UIAnim.Scale(mainPanel.transform, TransitionState.Open);
            SetShow(true);
        }
        // close anim
        else
        {
            panelScaleTween = UIAnim.Scale(mainPanel.transform, TransitionState.Close,
                callback: OnCloseAnim);
        }
    }

    void OnCloseAnim()
    {
        SetShow(false);
    }


    public void RotateByPercent(float percent)
    {
        float rotation = -360 * percent;
        timerArrow.localEulerAngles = new Vector3(0, 0, rotation);


        // update timer variant
        if (percent >= 0.8f)
        {
            SetTimerVariant(2);
        }
        else if (percent >= 0.5f)
        {
            SetTimerVariant(1);
        }
        else
        {
            SetTimerVariant(0);
        }
    }

    public void SetTimerVariant(int index)
    {
        if (index < 0 || index >= timerIconVariants.Length)
            if (timerIcon.sprite == timerIconVariants[index])
                return;

        timerIcon.sprite = timerIconVariants[index];
    }
}
