using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Real-time timer </summary>
[System.Serializable]
public class Timer
{
    float startedTime;
    [SerializeField] float timerSeconds;
    public float TimerSeconds => timerSeconds + TimerSecondsPlus;
    public float TimerSecondsPlus { get; protected set; }

    public float ElapsedTime => Time.time - startedTime;
    public float RemainingTime => TimerSeconds - ElapsedTime;

    public bool IsOver => RemainingTime <= 0;
    public bool IsCompleted { get; protected set; } = true;


    // events
    public readonly UnityEvent onStarted = new();
    public readonly UnityEvent onCompleted = new();

    public void Restart()
    {
        startedTime = Time.time;
        IsCompleted = false;
        TimerSecondsPlus = 0;

        onStarted.Invoke();
    }

    public void StartNew(float seconds)
    {
        timerSeconds = seconds;
        Restart();
    }

    public void Update()
    {
        if (IsCompleted)
            return;

        if (IsOver)
        {
            IsCompleted = true;
            onCompleted.Invoke();
        }
    }

    /// <summary> </summary>
    /// <param name="plusTime"> Can be negative, to reduce timer </param>
    public void AddTime(float plusTime)
    {
        TimerSecondsPlus += plusTime;
        Update();
    }

    public void Stop()
    {
        IsCompleted = true;
    }
}
