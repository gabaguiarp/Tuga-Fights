#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
#else
using UnityEngine;
#endif

namespace MemeFight
{
    public class PlatformManager
    {
#if UNITY_EDITOR
        public static NamedBuildTarget CurrentBuildTarget => NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
#endif

        public static bool IsStandalone
        {
            get
            {
#if UNITY_EDITOR
                return EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows ||
                       EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows64 ||
                       EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneOSX ||
                       EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneLinux64;
#else
            return Application.platform == RuntimePlatform.WindowsPlayer ||
                   Application.platform == RuntimePlatform.OSXPlayer ||
                   Application.platform == RuntimePlatform.LinuxPlayer;
#endif
            }
        }

        public static bool IsMobile
        {
            get
            {
#if UNITY_EDITOR
                return EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android ||
                       EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS;
#else
            return Application.platform == RuntimePlatform.Android ||
                   Application.platform == RuntimePlatform.IPhonePlayer;
#endif
            }
        }

        public static void Init()
        {
            GameManager.SetTargetFrameRate(GetTargetFrameRateForCurrentPlatform());
        }

        static int GetTargetFrameRateForCurrentPlatform()
        {
            var settings = GameManager.GlobalSettings;
            if (!settings.UseGlobalTargetFrameRateForAllPlatforms)
            {
                if (IsStandalone)
                {
                    return settings.TargetFrameRateStandalone;
                }
                else
                {
#if UNITY_ANDROID
                    return settings.TargetFrameRateAndroid;
#elif UNITY_IOS
                    return settings.TargetFrameRateIOS;
#endif
                }
            }

            return settings.TargetFrameRateGlobal;
        }
    }
}
