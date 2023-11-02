using UnityEngine;
using UnityEngine.Localization.Settings;

namespace MemeFight
{
    using Core;
    using Mobile;

    public enum TimeScale { Normal = 1, Paused = 0 }
    public enum GameMode { Campaign, FreeFight }
    public enum Language { Portuguese, English }

    public class GameManager
    {
        #region Public Getters
        public static GlobalGameSettings GlobalSettings => GlobalGameSettings.Instance;
        public static int TargetFrameRate => Application.targetFrameRate;
        public static TimeScale CurrentTimeScale => Time.timeScale > 0 ? TimeScale.Normal : TimeScale.Paused;

        public static bool IsDebugMode
        {
            get
            {
#if DEBUG_MODE
                return true;
#else
                return false;
#endif
            }
        }

        public static Language CurrentLanguage
        {
            get
            {
                if (LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.GetLocale("pt-PT"))
                {
                    return Language.Portuguese;
                }

                return Language.English;
            }
        }
        #endregion

        public static void Init()
        {
            // NOTE: Initialization order matters, here. For instance, the Save System needs to be initialized first, so the
            // ResourcesManager can check if any bonus fighters have been unlocked.
            PlatformManager.Init();
            SaveSystem.Init(true);
            ResourcesManager.Init();
            SettingsManager.Init();
            QuestSystem.Init();

            if (PlatformManager.IsMobile)
            {
                MobileNotifications.Init();
            }

            Debug.Log("GameManager initialized");
        }

        public static void SetTargetFrameRate(int frameRate)
        {
            Application.targetFrameRate = frameRate;
            Debug.Log("Target frame rate set to " + frameRate);
        }

        public static void SetTimeScale(TimeScale scale)
        {
            Time.timeScale = (int)scale;
            AudioListener.pause = scale.Equals(TimeScale.Paused);
        }

        public static void PauseGame(bool pause = true)
        {
            TimeScale targetTimeScale = pause ? TimeScale.Paused : TimeScale.Normal;
            SetTimeScale(targetTimeScale);
        }

        public static void OpenURL(string url)
        {
            Debug.Log("Opening URL: " + url);
            Application.OpenURL(url);
        }

        public static void ExitGame()
        {
            Debug.Log("EXITING GAME...");

#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }
    }
}
