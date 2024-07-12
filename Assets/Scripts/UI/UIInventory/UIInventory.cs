using Items;
using System.Collections.Generic;
using UIGridItems;
using UnityEngine;

public class UIInventory<T> : MonoBehaviour
    where T : Item
{
    [SerializeField] protected UIGridHandler gridHandler;
    protected ItemStorage<T> itemStorage;

    protected virtual void Awake()
    {
        gridHandler.onItemDropped.AddListener(OnItemDropped);
    }

    protected virtual void Start()
    {
        UpdateInventoryItems();
        itemStorage.onChange.AddListener(UpdateInventoryItems);
    }


    public virtual void UpdateInventoryItems()
    {
        if (itemStorage == null)
            return;

        List<GridItem> items = new();
        foreach (T item in itemStorage.Enumerable)
        {
            if (item == null)
            {
                // put empty slot
                items.Add(new());
                continue;
            }

            items.Add(new()
            {
                ID = item.ID.ToString(),
                Icon = item.Icon
            });
        }
        gridHandler.UpdateItems(items);
    }



    public virtual void SetSlotItem(UIGridItem slot, T newItem)
    {
        // no slot
        if (slot == null)
            return;

        // set new item
        int slotIndex = slot.transform.GetSiblingIndex() - 1;
        itemStorage.Set(slotIndex, newItem);
    }

    public virtual T GetSlotItem(UIGridItem slot)
    {
        // no slot
        if (slot == null || string.IsNullOrEmpty(slot.Item.ID))
            return null;

        T item = itemStorage.Get(int.Parse(slot.Item.ID));
        return item;
    }


    protected virtual void OnItemDropped(UIGridItem fromSlot, UIGridItem targetSlot)
    {
        // Same ItemsHandlers, same inventory
        if (fromSlot.ItemsHandler == targetSlot.ItemsHandler)
        {
            SwapInternalSlots(fromSlot, targetSlot);
        }
        // Different ItemsHandlers, two inventories
        else
        {
            SwapInventoriesSlots(fromSlot, targetSlot);
        }
    }


    public virtual void SwapInternalSlots(UIGridItem fromSlot, UIGridItem targetSlot)
    {
        // get slots indexes
        int fromSlotIndex = fromSlot.transform.GetSiblingIndex() - 1;
        int targetSlotIndex = targetSlot.transform.GetSiblingIndex() - 1;

        // swap
        itemStorage.InternalSwap(fromSlotIndex, targetSlotIndex);
    }

    protected virtual void SwapInventoriesSlots(UIGridItem fromSlot, UIGridItem targetSlot)
    {
        Transform targetParent = targetSlot.ItemsHandler.transform.parent;

        // Other UIInventory of the same item type
        if (!targetParent.TryGetComponent(out UIInventory<T> otherUIInventory))
            return;

        T myItem = this.GetSlotItem(fromSlot);
        T otherItem = otherUIInventory.GetSlotItem(targetSlot);

        otherUIInventory.SetSlotItem(targetSlot, myItem);
        this.SetSlotItem(fromSlot, otherItem);
    }

}
