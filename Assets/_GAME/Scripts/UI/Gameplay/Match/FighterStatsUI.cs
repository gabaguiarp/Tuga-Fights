using TMPro;
using UnityEngine;

namespace MemeFight.UI
{
    public class FighterStatsUI : MonoBehaviour
    {
        enum Alignment { Left, Right }

        [SerializeField] Alignment _alignment;
        [SerializeField] SliderBarUI _healthBar;
        [SerializeField] SliderBarUI _blockingBar;
        [SerializeField] TextMeshProUGUI _fighterNameText;
        [SerializeField] RoundsScorerUI _scorer;

        void OnValidate()
        {
            if (_healthBar != null)
                _healthBar.SetDirection(_alignment == Alignment.Left);

            if (_fighterNameText != null)
            {
                if (_alignment == Alignment.Left)
                    _fighterNameText.rectTransform.AnchorToBottomLeft();
                else
                    _fighterNameText.rectTransform.AnchorToBottomRight();

                _fighterNameText.alignment = _alignment == Alignment.Left ? TextAlignmentOptions.Left : TextAlignmentOptions.Right;
            }

            if (_scorer != null)
            {
                if (_alignment == Alignment.Left)
                    _scorer.rectTransform.AnchorToBottomRight();
                else
                    _scorer.rectTransform.AnchorToBottomLeft();

                _scorer.arrangement = _alignment == Alignment.Left ? RoundsScorerUI.Arrangement.RightToLeft : RoundsScorerUI.Arrangement.LeftToRight;
            }
        }

        public void SetFighterName(string fighterName)
        {
            _fighterNameText.SetText(fighterName);
        }

        public void ConfigureScorerDisplay(int totalIcons)
        {
            _scorer.PopulateWinIcons(totalIcons);
        }

        public void DisplayWin()
        {
            _scorer.HighlightNewIcon();
        }

        public void SetMaxHealth(float maxHealth)
        {
            _healthBar.SetMaxValue(maxHealth);
        }

        public void UpdateHealth(float value)
        {
            _healthBar.UpdateDisplay(value);
        }

        public void SetMaxBlockingStrength(float maxStrength)
        {
            _blockingBar.SetMaxValue(maxStrength);
        }

        public void UpdateBlockingStrength(float value)
        {
            _blockingBar.UpdateDisplay(value);
        }
    }
}
