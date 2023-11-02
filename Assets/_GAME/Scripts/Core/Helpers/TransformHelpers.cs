using UnityEngine;

public static class TransformHelpers
{
    static Vector2 V2OneZero = new Vector2(1, 0);
    static Vector2 V2ZeroOne = new Vector2(0, 1);
    static Vector2 V2OneOne = new Vector2(1, 1);

    public static void AnchorToBottomLeft(this RectTransform rect)
    {
        SetRectAnchor(rect, Vector2.zero);
    }

    public static void AnchorToBottomRight(this RectTransform rect)
    {
        SetRectAnchor(rect, V2OneZero);
    }

    public static void AnchorToTopLeft(this RectTransform rect)
    {
        SetRectAnchor(rect, V2ZeroOne);
    }

    public static void AnchorToTopRight(this RectTransform rect)
    {
        SetRectAnchor(rect, V2OneOne);
    }

    static void SetRectAnchor(RectTransform rect, Vector2 anchorPos)
    {
        rect.anchorMin = anchorPos;
        rect.anchorMax = anchorPos;
        rect.pivot = anchorPos;
    }
}
