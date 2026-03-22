using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    public bool IsRunning { get; private set; } = false;
    public float TimeCount => _timeCount;
    public float TimerDuration => _timerDuration;

    [Header("Events")]
    public UnityEvent OnTimerStarted = new();
    public UnityEvent OnTimerTick = new();
    public UnityEvent OnTimerCompleted = new();

    protected float _timerDuration = 1f;
    protected float _timeCount = 0f;

    protected float _countdownSpeed;

    protected bool _isPaused = false;

    protected IEnumerator _timerCoroutine;

    public virtual void SetupTimer(float duration, float speed = 1f)
    {
        _timerDuration = duration;
        _countdownSpeed = Mathf.Abs(speed);
    }

    public virtual void StartTimer()
    {
        _timeCount = _timerDuration;
        IsRunning = true;
    }

    public virtual void AddTime(float amount)
    {
        _timeCount += amount;
        if (_timeCount > _timerDuration)
            _timeCount = _timerDuration;
    }

    private void Update()
    {
        if (!IsRunning || _isPaused)
            return;

        if (_timeCount > 0)
        {
            //Debug.Log(timeCount + " / " + timerDuration);
            _timeCount -= Time.deltaTime * _countdownSpeed;
            OnTimerTick?.Invoke();
        }
        else
        {
            CompleteTimer();
        }
    }

    public virtual void CompleteTimer()
    {
        IsRunning = false;
        OnTimerCompleted?.Invoke();
    }

    public virtual void ToggleTimerPause(bool paused = true)
    {
        _isPaused = paused;
    }

    public void ChangeTimerSpeed(float newSpeed)
    {
        _countdownSpeed = Mathf.Abs(newSpeed);
    }

    public float GetProgression() => 1 - _timeCount / _timerDuration;
}
