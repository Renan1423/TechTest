using System.Collections.Generic;
using UnityEngine;

public class PuzzleGrid : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField]
    private Vector2 _gridSize = Vector2.one * 3;

    [SerializeField]
    private float _tilesSize = 250f;

    [SerializeField]
    private Vector2 _tilesSpacing = new Vector2(250f, 250f);

    [SerializeField]
    private Transform _tilesParent;

    private PuzzleTile[,] _puzzleTilesOnGrid = new PuzzleTile[,] { };
    private Vector2 _emptySpaceGridPos;
    private Vector2 _firstPosition =>
        new Vector2(
            -(_tilesSpacing.x * (_gridSize.x - 1) / 2f),
            (_tilesSpacing.y * (_gridSize.y - 1) / 2f)
        );

    public void GenerateGrid(PuzzleTilesSpawner puzzleTilesSpawner)
    {
        ClearGrid();

        int[,] _puzzleGridPositions = new int[(int)_gridSize.x, (int)_gridSize.y];

        _puzzleTilesOnGrid = puzzleTilesSpawner.SpawnTilesOnGrid(
            _puzzleGridPositions.GetLength(0),
            _puzzleGridPositions.GetLength(1),
            _tilesSpacing,
            _tilesParent,
            _firstPosition
        );

        _emptySpaceGridPos = new Vector2Int(
            _puzzleTilesOnGrid.GetLength(0) - 1,
            _puzzleTilesOnGrid.GetLength(1) - 1
        );
    }

    public void ShufflePuzzle(int shuffleMoves)
    {
        //Setting the initial empty space to the bottom right of the grid
        _emptySpaceGridPos = new Vector2Int(
            _puzzleTilesOnGrid.GetLength(0) - 1,
            _puzzleTilesOnGrid.GetLength(1) - 1
        );

        for (int i = 0; i < shuffleMoves; i++)
        {
            List<Vector2> validMoves = GetValidNeighbors(_emptySpaceGridPos);

            // Picking a random neighbor to swap with the empty space
            Vector2 tileToMove = validMoves[Random.Range(0, validMoves.Count)];

            //Swapping the tiles in the matrix
            PuzzleTile tile = _puzzleTilesOnGrid[(int)tileToMove.x, (int)tileToMove.y];
            _puzzleTilesOnGrid[(int)_emptySpaceGridPos.x, (int)_emptySpaceGridPos.y] = tile;
            _puzzleTilesOnGrid[(int)tileToMove.x, (int)tileToMove.y] = null;

            tile.InitializeTile(_emptySpaceGridPos, tile.CorrectPosition);

            _emptySpaceGridPos = tileToMove;
        }

        SnapTilesToGridPositions();
    }

    private void SnapTilesToGridPositions()
    {
        for (int xx = 0; xx < _gridSize.x; xx++)
        {
            for (int yy = 0; yy < _gridSize.y; yy++)
            {
                if (_puzzleTilesOnGrid[xx, yy] == null)
                    continue;

                _puzzleTilesOnGrid[xx, yy].transform.localPosition =
                    _firstPosition + (_tilesSpacing * new Vector2(xx, -yy));
            }
        }
    }

    private List<Vector2> GetValidNeighbors(Vector2 emptyPos)
    {
        List<Vector2> neighbors = new List<Vector2>();

        if (emptyPos.x > 0)
            neighbors.Add(new Vector2(emptyPos.x - 1, emptyPos.y)); //Left
        if (emptyPos.x < _puzzleTilesOnGrid.GetLength(0) - 1)
            neighbors.Add(new Vector2(emptyPos.x + 1, emptyPos.y)); //Right

        if (emptyPos.y > 0)
            neighbors.Add(new Vector2(emptyPos.x, emptyPos.y - 1)); //Down
        if (emptyPos.y < _puzzleTilesOnGrid.GetLength(1) - 1)
            neighbors.Add(new Vector2(emptyPos.x, emptyPos.y + 1)); //Up

        return neighbors;
    }

    public void ClearGrid()
    {
        if (_puzzleTilesOnGrid.GetLength(0) == 0 && _puzzleTilesOnGrid.GetLength(1) == 0)
            return;

        for (int xx = 0; xx < _puzzleTilesOnGrid.GetLength(0); xx++)
        {
            for (int yy = 0; yy < _puzzleTilesOnGrid.GetLength(1); yy++)
            {
                if (_puzzleTilesOnGrid[xx, yy] != null)
                    _puzzleTilesOnGrid[xx, yy].Unspawn();
            }
        }

        _emptySpaceGridPos = new Vector2Int(0, 0);
    }

    public bool GetPossibleMovement(
        PuzzleTile tileToMove,
        out Vector2 possibleAxis,
        out Vector2 targetLocalPosition
    )
    {
        Vector2Int tilePos = Vector2Int.RoundToInt(tileToMove.CurrentPosition);
        possibleAxis = Vector2.zero;
        targetLocalPosition = Vector3.zero;

        if (tilePos.x == _emptySpaceGridPos.x && Mathf.Abs(tilePos.y - _emptySpaceGridPos.y) == 1)
        {
            possibleAxis = new Vector2(0, 1);
        }
        else if (
            tilePos.y == _emptySpaceGridPos.y
            && Mathf.Abs(tilePos.x - _emptySpaceGridPos.x) == 1
        )
        {
            possibleAxis = new Vector2(1, 0);
        }

        //Getting the position if the neighbor is valid
        if (possibleAxis != Vector2.zero)
        {
            targetLocalPosition =
                _firstPosition
                + (_tilesSpacing * new Vector2(_emptySpaceGridPos.x, -_emptySpaceGridPos.y));
            return true;
        }

        return false;
    }

    public void SwapTileWithEmptySpace(PuzzleTile tileToSwap)
    {
        Vector2 oldTilePos = tileToSwap.CurrentPosition;

        _puzzleTilesOnGrid[(int)_emptySpaceGridPos.x, (int)_emptySpaceGridPos.y] = tileToSwap;
        _puzzleTilesOnGrid[(int)oldTilePos.x, (int)oldTilePos.y] = null;

        tileToSwap.InitializeTile(_emptySpaceGridPos, tileToSwap.CorrectPosition);
        _emptySpaceGridPos = oldTilePos;
    }

    public List<PuzzleTile> GetAllTiles()
    {
        List<PuzzleTile> puzzleTiles = new List<PuzzleTile>();

        for (int xx = 0; xx < _puzzleTilesOnGrid.GetLength(0); xx++)
        {
            for (int yy = 0; yy < _puzzleTilesOnGrid.GetLength(1); yy++)
            {
                if (_puzzleTilesOnGrid[xx, yy] != null)
                    puzzleTiles.Add(_puzzleTilesOnGrid[xx, yy]);
            }
        }

        return puzzleTiles;
    }

    private void OnValidate()
    {
        if (_gridSize.x < 0 || _gridSize.y < 0)
            _gridSize = new Vector2(Mathf.Abs(_gridSize.x), Mathf.Abs(_gridSize.y));

        if (_gridSize.x % 1 != 0 || _gridSize.y % 1 != 0)
        {
            int newSizeX = Mathf.RoundToInt(_gridSize.x);
            int newSizeY = Mathf.RoundToInt(_gridSize.y);
            _gridSize = new Vector2(newSizeX, newSizeY);
        }
    }
}
