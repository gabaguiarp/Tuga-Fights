using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace MemeFight.UI
{
    public class SettingsUI : MenuScreenUI
    {
        [Header("References")]
        [SerializeField] ButtonUI _closeButton;
        [SerializeField] SettingsButtonUI _languageButton;
        [SerializeField] SettingsButtonUI _cameraNoiseButton;
        [SerializeField] SettingsButtonUI _musicButton;
        [SerializeField] SettingsButtonUI _sfxButton;
        [SerializeField] ButtonUI _policyButton;
        [SerializeField] ButtonUI _contactButton;
        [SerializeField] Dropdown _screenResDropdown;
        [SerializeField] RowToggle _fullscreenToggle;

        public event UnityAction<int> OnLanguageDropdownValueChanged;
        public event UnityAction<int> OnScreenResDropdownValueChanged;
        public event UnityAction<bool> OnFullscreenToggle;
        public event UnityAction<bool> OnMusicToggle;
        public event UnityAction<bool> OnSFXToggle;
        public event UnityAction<bool> OnCameraNoiseToggle;

        public event UnityAction OnCloseButtonClicked;

        protected override void Awake()
        {
            base.Awake();

            _closeButton.OnClicked += HandleCloseButtonClicked;

            _languageButton.OnValueChanged.AddListener(HandleLanguageValueChanged);
            _cameraNoiseButton.OnValueChanged.AddListener(HandleCameraNoiseToggle);
            _musicButton.OnValueChanged.AddListener(HandleMusicToggle);
            _sfxButton.OnValueChanged.AddListener(HandleSFXToggle);
            _policyButton.OnClicked += HandlePolicyButtonClicked;
            _contactButton.OnClicked += HandleContactButtonClicked;
            _screenResDropdown.onValueChanged.AddListener(HandleScreenResValueChanged);
            _fullscreenToggle.onValueChanged.AddListener(HandleFullscreenToggle);
        }

        protected override void OnBackCommand()
        {
            _closeButton.Click();
        }

        #region Initialization
        public void PopulateLanguageToggle(List<string> options, int currentLangIndex = -1)
        {
            //string label1 = options[0];
            //string label2 = options.Count > 1 ? options[1] : string.Empty;
            //PopulateRowToggle(_languageToggle, label1, label2, currentLangIndex == 0);
        }

        public void PopulateScreenResDrowpdown(List<string> options, int currentResIndex = -1)
        {
            PopulateDropdown(_screenResDropdown, options, currentResIndex);
        }

        void PopulateRowToggle(RowToggle toggle, string label1, string label2, bool isLabel1Selected)
        {
            toggle.Setup(label1, label2);
            toggle.SelectOption(isLabel1Selected);
        }

        void PopulateDropdown(Dropdown dropdown, List<string> options, int currentOptionIndex = -1)
        {
            dropdown.ClearOptions();
            dropdown.AddOptions(options);

            if (currentOptionIndex >= 0 && dropdown.options.IsIndexValid(currentOptionIndex))
            {
                dropdown.SetValueWithoutNotify(currentOptionIndex);
            }
        }
        #endregion

        #region Default Values Control
        public void SetLanguageToggleValue(bool isEnglish) => _languageButton.IsOn = isEnglish;
        public void SetCameraNoiseToggleValue(bool isOn) => _cameraNoiseButton.IsOn = isOn;
        public void SetMusicToggleValue(bool isOn) => _musicButton.IsOn = isOn;
        public void SetSFXToggleValue(bool isOn) => _sfxButton.IsOn = isOn;
        public void SetFullscreenToggleValue(bool isOn) => _fullscreenToggle.SelectOptionWithoutNotify(isOn);
        #endregion

        #region Event Handling
        void HandleLanguageValueChanged(int value)
        {
            OnLanguageDropdownValueChanged?.Invoke(value);
        }

        void HandleCameraNoiseToggle(int value)
        {
            OnCameraNoiseToggle?.Invoke(value == 1);
        }

        void HandleMusicToggle(int value)
        {
            OnMusicToggle?.Invoke(value == 1);
        }

        void HandleSFXToggle(int value)
        {
            OnSFXToggle?.Invoke(value == 1);
        }

        void HandlePolicyButtonClicked()
        {
            if (GameManager.CurrentLanguage == Language.Portuguese)
            {
                GameManager.OpenURL(GameManager.GlobalSettings.PrivacyPolicyURLs.Portuguese);
            }
            else
            {
                GameManager.OpenURL(GameManager.GlobalSettings.PrivacyPolicyURLs.English);
            }
        }

        void HandleContactButtonClicked()
        {
            GameManager.OpenURL(GameManager.GlobalSettings.ContactURL);
        }

        void HandleScreenResValueChanged(int value)
        {
            OnScreenResDropdownValueChanged?.Invoke(value);
        }

        void HandleFullscreenToggle(int value)
        {
            OnFullscreenToggle?.Invoke(value == 0);
        }

        void HandleCloseButtonClicked()
        {
            OnCloseButtonClicked?.Invoke();
        }
        #endregion
    }
}
