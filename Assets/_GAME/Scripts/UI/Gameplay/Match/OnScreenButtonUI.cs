using UnityEngine;
using UnityEngine.UI;

namespace MemeFight.UI
{
    public class OnScreenButtonUI : MonoBehaviour
    {
        [SerializeField] Image _maskGraphic;
        [SerializeField] Image _icon;
        [SerializeField] Image _outline;

        const float FadedAlpha = 0.6f;

        public void SetFillValue(float value)
        {
            _maskGraphic.fillAmount = Mathf.Clamp01(value);
            _icon.color = GraphicHelpers.GetColorWithAlpha(_icon.color, GetTargetAlphaFromFillValue(value));
            _outline.color = GraphicHelpers.GetColorWithAlpha(_outline.color, GetTargetAlphaFromFillValue(value));
        }

        float GetTargetAlphaFromFillValue(float fillValue)
        {
            return fillValue >= 1.0f ? 1.0f : FadedAlpha;
        }
    }
}
