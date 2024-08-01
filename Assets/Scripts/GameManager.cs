using CombatSystem;
using Items;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager Singleton;

    [Header("UI References")]
    [SerializeField] Transform canvas;
    public static Transform Canvas => Singleton.canvas;

    [SerializeField] Transform uiTopLevel;
    public static Transform UITopLevel => Singleton.uiTopLevel;
    [SerializeField] UIMainMenu mainMenu;

    [Header("Scene References")]
    [SerializeField] CombatManager combatManager;
    public static CombatManager CombatManager => Singleton.combatManager;

    [SerializeField] UICombatTutorial combatTutorial;
    public static UICombatTutorial CombatTutorial => Singleton.combatTutorial;

    [SerializeField] OrderCounter orderCounter;
    public static OrderCounter OrderCounter => Singleton.orderCounter;
    [SerializeField] GameSequencer gameSequencer;
    public static GameSequencer GameSequencer => Singleton.gameSequencer;

    [Header("Assets References")]
    [SerializeField] ItemIngredientObj[] ingredientObjs;
    public static ItemIngredientObj[] IngredientObjs => Singleton.ingredientObjs;

    [SerializeField] ItemRecipeObj[] recipeObjs;
    public static ItemRecipeObj[] RecipeObjs => Singleton.recipeObjs;

    [SerializeField] ItemWeaponObj[] weaponObjs;
    public static ItemWeaponObj[] WeaponObjs => Singleton.weaponObjs;


    [Header("Prefabs")]
    [SerializeField] GameObject itemDropPrefab;
    public static GameObject ItemDropPrefab => Singleton.itemDropPrefab;


    public static GameObject CreateOnUI(GameObject prefab, bool firstSibling = true)
    {
        GameObject gm = Instantiate(prefab, Canvas);

        if (firstSibling)
            gm.transform.SetAsFirstSibling();

        return gm;
    }

    public static T CreateOnUI<T>(GameObject prefab)
        where T : Component
    {
        return CreateOnUI(prefab).GetComponent<T>();
    }

    private void Awake()
    {
        Singleton = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // toggle main menu panel
            mainMenu.Show(!mainMenu.ShowState);
        }
    }

    public void OnCloseGameBtn()
    {
        LevelLoader.LoadMenu();
    }
}
