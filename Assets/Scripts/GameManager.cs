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


    private void Awake()
    {
        Singleton = this;
        DontDestroyOnLoad(gameObject);
    }
}
