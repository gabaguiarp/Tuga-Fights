using UnityEngine;
using UnityEngine.Events;

namespace MemeFight.UI
{
    public class PauseMenuUI : MenuScreenUI
    {
        public event UnityAction OnResumeClicked;
        public event UnityAction OnSettingsClicked;
        public event UnityAction OnQuitClicked;
        public event UnityAction OnBackCommandInvoked;

        [Header("Buttons")]
        [SerializeField] ButtonUI _resumeButton;
        [SerializeField] ButtonUI _settingsbutton;
        [SerializeField] ButtonUI _quitButton;

        protected override void Awake()
        {
            base.Awake();
            _resumeButton.OnClicked += ResumeButtonClicked;
            _settingsbutton.OnClicked += SettingsButtonClicked;
            _quitButton.OnClicked += QuitButtonClicked;
        }

        protected override void OnBackCommand()
        {
            OnBackCommandInvoked?.Invoke();
        }

        void ResumeButtonClicked() => OnResumeClicked?.Invoke();
        void SettingsButtonClicked() => OnSettingsClicked?.Invoke();
        void QuitButtonClicked() => OnQuitClicked?.Invoke();
    }
}
