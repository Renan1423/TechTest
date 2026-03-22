using UnityEngine;

public static class ResourcesManager
{
    public static string[] Load(string path)
    {
        TextAsset TA = Resources.Load(path, typeof(TextAsset)) as TextAsset;
        return TA.text.Split(
            new string[] { "\r\n", "\n\r", "\n" },
            System.StringSplitOptions.RemoveEmptyEntries
        );
    }

    public static string LoadAllFile(string path)
    {
        TextAsset TA = Resources.Load(path, typeof(TextAsset)) as TextAsset;
        return TA.text;
    }

    public static Sprite LoadImage(string path, string imageName)
    {
        string completePath = path + "/" + imageName;
        Sprite image = Resources.Load<Sprite>(completePath);

        if (image == null)
            Debug.LogError($"Error to load the sprite: {imageName} on path: {path}");
        return image;
    }
}
