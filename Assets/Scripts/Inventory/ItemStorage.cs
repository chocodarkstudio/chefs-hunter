using Items;
using System;
using System.Collections;
using UnityEngine.Events;


public class ItemStorage<T>
    where T : Item
{
    protected readonly T[] items;
    public int Count { get; private set; } = 0;

    public IEnumerable Enumerable => items;

    // Events
    public readonly UnityEvent onChange = new();


    public ItemStorage(int slotsCount)
    {
        if (slotsCount <= 0)
            throw new ArgumentException("slotsCount cannot be less than 1", nameof(slotsCount));

        items = new T[slotsCount];
    }

    public ItemStorage(T[] values)
    {
        if (values.Length <= 0)
            throw new ArgumentException("values.Length cannot be less than 1", nameof(values));

        items = values;
    }


    public void OnChange()
    {
        // event
        onChange.Invoke();
    }

    /// <summary>
    /// Finds an item by ID </summary>
    public T GetSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= items.Length)
            return default;

        if (IsEmpty())
            return default;

        return items[slotIndex];
    }

    /// <summary>
    /// Finds an item by ID </summary>
    public T Get(int itemID)
    {
        if (IsEmpty())
            return default;

        for (int i = 0; i < items.Length; i++)
        {
            T slot = items[i];
            if (slot == null)
                continue;

            if (slot.ID == itemID)
                return slot;
        }
        return default;
    }

    /// <summary>
    /// Finds an item by ID </summary>
    public T Get(T item) => Get(item.ID);

    /// <summary>
    /// Check if an item exists in the storage by ID </summary>
    public bool Has(int itemID) => Get(itemID) != null;

    /// <summary>
    /// Check if an item exists in the storage by ID </summary>
    public bool Has(T item) => Get(item.ID) != null;

    /// <summary>
    /// Check if an item reference exists in the storage </summary>
    public bool HasRef(T item)
    {
        if (item == null || IsEmpty())
            return false;

        for (int i = 0; i < items.Length; i++)
        {
            T slot = items[i];
            if (slot == null)
                continue;

            if (slot == item)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Check if the storage is full </summary>
    public bool IsFull() => items.Length > 0 && Count >= items.Length;

    /// <summary>
    /// Check if the storage is empty </summary>
    public bool IsEmpty() => Count <= 0;

    /// <summary>
    /// Adds an item reference to the storage </summary>
    public bool AddRef(T item)
    {
        if (item == null || IsFull())
            return false;

        for (int i = 0; i < items.Length; i++)
        {
            T slot = items[i];
            if (slot == null)
            {
                items[i] = item;
                Count++;
                OnChange();
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Adds a copy of the item to the storage </summary>
    public bool AddCopy(T item)
    {
        if (item == null)
            return false;

        return AddRef((T)item.Clone());
    }

    /// <summary>
    /// Removes an item by ID from the storage </summary>
    public bool Remove(int itemID)
    {
        for (int i = 0; i < items.Length; i++)
        {
            T slot = items[i];
            if (slot == null)
                continue;

            if (slot.ID == itemID)
            {
                items[i] = null;
                Count--;
                OnChange();
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Removes an item by ID from the storage </summary>
    public bool Remove(T item) => Remove(item.ID);

    /// <summary>
    /// Removes an item by reference from the storage </summary>
    public bool RemoveRef(T item)
    {
        for (int i = 0; i < items.Length; i++)
        {
            T slot = items[i];
            if (slot == item)
            {
                items[i] = null;
                Count--;
                OnChange();
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Set an item on the slot (can be null item) </summary>
    public void Set(int slotIndex, T item)
    {
        if (slotIndex < 0 || slotIndex >= items.Length)
            return;

        if (items[slotIndex] != null && item == null)
            Count--;
        else if (items[slotIndex] == null && item != null)
            Count++;

        items[slotIndex] = item;
        OnChange();
    }

    public T TakeSlot(int slotIndex)
    {
        T item = GetSlot(slotIndex);
        if (item == null)
            return default;

        Set(slotIndex, null);
        return item;
    }

    /// <summary>
    /// Swaps the slots items in this storage </summary>
    public bool InternalSwap(int slotIndex1, int slotIndex2)
    {
        if (slotIndex1 < 0 || slotIndex1 >= items.Length)
            return false;
        if (slotIndex2 < 0 || slotIndex2 >= items.Length)
            return false;

        // do swap
        (items[slotIndex2], items[slotIndex1]) = (items[slotIndex1], items[slotIndex2]);
        OnChange();
        return true;
    }
}
