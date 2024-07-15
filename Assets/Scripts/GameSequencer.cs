using UnityEngine;

public class GameSequencer : MonoBehaviour
{
    [SerializeField] UIGameTimer uiGameTimer;
    [field: SerializeField] public Timer GameTimer { get; protected set; }

    private void Start()
    {
        GameTimer.Restart();
        GameTimer.onCompleted.AddListener(() =>
        {
            GameManager.OrderCounter.CancelAllOrders();
        });
    }

    private void Update()
    {
        GameTimer.Update();
        uiGameTimer.RotateByPercent(GameTimer.RemainingPercent);
    }
}
