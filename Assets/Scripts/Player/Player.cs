using UnityEngine;

public class Player : MonoBehaviour
{
    [field: SerializeField] public PlayerInventory Inventory { get; private set; }


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out DroppeableItem droppeableItem))
        {
            droppeableItem.AddToStorage(Inventory.ingredientsStorage);
        }
    }
}
