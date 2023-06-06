using UnityEngine;
using UnityEditor;

namespace EntityEditor {
    public class DragHandler {
        public DragHandler(EntityEditor editor) {
            this.editor = editor;
        }
        private EntityEditor editor { get; set; }
        public void Handle() {
            Event currentEvent = Event.current;
            Rect dropArea = new Rect(0, 0, Screen.width, Screen.height);
            switch (currentEvent.type) {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(currentEvent.mousePosition)) return;
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (currentEvent.type != EventType.DragPerform) return;
                    DragAndDrop.AcceptDrag();
                    UnityEngine.Object dragged_object = DragAndDrop.objectReferences[0];
                    if (dragged_object.GetType() != typeof(TextAsset)) return;
                    if (editor.entity == null) {
                        editor.HandlePath(AssetDatabase.GetAssetPath(dragged_object));
                    } else {
                        int result = EditorUtility.DisplayDialogComplex("Entity already set", "Do you want to replace current entity?", "Yes", "No", "Save current and replace");
                        if (result == 0) {
                            editor.HandlePath(AssetDatabase.GetAssetPath(dragged_object));
                            break;
                        }
                        if (result == 2) {
                            editor.SaveEntity(editor.path);
                            editor.HandlePath(AssetDatabase.GetAssetPath(dragged_object));
                            break;
                        }
                    }
                    break;
            }
        }
    }
}