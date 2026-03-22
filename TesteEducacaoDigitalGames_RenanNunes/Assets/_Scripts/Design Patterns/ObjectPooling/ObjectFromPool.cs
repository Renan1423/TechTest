using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectFromPool : MonoBehaviour, IPooledObject
{
    [SerializeField]
    protected float timeToUnspawn = 0.5f;

    [SerializeField]
    private bool unspawnAutomatically = true;

    [HideInInspector]
    public UnityEvent OnDisablePooledObject;

    private Timer timer;
    protected bool isPaused;
    private GameStateManager gameStateManagerCache;

    public virtual void OnObjectSpawn()
    {
        if (gameStateManagerCache == null)
            gameStateManagerCache = GameStateManager.Instance;

        gameStateManagerCache.OnGameStateChanged += OnGameStateChanged;

        if (unspawnAutomatically)
        {
            if (timer == null)
            {
                GameObject timerGO = new();
                timerGO.name = gameObject.name + "Timer";
                timerGO.transform.parent = transform;
                timer = timerGO.AddComponent<Timer>();
                timer.SetupTimer(timeToUnspawn, 1f);
                timer.OnTimerCompleted = new UnityEvent();
                timer.OnTimerCompleted.AddListener(Unspawn);
            }

            timer.StartTimer();
            OnGameStateChanged(gameStateManagerCache.CurrentGameState);
        }
    }

    public virtual void Unspawn()
    {
        gameStateManagerCache.OnGameStateChanged -= OnGameStateChanged;
        OnDisablePooledObject?.Invoke();
        gameObject.SetActive(false);
    }

    protected virtual void OnGameStateChanged(GameState newGameState)
    {
        isPaused = newGameState == GameState.Paused;
        if (timer != null)
            timer.ToggleTimerPause(isPaused);
    }
}
