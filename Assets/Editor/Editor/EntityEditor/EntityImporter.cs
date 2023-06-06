using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.AssetImporters;
namespace EntityEditor {
    [ScriptedImporter(version: 1, ext: "entity", AllowCaching = true)]
    public class EntityImporter : ScriptedImporter {
        public override void OnImportAsset(AssetImportContext ctx) {
            TextAsset asset = new TextAsset(File.ReadAllText(ctx.assetPath));
            Texture2D icon = Resources.Load<Texture2D>("Editor/EntityIcon");
            ctx.AddObjectToAsset("Entity icon", asset, icon);
            ctx.SetMainObject(asset);
        }

        [OnOpenAssetAttribute(1)]
        public static bool OnEntityOpen(int instanceID, int line) {
            Object instance = EditorUtility.InstanceIDToObject(instanceID);
            string path = AssetDatabase.GetAssetPath(instance);
            string ext = Path.GetExtension(path);
            if (ext == ".entity") {
                EntityEditor.StaticInit(path);
                return true;
            }
            return false;
        }
    }
}