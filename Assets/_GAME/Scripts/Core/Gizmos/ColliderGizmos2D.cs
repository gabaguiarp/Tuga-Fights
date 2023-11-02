using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderGizmos2D : MonoBehaviour
{
    public enum DisplayMode { Always, WhenSelected }

    public DisplayMode displayMode = default;
    public Color color = Color.green;
    [Range(0.0f, 1.0f)]
    public float colorAlpha = kDefaultAlpha;

    private Collider2D _collider;

    const float kDefaultAlpha = 0.45f;

    [ContextMenu("Use Default Alpha")]
    void ResetAlpha()
    {
        colorAlpha = kDefaultAlpha;
    }

    void OnDrawGizmos()
    {
        if (displayMode == DisplayMode.Always)
            DisplayGizmos();
    }

    void OnDrawGizmosSelected()
    {
        if (displayMode == DisplayMode.WhenSelected)
            DisplayGizmos();
    }

    void DisplayGizmos()
    {
        if (_collider == null)
            _collider = GetComponent<Collider2D>();

        if (_collider != null && _collider.enabled)
        {
            Gizmos.color = GetColorWithAlpha();

            if (_collider is BoxCollider2D)
            {
                BoxCollider2D box = (BoxCollider2D)_collider;
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(Vector2.zero + box.offset, box.size);
            }
            else if (_collider is CircleCollider2D)
            {
                CircleCollider2D circle = (CircleCollider2D)_collider;
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawSphere(Vector2.zero + circle.offset, circle.radius);
            }
        }
    }

    Color GetColorWithAlpha()
    {
        Color col = color;
        col.a = colorAlpha;
        return col;
    }
}
