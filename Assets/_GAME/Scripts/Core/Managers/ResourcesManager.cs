using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;

namespace MemeFight
{
    public class ResourcesManager : CoreManager<ResourcesManager>
    {
        [Header("Resource Assets")]
        [SerializeField] PersistentDataSO _persistentData;
        [SerializeField] FightersRosterSO _fightersRoster;
        [SerializeField] CampaignStreamSO _campaignStream;

        [Space(10)]
        [SerializeField] QuestLineSO[] _mainQuestLines;

        [Space(10)]
        [SerializeField] QuestLineSO[] _bonusQuestLines;

        public static FightersDatabase Fighters { get; private set; } = null;
        public static CampaignDatabase CampaignStream { get; private set; } = null;

        public static bool WasLocalizationInit { get; private set; } = false;
        public static bool ResourcesLoadingDone { get; private set; } = false;

        public static event UnityAction OnLocalizationInitialized;
        public static event UnityAction OnResourcesLoadingComplete;
        public static event UnityAction OnDatabasesRefreshed;

        #region Getters
        public static PersistentDataSO PersistentData
        {
            get
            {
                if (Instance == null)
                    return null;

                return Instance._persistentData;
            }
        }

        public static QuestLineSO ActiveQuestline
        {
            get
            {
                if (Instance == null)
                    return null;

                return Instance._mainQuestLines[PersistentData.ActiveQuestlineIndex];
            }
        }

        public static IReadOnlyList<QuestLineSO> BonusQuestLines
        {
            get
            {
                if (Instance == null)
                    return null;

                return Instance._bonusQuestLines;
            }
        }
        #endregion

        protected override void OnInitialize()
        {
            // This runs first, so the fighters database can get any unlocked bonus fighters
            ManageUnlockedRewards();
            ManagePurchases();

            // Initialize fighters and campaign databases
            InitializeDatabases();

            // Track asynchronous resources loading process
            StartCoroutine(AsyncResourcesLoading());
        }

        void ManageUnlockedRewards()
        {
            foreach (RewardID reward in _persistentData.UnlockedRewards)
            {
                RewardSystem.ClaimReward(reward);
            }
        }

        void ManagePurchases()
        {
            // TODO: claim items from any in-app purchases
        }

        void InitializeDatabases()
        {
            Fighters = new FightersDatabase(_fightersRoster, _persistentData);
            CampaignStream = new CampaignDatabase(_campaignStream, _persistentData);
        }

        IEnumerator AsyncResourcesLoading()
        {
            // Wait for Localization to finish initializing
            yield return LocalizationSettings.InitializationOperation;
            WasLocalizationInit = true;
            OnLocalizationInitialized?.Invoke();

            // Finish the process
            ResourcesLoadingDone = true;
            OnResourcesLoadingComplete?.Invoke();

            Debug.Log("Resources loading complete!");
        }

        /// <summary>
        /// Refreshes the ResourcesManager by regenerating databases.
        /// </summary>
        public static void Refresh()
        {
            if (Instance == null)
            {
                Debug.LogError("Failed to refresh ResourcesManager because no instance was found!");
                return;
            }

            Debug.Log("Refreshing databases...");

            Instance.InitializeDatabases();
            OnDatabasesRefreshed?.Invoke();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Fighters = null;
            CampaignStream = null;
            WasLocalizationInit = false;
            ResourcesLoadingDone = false;
        }
    }
}
