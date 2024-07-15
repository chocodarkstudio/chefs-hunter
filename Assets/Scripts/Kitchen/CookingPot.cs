using Items;
using System.Linq;
using UnityEngine;

public class CookingPot : MonoBehaviour
{

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out Player player))
        {
            CompleteMatchingRecipes(player.Inventory.ingredientsStorage);
        }
    }

    public void CompleteMatchingRecipes(ItemStorage<ItemIngredient> storage)
    {
        if (GameManager.OrderCounter == null)
            return;

        foreach (Customer customer in GameManager.OrderCounter.Customers)
        {
            // customer is not ordering
            if (!customer.IsOrdering)
                continue;

            // check if player has the ingredients to complete the customer order
            bool hasIngredients = HasRecipeIngredients(storage, customer.Order);
            if (!hasIngredients)
                continue;

            // remove ingredients from player inventory
            customer.Order.Ingredients.ForEach((ingredient) => storage.Remove(ingredient.Item));

            // complete the order
            GameManager.OrderCounter.CompleteCustomer(customer);
        }
    }

    public bool HasRecipeIngredients(ItemStorage<ItemIngredient> storage, ItemRecipe recipe)
    {
        return recipe.Ingredients.All((ingredient) => storage.Has(ingredient.Item));
    }
}
