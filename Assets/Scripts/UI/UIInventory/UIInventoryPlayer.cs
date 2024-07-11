using Items;
using UnityEngine;

public class UIInventoryPlayer : UIInventory<ItemIngredient>
{
    [SerializeField] PlayerInventory playerInventory;

    protected override void Start()
    {
        itemStorage = playerInventory.ingredientsStorage;
        base.Start();
    }

}
