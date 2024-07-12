using Items;
using UnityEngine;

public class DroppeableItem : MonoBehaviour
{
    [field: SerializeField] public virtual Item Item { get; protected set; }
    [field: SerializeField] public virtual SpriteRenderer SpriteRenderer { get; protected set; }

    public static DroppeableItem CreateNew(Item item, Vector3 position)
    {
        // invalid item
        if (item == null)
            return null;

        GameObject gm = Instantiate(GameManager.DroppeableItemPrefab);
        if (!gm.TryGetComponent(out DroppeableItem droppeable))
            return null;

        droppeable.transform.position = position;
        droppeable.Item = item.Copy();
        droppeable.UpdateItemData();
        return droppeable;
    }

    public void UpdateItemData()
    {
        // invalid item
        if (Item == null)
            return;

        SpriteRenderer.sprite = Item.Icon;
    }

    /// <summary>
    /// Adds the item to the storage and destroy the droppeable </summary>
    public void AddToStorage<T>(ItemStorage<T> itemStorage)
        where T : Item
    {
        bool added = itemStorage.AddRef((T)Item);

        if (added)
            DestroyDroppeable();
    }

    public void DestroyDroppeable()
    {
        Destroy(gameObject);
    }
}
