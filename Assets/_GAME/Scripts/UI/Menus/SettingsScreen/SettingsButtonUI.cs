using UnityEngine;
using UnityEngine.Events;

namespace MemeFight.UI
{
    public class SettingsButtonUI : ButtonUI
    {
        [Header("Toggle Icons")]
        [SerializeField] Sprite _onIcon;
        [SerializeField] Sprite _offIcon;

        [SerializeField, ReadOnly] bool _isOn = false;

        public UnityEvent<int> OnValueChanged;

        public bool IsOn
        {
            get { return _isOn; }
            set
            {
                _isOn = value;
                OnValueUpdated(false);
            }
        }

        protected override void OnPress()
        {
            _isOn = !_isOn;
            OnValueUpdated(true);
        }

        void OnValueUpdated(bool raiseEvent)
        {
            if (_isOn && _onIcon != null)
            {
                _icon.sprite = _onIcon;
            }
            else if (!_isOn && _offIcon != null)
            {
                _icon.sprite = _offIcon;
            }

            if (raiseEvent)
                OnValueChanged.Invoke(Logic.BoolToInt(_isOn));
        }
    }
}
