using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

public class DAssetDatabase : AssetPostprocessor {
    private static Dictionary<int, string> database = new Dictionary<int, string>();
    public static UnityEvent databaseUpdated = new UnityEvent();
    private static int maxKey = 0;
    private static readonly List<Type> referenceTypes = new List<Type>() {
        typeof(ScriptableObject),
        typeof(Sprite),
        //typeof(Texture2D),
    };
    private static readonly string[] assetsFolders = new[] { "Assets/Data", "Assets/Game", "Assets/Editor", "Assets/Utils" };
    #region Editor

    public static Dictionary<int, string> GetDatabase() {
        LoadDatabase();
        return database;
    }

    public static void Clear() {
        File.Delete(GetPath());
        database.Clear();
    }

    public static void Regenerate() {
        LoadDatabase();
        for (int i = 0; i < referenceTypes.Count; i++) {
            Type type = referenceTypes[i];
            IList objList;
            List<string> paths;
            (objList, paths) = FindAssetsOfType(type);
            for (int k = 0; k < objList.Count; k++) {
                HandleAsset(type, paths[k], false);
            }
        }
        databaseUpdated.Invoke();
        SaveDatabase();
    }

    public static void Fit() {
        LoadDatabase();
        List<int> keys = new List<int>(database.Keys);
        for (int i = 0; i < keys.Count; i++) {
            string value = database[keys[i]];
            database.Remove(keys[i]);
            database.Add(i, value);
        }
        databaseUpdated.Invoke();
        SaveDatabase();
    }

    public static void RemoveAt(int index) {
        database.Remove(index);
        SaveDatabase();
    }

    #endregion
    #region Getters
    public static T GetAssetByKey<T>(int key) where T : UnityEngine.Object {
        LoadDatabase();
        string path = database[key];
        UnityEngine.Object[] objects = LoadAllAssetsAtPath(path);
        return objects[0] as T;
    }

    public static object[] GetAssetsByKey(int key) {
        LoadDatabase();
        string path = database[key];
        UnityEngine.Object[] objects = LoadAllAssetsAtPath(path);
        return objects;
    }

    public static object GetAssetByKey(int key) {
        LoadDatabase();
        string path = database[key];
        UnityEngine.Object[] objects = LoadAllAssetsAtPath(path);
        return objects[0];
    }

    public static int GetKeyByAsset(UnityEngine.Object asset) {
        string path = AssetDatabase.GetAssetPath(asset);
        bool contains = database.ContainsValue(path);
        if (contains) {
            int key = database.FirstOrDefault(i => i.Value == path).Key;
            return key;
        }
        HandleAsset(asset.GetType(), path, false);
        SaveDatabase();
        return maxKey;
    }

    public static int GetKeyByPath(string path, System.Type type) {
        bool contains = database.ContainsValue(path);
        if (contains) {
            int key = database.FirstOrDefault(i => i.Value == path).Key;
            return key;
        }
        HandleAsset(type, path, false);
        SaveDatabase();
        return maxKey;
    }
    #endregion
    #region Internal
    [MenuItem("Assets/Add Asset to database")]
    private static void AddAssetToDatabase() {
        LoadDatabase();
        UnityEngine.Object[] assets = Selection.objects;
        for (int i = 0; i < assets.Length; i++) {
            string path = AssetDatabase.GetAssetPath(assets[i]);
            HandleAsset(assets[i].GetType(), path, true);
        }
        SaveDatabase();
    }

    private static void OnPostprocessAllAssets(System.String[] importedAssets, System.String[] deletedAssets, System.String[] movedAssets, System.String[] movedFromAssetPaths) {
        LoadDatabase();
        for (int i = 0; i < movedAssets.Length; i++) {
            if (!database.ContainsValue(movedFromAssetPaths[i])) {
                HandleObjectsAddition(importedAssets, i);
                continue;
            }
            KeyValuePair<int, string> kvp = database.First(v => v.Value == movedFromAssetPaths[i]);
            string value = movedAssets[i];
            database.Remove(kvp.Key);
            database.Add(kvp.Key, value);
        }
        for (int i = 0; i < importedAssets.Length; i++) {
            if (movedAssets.Contains(importedAssets[i])) continue; // Renamed assets are handled by movement cycle, so no need to handle them again
            HandleObjectsAddition(importedAssets, i);
        }
        for (int i = 0; i < deletedAssets.Length; i++) {
            if (!database.ContainsValue(deletedAssets[i])) continue;
            int key = database.First(v => v.Value == deletedAssets[i]).Key;
            database.Remove(key);
        }
        SaveDatabase();
    }

    private static void HandleObjectsAddition(string[] importedAssets, int index) {
        UnityEngine.Object[] objects = LoadAllAssetsAtPath(importedAssets[index]);
        for (int i = 0; i < objects.Length; i++) {
            UnityEngine.Object asset = objects[i];
            HandleAsset(asset.GetType(), importedAssets[index], false);
        }
    }

    private static void HandleAsset(Type type, string path, bool printError) {
        if (!IsAssetReferenceTyped(type)) return;
        if (IsPathInDatabase(path)) {
            if (printError) Debug.Log("Asset is already in database");
            return;
        }
        AddToDatabase(path);
        databaseUpdated.Invoke();
    }

    private static void AddToDatabase(string path) {
        database.Add(GetEmptyKey(), path);

    }

    private static void SaveDatabase() {
        BinaryFormatter formatter = new BinaryFormatter();
        if (!Directory.Exists(Application.persistentDataPath + "/database")) {
            Directory.CreateDirectory(Application.persistentDataPath + "/database");
        }
        DAssetSaveData saveData = new DAssetSaveData(database, maxKey);

        FileStream file = File.Create(GetPath());
        formatter.Serialize(file, saveData);
        file.Close();
    }

    private static void LoadDatabase() {
        if (!File.Exists(GetPath())) {
            database.Clear();
            return;
        }
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Open(GetPath(), FileMode.Open);
        try {
            object saveDataObj = formatter.Deserialize(file);
            DAssetSaveData saveData = saveDataObj as DAssetSaveData;
            database = saveData.database;
            maxKey = saveData.key;
            file.Close();
        } catch {
            Debug.LogError("Failed to load asset database");
            file.Close();
        }
    }

    private static string GetPath() {
        return Application.persistentDataPath + "/database/assetDatabase.dat";
    }
    #endregion
    #region Helpers
    public static bool IsAssetReferenceTyped(Type type) {
        for (int i = 0; i < referenceTypes.Count; i++) {
            bool assignable = type.IsSubclassOf(referenceTypes[i]) || type == referenceTypes[i];
            if (assignable) return true;
        }
        return false;
    }

    public static bool IsPathInDatabase(string path) {
        return database.Any(i => i.Value == path);
    }

    public static int GetEmptyKey() {
        for (int i = 0; i <= maxKey; i++) {
            if (!database.TryGetValue(i, out _)) {
                return i;
            }
        }
        maxKey++;
        return maxKey;
    }

    public static (List<object>, List<string>) FindAssetsOfType(Type type) {
        List<object> assets = new List<object>();
        List<string> paths = new List<string>();
        string[] guids = AssetDatabase.FindAssets(string.Format("t:{0} a:assets", type), assetsFolders);
        for (int i = 0; i < guids.Length; i++) {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            object asset = AssetDatabase.LoadAssetAtPath(assetPath, type);
            if (asset != null) {
                assets.Add(asset);
                paths.Add(assetPath);
            }
        }
        return (assets,paths);
    }
    #endregion

    [Serializable]
    private class DAssetSaveData {
        public Dictionary<int, string> database;
        public int key;
        public DAssetSaveData(Dictionary<int, string> database, int key) {
            this.database = database;
            this.key = key;
        }
    }

    //Put this in utils
    public static UnityEngine.Object[] LoadAllAssetsAtPath(string assetPath) {
        return typeof(SceneAsset).Equals(AssetDatabase.GetMainAssetTypeAtPath(assetPath)) ?
            // prevent error "Do not use readobjectthreaded on scene objects!"
            new[] { AssetDatabase.LoadMainAssetAtPath(assetPath) } :
            AssetDatabase.LoadAllAssetsAtPath(assetPath);
    }
}
