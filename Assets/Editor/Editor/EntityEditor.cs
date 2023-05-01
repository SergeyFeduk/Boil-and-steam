using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEditor;

public class EntityEditor : EditorWindow {
    //Get this into standalone class
    Color background = new Color(0.15f, 0.15f, 0.15f, 1f);
    Color outline = new Color(0.15f, 0.15f, 0.15f, 1f);

    private readonly EntitySerializer serializer = new EntitySerializer();

    private string path;
    private Entity entity;
    private string data;

    [MenuItem("Window/Entity editor")]
    static void StaticInit() {
        EntityEditor window = (EntityEditor)GetWindow(typeof(EntityEditor));
        window.minSize = new Vector2(600, 500);
        window.maxSize = new Vector2(750, 900);
        window.Show();
        window.Init();
    }

    public void Init() {
        entity = new Wall();
        Wall entityWall = (Wall)entity;
        Renderable renderable = entityWall.GetComponent<Renderable>();
        Buildable buildable = entityWall.GetComponent<Buildable>();
        Positioned positioned = entityWall.GetComponent<Positioned>();
        buildable.requirements = PlayerInteractor.bso;
        renderable.sprite = PlayerInteractor.sprite;
        positioned.occupiedPositions = new List<Address>() { new Address(0, 0), new Address(1, 3) };
        positioned.origin = new Address(7, 3);
        //renderable.ySorted = false;
        //entityWall.rotation = 30f;
        //Debug.Log(System.Convert.ToBase64String(positioned.occupiedPositions));
        entity = entityWall;
    }

    private void OnGUI() {
        DrawMain();
    }

    private void DrawMain() {
        float offset = 10;
        float inspectorSize = 300;
        float previewSize = Mathf.Min(Screen.width - 2 * offset, Screen.width - inspectorSize);
        FileDragAndDrop(offset, previewSize);

        /*EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Entity name: ", GUILayout.MaxWidth(100));
        EditorGUILayout.TextField("");
        EditorGUILayout.EndHorizontal();*/

        Rect previewRect = new Rect(offset, 2 * offset + 20, previewSize, previewSize);
        Handles.DrawSolidRectangleWithOutline(previewRect, background, outline);
    }

    private void FileDragAndDrop(float offset, float previewSize) {
        Event currentEvent = Event.current;
        float saveButtonSize = 60;

        Rect dropArea = new Rect(offset, offset, previewSize - 2 * saveButtonSize - 2 * offset, 20);
        Handles.DrawSolidRectangleWithOutline(dropArea, background, outline);
        GUI.Label(dropArea, path ?? "None");
        if (GUI.Button(new Rect(previewSize - saveButtonSize + offset, offset, saveButtonSize, 20), "Save")) {
            SaveEntity();
        }
        if (GUI.Button(new Rect(previewSize - 2 * saveButtonSize, offset, saveButtonSize, 20), "Load")) {
            LoadEntity();
        }

        switch (currentEvent.type) {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(currentEvent.mousePosition))
                    return;
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if (currentEvent.type == EventType.DragPerform) {
                    DragAndDrop.AcceptDrag();
                    Object dragged_object = DragAndDrop.objectReferences[0];
                    path = AssetDatabase.GetAssetPath(dragged_object);
                }
                break;
        }
    }

    private void LoadEntity() {
        object obj = serializer.Deserialize(data);
        Wall wallObj = (Wall)obj;
        Renderable renderable = wallObj.GetComponent<Renderable>();
        Buildable buildable = wallObj.GetComponent<Buildable>();
        Positioned positioned = wallObj.GetComponent<Positioned>();
        Debug.Log(buildable.requirements.name);
        Debug.Log(renderable.sprite.name);
        
        
        Debug.Log(positioned.origin);
        Debug.Log(renderable.ySorted);
        Debug.Log(wallObj.rotation);
        Debug.Log(positioned.occupiedPositions);
        List<Address> adlist = positioned.occupiedPositions;
        for (int i = 0; i < adlist.Count; i++) {
            Debug.Log(adlist[i]);
        }
    }
    private void SaveEntity() {
        data = serializer.Serialize(entity);
        Debug.Log(data);
    }
}
