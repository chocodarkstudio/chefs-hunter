using Combat_NM;
using Items;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [field: SerializeField] public Player Player { get; protected set; }
    [SerializeField] PlayerMovement playerMovement;

    public ItemWeapon SelectedWeapon { get; protected set; }

    private void Awake()
    {
        CombatManager.onCombatInitialized.AddListener(OnCombatInitialized);
        CombatManager.onCombatEnded.AddListener(OnCombatEnded);
        CombatManager.onTurnStep.AddListener(OnTurnStep);
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

        if (GameManager.CombatManager.InCombat
            && GameManager.CombatManager.Sequence.CurrentTurn.step == CombatTurnSteps.SelectWeapon)
        {
            ItemWeapon weapon = null;
            if (Input.GetKeyDown(KeyCode.J))
                weapon = GameManager.WeaponObjs[0].Item.Copy();
            else if (Input.GetKeyDown(KeyCode.K))
                weapon = GameManager.WeaponObjs[1].Item.Copy();
            else if (Input.GetKeyDown(KeyCode.L))
                weapon = GameManager.WeaponObjs[2].Item.Copy();

            if (weapon != null)
            {
                SelectedWeapon = weapon;
                GameManager.CombatManager.NextStep();
            }
        }

    }


    void OnCombatInitialized()
    {
        Debug.Log("OnCombatInitialized");
        playerMovement.LockInput();
    }

    void OnCombatEnded()
    {
        Debug.Log("OnCombatEnded");
        playerMovement.UnlockInput();
    }

    void OnTurnStep(CombatTurn combatTurn)
    {
        Debug.Log($"OnTurnStep {combatTurn.step} {combatTurn.type} {combatTurn.playerWin}");
        if (combatTurn.step == CombatTurnSteps.SelectWeapon)
        {
            SelectedWeapon = null;
            Debug.Log("Select a weapon! J: Fork, K: Knife, L: Spoon");
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



}
