using ChocoDark.GlobalAudio;
using CombatSystem;
using Items;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [field: SerializeField] public Player Player { get; protected set; }
    [SerializeField] GenericStateMachine stateMachine;

    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] UIWeaponSelector uiWeaponSelector;

    [field: SerializeField] public Timer SelectWeaponTimer { get; protected set; }
    public ItemWeapon SelectedWeapon { get; protected set; }

    private void Awake()
    {
        CombatManager.onCombatInitialized.AddListener(OnCombatInitialized);
        CombatManager.onCombatEnded.AddListener(OnCombatEnded);
        CombatManager.onTurnStep.AddListener(OnTurnStep);

        if (uiWeaponSelector != null)
            uiWeaponSelector.onWeaponSelected.AddListener(OnWeaponSelected);
        SelectWeaponTimer.onCompleted.AddListener(OnSelectWeaponTimerOver);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out EnemyCombat enemyCombat))
        {
            // already in combat
            if (GameManager.CombatManager.InCombat)
                return;

            GameManager.CombatManager.InitializeCombat(this, enemyCombat);
            GameManager.CombatManager.StartCombat();
        }
    }

    private void Update()
    {
        SelectWeaponTimer.Update();
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
        uiWeaponSelector.Show(false);
    }

    void OnTurnStep(StepConfig stepConfig)
    {
        Debug.Log($"OnTurnStep {stepConfig.CurrentTurn.step} {stepConfig.CurrentTurn.type} {stepConfig.CurrentTurn.playerWin}");
        if (stepConfig.CurrentTurn.step == CombatTurnSteps.SelectWeapon &&
            (stepConfig.CurrentTurn.type == CombatTurnTypes.Attack || stepConfig.CurrentTurn.type == CombatTurnTypes.Defense))
        {
            Debug.Log("Select a weapon!");

            SelectedWeapon = null;
            uiWeaponSelector.Show(true);
            // dont use SelectWeaponTimer on tutorial
            if (!GameManager.CombatTutorial.TutorialEnabled)
                SelectWeaponTimer.Restart();
            stepConfig.MoveToNextStep = false;
        }
        else
        {
            uiWeaponSelector.Show(false);
            SelectWeaponTimer.Stop();
        }

        if (stepConfig.CurrentTurn.step == CombatTurnSteps.CheckWin && stepConfig.CurrentTurn.playerWin)
        {
            GlobalAudio.PlaySFX(GlobalAudio.GeneralClips.enemyHitClip);
        }

        // The player loses
        if (stepConfig.CurrentTurn.step == CombatTurnSteps.End && !stepConfig.CurrentTurn.playerWin)
        {
            // Attack -> End combat
            if (stepConfig.CurrentTurn.type == CombatTurnTypes.Attack)
            {
                // Calculate the direction vector from the enemy to the player
                Vector3 enemyPos = GameManager.CombatManager.EnemyCombat.transform.position;
                Vector3 dir = transform.position - enemyPos;

                // Push the player away from the enemy
                playerMovement.PushAway(dir, 10f);

                // End the combat
                GameManager.CombatManager.EndCombat();

                GlobalAudio.PlaySFX(GlobalAudio.GeneralClips.missHitClip);
            }
            // Defense -> Loses items.
            else if (stepConfig.CurrentTurn.type == CombatTurnTypes.Defense)
            {
                int halfItemCount = (int)(Player.Inventory.ingredientsStorage.Count * 0.5f);
                if (halfItemCount == 0)
                    halfItemCount = 1;
                GlobalAudio.PlaySFX(GlobalAudio.GeneralClips.missHitClip);
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

    void OnSelectWeaponTimerOver()
    {
        if (!GameManager.CombatManager.InCombat)
            return;

        if (GameManager.CombatManager.Sequence.CurrentTurn.step == CombatTurnSteps.SelectWeapon)
        {
            GameManager.CombatManager.NextStep();
        }
    }
}
