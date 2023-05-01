using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GlobalSettingsSO), true)]
public class GlobalSettingsSOEditor : Editor {
    public override void OnInspectorGUI() {
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();
        DrawPropertiesExcluding(serializedObject, "m_Script");
        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();
    }
}

[CustomEditor(typeof(GlobalSettings))]
public class GlobalSettingsEditor : Editor {
    private Editor editor;
    public override void OnInspectorGUI() {
        serializedObject.Update();
        var main = serializedObject.FindProperty("main");
        //Add check if main is null
        CreateCachedEditor(main.objectReferenceValue, null, ref editor);
        GUIStyle style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
        EditorGUI.LabelField(EditorGUILayout.GetControlRect(false, 20), "Main", style);

        Rect rect = EditorGUILayout.GetControlRect(false, 1);
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        EditorGUILayout.Space();

        editor.OnInspectorGUI();
        serializedObject.ApplyModifiedProperties();
    }
}