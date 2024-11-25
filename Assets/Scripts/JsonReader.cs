using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class JsonReader
{
    public static T LoadJsonFromFile<T>(string path) where T : class
    {
        if (!File.Exists(path))
        {
            Debug.LogError("Don't Find");
            return null;
        }

        StreamReader sr = new StreamReader(path);
        if (sr == null)
        {
            return null;
        }
        string json = sr.ReadToEnd();

        if (json.Length > 0)
        {
            return JsonUtility.FromJson<T>(json);
        }
        return null;
    }
    public static T LoadJsonFromString<T>(string json) where T : class
    {
        if (json.Length > 0)
        {
            return JsonUtility.FromJson<T>(json);
        }
        return null;
    }
}
