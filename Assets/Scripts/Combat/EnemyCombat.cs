using Combat_NM;
using Items;
using System.Linq;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [field: SerializeField] public Enemy Enemy { get; protected set; }

    [field: SerializeField] public ItemWeaponObj[] WeaknessesWeapons { get; protected set; }

    public ItemWeapon SelectWeapon { get; protected set; }


    private void Awake()
    {
        CombatManager.onCombatInitialized.AddListener(OnCombatInitialized);
        CombatManager.onCombatEnded.AddListener(OnCombatEnded);
        CombatManager.onTurnStep.AddListener(OnTurnStep);
    }

    public bool IsWeaponWeakness(ItemWeapon weapon)
    {
        return WeaknessesWeapons.FirstOrDefault((weaponObj) => weaponObj.Item.ID == weapon.ID) != null;
    }

    void OnCombatInitialized()
    {
        // not me
        if (GameManager.CombatManager.EnemyCombat != this)
            return;

    }

    void OnCombatEnded()
    {
        if (GameManager.CombatManager.EnemyCombat != this)
            return;

        bool playerLoseAttack = GameManager.CombatManager.Sequence.PlayedTurns.Exists((combatTurn) =>
            combatTurn.type == CombatTurnTypes.Attack && !combatTurn.playerWin
        );
        if (playerLoseAttack)
            return;

        foreach (var ingredientDrop in Enemy.Drops)
        {
            DroppeableItem.CreateNew(ingredientDrop.Item, transform.position + Vector3.forward * 2f);
        }

        Enemy.DestroyEnemy();
    }

    void OnTurnStep(CombatTurn combatTurn)
    {
        if (GameManager.CombatManager.EnemyCombat != this)
            return;

        if (combatTurn.step == CombatTurnSteps.Start)
        {
            if (combatTurn.type == CombatTurnTypes.Defense)
            {
                SelectRandomWeapon();
            }
            else if (combatTurn.type == CombatTurnTypes.Attack)
            {
                // unselect weapon
                SelectWeapon = null;
            }
        }

    }

    void SelectRandomWeapon()
    {
        int randomIndex = Random.Range(0, GameManager.WeaponObjs.Length);
        SelectWeapon = GameManager.WeaponObjs[randomIndex].Item.Copy();

        Debug.Log($"Enemy SelectedWeapon: {SelectWeapon.Name}");
    }
}
