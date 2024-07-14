using Items;
using System.Collections.Generic;
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ItemIngredient item = ingredientsStorage.TakeSlot(0);
            DroppeableItem.CreateNew(item, transform.position + Vector3.forward * 1)
                .SpawnAnim(transform.position);
        }

    }

    public void DiscardPlayerFirstItems(int amount)
    {
        if (amount <= 0)
            return;

        List<ItemIngredient> discardedItems = new();

        foreach (ItemIngredient ingredient in ingredientsStorage.All)
        {
            if (ingredient == null)
                continue;

            discardedItems.Add(ingredient);
            if (discardedItems.Count >= amount)
                break;
        }

        foreach (ItemIngredient ingredient in discardedItems)
        {
            ingredientsStorage.RemoveRef(ingredient);
        }
    }

    public void DiscardPlayerRandomItems(int amount)
    {
        if (amount <= 0)
            return;
    }
}
