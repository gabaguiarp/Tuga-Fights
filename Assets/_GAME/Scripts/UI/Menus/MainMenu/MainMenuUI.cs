using UnityEngine;
using UnityEngine.Events;

namespace MemeFight.UI
{
    public class MainMenuUI : MenuScreenUI
    {
        public event UnityAction OnCampaignClicked;
        public event UnityAction OnFreeFightClicked;
        public event UnityAction OnTutorialClicked;
        public event UnityAction OnSettingsClicked;
        public event UnityAction OnCreditsClicked;
        public event UnityAction OnExitClicked;
        public event UnityAction OnAchivementsClicked;

        [Header("Buttons")]
        [SerializeField] MainMenuButtonUI _campaignButton;
        [SerializeField] MainMenuButtonUI _freeFightButton;
        [SerializeField] MainMenuButtonUI _tutorialButton;
        [SerializeField] MainMenuButtonUI _settingsButton;
        [SerializeField] MainMenuButtonUI _creditsButton;
        [SerializeField] MainMenuButtonUI _exitButton;
        [SerializeField] MainMenuButtonUI _achivementsButton;

        protected override void Awake()
        {
            base.Awake();
            _campaignButton.OnClicked += CampaignButtonClick;
            _freeFightButton.OnClicked += FreeFightButtonClick;
            _tutorialButton.OnClicked += TutorialButtonClick;
            _settingsButton.OnClicked += SettingsButtonClick;
            _creditsButton.OnClicked += CreditsButtonClick;
            _exitButton.OnClicked += ExitButtonClick;
            _achivementsButton.OnClicked += AchievmeentsClick;
        }

        void CampaignButtonClick() => OnCampaignClicked?.Invoke();
        void FreeFightButtonClick() => OnFreeFightClicked?.Invoke();
        void TutorialButtonClick() => OnTutorialClicked?.Invoke();
        void SettingsButtonClick() => OnSettingsClicked?.Invoke();
        void CreditsButtonClick() => OnCreditsClicked?.Invoke();
        void ExitButtonClick() => OnExitClicked?.Invoke();
        void AchievmeentsClick() => OnAchivementsClicked?.Invoke();
    }
}
