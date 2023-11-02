using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MemeFight.EditorTools
{
    using Core;

    public class MemeFightMenu
    {
        const string MenuRoot = "Tuga Fights/";
        const string SavesPath = MenuRoot + "Save Data/";
        const string DebugModeDefine = "DEBUG_MODE";

        #region Debug Mode
#if DEBUG_MODE
        [MenuItem(MenuRoot + "Disable Debug Mode", priority = 20)]
        static void DisableDebugMode() => SetDebugModeEnabled(false);
#else
        [MenuItem(MenuRoot + "Enable Debug Mode", priority = 20)]
        static void EnableDebugMode() => SetDebugModeEnabled(true);
#endif

        static void SetDebugModeEnabled(bool enable)
        {
            PlayerSettings.GetScriptingDefineSymbols(PlatformManager.CurrentBuildTarget, out string[] defines);
            List<string> activeDefines = defines.ToList();

            if (enable)
            {
                if (!activeDefines.Contains(DebugModeDefine))
                    activeDefines.Add(DebugModeDefine);
            }
            else
            {
                if (activeDefines.Contains(DebugModeDefine))
                    activeDefines.Remove(DebugModeDefine);
            }

            PlayerSettings.SetScriptingDefineSymbols(PlatformManager.CurrentBuildTarget, activeDefines.ToArray());
        }
        #endregion

        #region Configuration Assets
        [MenuItem(MenuRoot + "Global Game Settings", priority = 1)]
        static void OpenGameSettings()
        {
            var settings = Resources.Load(GlobalGameSettings.SettingsAssetName);
            SelectAssetOrLogError(settings, "GlobalGameSettings");
        }

        [MenuItem(MenuRoot + "Persistent Data", priority = 1)]
        static void OpenPersistentData()
        {
            var data = Resources.Load("PersistentData");
            SelectAssetOrLogError(data, "PersistentData");
        }
        #endregion

        #region Save Data
        [MenuItem(SavesPath + "Open Saves Folder", priority = 30)]
        static void OpenSavesFolder()
        {
            OpenInExplorer(SaveSystem.SaveDirectory);
        }

        [MenuItem(SavesPath + "Delete Save Data", priority = 30)]
        static void DeleteSaves()
        {
            SaveSystem.DeleteSaveData();
        }
        #endregion

        #region Screenshots
        [MenuItem(MenuRoot + "Capture Screenshot #&c", priority = 31)]
        static void CaptureScreenshot()
        {
            Screenshots.Capture();
        }

        [MenuItem(MenuRoot + "Open Screenshots Folder", priority = 31)]
        static void OpenScreenshotsFolder()
        {
            OpenInExplorer(Screenshots.ScreenshotsDefaultPath);
        }
        #endregion

        #region Helpers
        static void SelectAssetOrLogError(Object asset, string assetName)
        {
            if (asset != null)
            {
                Selection.activeObject = asset;
            }
            else
            {
                Debug.LogErrorFormat("No {0} asset was found!", assetName);
            }
        }

        static void OpenInExplorer(string path)
        {
            if (FileManager.DirectoryExists(path))
            {
                path = path.Replace(@"/", @"\");   // Explorer doesn't like front slashes
                System.Diagnostics.Process.Start("explorer.exe", "/open," + path);
            }
            else
            {
                EditorUtility.DisplayDialog("Directory does not exist", $"Unable to open directory '{path}' because it does " +
                    $"not exist!", "OK");
            }
        }
        #endregion
    }
}
