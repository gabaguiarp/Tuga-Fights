using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MemeFight.UI
{
    [RequireComponent(typeof(Slider))]
    public class SliderBarUI : MonoBehaviour
    {
        [SerializeField] Slider _slider;
        [SerializeField] RectTransform _fill;
        [Tooltip("Assign this field only when you have a dedicated sprite to fit within the Fill area, " +
            "if the latter acts as a mask.")]
        [SerializeField] RectTransform _maskedFill;
        [Tooltip("When enabled, the Masked Fill area anchors will be adjusted to fit only vertically. This prevents the Masked " +
            "Fill from stretching horizontally along with the Fill area, whenever its size changes, while also keeping it consistent " +
            "with screen width variations.")]
        [SerializeField] bool _stretchMaskedFillAnchors = true;

        RectTransform _parentRect;
        Vector2 _stretchedSize;

        void Awake()
        {
            _parentRect = GetComponentInParent<RectTransform>();

            if (_maskedFill != null && _stretchMaskedFillAnchors)
            {
                FixFillAnchors();
            }
        }

        void LateUpdate()
        {
            if (_maskedFill != null && _stretchMaskedFillAnchors)
            {
                StretchToFitWithinParentRect();
            }
        }

        void FixFillAnchors()
        {
            bool isLeftToRight = _slider.direction == Slider.Direction.LeftToRight;
            _maskedFill.anchorMin = isLeftToRight ? Vector2.zero : Vector2.right;
            _maskedFill.anchorMax = isLeftToRight ? Vector2.up : Vector2.one;
            _maskedFill.pivot = isLeftToRight ? new Vector2(0.0f, 0.5f) : new Vector2(1.0f, 0.5f);
        }

        void StretchToFitWithinParentRect()
        {
            if (_parentRect == null)
                return;

            _stretchedSize = _maskedFill.sizeDelta;
            _stretchedSize.x = _parentRect.rect.width;
            _maskedFill.sizeDelta = _stretchedSize;
        }

        public void SetMaxValue(float max)
        {
            _slider.maxValue = max;
        }

        public void UpdateDisplay(float value)
        {
            _slider.value = value;
        }

        public void SetDirection(bool leftToRight)
        {
            if (TryGetComponent(out Slider slider))
            {
                slider.direction = leftToRight ? Slider.Direction.LeftToRight : Slider.Direction.RightToLeft;
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SliderBarUI))]
    public class SlideBarUIEditor : Editor
    {
        SerializedProperty _propMaskedFill;

        readonly string[] ExcludedProperties = new string[] { "_stretchMaskedFillAnchors" };

        void OnEnable()
        {
            _propMaskedFill = serializedObject.FindProperty("_maskedFill");
        }

        public override void OnInspectorGUI()
        {
            if (_propMaskedFill.objectReferenceValue)
            {
                DrawDefaultInspector();
            }
            else
            {
                DrawPropertiesExcluding(serializedObject, ExcludedProperties);
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif
}
