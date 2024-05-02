using UnityEngine;

namespace MemeFight.UI
{
    public class RectSizeConstraintor : MonoBehaviour
    {
        public bool constainWidth = true;
        public float maxWidth = 1920;
        public bool constrainHeight = true;
        public float maxHeight = 1080;

        void Awake()
        {
            if (TryGetComponent(out RectTransform rect))
            {
                if (constainWidth && rect.rect.width > maxWidth)
                    rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth);

                if (constrainHeight && rect.rect.height > maxHeight)
                    rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxHeight);
            }
        }
    }
}
