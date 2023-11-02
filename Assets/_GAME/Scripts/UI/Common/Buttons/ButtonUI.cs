using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.EventSystems;

namespace MemeFight.UI
{
    public class ButtonUI : ClickableUI<Button>, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField] protected AudioCueSO _clickSound;

        [Header("References")]
        [SerializeField] protected Image _icon;
        [SerializeField] protected Image _disabledOveraly;
        [SerializeField] protected LocalizedTextUI _text;

        protected Button _button;
        protected ButtonAnimator _btnAnim;

        public bool IsInteractable => _button != null && _button.interactable;

        public LocalizedString Text
        {
            set
            {
                if (_text)
                    _text.UpdateTextString(value);
            }
        }

        public Sprite Icon
        {
            set
            {
                if (_icon && value != null)
                    _icon.sprite = value;
            }
        }

        public event UnityAction OnButtonPressed;
        public event UnityAction OnButtonReleased;

        bool _isPressed;

        protected override void Awake()
        {
            base.Awake();

            _btnAnim = GetComponent<ButtonAnimator>();

            if (TryGetComponent(out _button))
            {
                _button.onClick.AddListener(RaiseClickedEvent);
                _button.onClick.AddListener(PlayClickSound);
            }
            else
            {
                Debug.LogWarning($"Unable to configure {name} because no Button component was found inside!");
            }
        }

        void PlayClickSound()
        {
            if (_clickSound != null)
                AudioManager.Instance.PlaySoundUI(_clickSound, true);
        }

        public void Configure(LocalizedString textString, Sprite icon, bool isEnabled = true)
        {
            if (_text)
                Text = textString;

            if (_icon)
                Icon = icon;

            SetEnabled(isEnabled);
        }

        public void SetEnabled(bool enabled)
        {
            if (_button)
                _button.interactable = enabled;

            if (_disabledOveraly != null)
                _disabledOveraly.enabled = !enabled;
        }

        public void Animate()
        {
            if (_btnAnim)
                _btnAnim.Animate();
        }

        public void Click()
        {
            if (_button)
                _button.onClick.Invoke();
        }

        protected virtual void OnPress() { }
        protected virtual void OnRelease() { }

        #region EventSystem Callbacks
        void HandleButtonPressed()
        {
            if (_isPressed)
                return;

            OnPress();
            OnButtonPressed?.Invoke();
            _isPressed = true;
        }

        void HandleButtonReleased()
        {
            if (!_isPressed)
                return;

            OnRelease();
            OnButtonReleased?.Invoke();
            _isPressed = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            HandleButtonPressed();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            HandleButtonReleased();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HandleButtonReleased();
        }
        #endregion
    }
}
