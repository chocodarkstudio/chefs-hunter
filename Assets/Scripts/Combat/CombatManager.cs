using UnityEngine;
using UnityEngine.Events;

namespace CombatSystem
{
    /// <summary>
    /// Configures how the step should be treated. </summary>
    /// <remarks>
    /// StepConfig will be checked after onTurnStep are invoked. </remarks>
    public class StepConfig
    {
        public CombatTurn CurrentTurn { get; protected set; }

        /// <summary>
        /// Moves to the next step when the current step ends. </summary>
        public bool MoveToNextStep { get; set; } = true;

        public StepConfig(CombatTurn currentStep)
        {
            CurrentTurn = currentStep;
        }
    }

    public class CombatManager : MonoBehaviour
    {
        [SerializeField] protected CombatSequence StandardSequence;

        public PlayerCombat PlayerCombat { get; protected set; }
        public EnemyCombat EnemyCombat { get; protected set; }
        public CombatSequence Sequence { get; protected set; }

        public bool InCombat => Sequence != null && !Sequence.IsOver;

        // events
        public static readonly UnityEvent onCombatInitialized = new();
        public static readonly UnityEvent onCombatEnded = new();
        public static readonly UnityEvent<StepConfig> onTurnStep = new();

        public void InitializeCombat(CombatSequence sequence, PlayerCombat playerCombat, EnemyCombat enemyCombat)
        {
            // already in combat
            if (InCombat)
                return;

            // invalid participants
            if (playerCombat == null || enemyCombat == null)
                return;

            Sequence = Instantiate(sequence);
            PlayerCombat = playerCombat;
            EnemyCombat = enemyCombat;

            onCombatInitialized.Invoke();
        }
        public void InitializeCombat(PlayerCombat playerCombat, EnemyCombat enemyCombat)
            => InitializeCombat(StandardSequence, playerCombat, enemyCombat);


        public void StartCombat()
        {
            // not in combat or already started
            if (!InCombat || Sequence.IsStarted)
                return;


            StepConfig stepConfig = new(Sequence.CurrentTurn);
            onTurnStep.Invoke(stepConfig);

            // auto next step
            if (stepConfig.MoveToNextStep)
            {
                DelayedNextStep();
            }
        }


        public void NextTurn()
        {
            // not in combat or not started
            if (!InCombat)
                return;

            Sequence.NextTurn();

            if (Sequence.IsOver)
            {
                EndCombat();
                return;
            }

            StepConfig stepConfig = new(Sequence.CurrentTurn);
            onTurnStep.Invoke(stepConfig);

            // auto next step
            if (stepConfig.MoveToNextStep)
            {
                DelayedNextStep();
            }
        }

        public void EndCombat()
        {
            Sequence.EndTurn();
            onCombatEnded.Invoke();

            // reset properties
            Sequence = null;
            PlayerCombat = null;
            EnemyCombat = null;
        }


        public void NextStep()
        {
            // not in combat or not started
            if (!InCombat)
                return;


            if (Sequence.CurrentTurn.step == CombatTurnSteps.End)
            {
                NextTurn();
                return;
            }

            Sequence.NextTurnStep();

            if (Sequence.CurrentTurn.step == CombatTurnSteps.CheckWin)
                OnCheckWinStep();

            StepConfig stepConfig = new(Sequence.CurrentTurn);
            onTurnStep.Invoke(stepConfig);

            // auto next step
            if (stepConfig.MoveToNextStep)
                DelayedNextStep();
        }

        void DelayedNextStep()
        {
            Invoke(nameof(NextStep), 0.2f);
        }


        void OnCheckWinStep()
        {
            // not in combat or already started
            if (!InCombat)
                return;

            if (Sequence.CurrentTurn.type == CombatTurnTypes.Attack)
            {
                // player has no weapon selected,  dont give player the win
                if (PlayerCombat.SelectedWeapon == null)
                {
                    SetTurnWin(false);
                    return;
                }

                // check if the player weapon is an enemy weakness
                bool isWeaponWeakness = EnemyCombat.IsWeaponWeakness(PlayerCombat.SelectedWeapon);
                SetTurnWin(isWeaponWeakness);
            }
            else if (Sequence.CurrentTurn.type == CombatTurnTypes.Defense)
            {
                // player has no weapon selected, dont give player the win
                if (PlayerCombat.SelectedWeapon == null)
                {
                    SetTurnWin(false);
                    return;
                }

                // enemy dosnt have selected weapon, give player the win
                if (EnemyCombat.SelectedWeapon == null)
                {
                    SetTurnWin(true);
                    return;
                }

                // Checks if the player weapon is the same as the enemy weapon.
                bool sameWeapon = PlayerCombat.SelectedWeapon.ID == EnemyCombat.SelectedWeapon.ID;
                SetTurnWin(sameWeapon);
            }
        }


        void SetTurnWin(bool playerWin)
        {
            Debug.Log($"PlayerWin: {playerWin}");

            // already in that state
            if (Sequence.CurrentTurn.playerWin == playerWin)
                return;

            CombatTurn turn = Sequence.CurrentTurn;
            turn.playerWin = playerWin;
            Sequence.Turns[Sequence.CurrentTurnIndex] = turn;
        }
    }

}