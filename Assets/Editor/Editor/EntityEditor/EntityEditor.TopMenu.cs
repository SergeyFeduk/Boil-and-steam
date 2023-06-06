using UnityEngine;
using UnityEditor;

namespace EntityEditor {
    public class TopMenu {
        //Get this into standalone class
        Color outline = new Color(0.2f, 0.2f, 0.2f, 1f);
        Color darkBackground = new Color(0.102f, 0.102f, 0.102f);
        Color background = new Color(0.15f, 0.15f, 0.15f, 1f);
        public TopMenu(EntityEditor editor, float offset, float saveButtonSize) {
            this.editor = editor;
            this.offset = offset;
            this.saveButtonSize = saveButtonSize;
        }
        public float saveButtonSize { get; set; }
        private float offset { get; set; }
        private EntityEditor editor { get; set; }
        public void Draw(float width) {
            Rect pathArea = new Rect(offset, offset, width - saveButtonSize - offset, 20);
            Rect pathTextArea = new Rect(offset + 5, offset, width - saveButtonSize - offset - 5, 20);
            Handles.DrawSolidRectangleWithOutline(pathArea, background, outline);
            if (GUI.Button(pathTextArea, editor.path ?? "None", LabelButtonStyle()) && editor.path != null) {
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(editor.path, typeof(UnityEngine.Object));
                Selection.activeObject = obj;
            }

            if (GUI.Button(new Rect(width - saveButtonSize + offset, offset, saveButtonSize, 20), "Save")) {
                editor.SaveEntity(editor.path);
            }

            Handles.DrawSolidRectangleWithOutline(new Rect(width + 2 * offset, 0, 2, Screen.height), darkBackground, outline);
        }
        //Same as DAssetDatabase should be moved into separate class
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
    }
}
