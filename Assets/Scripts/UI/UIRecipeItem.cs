using Items;
using UIItem_NM;
using UnityEngine;

public class UIRecipeItem : MonoBehaviour
{
    public ItemRecipe ItemRecipe { get; protected set; }

    [SerializeField] UIItem[] ingredientsUIItems;
    [SerializeField] UIItem resultUIItems;


    public void UpdateRecipe(ItemRecipe itemRecipe)
    {
        ItemRecipe = itemRecipe;

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
}
