using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace MemeFight.UI
{
    public class SettingsUIController : ManagedBehaviour
    {
        [SerializeField] SettingsUI _settingsScreen;

        [Header("Listening On")]
        [SerializeField] VoidEventSO _openSettingsEvent;

        [Header("Broadcasting On")]
        [SerializeField] VoidEventSO _settingsClosedEvent;

        bool _wereSettingsModified = false;
        AudioManager _audioManager;

        void OnEnable()
        {
            _openSettingsEvent.OnRaised += OpenSettingsScreen;
        }

        void OnDisable()
        {
            _openSettingsEvent.OnRaised -= OpenSettingsScreen;
        }

        protected override void Awake()
        {
            base.Awake();

            _settingsScreen.OnLanguageDropdownValueChanged += SetLanguage;
            _settingsScreen.OnScreenResDropdownValueChanged += SetScreenResolution;
            _settingsScreen.OnFullscreenToggle += SetFullscreen;
            _settingsScreen.OnMusicToggle += SetMusicOutput;
            _settingsScreen.OnSFXToggle += SetSFXOutput;
            _settingsScreen.OnCameraNoiseToggle += SetCameraNoise;

            _settingsScreen.OnCloseButtonClicked += CloseSettingsScreen;
        }

        protected override void OnSceneReady()
        {
            _audioManager = AudioManager.Instance;
            SetupSettingsUI();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ResourcesManager.OnLocalizationInitialized -= SetupLanguageSettings;
        }

        void OpenSettingsScreen()
        {
            _settingsScreen.Open();
        }

        #region UI Setup
        void SetupSettingsUI()
        {
            if (ResourcesManager.WasLocalizationInit)
            {
                SetupLanguageSettings();
            }
            else
            {
                // When playing inside the editor from a random scene, we have to let the LocalizationSettings be initialized
                // asynchronously first, before getting all the available locales
                ResourcesManager.OnLocalizationInitialized += SetupLanguageSettings;
            }

            SetupAudioSettings();
            SetupScreenSettings();
            SetupCameraSettings();
        }

        void SetupLanguageSettings()
        {
            // Generate list of available locales
            List<string> languages = new List<string>();
            int currentLangIndex = -1;

            for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
            {
                var locale = LocalizationSettings.AvailableLocales.Locales[i];

                if (LocalizationSettings.SelectedLocale == locale)
                    currentLangIndex = i;

                languages.Add(locale.LocaleName);
            }

            _settingsScreen.PopulateLanguageToggle(languages, currentLangIndex);
            _settingsScreen.SetLanguageToggleValue(currentLangIndex > 0);
        }

        void SetupAudioSettings()
        {
            _settingsScreen.SetMusicToggleValue(_audioManager.IsMusicEnabled);
            _settingsScreen.SetSFXToggleValue(_audioManager.IsSFXEnabled);
        }

        void SetupScreenSettings()
        {
            // Generate list of available screen resolutions
            List<string> resolutionLabels = new List<string>();
            int currentResIndex = -1;
            Resolution res;

            for (int i = 0; i < SettingsManager.AvailableScreenResolutions.Length; i++)
            {
                res = SettingsManager.AvailableScreenResolutions[i];

                if (res.Equals(SettingsManager.CurrentScreenResolution))
                    currentResIndex = i;

                resolutionLabels.Add($"{res.width}x{res.height}");
            }

            _settingsScreen.PopulateScreenResDrowpdown(resolutionLabels, currentResIndex);

            // Fullscreen
            _settingsScreen.SetFullscreenToggleValue(SettingsManager.Fullscreen);
        }

        void SetupCameraSettings()
        {
            _settingsScreen.SetCameraNoiseToggleValue(SettingsManager.CameraNoiseEnabled);
        }
        #endregion

        #region Settings Event Responders
        void SetLanguage(int localeIndex)
        {
            SettingsManager.SetLanguage(LocalizationSettings.AvailableLocales.Locales[localeIndex]);
        }

        void SetScreenResolution(int resolutionIndex)
        {
            SettingsManager.SetScreenResolution(resolutionIndex);
            _wereSettingsModified = true;
        }

        void SetFullscreen(bool fullscreen)
        {
            SettingsManager.SetFullscreen(fullscreen);
            _wereSettingsModified = true;
        }

        void SetMusicOutput(bool isOn)
        {
            SettingsManager.SetMusicOutput(isOn);
            _wereSettingsModified = true;
        }

        void SetSFXOutput(bool isOn)
        {
            SettingsManager.SetSFXOutput(isOn);
            _wereSettingsModified = true;
        }

        void SetCameraNoise(bool isOn)
        {
            SettingsManager.SetCameraNoise(isOn);
            _wereSettingsModified = true;
        }

        void CloseSettingsScreen()
        {
            _settingsScreen.Close();

            // Save any modified settings
            if (_wereSettingsModified)
                SaveSystem.SaveData();

            _wereSettingsModified = false;

            // Let other instances know that the settings screen was closed
            _settingsClosedEvent.Raise();
        }
        #endregion
    }
}
