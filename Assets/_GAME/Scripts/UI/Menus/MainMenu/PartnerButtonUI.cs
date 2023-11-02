using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MemeFight.UI
{
    [RequireComponent(typeof(Button))]
    public class PartnerButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISubmitHandler
    {
        public event UnityAction<string> OnClicked;

        [SerializeField, SpritePreview] Sprite _partnerIcon;
        [SerializeField] string _partnerName = "Partner Name";
        [SerializeField] string _partnerURL;

        [Header("References")]
        [SerializeField] Image _icon;
        [SerializeField] TextMeshProUGUI _nameText;

        RectTransform _rect;
        Vector2 _originalAnchoredPos;

        const int SlideOutFactor = 40;
        const float SlideAnimationDuration = 0.2f;

        void OnValidate()
        {
            if (_icon != null && _partnerIcon != null)
            {
                _icon.sprite = _partnerIcon;
            }

            if (_nameText != null && _partnerName != string.Empty)
            {
                _nameText.text = _partnerName;
            }
        }

        void Awake()
        {
            if (TryGetComponent(out Button btn))
            {
                btn.onClick.AddListener(HandleButtonClicked);
            }

            _rect = GetComponent<RectTransform>();
            _originalAnchoredPos = _rect.anchoredPosition;
        }

        void HandleButtonClicked()
        {
            OnClicked?.Invoke(_partnerURL);
        }

        #region Animation Events
        public void OnPointerEnter(PointerEventData eventData)
        {
            Slide(true);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Slide(true);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            Slide(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Slide(false);
        }

        void Slide(bool slideOut)
        {
            if (DOTween.IsTweening(_rect))
            {
                _rect.DOKill(true);
            }

            int factor = slideOut ? -SlideOutFactor : 0;
            _rect.DOAnchorPosX(_originalAnchoredPos.x + factor, SlideAnimationDuration);
        }
        #endregion
    }
}
