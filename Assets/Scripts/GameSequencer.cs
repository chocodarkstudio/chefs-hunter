using UnityEngine;

public class GameSequencer : MonoBehaviour
{
    [SerializeField] UIGameTimer uiGameTimer;
    [field: SerializeField] public Timer GameTimer { get; protected set; }

    private void Start()
    {
        GameTimer.onCompleted.AddListener(OnTimerOver);
        GameTimer.onStarted.AddListener(uiGameTimer.OnTimerStarted);
        RestartTimer();
    }

    private void Update()
    {
        GameTimer.Update();
        uiGameTimer.RotateByPercent(GameTimer.RemainingPercent);
    }

    void OnTimerOver()
    {
        uiGameTimer.OnTimerOver();
        GameManager.OrderCounter.CancelAllOrders();
    }


    public void RestartTimer()
    {
        GameTimer.Restart();
    }
}
