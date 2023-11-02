using UnityEngine;
using UnityEngine.UI;

namespace MemeFight.UI
{
    using Services;

    public class TutorialUI : MenuScreenUI
    {
        [Header("Tutorial")]
        [SerializeField] TutorialSO _tutorialMobile;
        [SerializeField] TutorialSO _tutorialStandalone;
        [SerializeField, ReadOnly] int _currentPage = -1;

        [Header("References")]
        [SerializeField] Image _tutorialPicture;
        [SerializeField] LocalizedTextUI _instructionsText;
        [SerializeField] ButtonUI _previousPageButton;
        [SerializeField] ButtonUI _nextPageButton;
        [SerializeField] ButtonUI _closeButton;

        protected override void Awake()
        {
            base.Awake();
            _nextPageButton.OnClicked += OnNextPageButtonClicked;
            _previousPageButton.OnClicked += OnPreviousPageButtonClicked;
            _closeButton.OnClicked += Close;
        }

        protected override void OnBackCommand()
        {
            _closeButton.Click();
        }

        void OnNextPageButtonClicked() => ChangeTutorialPage(1);
        void OnPreviousPageButtonClicked() => ChangeTutorialPage(-1);

        void ChangeTutorialPage(int direction)
        {
            OpenPage(Mathf.Clamp(_currentPage + direction, 0, CurrentTutorial().items.LastIndex()));
        }

        void OpenPage(int pageIndex)
        {
            var pages = CurrentTutorial().items;

            if (!pages.IsIndexValid(pageIndex))
            {
                Debug.LogError(pageIndex + " is not a valid tutorial page index!");
                return;
            }

            _tutorialPicture.sprite = pages[pageIndex].Image;
            _instructionsText.UpdateTextString(pages[pageIndex].InstructionsString);

            _nextPageButton.gameObject.SetActive(pageIndex < pages.LastIndex());
            _previousPageButton.gameObject.SetActive(pageIndex > 0);

            _currentPage = pageIndex;
        }

        void RestartTutorial() => OpenPage(0);

        TutorialSO CurrentTutorial()
        {
            return PlatformManager.IsMobile ? _tutorialMobile : _tutorialStandalone;
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            RestartTutorial();

            if (!ResourcesManager.PersistentData.HasOpenedTutorial)
            {
                Analytics.RegisterEvent(Analytics.Event.OPEN_TUTORIAL);
                ResourcesManager.PersistentData.HasOpenedTutorial = true;
            }
        }
    }
}
