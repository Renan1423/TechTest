using UnityEngine;

public class PuzzleTilesSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField]
    private GameObject _tilesPrefab;
    private ObjectPooler _objectPoolerCache;

    public PuzzleTile[,] SpawnTilesOnGrid(
        int gridSizeX,
        int gridSizeY,
        Vector2 tilesSpacing,
        Transform tilesParent,
        Vector2 firstPosition
    )
    {
        PuzzleTile[,] puzzleTilesMatrix = new PuzzleTile[gridSizeX, gridSizeY];

        int totalTiles = gridSizeX * gridSizeY;
        int tileCount = 1; //starts in 1 in order to leave the bottom-right slot empty

        if (_objectPoolerCache == null)
            _objectPoolerCache = ObjectPooler.Instance;

        for (int yy = 0; yy < gridSizeY; yy++)
        {
            for (int xx = 0; xx < gridSizeX; xx++)
            {
                if (tileCount == totalTiles)
                    continue;

                Vector2 localPos = firstPosition + (tilesSpacing * new Vector2(xx, -yy));

                GameObject tileInstance = _objectPoolerCache.SpawnFromPool(
                    "PuzzleTile",
                    localPos,
                    Quaternion.identity,
                    tilesParent
                );

                PuzzleTile newPuzzleTile = tileInstance.GetComponent<PuzzleTile>();

                Vector2 gridCoodinate = new Vector2(xx, yy);
                newPuzzleTile.InitializeTile(gridCoodinate, gridCoodinate);

                puzzleTilesMatrix[xx, yy] = newPuzzleTile;

                tileCount++;
            }
        }

        return puzzleTilesMatrix;
    }
}
