using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEditor;

public class EntityEditor : EditorWindow {
    //Get this into standalone class
    Color outline = new Color(0.2f, 0.2f, 0.2f, 1f);
    Color darkBackground = new Color(0.102f, 0.102f, 0.102f);
    Color background = new Color(0.15f, 0.15f, 0.15f, 1f);

    private readonly EntitySerializer serializer = new EntitySerializer();

    private string path;
    private Type selectedType;
    private Entity entity;

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
        float bottomOffset = offset * 2 + 40;
        float saveButtonSize = 60;

        TopMenu(offset, previewSize, saveButtonSize);
        BottomMenu(bottomOffset, offset, previewSize, saveButtonSize);

        Rect previewRect = new Rect(offset, 2 * offset + 20, previewSize, previewSize);
        Handles.DrawSolidRectangleWithOutline(previewRect, darkBackground, outline);
    }

    private void TopMenu(float offset, float previewSize, float saveButtonSize) {
        Event currentEvent = Event.current;
        

        Rect pathArea = new Rect(offset, offset, previewSize - saveButtonSize - offset, 20);
        Handles.DrawSolidRectangleWithOutline(pathArea, background, outline);
        GUI.Label(pathArea, path ?? "None");
        if (GUI.Button(new Rect(previewSize - saveButtonSize + offset, offset, saveButtonSize, 20), "Save")) {
            SaveEntity();
        }
        Handles.DrawSolidRectangleWithOutline(new Rect(previewSize + 2 * offset, 0, 2, Screen.height), darkBackground, outline);

        Rect dropArea = new Rect(0, 0, Screen.width, Screen.height);
        switch (currentEvent.type) {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(currentEvent.mousePosition))
                    return;
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if (currentEvent.type == EventType.DragPerform) {
                    DragAndDrop.AcceptDrag();
                    UnityEngine.Object dragged_object = DragAndDrop.objectReferences[0];
                    HandlePath(AssetDatabase.GetAssetPath(dragged_object));
                }
                break;
        }
    }

    private void BottomMenu(float bottomOffset, float offset, float previewSize, float saveButtonSize) {
        Handles.DrawSolidRectangleWithOutline(new Rect(0, Screen.height - bottomOffset, previewSize + 2 * offset, 2), darkBackground, outline);
        Rect typeSelectorArea = new Rect(offset, Screen.height + offset - bottomOffset, previewSize - saveButtonSize - offset, 20);
        Handles.DrawSolidRectangleWithOutline(typeSelectorArea, background, outline);
        GUI.Label(typeSelectorArea, selectedType == null ? "None" : selectedType.ToString());
        if (GUI.Button(new Rect(previewSize - saveButtonSize + offset, Screen.height + offset - bottomOffset, saveButtonSize, 20), "Create")) {
            //SaveEntity();
        }
    }

    private void HandlePath(string newPath) {
        if (newPath.Substring(newPath.LastIndexOf('.') + 1) != "entity") return;
        path = newPath;
        //LoadEntity();
    }

    private void LoadEntity(string data) {
        /*object obj = serializer.Deserialize(data);
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
        }*/
    }
    private void SaveEntity() {
        //data = serializer.Serialize(entity);
        //Debug.Log(data);
    }
}
