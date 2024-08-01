using ChocoDark.GlobalAudio;
using CombatSystem;
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
        enemyMovement.StopAndLock();

        // not me
        if (GameManager.CombatManager.EnemyCombat != this)
            return;
    }

    void OnCombatEnded()
    {
        enemyMovement.Unlock();

        if (GameManager.CombatManager.EnemyCombat != this)
            return;

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

            ItemDrop.CreateNew(ingredientDrop.Item, transform.position + offset)
                .SpawnAnim(transform.position);
        }

        GlobalAudio.PlaySFX(GlobalAudio.GeneralClips.enemyDeathClip);
        Enemy.DestroyEnemy();
    }

    void OnTurnStep(StepConfig stepConfig)
    {
        if (GameManager.CombatManager.EnemyCombat != this)
            return;

        if (stepConfig.CurrentTurn.step == CombatTurnSteps.Start)
        {
            if (stepConfig.CurrentTurn.type == CombatTurnTypes.Defense)
            {
                SelectRandomWeapon();
                ShowAttackHighlight(true);
                stateMachine.Play("pre_attack", fade: false);
            }
            else if (stepConfig.CurrentTurn.type == CombatTurnTypes.Attack)
            {
                // unselect weapon
                SelectedWeapon = null;
                stateMachine.Play("idle", fade: false);
            }
        }
        else if (stepConfig.CurrentTurn.step == CombatTurnSteps.CheckWin)
        {
            ShowAttackHighlight(false);

            if (stepConfig.CurrentTurn.type == CombatTurnTypes.Defense && !stepConfig.CurrentTurn.playerWin)
            {
                stateMachine.Play("attack", fade: false);
                GlobalAudio.PlaySFX(GlobalAudio.GeneralClips.playerHitClip);
            }
            else if (stepConfig.CurrentTurn.type == CombatTurnTypes.Attack && stepConfig.CurrentTurn.playerWin)
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
