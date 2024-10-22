using DG.Tweening;
using Items;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [field: SerializeField] public virtual Transform Visuals { get; protected set; }
    [field: SerializeField] public virtual SpriteRenderer SpriteRenderer { get; protected set; }
    [field: SerializeField] public virtual SpriteRenderer NotTakeableIcon { get; protected set; }

    [field: SerializeField] public virtual Item Item { get; protected set; }
    [SerializeField] ItemObj<ItemIngredient> itemObj;
    [field: SerializeField] public virtual bool IsTakeable { get; set; } = true;
    [field: SerializeField] public virtual Timer DestroyTimer { get; protected set; }

    Tween spawnTween;

    private void Awake()
    {
        if (itemObj != null)
        {
            Item = itemObj.Get;
            UpdateItemData();
        }

        DestroyTimer.onCompleted.AddListener(() =>
        {
            Destroy(gameObject);
        });
        DestroyTimer.Restart();
    }

    private void Start()
    {
        if (!IsTakeable)
        {
            SpriteRenderer.color = new Color(1, 1, 1, 0.8f);
            NotTakeableIcon.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        DestroyTimer.Update();
    }

    public static ItemDrop CreateNew(Item item, Vector3 position)
    {
        // invalid item
        if (item == null)
            return null;

        GameObject gm = LevelLoader.CreateOnLevel(GameManager.ItemDropPrefab, position);
        if (!gm.TryGetComponent(out ItemDrop drop))
            return null;

        drop.Item = item.Copy();
        drop.UpdateItemData();
        return drop;
    }

    /// <summary>
    ///  </summary>
    /// <param name="animPos">WorldSpace from where to start moving </param>
    public ItemDrop SpawnAnim(Vector3 animPos)
    {
        if (!spawnTween.IsActive())
            spawnTween.Kill();

        Visuals.position = animPos;
        spawnTween = Visuals.DOLocalJump(
            endValue: Vector3.zero,
            jumpPower: 0.5f,
            numJumps: 1,
            duration: 0.3f
        );
        return this;
    }
    public ItemDrop SpawnAnim() => SpawnAnim(Visuals.position + Vector3.up * 0.5f);


    public void UpdateItemData()
    {
        // invalid item
        if (Item == null)
            return;

        SpriteRenderer.sprite = Item.Icon;
    }

    /// <summary>
    /// Adds the item to the storage and destroy the drop </summary>
    public void AddToStorage<T>(ItemStorage<T> itemStorage)
        where T : Item
    {
        bool added = itemStorage.AddRef((T)Item);

        if (added)
            DestroyDrop();
    }

    public void DestroyDrop()
    {
        Destroy(gameObject);
    }
}
