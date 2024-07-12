using Items;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [field: SerializeField] public ItemIngredientObj Type { get; protected set; }
    [field: SerializeField] public ItemIngredientObj[] Drops { get; protected set; }

    [field: SerializeField] public ItemWeaponObj[] WeaknessesWeapons { get; protected set; }
}
