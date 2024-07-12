namespace Combat_NM
{
    public enum CombatTurnType { Attack, Defense }

    [System.Serializable]
    public struct CombatTurn
    {
        public CombatTurnType turnType;
        public bool playerWin;
    }

}
