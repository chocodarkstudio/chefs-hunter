using Items;
using UnityEngine;

public class KitchenTableInventory : MonoBehaviour
{
    [SerializeField] GameObject uiInventoryKitchenPrefab;
    UIInventoryKitchenTable uiInventoryKitchen;

    [field: SerializeField] public int MaxSlots { get; private set; } = 2;
    public ItemStorage<ItemIngredient> ingredientsStorage;


    private void Awake()
    {
        uiInventoryKitchen = GameManager.CreateOnUI<UIInventoryKitchenTable>(uiInventoryKitchenPrefab);
        uiInventoryKitchen.kitchenTableInventory = this;
        uiInventoryKitchen.Show(true);


        ingredientsStorage = new(MaxSlots);
        foreach (ItemIngredientObj ingredientObj in GameManager.IngredientObjs)
        {
            ingredientsStorage.AddCopy(ingredientObj.Item);
            if (ingredientsStorage.IsFull())
                break;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        return;
        if (other.TryGetComponent(out Player player))
        {
            uiInventoryKitchen.Show(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        return;
        if (other.TryGetComponent(out Player player))
        {
            uiInventoryKitchen.Show(false);
        }
    }

}
