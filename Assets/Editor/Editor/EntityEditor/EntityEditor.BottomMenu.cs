using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEditor;
using UnityDropdown.Editor;

namespace EntityEditor {
    public class BottomMenu {
        //Get this into standalone class
        Color outline = new Color(0.2f, 0.2f, 0.2f, 1f);
        Color darkBackground = new Color(0.102f, 0.102f, 0.102f);
        Color background = new Color(0.15f, 0.15f, 0.15f, 1f);
        public BottomMenu(EntityEditor editor,float bottomOffset, float saveButtonSize, float offset) {
            this.editor = editor;
            this.saveButtonSize = saveButtonSize;
            this.bottomOffset = bottomOffset;
            this.offset = offset;
            BuildCreateDropdown();
        }
        public float saveButtonSize { get; set; }
        private float offset { get; set; }
        private float bottomOffset { get; set; }
        private Type selectedType { get; set; }
        private EntityEditor editor { get; set; }
        private readonly List<DropdownItem<Type>> dropdownItems = new List<DropdownItem<Type>>();
        private static List<Type> entityTypes = new List<Type>();
        public void Draw(float previewSize) {
            Handles.DrawSolidRectangleWithOutline(new Rect(0, Screen.height - bottomOffset, previewSize + 2 * offset, 2), darkBackground, outline);
            Rect refreshIconRect = new Rect(offset, Screen.height + offset - bottomOffset, 20, 18);
            if (GUI.Button(refreshIconRect, "R")) {
                RegenerateEntityTypes();
            }

            Rect typeSelectorRect = new Rect(2 * offset + 20, Screen.height + offset - bottomOffset, previewSize - saveButtonSize - 2 * offset - 20, 20);
            if (EditorGUI.DropdownButton(typeSelectorRect, new GUIContent(selectedType != null ? selectedType.Name : "None"), FocusType.Keyboard)) {
                DropdownMenu<Type> dropdownMenu = new DropdownMenu<Type>(dropdownItems, (i) => { SetCreateType(i); });
                dropdownMenu.ShowAsContext();
            }

            if (GUI.Button(new Rect(previewSize - saveButtonSize + offset, Screen.height + offset - bottomOffset, saveButtonSize, 18), "Create")) {
                CreateEntity();
            }
        }

        private void SetCreateType(Type type) {
            selectedType = type;
            editor.Repaint();
        }

        private void BuildCreateDropdown() {
            if (entityTypes.Count == 0) {
                RegenerateEntityTypes();
            }

            for (int i = 0; i < entityTypes.Count; i++) {
                dropdownItems.Add(new DropdownItem<Type>(entityTypes[i], entityTypes[i].Name));
            }
        }

        private void RegenerateEntityTypes() {
            entityTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(Entity))).ToList();
        }
        private void CreateEntity() {
            if (selectedType == null) return;
            if (editor.entity != null) {
                int result = EditorUtility.DisplayDialogComplex("Entity already set", "Do you want to replace current entity?", "Yes", "No", "Save current and replace");
                if (result == 1) {
                    return;
                }
                if (result == 2) {
                    editor.SaveEntity(editor.path);
                }
            }
            string createPath = EditorUtility.SaveFilePanelInProject("Create new entity", selectedType.Name, "entity", "");
            editor.entity = Activator.CreateInstance(selectedType) as Entity;
            editor.entity.Init();
            editor.path = createPath;
            editor.SaveEntity(editor.path);
            editor.Repaint();
        }
    }
}
