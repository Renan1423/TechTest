using UnityEngine;

public class GameStateManager : Singleton<GameStateManager>
{
    public GameState CurrentGameState { get; private set; }

    public delegate void GameStateChangeHandler(GameState newGameState);
    public event GameStateChangeHandler OnGameStateChanged;

    public void ChangeGameState(GameState newGameState)
    {
        if (newGameState == CurrentGameState)
            return;

        Debug.Log("" + CurrentGameState + " -> " + newGameState);

        CurrentGameState = newGameState;
        OnGameStateChanged?.Invoke(newGameState);
    }
}
