using DG.Tweening;
using Items;
using ScreenTransition;
using UIAnimShortcuts;
using UnityEngine;

public class UIInventoryKitchenTable : UIInventory<ItemIngredient>
{
    [SerializeField] GameObject mainPanel;
    public KitchenTableInventory kitchenTableInventory;


    public bool ShowState { get; private set; }
    Tween panelScaleTween;


    protected override void Start()
    {
        itemStorage = kitchenTableInventory.ingredientsStorage;
        base.Start();

        if (TryGetComponent(out UIFollowWorldTarget uiFollow))
        {
            uiFollow.SetFollowTarget(kitchenTableInventory.transform);
            uiFollow.StartFollow();
        }
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
