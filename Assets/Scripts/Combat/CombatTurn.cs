namespace CombatSystem
{
    public enum CombatTurnTypes
    {
        CombatStart,
        Attack,
        Defense,
        CombatEnd
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

        public bool Equals(CombatTurn other)
        {
            if (type != other.type)
                return false;
            if (step != other.step)
                return false;
            if (playerWin != other.playerWin)
                return false;
            return true;
        }
    }

}
