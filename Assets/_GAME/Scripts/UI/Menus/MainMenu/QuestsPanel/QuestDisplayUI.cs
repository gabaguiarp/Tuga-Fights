using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Localization;

namespace MemeFight.UI
{
    public class QuestDisplayUI : MonoBehaviour
    {
        [Header("Text")]
        [SerializeField] LocalizedTextUI _descriptionText;
        [SerializeField] TextMeshProUGUI _descriptionTextMesh;
        [SerializeField] TextMeshProUGUI _progressText;
        [SerializeField] Color _blinkTextColor = Color.yellow;

        [Header("Icons")]
        [SerializeField] RectTransform _medalIcon;
        [SerializeField] AudioCueSO _medalRewardCue;

        Color _defaultTextColor = Color.white;
        bool _isTextBlinking = false;

        readonly float TextBlinkDuration = 0.25f;

        void Awake()
        {
            _defaultTextColor = _descriptionTextMesh.color;
        }

        #region Management
        public void Setup(LocalizedString descriptionString, int currentValue, int totalValue, bool showMedal, bool blink = false)
        {
            if (!showMedal)
                SetProgress(currentValue, totalValue);

            _descriptionText.UpdateTextString(descriptionString);
            _progressText.gameObject.SetActive(!showMedal);
            _medalIcon.gameObject.SetActive(showMedal);

            if (blink)
                SetBlinkActive(true);
        }

        public void Clear()
        {
            SetProgress(0, 0);
            _descriptionText.UpdateText(string.Empty);
            _medalIcon.gameObject.SetActive(false);
        }

        void SetProgress(int current, int total)
        {
            _progressText.SetText(string.Format("{0} / {1}", current, total));
        }
        #endregion

        #region Text
        void SetTextColor(Color color)
        {
            _descriptionTextMesh.color = color;
            _progressText.color = color;
        }

        void TriggerTextBlink(TextMeshProUGUI text)
        {
            text.DOBlendableColor(_blinkTextColor, TextBlinkDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutBack).SetUpdate(true);
        }

        [ContextMenu("Toggle Text Blink")]
        void ToggleTextBlink()
        {
            if (Application.isPlaying)
                SetBlinkActive(!_isTextBlinking);
        }
        #endregion

        #region Animation
        [ContextMenu("Animate Medal")]
        public void AnimateMedal()
        {
            _progressText.gameObject.SetActive(false);
            _medalIcon.gameObject.SetActive(true);

            if (DOTween.IsTweening(_medalIcon))
                _medalIcon.DOKill(true);

            _medalIcon.DOPunchScale(Vector3.one, 1.0f, 12, 0.3f);

            AudioManager.Instance.PlaySoundUI(_medalRewardCue, true);
        }

        public void SetBlinkActive(bool active)
        {
            if (DOTween.IsTweening(_descriptionTextMesh))
            {
                _descriptionTextMesh.DOKill(true);
            }

            if (DOTween.IsTweening(_progressText))
            {
                _progressText.DOKill(true);
            }

            // Set the start value
            SetTextColor(_defaultTextColor);

            if (active)
            {
                TriggerTextBlink(_descriptionTextMesh);
                TriggerTextBlink(_progressText);
            }

            _isTextBlinking = active;
        }
        #endregion
    }
}
