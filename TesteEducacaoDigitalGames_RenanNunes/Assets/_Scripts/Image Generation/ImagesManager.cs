using System.Collections.Generic;
using UnityEngine;

public class PuzzleImage
{
    public int ID;
    public string FileName;
    public int Time;
}

public class ImagesManager : Singleton<ImagesManager>
{
    [HideInInspector]
    public Dictionary<int, PuzzleImage> PuzzleImagesDictionary { get; private set; } =
        new Dictionary<int, PuzzleImage>();

    protected override void Awake()
    {
        base.Awake();
        ImportData();
    }

    public void ImportData()
    {
        JsonImporter
            .ImportJsonList<PuzzleImage>("Jsons/PuzzleImagesData")
            .ForEach(x => PuzzleImagesDictionary.Add(x.ID, x));
    }

    public void CropPuzzleImage(
        int imageId,
        out List<Sprite> puzzleImages,
        float croppedImagesSize = 250f,
        float fullImageSize = 750f
    )
    {
        puzzleImages = new List<Sprite>();

        string imageName = PuzzleImagesDictionary[imageId].FileName;
        Sprite fullImage = ResourcesManager.LoadImage("PuzzleImages", "SPR_" + imageName);

        if (fullImage == null)
        {
            Debug.LogError($"IMAGES MANAGER: puzzle image with id {imageId} not found!");
            return;
        }

        Rect fullRect = fullImage.rect;
        float ppu = fullImage.pixelsPerUnit;

        int cols = Mathf.Max(1, Mathf.RoundToInt(fullRect.width / croppedImagesSize));
        int rows = Mathf.Max(1, Mathf.RoundToInt(fullRect.height / croppedImagesSize));

        for (int col = 0; col < cols; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                float x = fullRect.x + col * croppedImagesSize;
                float y = fullRect.y + (rows - 1 - row) * croppedImagesSize;

                Rect tileRect = new Rect(x, y, croppedImagesSize, croppedImagesSize);

                Sprite tile = Sprite.Create(
                    fullImage.texture,
                    tileRect,
                    new Vector2(0.5f, 0.5f),
                    ppu
                );

                puzzleImages.Add(tile);
            }
        }
    }
}
