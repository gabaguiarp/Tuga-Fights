using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace MemeFight
{
    using Audio;
    using Characters;
    using Services;
    using UI;
    using UI.Popups;
    using ControlMode = Characters.PlayerController.ControlMode;

    public class MatchManager : ManagedBehaviour
    {
        enum MatchState
        {
            RoundWarmup,
            Fighting,
            RoundAftermath,
            MatchOver
        }

        [Header("Settings")]
        [Tooltip("Whether to display the match intro panel at the beggining of each match.")]
        [SerializeField] bool _displayIntroPanel = true;
        [SerializeField] List<FighterData> _fighters = new List<FighterData>() { new FighterData(), new FighterData() };

        [Header("UI")]
        [SerializeField] MatchIntroUI _matchIntro;
        [SerializeField] MatchHUDController _matchHUDController;
        [SerializeField] EffectsPanelUI _effectsPanel;
        [SerializeField] CampaignWinnerPopupHandler _campaignWinnerPopupHandler;
        [SerializeField] MatchOverPopupHandler _matchOverPopupHandler;

        [Header("References")]
        [SerializeField] PersistentDataSO _persistentData;
        [SerializeField] MatchConfigurationSO _freeFightConfiguration;
        [SerializeField] VariablesGroupAsset _matchVariables;
        [Tooltip("Used as comparer to check if the current match is a Batatoon match.")]
        [SerializeField] MatchConfigurationSO _batatoonMatch;

        [Header("Audio")]
        [SerializeField] MatchMusicController _musicController;
        [SerializeField] AudioCueSO _bellRingCue;

        [Header("Broadcasting On")]
        [SerializeField] VoidEventSO _matchIntroEvent;
        [SerializeField] VoidEventSO _roundStartEvent;
        [SerializeField] VoidEventSO _roundEndEvent;

        [Header("Listening On")]
        [SerializeField] VoidEventSO _returnToMenuEvent;

        [Header("Info")]
        [SerializeField, ReadOnly] MatchState _currentMatchState;
        [SerializeField, ReadOnly] float _matchTime;
        [SerializeField, ReadOnly] int _totalRounds;
        [SerializeField, ReadOnly] bool _hasFighterBeenKnockedOut;
        [SerializeField, ReadOnly] List<RoundResults> _roundResults = new List<RoundResults>();

        MatchConfigurationSO _currentCampaignMatch;
        bool _matchConfigured;

        AudioManager _audioManager;
        InputManager _inputManager;
        SceneLoader _sceneLoader;

        public const int DefaultMatchRounds = 2;

        #region Localization Variables
        int CurrentRound
        {
            get
            {
                var result = _matchVariables["currentRound"] as IntVariable;
                return result.Value;
            }
            set
            {
                var result = _matchVariables["currentRound"] as IntVariable;
                result.Value = value;
            }
        }

        string WinnerName
        {
            set
            {
                var result = _matchVariables["winnerName"] as StringVariable;
                result.Value = value;
            }
        }
        #endregion

        #region Custom Classes
        [Serializable]
        class FighterData
        {
            public Fighter fighter;
            public Transform pivot;

            [ReadOnly] public FighterProfileSO profile;
            [SerializeField, ReadOnly] int _wins;

            public int Wins => _wins;

            public void AddWin() => _wins++;
        }

        [Serializable]
        struct RoundResults
        {
            public RoundResults(Fighter winner, WinContext context)
            {
                _winner = winner;
                _result = context;
            }

            public enum WinContext { KO, Perfect }

            [SerializeField, ReadOnly] Fighter _winner;
            [SerializeField, ReadOnly] WinContext _result;

            public Fighter Winner => _winner;
            public WinContext Result => _result;
        }
        #endregion

        #region Editor
        void OnValidate()
        {
            if (_fighters.Count > 2)
            {
                _fighters.Trim(2);
                Debug.LogWarning("There cannot be more than 2 fighters in a match!");
            }
        }
        #endregion

        #region Initialization
        void OnEnable()
        {
            _returnToMenuEvent.OnRaised += ReturnToMainMenu;
        }

        void OnDisable()
        {
            _returnToMenuEvent.OnRaised -= ReturnToMainMenu;
        }

        protected override void Awake()
        {
            base.Awake();
            Fighter.OnKnockedOut += HandleFighterKnockedOut;

            if (ResourcesManager.ResourcesLoadingDone)
            {
                PerformMatchSetup();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Fighter.OnKnockedOut -= HandleFighterKnockedOut;
            _matchIntro.OnAnimationComplete -= StartMatch;
        }

        protected override void OnSceneReady()
        {
            _audioManager = AudioManager.Instance;
            _inputManager = InputManager.Instance;
            _sceneLoader = SceneLoader.Instance;

            // Since the match can only be configured after the resources have been successfully loaded, in case we are entering
            // Play Mode from the editor directly in the Match scene we perform the match setup here, because it cannot be
            // performed in Awake, like in a regular gameplay session.
            if (!_matchConfigured)
            {
                PerformMatchSetup();
            }

            if (_displayIntroPanel)
            {
                if (_persistentData.IsCampaignMode && _currentCampaignMatch == null)
                {
                    throw new Exception("Unable to display intro panel because no current campaign match reference was assigned!");
                }

                _matchIntro.Populate(_fighters[0].profile, _fighters[1].profile);
                _matchIntro.OnAnimationComplete += StartMatch;

                _matchIntro.PlayAnimation();
                _musicController.PlayMatchIntroCue();
                _matchIntroEvent.Raise();
            }
            else
            {
                _matchIntro.HidePanel();
                StartMatch();
            }
        }

        void PerformMatchSetup()
        {
            ConfigureMatch();
            _matchHUDController.ConfigureForMatch(DefaultMatchRounds);
        }

        void ConfigureMatch()
        {
            switch (_persistentData.GameMode)
            {
                case GameMode.Campaign:
                    int matchIndex = _persistentData.CurrentCampaignMatchIndex;
                    if (ResourcesManager.CampaignStream.Matches.IsIndexValid(matchIndex))
                    {
                        _currentCampaignMatch = ResourcesManager.CampaignStream.Matches[_persistentData.CurrentCampaignMatchIndex];
                        ConfigureCampaignMatch();
                    }
                    else
                    {
                        Debug.LogError($"Unable to configure campaign match because the match index {matchIndex} is invalid!");
                    }

                    break;

                case GameMode.FreeFight:
                    ConfigureFreeFightMatch();
                    break;
            }

            void ConfigureCampaignMatch()
            {
                try
                {
                    Team opposingTeam = FightersDatabase.GetOpposingTeam(_persistentData.SelectedTeam);
                    var fighterOne = _currentCampaignMatch.GetFighterForTeam(_persistentData.SelectedTeam);
                    var fighterTwo = _currentCampaignMatch.GetFighterForTeam(opposingTeam);
                    ConfigureFighters(fighterOne, fighterTwo);

                    _matchConfigured = true;
                    Debug.Log("Match successfully configured");
                }
                catch (Exception e)
                {
                    Debug.LogError("Campaign match configuration failed with exception: " + e);
                }
            }

            void ConfigureFreeFightMatch()
            {
                ConfigureFighters(_freeFightConfiguration.FighterOne, _freeFightConfiguration.FighterTwo);
                _matchConfigured = true;
                Debug.Log("Match successfully configured");
            }
        }
        #endregion

        #region Match Control
        void Update()
        {
            if (_currentMatchState == MatchState.Fighting)
            {
                _matchTime += Time.deltaTime;
            }
        }

        void StartMatch()
        {
            if (!_matchConfigured)
            {
                Debug.LogWarning("Unable to start match because it was not configured!");
                return;
            }

            Debug.Log("STARTING MATCH...");
            _inputManager.EnableGameplayInput();
            StartCoroutine(MatchLoop());

            if (IsBatatoonMatch())
            {
                Analytics.RegisterEvent(Analytics.Event.BATATOON_MATCH_STARTED);
            }
        }

        IEnumerator MatchLoop()
        {
            CurrentRound = 0;
            _totalRounds = DefaultMatchRounds;

            while (CurrentRound < _totalRounds)
            {
                CurrentRound++;
                _hasFighterBeenKnockedOut = false;
                _matchTime = 0.0f;

                if (CurrentRound != 1)
                {
                    _effectsPanel.TriggerFlashEffect();
                    ResetFighters();
                }

                _currentMatchState = MatchState.RoundWarmup;
                _matchHUDController.HidePromptMessage();

                yield return CoroutineUtils.GetWaitTime(0.8f);  // Warmup delay

                _matchHUDController.DisplayPromptMessage(MatchPromptMessage.RoundNumber);

                yield return CoroutineUtils.GetWaitTime(1.5f);

                StartRound();
                _currentMatchState = MatchState.Fighting;
                _matchHUDController.DisplayPromptMessage(MatchPromptMessage.Fight, true, 1f);

                while (!_hasFighterBeenKnockedOut)
                {
                    yield return null;
                }

                EndRound();
                _currentMatchState = MatchState.RoundAftermath;

                var results = _roundResults[CurrentRound - 1];
                bool isPerfectWin = results.Result.Equals(RoundResults.WinContext.Perfect);
                var resultMsg = isPerfectWin ? MatchPromptMessage.ResultPerfect : MatchPromptMessage.ResultKO;
                _matchHUDController.DisplayPromptMessage(resultMsg, true, 2f, false);

                yield return CoroutineUtils.GetWaitTime(2f);

                if (CurrentRound == _totalRounds && AreWinsTied())
                {
                    _totalRounds++;
                    Debug.Log("WINS ARE TIED! Preparing extra round...");
                }

                results.Winner.Controller.SetVictorious();
                WinnerName = results.Winner.Profile.Name;
                _matchHUDController.DisplayPromptMessage(MatchPromptMessage.Winner, false);

                // Check if the player won the current round, and register analitical data accordingly
                if (IsFighterPlayer(results.Winner))
                {
                    _persistentData.RegisterPlayerWin(_matchTime);

                    if (isPerfectWin)
                    {
                        Analytics.RegisterEvent(Analytics.Event.PERFECT_WIN);
                        Achievements.Unlock(Achievement.PERFECT_WIN);
                    }
                }
                else
                {
                    // Register a loss for the player, while also passing the label of the opponent that has defeated them as a parameter
                    _persistentData.RegisterPlayerLoss(_matchTime, _fighters[1].profile.Label);
                }

                yield return CoroutineUtils.GetWaitTime(3f);
            }

            EndMatch();
            _currentMatchState = MatchState.MatchOver;
        }

        void StartRound()
        {
            SetFightersActive(true);
            TriggerBellRing();

            // Play the match music after the bell ring sound ends
            PlayMatchMusicWithDelay(_bellRingCue.Clip.length - 0.15f);
            _roundStartEvent.Raise();
        }

        void EndRound()
        {
            SetFightersActive(false);
            TriggerBellRing();
            _musicController.StopMatchMusic();
            _matchHUDController.TriggerBellAnimation();

            _roundEndEvent.Raise();
            _persistentData.RegisterRoundOver();
        }

        void EndMatch()
        {
            _persistentData.RegisterMatchComplete();

            // Get winner
            Fighter winner = GetMatchWinner();
            bool playerIsWinner = IsFighterPlayer(winner);

            if (winner != null)
            {
                Debug.Log(winner.Profile.Name + " wins the match!");
            }
            else
            {
                Debug.LogError("Unable to get winner because both fighters are tied!");
            }

            if (playerIsWinner)
            {
                Stats.RegisterStat(StatID.MATCH_WIN, 1);
            }

            if (IsBatatoonMatch())
            {
                Achievements.Unlock(Achievement.BATATOON_MATCH);
                Achievements.Reveal(Achievement.BATATINHA_WINS);
                Achievements.Reveal(Achievement.COMPANHIA_WINS);
            }

            // Handle next steps
            switch (_persistentData.GameMode)
            {
                case GameMode.Campaign:
                    HandleCampaignMatchCompletion();
                    break;

                case GameMode.FreeFight:
                    HandleFreeFightMatchCompletion();
                    break;
            }

            void HandleCampaignMatchCompletion()
            {
                if (playerIsWinner)
                {
                    if (_persistentData.CurrentCampaignMatchIndex < ResourcesManager.CampaignStream.LastMatchIndex)
                    {
                        _persistentData.ProceedCampaign();
                        _sceneLoader.ReloadCurrentScene();
                    }
                    else
                    {
                        Debug.Log("CAMPAIGN OVER!");

                        // Last match complete
                        bool isBacalhauTeamSelected = _persistentData.SelectedTeam.Equals(Team.Bacalhau);
                        StatID statID = isBacalhauTeamSelected ? StatID.BACALHAU_TEAM_WIN : StatID.AZEITE_TEAM_WIN;
                        Stats.RegisterStat(statID, 1);

                        Achievement achievement = isBacalhauTeamSelected ? Achievement.CAMPAIGN_BACALHAU_WIN :
                                                                           Achievement.CAMPAIGN_AZEITE_WIN;
                        Achievements.Unlock(achievement);

                        // Display winner team popup
                        _campaignWinnerPopupHandler.TriggerPopup(_persistentData.SelectedTeam);
                        Analytics.RegisterEvent(Analytics.Event.CAMPAIGN_COMPLETE);
                    }
                }
                else
                {
                    Debug.Log("MATCH LOST");
                    _persistentData.CampaignAttemptsRemaining = Mathf.Max(0, _persistentData.CampaignAttemptsRemaining - 1);
                    _matchOverPopupHandler.TriggerPopup(_persistentData.CampaignAttemptsRemaining);
                }
            }

            void HandleFreeFightMatchCompletion()
            {
                if (playerIsWinner)
                {
                    Stats.RegisterStat(StatID.FREE_FIGHT_WIN, 1);
                    Stats.RegisterStat(_freeFightConfiguration.FighterOne.WinStat, 1);
                }

                _matchOverPopupHandler.TriggerPopup(canReplay: true);
            }

            SaveSystem.SaveData();
        }
        #endregion

        #region Fighters Configuration
        void ConfigureFighters(FighterProfileSO fighterOne, FighterProfileSO fighterTwo)
        {
            int opponentIndex;

            for (int i = 0; i < _fighters.Count; i++)
            {
                var profile = i == 0 ? fighterOne : fighterTwo;
                opponentIndex = Logic.BoolToInt(i == 0);
                _fighters[i].fighter.Configure(profile, _fighters[opponentIndex].fighter, GetControlModeForFighter(i));
                _fighters[i].profile = profile;
                _matchHUDController.OnFighterConfigured(_fighters[i].fighter, i);
            }
        }

        ControlMode GetControlModeForFighter(int fighterIndex)
        {
            // Safeguard method for some test case where the InputManager component might be missing from the scene and we only want
            // to control one player via input
            if (InputManager.Instance == null)
            {
                Debug.LogWarning("No InputManager instance found! Control mode for fighters will be set to the default layout.");
                return fighterIndex == 0 ? ControlMode.Input : ControlMode.AI;
            }

            return fighterIndex < InputManager.Instance.ActivePlayers ? ControlMode.Input : ControlMode.AI;
        }

        void SetFightersActive(bool active)
        {
            foreach (var data in _fighters)
            {
                if (active)
                    data.fighter.Controller.Activate();
                else
                    data.fighter.Controller.Deactivate();
            }
        }

        /// <summary>
        /// Snaps the fighters to their original pivot positions.
        /// </summary>
        void ResetFighters()
        {
            foreach (var config in _fighters)
            {
                config.fighter.ResetToDefault(config.pivot.position);
            }
        }

        void HandleFighterKnockedOut(Fighter fighter)
        {
            // When a fighter is knocked out, the opponent is considered the winner
            var winner = fighter.Opponent;
            var winnerConfig = _fighters.Find(c => c.fighter == winner);

            // Register win
            winnerConfig.AddWin();
            _matchHUDController.RegisterWinForFighter(_fighters.IndexOf(winnerConfig));
            Debug.Log($"{fighter.Name} was knocked out! {winner.Name} wins!");

            // Register round results
            var winContext = winner.Stats.HasSufferedDamage ? RoundResults.WinContext.KO : RoundResults.WinContext.Perfect;
            _roundResults.Add(new RoundResults(winner, winContext));

            // This will make the Match Loop coroutine proceed
            _hasFighterBeenKnockedOut = true;
        }
        #endregion

        #region Utility Methods
        bool AreWinsTied() => _fighters[0].Wins == _fighters[1].Wins;

        bool IsFighterPlayer(Fighter fighter)
        {
            return fighter != null && fighter.Profile.Team.Equals(_persistentData.SelectedTeam);
        }

        bool IsBatatoonMatch()
        {
            bool fighterOneCheck = _fighters.Exists(f => f.profile.Equals(_batatoonMatch.FighterOne));
            bool fighterTwoCheck = _fighters.Exists(f => f.profile.Equals(_batatoonMatch.FighterTwo));
            return fighterOneCheck && fighterTwoCheck;
        }

        Fighter GetMatchWinner()
        {
            if (_fighters[0].Wins > _fighters[1].Wins)
            {
                return _fighters[0].fighter;
            }
            else if (_fighters[1].Wins > _fighters[0].Wins)
            {
                return _fighters[1].fighter;
            }

            return null;
        }

        void ReturnToMainMenu()
        {
            _sceneLoader.LoadMainMenu();
        }
        #endregion

        #region Audio Triggers
        void PlayMatchMusicWithDelay(float delay)
        {
            Invoke(nameof(PlayMatchMusic), delay);
        }

        void PlayMatchMusic()
        {
            _musicController.PlayMatchMusic();
        }

        void TriggerBellRing()
        {
            _matchHUDController.TriggerBellAnimation();
            _audioManager.PlaySoundEffect(_bellRingCue);
        }
        #endregion
    }
}
