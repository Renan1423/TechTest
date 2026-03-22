using Unity.VisualScripting;
using UnityEngine;

public class CheatsManager : MonoBehaviour
{
    [SerializeField]
    private PuzzleManager _puzzleManager;

    [SerializeField]
    private PuzzleGrid _puzzleGrid;

    [SerializeField]
    private TimeManager _timeManager;

    private void Update()
    {
        //#if UNITY_EDITOR
        //Toggles the CORRECT position in each puzzle tile
        if (Input.GetKeyDown(KeyCode.O))
        {
            var allTiles = _puzzleGrid.GetAllTiles();

            if (allTiles == null)
                return;

            foreach (PuzzleTile tile in allTiles)
            {
                tile.ToggleCorrectPositionTip();
            }
        }

        //Toggles the CURRENT position in each puzzle tile
        if (Input.GetKeyDown(KeyCode.U))
        {
            var allTiles = _puzzleGrid.GetAllTiles();

            if (allTiles == null)
                return;

            foreach (PuzzleTile tile in allTiles)
            {
                tile.ToggleCurrentPositionTip();
            }
        }

        //Toggles the victory screen, achieving to the next stage
        if (Input.GetKeyDown(KeyCode.V))
        {
            _puzzleManager.HandleVictory();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            _puzzleManager.HandleDefeat();
        }

        //Resets timer
        if (Input.GetKeyDown(KeyCode.T))
        {
            _timeManager.ResetTimer(_timeManager.TimerDuration);
        }

        //#endif
    }
}
