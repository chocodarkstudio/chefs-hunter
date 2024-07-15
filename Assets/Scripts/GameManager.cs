using Combat_NM;
using Items;
using ScreenTransition;
using UIAnimShortcuts;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager Singleton;

    [Header("UI References")]
    [SerializeField] Transform canvas;
    public static Transform Canvas => Singleton.canvas;

    [SerializeField] Transform uiTopLevel;
    public static Transform UITopLevel => Singleton.uiTopLevel;
    [SerializeField] Transform mainMenuPanel;

    [Header("Scene References")]
    [SerializeField] CombatManager combatManager;
    public static CombatManager CombatManager => Singleton.combatManager;
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
    [SerializeField] GameObject droppeableItemPrefab;
    public static GameObject DroppeableItemPrefab => Singleton.droppeableItemPrefab;


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
            if (mainMenuPanel.gameObject.activeSelf)
            {
                UIAnim.Scale(mainMenuPanel, TransitionState.Close,
                    callback: () =>
                    {
                        mainMenuPanel.gameObject.SetActive(false);
                    }
                );
            }
            else
            {
                mainMenuPanel.gameObject.SetActive(true);
                UIAnim.Scale(mainMenuPanel, TransitionState.Open);
            }


        }
    }

    public void OnCloseGameBtn()
    {
        LevelLoader.LoadMenu();
    }
}
