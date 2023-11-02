using MemeFight.UI;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace MemeFight.Menus
{
    public class CreditsPanelUI : MenuScreenUI
    {
        [Header("References")]
        [SerializeField] CreditsGenerator _generator;
        [SerializeField] ButtonUI _closeButton;

        [Header("Localized Credits References")]
        [SerializeField] GameCreditsSO _ptCredits;
        [SerializeField] GameCreditsSO _engCredits;
        [SerializeField] Locale _ptLocale;
        [SerializeField] Locale _engLocale;

        protected override void OnEnable()
        {
            base.OnEnable();
            LocalizationSettings.SelectedLocaleChanged += UpdateCredits;
        }

        void OnDisable()
        {
            LocalizationSettings.SelectedLocaleChanged -= UpdateCredits;
        }

        protected override void Awake()
        {
            base.Awake();
            _closeButton.OnClicked += Close;
        }

        protected override void OnBackCommand()
        {
            _closeButton.Click();
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            UpdateCredits(LocalizationSettings.SelectedLocale);
        }

        void UpdateCredits(Locale currentLocale)
        {
            _generator.credits = currentLocale.Equals(_engLocale) ? _engCredits : _ptCredits;
            _generator.GenerateCredits();
        }
    }
}
