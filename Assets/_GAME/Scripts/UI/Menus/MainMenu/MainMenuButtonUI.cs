using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

namespace MemeFight.UI
{
    public class MainMenuButtonUI : MonoBehaviour
    {
        public event UnityAction OnClicked;

        [SerializeField] ButtonUI _button;

        [Header("Configuration")]
        [SerializeField, SpritePreview] Sprite _buttonIcon;
        [SerializeField] LocalizedString _buttonText;

        void OnValidate()
        {
            ConfigureButton();
        }

        void Awake()
        {
            ConfigureButton();
            _button.OnClicked += () => OnClicked?.Invoke();
        }

        void ConfigureButton()
        {
            if (_button != null)
                _button.Configure(_buttonText, _buttonIcon);
        }
    }
}
