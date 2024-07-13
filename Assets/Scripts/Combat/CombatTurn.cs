namespace Combat_NM
{
    public enum CombatTurnTypes
    {
        Attack,
        Defense
    }

    public enum CombatTurnSteps
    {
        Start,
        SelectWeapon,
        CheckWin,
        End
    }

    [System.Serializable]
    public struct CombatTurn
    {
        public CombatTurnTypes type;
        public CombatTurnSteps step;

        public bool playerWin;
    }

}
