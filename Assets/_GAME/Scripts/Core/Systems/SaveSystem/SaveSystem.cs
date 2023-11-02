using UnityEngine;

namespace MemeFight
{
    public class SaveSystem
    {
        public static string SaveDirectory
        {
            get
            {
#if UNITY_EDITOR
                return "SaveData/";
#else
                return Application.persistentDataPath;
#endif
            }
        }

        public const string SaveDataFileName = "PlayerData.sav";

        /// <summary>
        /// Initializes the system, while clearing the <see cref="PersistentDataSO"/> container.
        /// </summary>
        /// <param name="loadData">Whether to load any existing save file immediately upon initialization.</param>
        public static void Init(bool loadData = true)
        {
            ResourcesManager.PersistentData.Clear();
            Debug.Log("[SAVE SYSTEM] Save System initialized");

            if (loadData)
                LoadData();
        }

        public static void LoadData()
        {
            if (!GameManager.GlobalSettings.LoadData)
            {
                Debug.LogWarning("[SAVE SYSTEM] Not loading data because the load data option is disabled in the game settings.");
                return;
            }

            if (!FileManager.FileExists(SaveDataFileName, SaveDirectory))
            {
                Debug.Log("[SAVE SYSTEM] No save data to load.");
                return;
            }

            if (FileManager.LoadFromFile(SaveDataFileName, SaveDirectory, out object result))
            {
                PlayerData data = new PlayerData();
                JsonUtility.FromJsonOverwrite(result.ToString(), data);
                ResourcesManager.PersistentData.RebuildFromReference(data);

                Debug.Log("[SAVE SYSTEM] Save data successfully loaded!");
            }
            else
            {
                Debug.LogWarning("[SAVE SYSTEM] Failed to load save data!");
            }
        }

        public static void SaveData()
        {
            if (!GameManager.GlobalSettings.SaveData)
            {
                Debug.LogWarning("[SAVE SYSTEM] Not saving data because the save data option is disabled in the game settings.");
                return;
            }

            string data = JsonUtility.ToJson(ResourcesManager.PersistentData.GetPlayerData());
            if (FileManager.WriteToFile(SaveDataFileName, SaveDirectory, data))
            {
                Debug.Log("[SAVE SYSTEM] Data successfully saved!");
            }
            else
            {
                Debug.LogWarning("[SAVE SYSTEM] Failed to save data to file!");
            }
        }

        public static void DeleteSaveData()
        {
            if (!FileManager.FileExists(SaveDataFileName, SaveDirectory))
                return;

            if (FileManager.DeleteFile(SaveDataFileName, SaveDirectory))
            {
#if UNITY_EDITOR
                string metaFileName = SaveDataFileName + ".meta";
                if (FileManager.FileExists(metaFileName, SaveDirectory))
                {
                    FileManager.DeleteFile(metaFileName, SaveDirectory);
                }
#endif
                Debug.Log("[SAVE SYSTEM] Save data successfully deleted");
            }
            else
            {
                Debug.LogWarning("[SAVE SYSTEM] Failed to delete save data!");
            }
        }
    }
}
