using UnityEngine;
using UnityEngine.UI;

public static class GraphicHelpers
{
    static Color _transparent = new Color(1, 1, 1, 0);

    public static Color TransparentColor => _transparent;

    /// <summary>
    /// Sets the color of the graphic.
    /// </summary>
    /// <param name="graphic">The graphic to affect.</param>
    /// <param name="color">The color to change to.</param>
    /// <param name="ignoreAlpha">Whether the default alpha set for the graphic should be ignored when changing to the new color.</param>
    public static Graphic SetColor(this Graphic graphic, Color color, bool ignoreAlpha = false)
    {
        if (!ignoreAlpha)
            color.a = graphic.color.a;

        graphic.color = color;
        return graphic;
    }

    public static Graphic SetColorAlpha(this Graphic graphic, float alpha)
    {
        Color newColor = graphic.color;
        newColor.a = Mathf.Clamp01(alpha);
        graphic.color = newColor;
        return graphic;
    }

    public static Color CombineColors(Color color1, Color color2)
    {
        Color result = (color1 + color2) / 2;
        return result;
    }

    public static Color GetColorWithAlpha(Color color, float alpha)
    {
        color.a = Mathf.Clamp01(alpha);
        return color;
    }
}
