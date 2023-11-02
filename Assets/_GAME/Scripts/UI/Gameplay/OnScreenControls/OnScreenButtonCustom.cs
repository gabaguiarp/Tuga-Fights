using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MemeFight.UI.OnScreenControls
{
    public class OnScreenButtonCustom : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public event UnityAction OnPress;
        public event UnityAction OnHeldDown;
        public event UnityAction OnRelease;

        [SerializeField] bool _triggerEventsOnPointerPress = true;
        [SerializeField] bool _triggerEventsOnPointerEnter = true;
        [SerializeField] Image _overlayGraphic;

        bool _isPressed = false;

        void Awake()
        {
            SetOverlayEnabled(false);
        }

        void Update()
        {
            if (_isPressed)
                OnHeldDown?.Invoke();
        }

        void HandlePointerPressed()
        {
            _isPressed = true;
            OnPress?.Invoke();
            SetOverlayEnabled(true);
        }

        void HandlePointerReleased()
        {
            _isPressed = false;
            OnRelease?.Invoke();
            SetOverlayEnabled(false);
        }

        void SetOverlayEnabled(bool enabled)
        {
            if (_overlayGraphic != null)
                _overlayGraphic.enabled = enabled;
        }

        // ----- EVENT SYSTEM CALLBACKS -----
        public void OnPointerDown(PointerEventData eventData)
        {
            if (_triggerEventsOnPointerPress)
                HandlePointerPressed();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            HandlePointerReleased();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
#if !UNITY_EDITOR
            if (_triggerEventsOnPointerEnter)
                HandlePointerPressed();
#endif
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HandlePointerReleased();
        }
    }
}
