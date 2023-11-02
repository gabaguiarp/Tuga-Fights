using UnityEngine;

namespace MemeFight.DebugSystem
{
    using Services;

    public class MainMenuUIDebug : DebugMenuUIBase
    {
        [Header("References")]
        [SerializeField] SceneReferenceSO _trainingScene;

        public void SaveGameClick()
        {
            SaveSystem.SaveData();
        }

        public void TrainingClick()
        {
            SceneLoader.Instance.LoadScene(_trainingScene);
        }

        public void UnlockAchievementClick()
        {
            //Achievements.Unlock(Achievement.TEST);
            Debug.LogWarning("Test achievement is currently unavailable");
        }

        public void ForceExeptionClick()
        {
            throw new System.Exception("Test exception. Please ignore.");
        }
    }
}
