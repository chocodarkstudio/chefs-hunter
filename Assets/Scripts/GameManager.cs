using Combat_NM;
using Items;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager Singleton;

    [Header("References")]
    [SerializeField] CombatManager combatManager;
    public static CombatManager CombatManager => Singleton.combatManager;

    [SerializeField] Transform uiTopLevel;
    public static Transform UITopLevel => Singleton.uiTopLevel;

    [SerializeField] ItemIngredientObj[] ingredientObjs;
    public static IEnumerable IngredientObjs => Singleton.ingredientObjs;

    [SerializeField] ItemRecipeObj[] recipeObjs;
    public static IEnumerable RecipeObjs => Singleton.recipeObjs;

    [SerializeField] ItemWeaponObj[] weaponObjs;
    public static IEnumerable WeaponObjs => Singleton.weaponObjs;


    [Header("Prefabs")]
    [SerializeField] GameObject droppeableItemPrefab;
    public static GameObject DroppeableItemPrefab => Singleton.droppeableItemPrefab;


    private void Awake()
    {
        Singleton = this;
        DontDestroyOnLoad(gameObject);
    }
}
