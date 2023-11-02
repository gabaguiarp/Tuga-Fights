using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SpritePreviewAttribute))]
public class SpritePreviewPropertyDrawer : PropertyDrawer
{
    enum Alignment { Left, Right }

    private Alignment _alignment = Alignment.Left;
    private const float TextureSize = 64;

    public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
    {
        if (prop.objectReferenceValue != null)
        {
            return TextureSize;
        }
        else
        {
            return base.GetPropertyHeight(prop, label);
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        if (property.objectReferenceValue != null)
        {
            position.width = EditorGUIUtility.labelWidth;

            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.UpperLeft;

            GUI.Label(position, property.displayName, style);

            if (_alignment == Alignment.Left)
            {
                position.x += position.width;
            }
            else
            {
                position.x = EditorGUIUtility.currentViewWidth - TextureSize - 5;
            }

            position.width = TextureSize;
            position.height = TextureSize;

            property.objectReferenceValue = EditorGUI.ObjectField(position, property.objectReferenceValue, typeof(Sprite), false);
        }
        else
        {
            EditorGUI.PropertyField(position, property, label, true);
        }

        EditorGUI.EndProperty();
    }
}
