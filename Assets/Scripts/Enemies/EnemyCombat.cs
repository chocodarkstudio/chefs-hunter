using Combat_NM;
using DG.Tweening;
using GameUtils;
using Items;
using System.Linq;
using UIAnimShortcuts;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField] EnemyMovement enemyMovement;
    [SerializeField] GenericStateMachine stateMachine;

    [SerializeField] SpriteRenderer attackHighlight;
    [SerializeField] float attackHighlightRotateSpeed = 1;
    Vector3 attackHighlightInitialScale;

    [field: SerializeField] public Enemy Enemy { get; protected set; }

    [field: SerializeField] public ItemWeaponObj[] WeaknessesWeapons { get; protected set; }

    public ItemWeapon SelectedWeapon { get; protected set; }

    // tweens
    Tween attackHighlightTween;

    private void Awake()
    {
        CombatManager.onCombatInitialized.AddListener(OnCombatInitialized);
        CombatManager.onCombatEnded.AddListener(OnCombatEnded);
        CombatManager.onTurnStep.AddListener(OnTurnStep);

        attackHighlightInitialScale = attackHighlight.transform.localScale;
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

        enemyMovement.StopAndLock();
    }

    void OnCombatEnded()
    {
        if (GameManager.CombatManager.EnemyCombat != this)
            return;

        enemyMovement.Unlock();

        // check if player kills the enemy
        bool playerLoseAttack = GameManager.CombatManager.Sequence.PlayedTurns.Exists((combatTurn) =>
            combatTurn.type == CombatTurnTypes.Attack && !combatTurn.playerWin
        );
        if (playerLoseAttack)
            return;

        // create drops
        foreach (var ingredientDrop in Enemy.Drops)
        {
            Vector3 offset = GUtils.GetRandomPosition(-1f, 1f);
            offset.y = 0;

            DroppeableItem.CreateNew(ingredientDrop.Item, transform.position + offset)
                .SpawnAnim(transform.position);
        }

        GlobalAudio.PlayEffect(GlobalAudio.GeneralClips.enemyDeathClip);
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
                ShowAttackHighlight(true);
                stateMachine.Play("pre_attack", fade: false);
            }
            else if (combatTurn.type == CombatTurnTypes.Attack)
            {
                // unselect weapon
                SelectedWeapon = null;
                stateMachine.Play("idle", fade: false);
            }
        }
        else if (combatTurn.step == CombatTurnSteps.CheckWin)
        {
            ShowAttackHighlight(false);

            if (combatTurn.type == CombatTurnTypes.Defense && !combatTurn.playerWin)
            {
                stateMachine.Play("attack", fade: false);
                GlobalAudio.PlayEffect(GlobalAudio.GeneralClips.playerHitClip);
            }
            else if (combatTurn.type == CombatTurnTypes.Attack && combatTurn.playerWin)
            {
                stateMachine.Play("damaged", fade: false);
            }
        }
    }


    void SelectRandomWeapon()
    {
        SelectedWeapon = GameManager.WeaponObjs.GetOneRandom().Get;
        attackHighlight.color = SelectedWeapon.PrimaryColor;
        Debug.Log($"Enemy SelectedWeapon: {SelectedWeapon.Name}");
    }


    public void ShowAttackHighlight(bool show)
    {
        // already in that state
        if (attackHighlight.gameObject.activeSelf == show)
            return;

        // create the tween
        if (attackHighlight == null || !attackHighlightTween.IsActive())
        {
            attackHighlightTween = UIAnim.SpinInfinity(attackHighlight.transform, attackHighlightRotateSpeed);
        }

        if (show)
        {
            attackHighlightTween.Play();

            attackHighlight.transform.localScale = Vector3.zero;
            attackHighlight.gameObject.SetActive(true);
            attackHighlight.transform.DOScale(attackHighlightInitialScale, 0.2f);
        }
        else
        {
            attackHighlightTween.Pause();

            attackHighlight.transform.DOScale(0, 0.2f).onComplete = () =>
            {
                attackHighlight.gameObject.SetActive(false);
            };
        }


    }
}
