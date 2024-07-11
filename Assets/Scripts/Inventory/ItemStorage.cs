using Items;
using System.Collections;
using UnityEngine.Events;


public class ItemStorage<T>
    where T : Item
{
    protected readonly T[] items;
    public int Count { get; private set; } = 0;

    public virtual int MaxSlots { get; private set; }

    public IEnumerable Enumerable => items;

    // Events
    public readonly UnityEvent onChange = new();


    public ItemStorage(int maxSlots = 0)
    {
        MaxSlots = maxSlots;
        items = new T[MaxSlots];
    }

    public void OnChange()
    {
        // event
        onChange.Invoke();
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
    public bool IsFull() => MaxSlots > 0 && Count >= MaxSlots;

    /// <summary>
    /// Check if the storage is empty </summary>
    public bool IsEmpty() => Count <= 0;

    /// <summary>
    /// Adds an item reference to the storage </summary>
    public void AddRef(T item)
    {
        if (item == null || IsFull())
            return;

        for (int i = 0; i < items.Length; i++)
        {
            T slot = items[i];
            if (slot == null)
            {
                items[i] = item;
                Count++;
                OnChange();
                return;
            }
        }
    }

    /// <summary>
    /// Adds a copy of the item to the storage </summary>
    public void AddCopy(T item)
    {
        if (item == null)
            return;

        AddRef((T)item.Clone());
    }

    /// <summary>
    /// Removes an item by ID from the storage </summary>
    public T Remove(int itemID)
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
                return slot;
            }
        }

        return default;
    }

    /// <summary>
    /// Removes an item by ID from the storage </summary>
    public T Remove(T item) => Remove(item.ID);

    /// <summary>
    /// Removes an item by reference from the storage </summary>
    public T RemoveRef(T item)
    {
        for (int i = 0; i < items.Length; i++)
        {
            T slot = items[i];
            if (slot == item)
            {
                items[i] = null;
                Count--;
                OnChange();
                return item;
            }
        }

        return default;
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
