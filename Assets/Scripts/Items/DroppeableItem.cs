using DG.Tweening;
using Items;
using UnityEngine;

public class DroppeableItem : MonoBehaviour
{
    [field: SerializeField] public virtual Transform Visuals { get; protected set; }
    [field: SerializeField] public virtual SpriteRenderer SpriteRenderer { get; protected set; }

    [field: SerializeField] public virtual Item Item { get; protected set; }
    [SerializeField] ItemObj<ItemIngredient> itemObj;

    Tween spawnTween;

    private void Awake()
    {
        if (itemObj != null)
        {
            Item = itemObj.Item.Copy();
            UpdateItemData();
        }

    }

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

    /// <summary>
    ///  </summary>
    /// <param name="animPos">WorldSpace from where to start moving </param>
    public DroppeableItem SpawnAnim(Vector3 animPos)
    {
        if (!spawnTween.IsActive())
            spawnTween.Kill();

        Visuals.position = animPos;
        spawnTween = Visuals.DOLocalMove(Vector3.zero, 0.2f);
        return this;
    }
    public DroppeableItem SpawnAnim() => SpawnAnim(Visuals.position + Vector3.up * 0.5f);


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
