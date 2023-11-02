using System.Collections;
using UnityEngine;

namespace MemeFight.UI
{
    using Services;

    public class PlayerSelectionUIController : ManagedBehaviour
    {
        [Header("Settings")]
        [Tooltip("Whether to reset the campaign to its first match when the current game mode is set to Campaign and a new match is started.")]
        [SerializeField] bool _resetCampaigntOnStart = true;

        [Header("Asset References")]
        [SerializeField] FightersRosterSO _fighterRoster;
        [SerializeField] MatchConfigurationSO _freeFightMatch;
        [SerializeField] PersistentDataSO _persistentData;
        [SerializeField] SceneReferenceSO _matchScene;

        [Header("Component References")]
        [SerializeField] TeamSelectorUI _teamSelectorScreen;
        [SerializeField] FighterSelectorUI _fighterSelectorScreen;

        [Header("Audio Cues")]
        [SerializeField] AudioCueSO _selectionReelCue;
        [SerializeField] AudioCueSO _cpuFighterSelectedCue;

        [Header("Listening On")]
        [SerializeField] MenusInputEventChannelSO _menusInputChannel;
        [SerializeField] VoidEventSO _switchTeamEvent;
        [SerializeField] VoidEventSO _startMatchEvent;

        [Header("Info")]
        [SerializeField, ReadOnly] bool _teamSwitchingAllowed = false;

        AudioManager _audioManager;
        InputManager _inputManager;
        SceneLoader _sceneLoader;

        const int RandomSelectionCycles = 40;

        #region Initialization
        void OnEnable()
        {
            _menusInputChannel.OnSwitchTeam += SwitchTeam;

            _switchTeamEvent.OnRaised += SwitchTeam;
            _startMatchEvent.OnRaised += StartMatch;
        }

        void OnDisable()
        {
            _menusInputChannel.OnSwitchTeam -= SwitchTeam;

            _switchTeamEvent.OnRaised -= SwitchTeam;
            _startMatchEvent.OnRaised -= StartMatch;
        }

        protected override void Awake()
        {
            base.Awake();

            InputManager.OnPlayerJoined += HandlePlayerJoined;
            InputManager.OnPlayerLeft += HandlePlayerLeft;

            _teamSelectorScreen.OnOpened += _ => OnSelectorScreenOpen();
            _teamSelectorScreen.OnClosed += _ => OnSelectorScreenClosed();

            _fighterSelectorScreen.OnOpened += _ => OnSelectorScreenOpen();
            _fighterSelectorScreen.OnClosed += _ => OnSelectorScreenClosed();

            _fighterSelectorScreen.OnFighterSelected += CommitFighterSelectionForTeam;
        }

        protected override void OnSceneReady()
        {
            _audioManager = AudioManager.Instance;
            _inputManager = InputManager.Instance;
            _sceneLoader = SceneLoader.Instance;

            InitializeDisplays();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            InputManager.OnPlayerJoined -= HandlePlayerJoined;
            InputManager.OnPlayerLeft -= HandlePlayerLeft;
        }

        void InitializeDisplays()
        {
            _teamSelectorScreen.PopulateSlots(ResourcesManager.Fighters.Roster);
            _fighterSelectorScreen.PopulateSlots(ResourcesManager.Fighters.Roster);
            UpdatePlayerPanels();
        }
        #endregion

        #region Event Handling
        void OnSelectorScreenOpen()
        {
            _teamSwitchingAllowed = true;

            if (!PlatformManager.IsMobile && _persistentData.GameMode.Equals(GameMode.FreeFight))
            {
                EnableJoining();
            }

            UpdatePlayerPanels();
        }

        void OnSelectorScreenClosed()
        {
            _teamSwitchingAllowed = false;

            if (_inputManager.IsJoiningEnabled)
            {
                DisableJoining();
            }
        }

        void HandlePlayerJoined(int playerIndex)
        {
            if (_fighterSelectorScreen.IsOpen)
                _fighterSelectorScreen.EnablePanelForPlayer((Player)playerIndex, true);
        }

        void HandlePlayerLeft(int playerIndex)
        {
            if (_fighterSelectorScreen.IsOpen)
                _fighterSelectorScreen.EnablePanelForPlayer((Player)playerIndex, false);
        }

        void StartMatch()
        {
            DisableJoining();

            if (_persistentData.IsCampaignMode)
            {
                // CAMPAIGN MODE START
                if (_resetCampaigntOnStart)
                    _persistentData.ResetCampaign();

                LoadMatchScene();
            }
            else
            {
                // FREE FIGHT MODE START
                _fighterSelectorScreen.DisableAllPanels();
                _fighterSelectorScreen.NotifySelectionValidated();
                _fighterSelectorScreen.ClearMenuCommands();

                _persistentData.LastSelectedFighterLabel = _freeFightMatch.FighterOne.Label;

                if (_inputManager.IsSinglePlayerMode)
                {
                    // When playing in single player mode, pick a random opponent before proceeding
                    _fighterSelectorScreen.EnablePanelForPlayer(Player.Two, true, false);
                    StartCoroutine(PickRandomCPUFighter());
                }
                else
                {
                    // When in multiplayer mode, proceed immediately to the match scene
                    LoadMatchScene();
                }
            }

            IEnumerator PickRandomCPUFighter()
            {
                Debug.Log("Picking random CPU fighter...");

                // Select a random fighter for the CPU in the background
                Team cpuTeam = FightersDatabase.GetOpposingTeam(_persistentData.SelectedTeam);
                int cpuSelectionIndex = _fighterSelectorScreen.SelectRandomFighterForPlayer(Player.Two);

                // Generate array for the faked CPU selection cycles and draw a random number for each cycle
                int[] cycleIndexes = new int[RandomSelectionCycles];
                int totalFighters = ResourcesManager.Fighters.GetTotalFightersForTeam(cpuTeam);

                for (int i = 0; i < RandomSelectionCycles; i++)
                {
                    cycleIndexes[i] = Randomizer.GetRandom(0, totalFighters, true);
                }

                // Animate the displays to reflect the current cycle selection
                var reelCue = _audioManager.PlaySoundUI(_selectionReelCue);

                for (int cycle = 0; cycle < RandomSelectionCycles; cycle++)
                {
                    _fighterSelectorScreen.HighlightSlotForPlayer(Player.Two, cycleIndexes[cycle]);
                    _fighterSelectorScreen.SetDisplayedFighterForPlayer(Player.Two, cpuTeam, cycleIndexes[cycle]);
                    yield return CoroutineUtils.GetWaitTime(0.06f);

                    // Undo highlight for next cycle
                    _fighterSelectorScreen.HighlightSlotForPlayer(Player.Two, cycleIndexes[cycle], false);
                }

                reelCue.Stop();

                // Finally, display the actual CPU selection
                _fighterSelectorScreen.SetDisplayedFighterForPlayer(Player.Two, cpuTeam, cpuSelectionIndex, true);
                _audioManager.PlaySoundUI(_cpuFighterSelectedCue);

                yield return CoroutineUtils.GetWaitTime(4.0f);

                LoadMatchScene();
            }
        }

        void EnableJoining() => _inputManager.EnableJoining();
        void DisableJoining() => _inputManager.DisableJoining();
        #endregion

        #region Selection Control
        void SwitchTeam()
        {
            if (!_teamSwitchingAllowed)
                return;

            _persistentData.SwitchTeam();
            UpdatePlayerPanels(true);
        }

        void UpdatePlayerPanels(bool animate = false)
        {
            _teamSelectorScreen.SetSlotsTeam(ResourcesManager.Fighters.GetTeamData(_persistentData.SelectedTeam), animate);

            _fighterSelectorScreen.SetSlotsTeamForPlayer(Player.One, ResourcesManager.Fighters.GetTeamData(_persistentData.SelectedTeam));
            _fighterSelectorScreen.SetSlotsTeamForPlayer(Player.Two, ResourcesManager.Fighters.GetTeamData(FightersDatabase.GetOpposingTeam(_persistentData.SelectedTeam)));

            if (_fighterSelectorScreen.IsOpen && _inputManager != null)
            {
                _fighterSelectorScreen.EnablePanelForPlayer(Player.One, _inputManager.IsPlayerJoined(Player.One));
                _fighterSelectorScreen.EnablePanelForPlayer(Player.Two, _inputManager.IsPlayerJoined(Player.Two));
            }
        }

        void CommitFighterSelectionForTeam(Team team, int fighterIndex)
        {
            Player player = team == _persistentData.SelectedTeam ? Player.One : Player.Two;

            try
            {
                FighterProfileSO fighter = ResourcesManager.Fighters.GetFightersForTeam(team)[fighterIndex];

                _fighterSelectorScreen.SetDisplayedFighterForPlayer(player, fighter);
                _freeFightMatch.SetFighterForPlayer(player, fighter);

                Debug.Log($"Player {(int)player} has selected {fighter.Name}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to validate fighter selection with exception: " + e);
            }
        }
        #endregion

        public void LoadMatchScene()
        {
            if (_persistentData.IsCampaignMode)
            {
                Analytics.RegisterEvent(Analytics.Event.CAMPAIGN_START);
            }
            else
            {
                Analytics.RegisterEvent(Analytics.Event.FREEFIGHT_MATCH_START, _freeFightMatch.FighterOne.Label);
            }

            _sceneLoader.LoadScene(_matchScene);
        }
    }
}
