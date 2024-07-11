using Items;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [field: SerializeField] public int MaxSlots { get; private set; } = 5;
    public ItemStorage<ItemIngredient> ingredientsStorage;

    private void Awake()
    {
        ingredientsStorage = new(MaxSlots);
        foreach (ItemIngredientObj ingredientObj in GameManager.IngredientObjs)
        {
            ingredientsStorage.AddCopy(ingredientObj.Item);
            if (ingredientsStorage.IsFull())
                break;
        }
    }
}
