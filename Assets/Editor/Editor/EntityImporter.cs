using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.AssetImporters;

[ScriptedImporter(version: 1, ext: "entity", AllowCaching = true)]
public class EntityImporter : ScriptedImporter {
    public override void OnImportAsset(AssetImportContext ctx) {
        TextAsset desc = new TextAsset("Serialized instance of an entity");
        Texture2D icon = Resources.Load<Texture2D>("Editor/EntityIcon");
        ctx.AddObjectToAsset("Entity icon", desc, icon);
    }
}
