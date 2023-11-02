using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace MemeFight
{
    public class SettingsManager
    {
        static AudioManager _cachedAudioManager;
        static AudioManager AudioManager
        {
            get
            {
                if (_cachedAudioManager == null)
                    _cachedAudioManager = AudioManager.Instance;

                return _cachedAudioManager;
            }
        }

        public static void Init()
        {
            try
            {
                var persistentData = ResourcesManager.PersistentData;
                SetMusicOutput(persistentData.Settings.music);
                SetSFXOutput(persistentData.Settings.sfx);
                SetCameraNoise(persistentData.Settings.cameraNoise);
            }
            catch (System.Exception e)
            {
                Debug.LogError("SettingsManager failed to initialize with exception: " + e);
            }
        }

        #region Screen Settings
        public static Resolution[] AvailableScreenResolutions => Screen.resolutions;
        public static Resolution CurrentScreenResolution => Screen.currentResolution;
        public static bool Fullscreen => Screen.fullScreen;

        public static void SetScreenResolution(int resolutionIndex)
        {
            if (AvailableScreenResolutions.IsIndexValid(resolutionIndex))
            {
                Resolution res = AvailableScreenResolutions[resolutionIndex];
                Screen.SetResolution(res.width, res.height, Fullscreen);
                Debug.Log("Screen resolution changed to: " + res.ToString());
            }
            else
            {
                Debug.LogError($"{resolutionIndex} is not a valid screen resolution index! Unable to set screen resolution.");
            }
        }

        public static void SetFullscreen(bool fullscreen)
        {
            // NOTE: A full-screen switch does not happen immediately; it happens when the current frame is finished.
            // Docs: https://docs.unity3d.com/ScriptReference/Screen-fullScreen.html
            Screen.fullScreen = fullscreen;
            Debug.Log("Fullscreen: " + fullscreen);
        }

        #endregion

        #region Audio Settings
        public static void SetMusicOutput(bool isOn)
        {
            AudioManager.EnableMusicOutput(isOn);
            ResourcesManager.PersistentData.Settings.music = isOn;
        }

        public static void SetSFXOutput(bool isOn)
        {
            AudioManager.EnableSFXOutput(isOn);
            ResourcesManager.PersistentData.Settings.sfx = isOn;
        }
        #endregion

        #region Language Settings
        public static void SetLanguage(Locale locale)
        {
            LocalizationSettings.SelectedLocale = locale;
        }
        #endregion

        #region Camera Settings
        static bool _cameraNoiseEnabled = true;
        public static bool CameraNoiseEnabled => _cameraNoiseEnabled;

        public static event UnityAction OnCameraNoiseUpdated;

        public static void SetCameraNoise(bool enabled)
        {
            _cameraNoiseEnabled = enabled;
            ResourcesManager.PersistentData.Settings.cameraNoise = enabled;
            OnCameraNoiseUpdated?.Invoke();
        }
        #endregion
    }
}
