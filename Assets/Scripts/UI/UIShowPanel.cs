using DG.Tweening;
using ScreenTransition;
using UIAnimShortcuts;
using UnityEngine;

public class UIShowPanel : MonoBehaviour
{
    [field: SerializeField] public GameObject MainPanel { get; protected set; }
    public bool ShowState { get; protected set; }
    protected Tween panelScaleTween;

    /// <summary>
    /// Instant apply show state </summary>
    public virtual void SetShow(bool show)
    {
        ShowState = show;
        MainPanel.SetActive(show);

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
    public virtual void Show(bool show)
    {
        // prevent run multiple animations
        if (panelScaleTween != null)
            panelScaleTween.Kill();

        // open anim
        if (show)
        {
            panelScaleTween = UIAnim.Scale(MainPanel.transform, TransitionState.Open);
            SetShow(true);
        }
        // close anim
        else
        {
            panelScaleTween = UIAnim.Scale(MainPanel.transform, TransitionState.Close,
                callback: OnCloseAnim);
        }
    }

    protected virtual void OnCloseAnim()
    {
        SetShow(false);
    }

}
