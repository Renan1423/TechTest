using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleManager : MonoBehaviour
{
    [Header("Puzzle Settings")]
    [SerializeField]
    private int _shuffleMoves = 100;

    [Space(20), Header("Events")]
    [SerializeField]
    private UnityEvent<int> _OnVictory;

    [SerializeField]
    private UnityEvent _OnDefeat;

    [Space(20), Header("References")]
    [SerializeField]
    private PuzzleGrid _puzzleGrid;

    [SerializeField]
    private PuzzleTilesSpawner _puzzleTilesSpawner;

    [SerializeField]
    private TimeManager _timeManager;

    private int _currentStage = 0;

    private ImagesManager _imagesManagerReference;
    private ImagesManager _imagesManagerCache
    {
        get
        {
            if (_imagesManagerReference == null)
                _imagesManagerReference = ImagesManager.Instance;

            return _imagesManagerReference;
        }
    }

    private List<Sprite> _croppedPuzzleImage;

    private void Start()
    {
        Invoke(nameof(StartNextPuzzle), 0.05f);
    }

    public void StartNextPuzzle()
    {
        if (_currentStage >= _imagesManagerCache.PuzzleImagesDictionary.Count)
            _currentStage = 0;

        RemovePreviousPuzzleDelegates();

        _puzzleGrid.GenerateGrid(_puzzleTilesSpawner);

        _imagesManagerCache.CropPuzzleImage(_currentStage, out _croppedPuzzleImage);

        _timeManager.ResetTimer(_imagesManagerCache.PuzzleImagesDictionary[_currentStage].Time);

        List<PuzzleTile> allTiles = _puzzleGrid.GetAllTiles();

        for (int i = 0; i < allTiles.Count; i++)
        {
            allTiles[i].SetTileImage(_croppedPuzzleImage[i]);
            allTiles[i].OnDragMovementRequested += HandleDragRequested;
            allTiles[i].OnDragMovementEnded += HandleDragEnded;
        }

        _puzzleGrid.ShufflePuzzle(_shuffleMoves);
    }

    private void RemovePreviousPuzzleDelegates()
    {
        List<PuzzleTile> allTiles = _puzzleGrid.GetAllTiles();

        if (allTiles == null)
            return;

        for (int i = 0; i < allTiles.Count; i++)
        {
            allTiles[i].OnDragMovementRequested -= HandleDragRequested;
            allTiles[i].OnDragMovementEnded -= HandleDragEnded;
            allTiles[i].SetupDragParameters(false, Vector3.zero, Vector2.zero);
        }
    }

    private void HandleDragRequested(PuzzleTile tile)
    {
        // Asking the grid if this specific tile is allowed to move
        if (_puzzleGrid.GetPossibleMovement(tile, out Vector2 allowedAxis, out Vector2 targetPos))
        {
            // Giving it permission to drag if is adjacent to an empty tile
            tile.SetupDragParameters(true, targetPos, allowedAxis);
        }
        else
        {
            tile.SetupDragParameters(false, Vector3.zero, Vector2.zero);
        }
    }

    private void HandleDragEnded(PuzzleTile tile, bool successfullyMoved)
    {
        if (successfullyMoved)
        {
            _puzzleGrid.SwapTileWithEmptySpace(tile);

            if (CheckWinCondition())
            {
                Debug.Log("Victory! All tiles are in the correct position.");
                HandleVictory();
            }
        }
    }

    private bool CheckWinCondition()
    {
        foreach (PuzzleTile tile in _puzzleGrid.GetAllTiles())
        {
            if (tile.CurrentPosition != tile.CorrectPosition)
                return false;
        }

        return true;
    }

    public void HandleVictory()
    {
        GameStateManager.Instance.ChangeGameState(GameState.Paused);
        _OnVictory?.Invoke(_currentStage);
        _currentStage++;
    }

    public void HandleDefeat()
    {
        GameStateManager.Instance.ChangeGameState(GameState.Paused);
        _OnDefeat?.Invoke();
    }

    private void OnDestroy()
    {
        foreach (PuzzleTile tile in _puzzleGrid.GetAllTiles())
        {
            if (tile != null)
            {
                tile.OnDragMovementRequested -= HandleDragRequested;
                tile.OnDragMovementEnded -= HandleDragEnded;
            }
        }
    }
}
