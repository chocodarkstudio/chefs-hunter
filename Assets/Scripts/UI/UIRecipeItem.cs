using DG.Tweening;
using Items;
using ScreenTransition;
using UIAnimShortcuts;
using UIItem_NM;
using UnityEngine;

public class UIRecipeItem : MonoBehaviour
{
    [SerializeField] GameObject mainPanel;

    /// <summary>
    /// Can be null to show empty </summary>
    public ItemRecipe ItemRecipe { get; protected set; }

    [SerializeField] UIItem[] ingredientsUIItems;
    [SerializeField] UIItem resultUIItems;


    public bool ShowState { get; private set; }
    Tween panelScaleTween;

    public void UpdateRecipe(ItemRecipe itemRecipe)
    {
        ItemRecipe = itemRecipe;

        // invalid item
        if (itemRecipe == null)
        {
            for (int i = 0; i < ingredientsUIItems.Length; i++)
            {
                ingredientsUIItems[i].UpdateItem(new());
            }

            resultUIItems.UpdateItem(new());
            return;
        }

        for (int i = 0; i < ItemRecipe.Ingredients.Count; i++)
        {
            ItemIngredientObj ingredientObj = ItemRecipe.Ingredients[i];
            ingredientsUIItems[i].UpdateItem(new()
            {
                ID = ingredientObj.Item.ID.ToString(),
                Icon = ingredientObj.Item.Icon
            });
        }

        resultUIItems.UpdateItem(new()
        {
            ID = ItemRecipe.ID.ToString(),
            Icon = ItemRecipe.Icon
        });
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
