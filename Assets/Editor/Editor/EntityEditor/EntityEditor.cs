using UnityEngine;
using UnityEditor;

namespace EntityEditor {
    public class EntityEditor : EditorWindow {
        //Get this into standalone class
        Color outline = new Color(0.2f, 0.2f, 0.2f, 1f);
        Color darkBackground = new Color(0.102f, 0.102f, 0.102f);
        Color background = new Color(0.15f, 0.15f, 0.15f, 1f);

        private readonly EntitySerializer serializer = new EntitySerializer();

        public Entity entity { get; set; }
        public string path { get; set; }

        private float offset = 10;
        private float inspectorSize = 400;

        public Inspector inspector { get; set; }
        public TopMenu topMenu { get; set; }
        public BottomMenu bottomMenu { get; set; }
        public DragHandler dragHandler { get; set; }

        public static void StaticInit(string path) {
            EntityEditor window = StaticInit();
            window.LoadEntity(path);
        }

        [MenuItem("Window/Entity editor")]
        public static EntityEditor StaticInit() {
            EntityEditor window = (EntityEditor)GetWindow(typeof(EntityEditor));
            window.minSize = new Vector2(700, 500);
            window.maxSize = new Vector2(800, 900);
            window.titleContent = new GUIContent("Entity editor");
            window.Show();
            window.Init();
            return window;
        }

        public void Init() {
            float saveButtonSize = 60;
            float bottomOffset = offset * 2 + 40;
            inspector = new Inspector(this, inspectorSize, offset);
            topMenu = new TopMenu(this, offset, saveButtonSize);
            bottomMenu = new BottomMenu(this, bottomOffset, saveButtonSize, offset);
            dragHandler = new DragHandler(this);
        }

        private void OnGUI() {
            float previewSize = Mathf.Min(Screen.width - 2 * offset, Screen.width - inspectorSize);
            topMenu.Draw(previewSize);
            bottomMenu.Draw(previewSize);
            inspector.Draw();
            dragHandler.Handle();
            //Get this into standalone class
            Rect previewRect = new Rect(offset, 2 * offset + 20, previewSize, previewSize);
            Handles.DrawSolidRectangleWithOutline(previewRect, darkBackground, outline);
        }

        public void HandlePath(string newPath) {
            if (newPath.Substring(newPath.LastIndexOf('.') + 1) != "entity") return;
            path = newPath;
            LoadEntity(path);
        }

        private void LoadEntity(string path) {
            if (path == null || path.Length == 0) return;
            string data = FileManager.LoadString(path);
            this.path = path;
            object obj = serializer.Deserialize(data);
            entity = obj as Entity;
        }
        public void SaveEntity(string path) {
            if (path == null || path.Length == 0 || entity == null) return;
            string data = serializer.Serialize(entity);
            FileManager.SaveString(path, data);
            AssetDatabase.Refresh();
        }

        private void OnDestroy() {
            SaveEntity(path);
        }
    }
}