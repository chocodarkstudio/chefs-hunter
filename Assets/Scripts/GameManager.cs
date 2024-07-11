using Items;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager Singleton;

    [SerializeField] ItemIngredientObj[] ingredientObjs;
    [SerializeField] ItemRecipeObj[] recipeObjs;

    public static IEnumerable IngredientObjs => Singleton.ingredientObjs;
    public static IEnumerable RecipeObjs => Singleton.recipeObjs;

    // Prefabs
    [SerializeField] GameObject droppeableItemPrefab;
    public static GameObject DroppeableItemPrefab => Singleton.droppeableItemPrefab;


    private void Awake()
    {
        Singleton = this;
        DontDestroyOnLoad(gameObject);
    }
}
