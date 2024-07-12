using UnityEngine;
using UnityEngine.Events;

namespace Combat_NM
{
    public class CombatManager : MonoBehaviour
    {
        [SerializeField] protected CombatSequence StandardSequence;

        public Player Player { get; protected set; }
        public Enemy Enemy { get; protected set; }
        public CombatSequence Sequence { get; protected set; }

        public bool InCombat => Sequence != null && !Sequence.IsOver;

        // events
        public static readonly UnityEvent onCombatInitialized = new();
        public static readonly UnityEvent onCombatEnded = new();

        public void InitializeCombat(CombatSequence sequence, Player player, Enemy enemy)
        {
            // already in combat
            if (InCombat)
                return;

            // invalid participants
            if (player == null || enemy == null)
                return;

            Sequence = Instantiate(sequence);
            Player = player;
            Enemy = enemy;

            onCombatInitialized.Invoke();
            Debug.Log("InitializeCombat");
        }
        public void InitializeCombat(Player player, Enemy enemy)
            => InitializeCombat(StandardSequence, player, enemy);


        public void StartCombat()
        {
            // not in combat or already started
            if (!InCombat || Sequence.IsStarted)
                return;

            NextTurn();
        }

        public void NextTurn()
        {
            Sequence.NextTurn();
        }

        public void EndCombat()
        {
            // reset properties
            Sequence = null;
            Player = null;
            Enemy = null;

            onCombatEnded.Invoke();
        }
    }
}