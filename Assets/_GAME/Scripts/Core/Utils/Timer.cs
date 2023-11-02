using System;
using UnityEngine;

[Serializable]
public class Timer
{
    public Timer(float duration)
    {
        _duration = duration;
        _remainingSeconds = 0.0f;
    }

    [SerializeField, ReadOnly] float _duration;
    [SerializeField, ReadOnly] float _remainingSeconds;

    public float RemainingSeconds => _remainingSeconds;

    /// <summary>Whether the remaining seconds value is greater than 0.</summary>
    public bool HasTimeLeft => _remainingSeconds > 0;

    public event Action OnTimerEnd;

    public void Restart()
    {
        _remainingSeconds = _duration;
    }

    public void Tick(float deltaTime)
    {
        if (_remainingSeconds == 0.0f)
            return;

        _remainingSeconds -= deltaTime;

        CheckForTimerEnd();
    }

    /// <summary>
    /// Sets the <i>RemainingSeconds</i> value to 0 and raises the <i>OnTimerEnd</i> event.
    /// </summary>
    public void End()
    {
        _remainingSeconds = 0.0f;
        OnTimerEnd?.Invoke();
    }

    private void CheckForTimerEnd()
    {
        if (_remainingSeconds > 0)
            return;

        End();
    }
}
