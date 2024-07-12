using DG.Tweening;
using Items;
using ScreenTransition;
using UIAnimShortcuts;
using UnityEngine;

public class UIInventoryPlayer : UIInventory<ItemIngredient>
{
    [SerializeField] GameObject mainPanel;
    [SerializeField] PlayerInventory playerInventory;

    public static bool ShowState { get; private set; }
    static Tween panelScaleTween;


    protected override void Start()
    {
        itemStorage = playerInventory.ingredientsStorage;
        base.Start();

        Show(true);
    }



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

}
