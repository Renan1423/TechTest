using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public static class JsonImporter
{
    public static T ImportJson<T>(string path)
    {
        string completePath = Path.Combine(Application.dataPath, path);
        return JsonConvert.DeserializeObject<T>(ResourcesManager.LoadAllFile(completePath));
    }

    public static System.Collections.Generic.List<T> ImportJsonList<T>(
        string path,
        bool debug = false
    )
    {
        var serialized = JsonConvert
            .DeserializeObject<T[]>(ResourcesManager.LoadAllFile(path))
            .ToList();
        if (debug)
            serialized.ForEach(x => Debug.Log(x.ToString()));

        return serialized;
    }
}
