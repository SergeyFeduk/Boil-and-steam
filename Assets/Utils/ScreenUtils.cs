using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class ScreenUtils {
    public static Camera cam = null;
    public static Vector2 WorldMouse() {
        return GetCamera().ScreenToWorldPoint(Input.mousePosition);
    }

    public static Vector2 WorldPosition(Vector2 position) {
        return GetCamera().ScreenToWorldPoint(position);
    }

    public static bool IsPositionOnScreen(Vector2 position) {
        Vector2 ScreenPosition = GetCamera().WorldToScreenPoint(position);
        return ScreenPosition.x > 0 && ScreenPosition.x < cam.pixelWidth &&
               ScreenPosition.y > 0 && ScreenPosition.y < cam.pixelHeight;
    }

    public static bool IsMouseOverUI() {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);
        int count = raycastResults.Count;
        //There goes implementation of ignorable interface detection
        return count > 0;
    }

    public static List<GameObject> GetObjectsUnderMouse() {
        RaycastHit2D[] hits = Physics2D.RaycastAll(WorldMouse(), cam.transform.position - (Vector3)WorldMouse());
        return hits.Select(i => i.collider.gameObject).ToList();
    }

    private static Camera GetCamera() {
        if (cam == null) { CacheCamera(Camera.main); }
        return cam;
    }

    private static void CacheCamera(Camera newCamera) {
        cam = newCamera;
    }
}
