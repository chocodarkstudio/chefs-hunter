using Combat_NM;
using Items;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [field: SerializeField] public Player Player { get; protected set; }
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] UIWeaponSelector uiWeaponSelector;

    public ItemWeapon SelectedWeapon { get; protected set; }

    private void Awake()
    {
        CombatManager.onCombatInitialized.AddListener(OnCombatInitialized);
        CombatManager.onCombatEnded.AddListener(OnCombatEnded);
        CombatManager.onTurnStep.AddListener(OnTurnStep);

        uiWeaponSelector.onWeaponSelected.AddListener(OnWeaponSelected);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out EnemyCombat enemyCombat))
        {
            GameManager.CombatManager.InitializeCombat(this, enemyCombat);
            GameManager.CombatManager.StartCombat();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            GameManager.CombatManager.NextStep();
    }


    void OnCombatInitialized()
    {
        Debug.Log("OnCombatInitialized");
        playerMovement.LockInput();
        Player.UIInventoryPlayer.Show(false);
    }

    void OnCombatEnded()
    {
        Debug.Log("OnCombatEnded");
        playerMovement.UnlockInput();
        Player.UIInventoryPlayer.Show(true);
    }

    void OnTurnStep(CombatTurn combatTurn)
    {
        Debug.Log($"OnTurnStep {combatTurn.step} {combatTurn.type} {combatTurn.playerWin}");
        if (combatTurn.step == CombatTurnSteps.SelectWeapon)
        {
            SelectedWeapon = null;
            uiWeaponSelector.Show(true);
            Debug.Log("Select a weapon!");
        }

        // The player loses
        if (combatTurn.step == CombatTurnSteps.End && !combatTurn.playerWin)
        {
            // Attack -> End combat
            if (combatTurn.type == CombatTurnTypes.Attack)
            {
                // Calculate the direction vector from the enemy to the player
                Vector3 enemyPos = GameManager.CombatManager.EnemyCombat.transform.position;
                Vector3 dir = transform.position - enemyPos;

                // Push the player away from the enemy
                playerMovement.PushAway(dir, 10f);

                // End the combat
                GameManager.CombatManager.EndCombat();
            }
            // Defense -> Loses items.
            else if (combatTurn.type == CombatTurnTypes.Defense)
            {
                int halfItemCount = (int)(Player.Inventory.ingredientsStorage.Count * 0.5f);
                if (halfItemCount == 0)
                    halfItemCount = 1;
                Player.Inventory.DiscardPlayerFirstItems(halfItemCount);
            }
        }

    }


    void OnWeaponSelected(ItemWeapon weapon)
    {
        uiWeaponSelector.Show(false);
        if (!GameManager.CombatManager.InCombat)
            return;

        Debug.Log($"Player SelectedWeapon: {weapon.Name}");

        SelectedWeapon = weapon;

        if (GameManager.CombatManager.Sequence.CurrentTurn.step == CombatTurnSteps.SelectWeapon)
        {
            GameManager.CombatManager.NextStep();
        }
    }
}
