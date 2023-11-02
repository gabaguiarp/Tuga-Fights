using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MemeFight.UI
{
    public class FighterSlotUI : MultiplayerButtonUI, ISelectHandler, IDeselectHandler
    {
        [SerializeField] Image _slotImage;
        [SerializeField] Image _highlight;
        [SerializeField] BlinkOverlayGraphicUI _blinkingOverlay;

        readonly Color HighlightTint = new Color(0.85f, 0.85f, 0.85f, 1);
        readonly Color PressedTint = new Color(0.5f, 0.5f, 0.5f, 1);

        public void Configure(Sprite avatar, Color? color = null)
        {
            if (color.HasValue)
                SetColor(color.Value);

            _slotImage.sprite = avatar;
        }

        public void SetColor(Color c)
        {
            ColorBlock colors = ColorBlock.defaultColorBlock;
            colors.normalColor = GraphicHelpers.TransparentColor;
            colors.highlightedColor = GraphicHelpers.CombineColors(c, HighlightTint);
            colors.pressedColor = GraphicHelpers.CombineColors(c, PressedTint);
            colors.selectedColor = c;
            colors.disabledColor = Color.clear;

            Clickable.colors = colors;
        }

        public void Select()
        {
            RaiseClickedEvent();
        }

        public void SetHighlighted(bool highlighted)
        {
            Color targetColor = highlighted ? Clickable.colors.selectedColor : Clickable.colors.normalColor;
            _highlight.CrossFadeColor(targetColor, 0f, true, true);
        }

        public void SetBlinking(bool enabled)
        {
            _blinkingOverlay.gameObject.SetActive(enabled);
        }

        // These methods are driven by the EventSystem
        public void OnSelect(BaseEventData eventData)
        {
            SetBlinking(true);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            SetBlinking(false);
        }
    }
}
