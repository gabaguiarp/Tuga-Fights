using UnityEngine;

namespace MemeFight.UI
{
    using Popups;
    using Services;

    public class PauseMenuController : ManagedBehaviour
    {
        [SerializeField] PauseMenuUI _pauseMenu;
        [SerializeField] ModalWindowTrigger _quitConfirmPopup;

        [Header("Listening On")]
        [SerializeField] VoidEventSO _settingsClosedEvent;

        [Header("Broadcasting On")]
        [SerializeField] VoidEventSO _openSettingsEvent;

        PopupsManager _popupsManager;

        void OnEnable()
        {
            _settingsClosedEvent.OnRaised += HandleSettingsClosed;
        }

        void OnDisable()
        {
            _settingsClosedEvent.OnRaised -= HandleSettingsClosed;
        }

        protected override void Awake()
        {
            base.Awake();
            _pauseMenu.OnResumeClicked += ClosePauseMenu;
            _pauseMenu.OnSettingsClicked += OpenSettingsScreen;
            _pauseMenu.OnQuitClicked += OpenQuitConfirmationPopup;
            _pauseMenu.OnBackCommandInvoked += HandleBackCommandInvoked;
        }

        protected override void OnSceneReady()
        {
            _popupsManager = PopupsManager.Instance;
        }

        void ClosePauseMenu()
        {
            _pauseMenu.Close();
        }

        void OpenSettingsScreen()
        {
            _openSettingsEvent.Raise();
        }

        void OpenQuitConfirmationPopup()
        {
            _quitConfirmPopup.OpenWindow();
        }

        void HandleBackCommandInvoked()
        {
            if (_popupsManager != null && _popupsManager.IsPopupOpen)
                return;
            
            ClosePauseMenu();
        }

        void HandleSettingsClosed()
        {
            // TODO: Select previous
            _pauseMenu.RegisterMenuCommands();
        }

        // Called via UnityEvent by the Quit Confirmation popup
        public void QuitToMainMenu()
        {
            if (ResourcesManager.PersistentData.IsCampaignMode)
            {
                Analytics.RegisterEvent(Analytics.Event.CAMPAIGN_GIVE_UP);
            }

            SceneLoader.Instance.LoadMainMenu();
        }
    }
}
