using Items;
using UnityEngine;

public class UIInventoryKitchenTable : UIInventory<ItemIngredient>
{
    [SerializeField] KitchenTableInventory kitchenTableInventory;

    protected override void Start()
    {
        itemStorage = kitchenTableInventory.ingredientsStorage;
        base.Start();
    }
}
