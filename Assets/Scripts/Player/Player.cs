using UnityEngine;

public class Player : MonoBehaviour
{
    [field: SerializeField] public UIInventoryPlayer UIInventoryPlayer { get; private set; }
    [field: SerializeField] public PlayerInventory Inventory { get; private set; }

    private void Awake()
    {
        PlayerTeleporter.onPlayerTeleport.AddListener(OnTeleport);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ItemDrop droppeableItem))
        {
            if (droppeableItem.IsTakeable)
                droppeableItem.AddToStorage(Inventory.ingredientsStorage);
        }
    }

    void OnTeleport(string teleportName)
    {
        UIInventoryPlayer.UIFollow.UpdatePosition();
    }
}
