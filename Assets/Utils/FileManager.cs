using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FileManager 
{
    public static void SaveString(string path, string data) {
        FileStream file;
        if (File.Exists(path)) file = File.OpenWrite(path);
        else file = File.Create(path);
        using (StreamWriter writer = new StreamWriter(file)) {
            writer.Write(data);
        }
    }

    public static string LoadString(string path) {
        if (!File.Exists(path)) return null;
        return File.ReadAllText(path);
    }
}
