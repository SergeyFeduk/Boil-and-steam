using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RectUtils {
    public static Vector2 GetSize(this RectTransform source) => source.rect.size;
    public static float GetWidth(this RectTransform source) => source.rect.size.x;
    public static float GetHeight(this RectTransform source) => source.rect.size.y;

    public static void SetSize(this RectTransform source, RectTransform toCopy)
    {
        source.SetSize(toCopy.GetSize());
    }

    public static void SetSize(this RectTransform source, Vector2 newSize)
    {
        source.SetSize(newSize.x, newSize.y);
    }

    public static void SetSize(this RectTransform source, float width, float height)
    {
        source.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        source.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

    public static void SetWidth(this RectTransform source, float width)
    {
        source.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }

    public static void SetHeight(this RectTransform source, float height)
    {
        source.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }
}