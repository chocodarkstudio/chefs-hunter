using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem
{
    [CreateAssetMenu(fileName = "new CombatSequence", menuName = "CombatSequence", order = 0)]
    public class CombatSequence : ScriptableObject
    {
        [field: SerializeField] public List<CombatTurn> Turns { get; protected set; }
        [field: Tooltip("Repeat TurnList after first play?\n(-1 is inifinity, 0 is no repeat)")]
        [field: SerializeField] public int Repeat { get; protected set; }


        public readonly List<CombatTurn> PlayedTurns = new();
        public int CurrentTurnIndex { get; protected set; }
        public CombatTurn CurrentTurn => Turns[CurrentTurnIndex];
        public bool IsLastTurnIndex => CurrentTurnIndex >= Turns.Count - 1;

        public bool IsOver => Repeat > -1 &&
            PlayedTurns.Count >= Turns.Count * (Repeat + 1);
        public bool IsStarted => PlayedTurns.Count > 0;

        public void EndTurn()
        {
            PlayedTurns.Add(CurrentTurn);
        }

        public void NextTurn()
        {
            if (IsOver)
            {
                Debug.LogWarning("Cant do next turn, sequence is over!");
                return;
            }

            EndTurn();

            // reset if has repeat
            if (IsLastTurnIndex)
                CurrentTurnIndex = 0;
            else
                CurrentTurnIndex++;
        }

        public void NextTurnStep()
        {
            if (CurrentTurn.step == CombatTurnSteps.End)
                return;

            CombatTurn turn = CurrentTurn;
            turn.step++;

            Turns[CurrentTurnIndex] = turn;
        }
    }
}