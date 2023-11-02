using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Logger = MemeFight.DebugSystem.Logger;

namespace MemeFight
{
    using Services;

    [CreateAssetMenu(fileName = "PersistentData", menuName = MenuPaths.Data + "Persistent Data")]
    public class PersistentDataSO : BaseSO
    {
        [Header("Configuration")]
        [SerializeField] int _defaultCampaignAttempts = 3;

        [Space(10)]
        [Tooltip("The data carried through saves.")]
        [SerializeField] PlayerData _playerData = new PlayerData();
        [Tooltip("All the data stored for the current game session.")]
        [SerializeField] GameplayStream _gameplayStream = new GameplayStream();
        [Tooltip("Contains paramteres stored during the active game session, meant for data collection.")]
        [SerializeField] AnalyticsData _analyticsData = new AnalyticsData();

        #region Global Members
        /// <summary>Whether the current game mode is set to Campaign.</summary>
        public bool IsCampaignMode => GameMode == GameMode.Campaign;
        public Team SelectedTeam => _playerData.profile.selectedTeam;
        public IReadOnlyList<RewardID> UnlockedRewards => _playerData.profile.unlockedRewards.AsReadOnly();
        public IReadOnlyList<FightersBundleID> FighterBundles => _gameplayStream.fighterBundles.AsReadOnly();
        public PlayerData.Settings Settings => _playerData.settings;
        #endregion

        #region Gameplay
        public GameMode GameMode
        {
            get => _gameplayStream.gameMode;
            set => _gameplayStream.gameMode = value;
        }

        public int CampaignAttemptsRemaining
        {
            get => _gameplayStream.campaignAttemptsRemaining;
            set => _gameplayStream.campaignAttemptsRemaining = value;
        }

        public int CurrentCampaignMatchIndex => _gameplayStream.currentCampaignMatchIndex;

        /// <summary>Switches to the team opposite of the currently selected one.</summary>
        public void SwitchTeam() => _playerData.profile.selectedTeam = FightersDatabase.GetOpposingTeam(SelectedTeam);
        public void ProceedCampaign() => _gameplayStream.currentCampaignMatchIndex++;

        [ContextMenu("Reset Campaign")]
        public void ResetCampaign()
        {
            _gameplayStream.currentCampaignMatchIndex = 0;
            _gameplayStream.campaignAttemptsRemaining = _defaultCampaignAttempts;
        }
        #endregion

        #region Stats Methods
        /// <summary>
        /// Adds a new stat or updates the existing one by increasing its value according to the amount defined.
        /// </summary>
        /// <param name="statID">The ID of the stat to add/update.</param>
        /// <param name="value">The amount to update the stat value with.</param>
        /// <returns>The final value of the stat, after adding or incrementing it.</returns>
        public int AddStat(StatID statID, int value)
        {
            StatData stat = null;

            if (GetStat(statID, out stat))
            {
                stat.Value += value;
            }
            else
            {
                stat = new StatData(statID, value);
                _playerData.stats.Add(stat);
            }

            Logger.LogMessageFormat("Stat {0} updated with {1}. Current value is {2}", DebugSystem.MessageType.Default, statID, value,
                                    stat.Value);

            return stat.Value;
        }

        public bool GetStat(StatID statID, out StatData statData)
        {
            statData = _playerData.stats.FirstOrDefault(s => s.ID.Equals(statID));
            return statData != null;
        }
        #endregion

        #region Quests
        public int ActiveQuestlineIndex
        {
            get => _playerData.profile.activeQuestlineIndex;
            set => _playerData.profile.activeQuestlineIndex = Mathf.Max(0, value);
        }

        public float QuestProgressPercentage
        {
            get => _gameplayStream.questProgressPercentage;
            set => _gameplayStream.questProgressPercentage = Mathf.Clamp01(value);
        }

        public bool WasCurrentQuestlineComplete => _gameplayStream.questProgressPercentage >= 1.0f;

        public void AddQuest(string questID, bool isComplete)
        {
            PlayerData.QuestProgress questProgress = _playerData.profile.questProgress.FirstOrDefault(q => q.ID.Equals(questID));
            if (questProgress != null)
            {
                questProgress.IsComplete = isComplete;
            }
            else
            {
                questProgress = new PlayerData.QuestProgress(questID, isComplete);
                _playerData.profile.questProgress.Add(questProgress);
            }
        }

        public bool IsQuestComplete(string questID)
        {
            PlayerData.QuestProgress questProgress = _playerData.profile.questProgress.FirstOrDefault(q => q.ID.Equals(questID));
            if (questProgress != null)
                return questProgress.IsComplete;

            return false;
        }
        #endregion

        #region Reward Methods
        public bool WasRewardUnlocked(RewardID reward)
        {
            return _playerData.profile.unlockedRewards.Contains(reward);
        }

        public void UnlockReward(RewardID reward)
        {
            if (!_playerData.profile.unlockedRewards.Contains(reward))
                _playerData.profile.unlockedRewards.Add(reward);
        }
        #endregion

        #region Bundle Methods
        public void AddFighterBundle(FightersBundleID id)
        {
            if (!_gameplayStream.fighterBundles.Contains(id))
                _gameplayStream.fighterBundles.Add(id);
        }

        public bool ContainsBundle(FightersBundleID id)
        {
            return _gameplayStream.fighterBundles.Contains(id);
        }
        #endregion

        #region Analytics
        public int ActiveSessionRoundsPlayed => _analyticsData.activeSessionRoundsPlayed;
        public int ActiveSessionMatchesPlayed => _analyticsData.activeSessionMatchesPlayed;
        public int ConsecutiveWins => _analyticsData.consecutiveWins;
        public int ConsecutiveLosses => _analyticsData.consecutiveLosses;
        /// <summary>The duration of the latest round played until the end (rounded to seconds)</summary>
        public int LatestRoundDurationRounded => Mathf.RoundToInt(_analyticsData.latestRoundDuration);
        public string LastSelectedFighterLabel
        {
            get => _analyticsData.lastSelectedFighter;
            set => _analyticsData.lastSelectedFighter = value;
        }

        public bool HasOpenedTutorial
        {
            get => _analyticsData.hasOpenedTutorial;
            set => _analyticsData.hasOpenedTutorial = value;
        }

        public void RegisterRoundOver()
        {
            _analyticsData.activeSessionRoundsPlayed++;
        }

        public void RegisterMatchComplete()
        {
            _analyticsData.activeSessionMatchesPlayed++;
            Analytics.RegisterEvent(Analytics.Event.MATCH_COMPLETE);
        }

        public void RegisterPlayerWin(float roundDuration)
        {
            _analyticsData.consecutiveWins++;
            _analyticsData.consecutiveLosses = 0;
            _analyticsData.latestRoundDuration = roundDuration;
            Analytics.RegisterEvent(Analytics.Event.ROUND_WIN);
        }

        public void RegisterPlayerLoss(float roundDuration, string opponentLabel)
        {
            _analyticsData.consecutiveLosses++;
            _analyticsData.consecutiveWins = 0;
            _analyticsData.latestRoundDuration = roundDuration;
            Analytics.RegisterEvent(Analytics.Event.ROUND_LOSS, opponentLabel);
        }
        #endregion

        #region Data Management
        public PlayerData GetPlayerData() => _playerData;
        public void RebuildFromReference(PlayerData reference) => _playerData = reference;

        [ContextMenu("Clear")]
        public void Clear()
        {
            _playerData = new PlayerData();
            
            if (!Application.isPlaying || !GameManager.IsDebugMode)
            {
                _gameplayStream = new GameplayStream();
            }

            _analyticsData = new AnalyticsData();

            Debug.Log("PersistentData cleared.");
        }
        #endregion

        #region Custom Classes
        /// <summary>
        /// Contains all the data that is internally modified throughout each gameplay session.
        /// </summary>
        [Serializable]
        class GameplayStream
        {
            public GameMode gameMode = GameMode.Campaign;
            public int currentCampaignMatchIndex = 0;
            public int campaignAttemptsRemaining = 0;
            public float questProgressPercentage = 0.0f;

            [Space(10)]
            [Tooltip("Lists all the fighter bundles currently unlocked by the player. This is not saved, but instead generated " +
                "when the game starts by loading data from the save file and the in-app purchases database.")]
            public List<FightersBundleID> fighterBundles = new List<FightersBundleID>();
        }

        [Serializable]
        class AnalyticsData
        {
            [Tooltip("The amount of rounds played until the end.")]
            public int activeSessionRoundsPlayed = 0;
            [Tooltip("The amount of matches played until the end.")]
            public int activeSessionMatchesPlayed = 0;
            public int consecutiveWins = 0;
            public int consecutiveLosses = 0;
            public float latestRoundDuration = 0;
            public string lastSelectedFighter = default;
            public bool hasOpenedTutorial = false;
        }
        #endregion
    }
}
