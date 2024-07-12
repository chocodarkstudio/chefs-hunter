using UnityEngine;

namespace Items
{

    [System.Serializable]
    public class ItemWeapon : Item
    {
        [field: SerializeField] public ItemIngredientObj[] Attack { get; protected set; }
        [field: SerializeField] public ItemIngredientObj[] Defense { get; protected set; }
    }
}