using MemeFight.UI;
using MemeFight.UI.Popups;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using QuestData = MemeFight.QuestSystem.QuestData;

namespace MemeFight.Menus
{
    using Services;

    public class QuestChecker : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] MainMenuUIController _mainMenuController;
        [SerializeField] PlayerSelectionUIController _playerSelectorController;
        [SerializeField] QuestsPanelUI _questsPanel;
        [SerializeField] ModalWindowTrigger _questRewardPopupTrigger;
        [SerializeField] BonusRewardsPopupTrigger _bonusRewardsPopupTrigger;

        [Header("References")]
        [SerializeField] PersistentDataSO _persistentData;
        [SerializeField] MatchConfigurationSO _bonusFightersMatch;
        [SerializeField] MatchConfigurationSO _freeFightMatch;

        [Header("Broadcasting On")]
        [SerializeField] VoidEventSO _questlineCompleteEvent;

        [Header("Debug")]
        [SerializeField, ReadOnly] bool _wereAllQuestsCompleted = false;

        QuestData[] _activeQuests;

        [field: SerializeField, ReadOnly]
        public bool IsCheckingQuests { get; private set; } = false;
        [field: SerializeField, ReadOnly]
        public bool IsGettingBonusRewards { get; private set; } = false;
        public bool IsGettingRewardForCompletingQuestline => _wereAllQuestsCompleted;

        public event UnityAction OnQuestCheckingStarted;

        void Awake()
        {
            _questsPanel.OnQuestsAnimationComplete += HandleQuestPanelAnimationComplete;
            _questsPanel.OnBellAnimationComplete += HandleBellAnimationComplete;
        }

        #region Event Responders
        void HandleQuestPanelAnimationComplete()
        {
            Debug.Log("Quest panel animation complete");

            _questsPanel.StopBlinkingAnimationForAllDisplays();

            if (_wereAllQuestsCompleted)
            {
                OnAllQuestsCompleted();
            }
            else
            {
                IsCheckingQuests = false;
            }
        }

        void HandleBellAnimationComplete()
        {
            // Show characters unlocked popup
            _questRewardPopupTrigger.OpenWindow();
            IsCheckingQuests = false;
        }
        #endregion

        #region Quest Checking Actions
        void OnQuestProgressionUpdated(bool[] newCompletedQuestsChecker, float questCompletionPct)
        {
            _questsPanel.AnimateQuestCompletion(newCompletedQuestsChecker, questCompletionPct);
            SaveSystem.SaveData();
        }

        void OnAllQuestsCompleted()
        {
            if (!HasReceivedReward())
            {
                Debug.Log("All quests complete!");
                _questlineCompleteEvent.Raise();
                _questsPanel.TriggerBellAnimation();
                RewardSystem.UnlockReward(ResourcesManager.ActiveQuestline.Reward);
                Analytics.RegisterEvent(Analytics.Event.QUESTLINE_COMPLETE);
                Achievements.Unlock(Achievement.QUESTLINE_COMPLETE);
            }
            else
            {
                IsCheckingQuests = false;
            }
        }

        bool HasReceivedReward() => _persistentData.WasRewardUnlocked(ResourcesManager.ActiveQuestline.Reward);
        #endregion

        #region Utility Methods
        /// <summary>
        /// Gets all the active quests from the <see cref="QuestSystem"/>, caching them in a variable to avoid calling the same
        /// method multiple times.
        /// </summary>
        QuestData[] GetQuests()
        {
            if (_activeQuests == null)
                _activeQuests = QuestSystem.GetAllQuests();

            return _activeQuests;
        }

        public bool WasQuestProgressUpdated()
        {
            return _persistentData.QuestProgressPercentage < QuestSystem.GetCompletionPercentage() || ShouldAwardAnyMedal();
        }

        bool ShouldAwardAnyMedal()
        {
            var quests = GetQuests();
            for (int i = 0; i < quests.Length; i++)
            {
                if (quests[i].IsComplete && !_persistentData.IsQuestComplete(quests[i].ID))
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        /// <summary>
        /// Refreshes the Quests Panel display with the latest values registered.
        /// </summary>
        public void RefreshDisplay()
        {
            var quests = GetQuests();
            for (int i = 0; i < quests.Length; i++)
            {
                // Here we check the last registered value for the quest completion in the persistent data.
                // This allows us to setup the displays with the last state of the quest that was shown to the player.
                bool wasQuestMedalAwarded = _persistentData.IsQuestComplete(quests[i].ID);
                _questsPanel.SetupQuestDisplay(i, quests[i], wasQuestMedalAwarded);
            }

            // Display the latest registered quest progress percentage.
            _questsPanel.UpdateProgressBarValue(_persistentData.QuestProgressPercentage, true);
        }

        [ContextMenu("Check Quests")]
        public void CheckQuests()
        {
            Debug.Log("Checking quests...");

            IsCheckingQuests = true;
            OnQuestCheckingStarted?.Invoke();

            string result = "no new quests completed";
            var quests = GetQuests();
            bool[] newCompletedQuestsChecker = new bool[quests.Length];

            _wereAllQuestsCompleted = quests.All(q => q.IsComplete);

            for (int i = 0; i < quests.Length; i++)
            {
                // Here we check the last registered value for the quest completion in the persistent data.
                // This allows us to setup the displays with the last state of the quest that was shown to the player.
                bool wasQuestMedalAwarded = _persistentData.IsQuestComplete(quests[i].ID);
                _questsPanel.SetupQuestDisplay(i, quests[i], wasQuestMedalAwarded);

                // Then, we verify if the quest has actually been completed and update its state in the persistent data.
                // We only consider the quest has been completed if it isn't registered as such in the persistent data.
                // Otherwise, that means the player had already seen the completion state for this quest before, so we ignore it.
                newCompletedQuestsChecker[i] = quests[i].IsComplete && !wasQuestMedalAwarded;
                _persistentData.AddQuest(quests[i].ID, quests[i].IsComplete);
            }

            // Display the latest registered quest progress percentage.
            _questsPanel.UpdateProgressBarValue(_persistentData.QuestProgressPercentage, true);

            bool newQuestsCompleted = newCompletedQuestsChecker.Any(q => q == true);

            // If any new quest was completed, raise the corresponding event.
            if (newQuestsCompleted)
            {
                result = "new quest(s) complete(d)!";
            }

            // If new quests were completed or the last registered bar percentage is lower than the current one, animate the quest
            // panel to display the changes.
            if (newQuestsCompleted || WasQuestProgressUpdated())
            {
                float currentQuestCompletionPct = QuestSystem.GetCompletionPercentage();
                _persistentData.QuestProgressPercentage = currentQuestCompletionPct;
                OnQuestProgressionUpdated(newCompletedQuestsChecker, currentQuestCompletionPct);
            }
            else
            {
                IsCheckingQuests = false;
            }

            Debug.Log("Quest checking complete: " + result);
        }

        public bool CheckBonusRewards()
        {
            if (!IsGettingBonusRewards)
            {
                var rewards = QuestSystem.GetUnlockedBonusRewards();
                if (rewards.Count > 0)
                {
                    StartCoroutine(GiveRewardsAndWaitForProcessCompletion(rewards));
                    return true;
                }
            }

            return false;

            IEnumerator GiveRewardsAndWaitForProcessCompletion(List<RewardID> rewardsList)
            {
                InputManager inputManager = InputManager.Instance;
                IsGettingBonusRewards = true;

                Debug.Log("Getting bonus rewards...");

                yield return CoroutineUtils.GetWaitRealtime(0.5f);

                for (int i = 0; i < rewardsList.Count; i++)
                {
                    RewardSystem.UnlockReward(rewardsList[i]);
                    _bonusRewardsPopupTrigger.TriggerPopupForReward(rewardsList[i]);
                    inputManager.EnableMenusInput();

                    while (_bonusRewardsPopupTrigger.IsPopupOpen)
                    {
                        yield return null;
                    }

                    if (i < rewardsList.LastIndex())
                    {
                        inputManager.DisableInput();
                        yield return CoroutineUtils.GetWaitRealtime(0.1f);
                    }
                }

                Debug.Log("All bonus rewards were given!");

                IsGettingBonusRewards = false;
            }
        }

        #region External Callbacks
        // Called externaly by the bonus match popup
        public void StartBonusFightersMatch()
        {
            Debug.Log("Starting bonus match");
            ConfigureCustomMatchWithBonusFighters();
            _persistentData.GameMode = GameMode.FreeFight;
            _playerSelectorController.LoadMatchScene();

            void ConfigureCustomMatchWithBonusFighters()
            {
                Team opposingTeam = FightersDatabase.GetOpposingTeam(_persistentData.SelectedTeam);
                _freeFightMatch.SetFighterForPlayer(Player.One, _bonusFightersMatch.GetFighterForTeam(_persistentData.SelectedTeam));
                _freeFightMatch.SetFighterForPlayer(Player.Two, _bonusFightersMatch.GetFighterForTeam(opposingTeam));
            }
        }
        #endregion
    }
}
