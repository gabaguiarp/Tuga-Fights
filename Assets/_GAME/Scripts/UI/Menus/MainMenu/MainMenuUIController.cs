using UnityEngine;

namespace MemeFight.UI
{
    using Services;

    public class MainMenuUIController : ManagedBehaviour
    {
        [Header("UI Screens")]
        [SerializeField] MainMenuUI _mainMenuScreen;
        [SerializeField] TeamSelectorUI _teamSelectorScreen;
        [SerializeField] FighterSelectorUI _fighterSelectorScreen;
        [SerializeField] TutorialUI _tutorialScreen;
        [SerializeField] MenuScreenUI _creditsScreen;

        [Header("Descriptions")]
        [SerializeField] FightersRenderDisplayUI _fightersRender;

        [Header("References")]
        [SerializeField] PersistentDataSO _persistentData;

        [Header("Listening On")]
        [SerializeField] VoidEventSO _settingsClosedEvent;

        [Header("Broadcasting On")]
        [SerializeField] VoidEventSO _openSettingsEvent;

        bool _wasFightersRenderUpdated = false;
        ServicesManager _services;

        void OnEnable()
        {
            _settingsClosedEvent.OnRaised += SelectPrevious;
            _mainMenuScreen.OnOpened += _ => UpdateFightersRender();
        }

        void OnDisable()
        {
            _settingsClosedEvent.OnRaised -= SelectPrevious;
            _mainMenuScreen.OnOpened -= _ => UpdateFightersRender();
        }

        protected override void Awake()
        {
            base.Awake();

            _mainMenuScreen.OnCampaignClicked += OpenCampaignModeScreen;
            _mainMenuScreen.OnFreeFightClicked += OpenFreeFightModeScreen;
            _mainMenuScreen.OnTutorialClicked += OpenTutorialScreen;
            _mainMenuScreen.OnSettingsClicked += OpenSettingsScreen;
            _mainMenuScreen.OnCreditsClicked += OpenCreditsScreen;
            _mainMenuScreen.OnExitClicked += ExitGame;
            _mainMenuScreen.OnAchivementsClicked += ShowAchievements;

            _tutorialScreen.OnClosed += (_) => SelectPrevious();
        }

        protected override void OnSceneReady()
        {
            _services = ServicesManager.Instance;
        }

        #region Button Responders
        void OpenCampaignModeScreen()
        {
            _persistentData.GameMode = GameMode.Campaign;
            _teamSelectorScreen.Open();
        }

        void OpenFreeFightModeScreen()
        {
            _persistentData.GameMode = GameMode.FreeFight;
            _fighterSelectorScreen.Open();
        }

        void OpenTutorialScreen()
        {
            _tutorialScreen.Open();
        }

        void OpenSettingsScreen()
        {
            _openSettingsEvent.Raise();
        }

        void OpenCreditsScreen()
        {
            _creditsScreen.Open();
        }

        void ExitGame()
        {
            GameManager.ExitGame();
        }

        void ShowAchievements()
        {
            _services.ShowAchievements();
        }
        #endregion

        void UpdateFightersRender()
        {
            // The fighters render display will be randomized if the player has won at least one match, or it has already been
            // updated once, since the menu scene was last loaded.
            bool shouldRandomizeDisplay = (_persistentData.GetStat(StatID.MATCH_WIN, out StatData data) && data.Value > 0) ||
                                           _wasFightersRenderUpdated;
            if (shouldRandomizeDisplay)
            {
                _fightersRender.DisplayRandom();
            }
            else
            {
                // ...otherwise, the first fighter from the current team will be displayed
                _fightersRender.DisplayFirstFighter();
            }

            _wasFightersRenderUpdated = true;
        }

        void SelectPrevious()
        {
            _mainMenuScreen.SelectPrevious();
        }
    }
}
