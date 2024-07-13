using DG.Tweening;
using Items;
using ScreenTransition;
using UIAnimShortcuts;
using UIGridItems;
using UnityEngine;
using UnityEngine.Events;

public class UIWeaponSelector : UIInventory<ItemWeapon>
{
    [SerializeField] GameObject mainPanel;

    public bool ShowState { get; private set; }
    Tween panelScaleTween;

    // events
    public readonly UnityEvent<ItemWeapon> onWeaponSelected = new();

    protected override void Awake()
    {
        base.Awake();
        SetShow(false);
    }

    protected override void Start()
    {
        itemStorage = new(GameManager.WeaponObjs.Length);
        foreach (var weaponObj in GameManager.WeaponObjs)
        {
            itemStorage.AddCopy(weaponObj.Item);
        }

        base.Start();

        gridHandler.onItemClick.AddListener(OnItemClick);
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


    void OnItemClick(UIGridItem slot)
    {
        ItemWeapon weapon = GetSlotItem(slot);
        onWeaponSelected.Invoke(weapon.Copy());
    }
}
