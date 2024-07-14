using Items;
using UnityEngine;

public class KitchenTableInventory : MonoBehaviour
{
    [SerializeField] UIInventoryKitchenTable uiInventoryKitchen;
    [field: SerializeField] public int MaxSlots { get; private set; } = 2;
    public ItemStorage<ItemIngredient> ingredientsStorage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            uiInventoryKitchen.Show(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            uiInventoryKitchen.Show(false);
        }
    }

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
