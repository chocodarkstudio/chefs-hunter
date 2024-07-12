using Combat_NM;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] PlayerMovement playerMovement;

    private void Awake()
    {
        CombatManager.onCombatInitialized.AddListener(OnCombatInitialized);
        CombatManager.onCombatEnded.AddListener(OnCombatEnded);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Enemy enemy))
        {
            GameManager.CombatManager.InitializeCombat(player, enemy);
        }
    }


    void OnCombatInitialized()
    {
        playerMovement.LockInput();
    }

    void OnCombatEnded()
    {
        playerMovement.UnlockInput();
    }
}
