using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DatabaseEditor : EditorWindow {
    //Get this into standalone class
    Color background = new Color(0.15f, 0.15f, 0.15f, 1f);
    Color outline = new Color(0.15f, 0.15f, 0.15f, 1f);
    static DatabaseEditor window;
    [MenuItem("Window/Database editor")]
    static void Init() {
        window = (DatabaseEditor)GetWindow(typeof(DatabaseEditor));
        window.Show();
        UpdateListener();
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    public static void UpdateListener() {
        //RegenerateWindow();
        if (window == null) return;
        DAssetDatabase.databaseUpdated.AddListener(() => {
            window.Repaint();
        });
    }

    public void OnGUI() {
        
        Dictionary<int, string> database = DAssetDatabase.GetDatabase();

        float offset = 30;

        if (GUI.Button(new Rect(10, 10, 80, 20), "Regenerate")) {
            DAssetDatabase.Regenerate();
        }
        if (GUI.Button(new Rect((Screen.width - 40) / 2, 10, 40, 20), "Fit")) {
            DAssetDatabase.Fit();
        }
        if (GUI.Button(new Rect(Screen.width - 70 - 10, 10, 70, 20), "Clear")) {
            if (EditorUtility.DisplayDialog("Clear database?", "This action will delete database from your disk", "Clear", "Cancel")) {
                DAssetDatabase.Clear();
            }
        }
        HandleRemoveButtons(database, offset);
    }

    private void HandleRemoveButtons(Dictionary<int, string> database, float offset) {
        List<int> removeIndexes = new List<int>();
        foreach (KeyValuePair<int, string> pair in database) {
            int? removeIndex = DrawElement(pair.Key, offset, database);
            if (removeIndex != null) removeIndexes.Add((int)removeIndex);
        }
        for (int i = 0; i < removeIndexes.Count; i++) {
            DAssetDatabase.RemoveAt(removeIndexes[i]);
        }
    }

    private int? DrawElement(int index, float offset, Dictionary<int, string> database) {
        //Key -- path (clickable) -- delete icon
        float keySpace = 40;
        float deleteSpace = 20;

        Rect keyRect = new Rect(10, index * 25 + 10 + offset, keySpace, 20);
        Rect keyLabelRect = new Rect(10, index * 25 + 12 + offset, keySpace, 18);
        Handles.DrawSolidRectangleWithOutline(keyRect, background, outline);
        GUI.Label(keyLabelRect, index.ToString(), LabelCenteredStyle());

        Rect pathRect = new Rect(20 + keySpace, index * 25 + 10 + offset, Screen.width - 40 - keySpace - deleteSpace, 20);
        Rect pathButtonRect = new Rect(25 + keySpace, index * 25 + 12 + offset, Screen.width - 45 - keySpace - deleteSpace, 18);
        Handles.DrawSolidRectangleWithOutline(pathRect, background, outline);
        if (Event.current.type == EventType.MouseMove && pathRect.Contains(Event.current.mousePosition)) {
            EditorGUIUtility.AddCursorRect(SceneView.lastActiveSceneView.position, MouseCursor.Arrow);
        }
        int maxPathLength = 53;
        string pathText = database[index].Length <= maxPathLength ? database[index] : database[index].Substring(0, maxPathLength) + "...";
        if (GUI.Button(pathButtonRect, pathText, LabelButtonStyle())) {
            Object obj = AssetDatabase.LoadAssetAtPath(database[index], typeof(Object));
            Selection.activeObject = obj;
        }

        Rect deleteRect = new Rect(Screen.width - 10 - deleteSpace, index * 25 + 10 + offset, deleteSpace, 20);
        if (GUI.Button(deleteRect, "X", LabelButtonStyle())) {
            return index;
        }
        Handles.DrawSolidRectangleWithOutline(deleteRect, background, outline);
        return null;
    }

    //Get this into standalone class
    private GUIStyle LabelButtonStyle() {
        GUIStyle style = new GUIStyle();
        RectOffset border = style.border;
        border.left = 0;
        border.top = 0;
        border.right = 0;
        border.bottom = 0;
        style.normal.textColor = Color.white;
        return style;
    }

    private GUIStyle LabelCenteredStyle() {
        GUIStyle style = GUI.skin.GetStyle("Label");
        style.alignment = TextAnchor.UpperCenter;
        return style;
    }

    private static void RegenerateWindow() {
        if (window == null) {
            window = GetWindow<DatabaseEditor>();
        }
    }
}
