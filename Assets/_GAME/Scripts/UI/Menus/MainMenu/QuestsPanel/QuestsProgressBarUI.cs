using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace MemeFight
{
    using Audio;
    using UI;
    using UnityEngine.Diagnostics;

    public class QuestsProgressBarUI : MonoBehaviour
    {
        [SerializeField] float _minBarIncrementSpeed = 0.3f;
        [SerializeField] float _maxBarIncrementSpeed = 1.0f;
        [SerializeField] float _bellAnimationDuration = 2.0f;
        [SerializeField] Slider _progressBar;
        [SerializeField] Image _handle;
        [SerializeField] Image _borderGlow;
        [SerializeField] RingBellUI _bell;

        [Header("Audio Cues")]
        [SerializeField] AudioCueSO _barIncreaseCue;
        [SerializeField] AudioCueSO _bellRingCue;

        AudioManager _audioManager;

        AudioManager AudioManager
        {
            get
            {
                if (_audioManager == null)
                    _audioManager = AudioManager.Instance;

                return _audioManager;
            }
        }

        [field: SerializeField, ReadOnly]
        public bool IsAnimating { get; private set; } = false;

        public float Value => _progressBar.value;
        public float BellAnimationDuration => _bellAnimationDuration;

        readonly float HandleFadeDuration = 0.4f;

        void Awake()
        {
            FadeHandle(false, true);
        }

        public void SetBarValue(float value, bool isImmedeate = false)
        {
            value = Mathf.Clamp01(value);

            if (!isImmedeate && !IsAnimating)
            {
                StartCoroutine(AnimateProgressBar(value));
            }
            else
            {
                _progressBar.value = value;
            }
        }

        public void EnableBellRingingAnimation(bool enable)
        {
            _bell.SetRingAnimationState(enable);

            if (enable)
                AudioManager.PlaySoundUI(_bellRingCue, true);
        }

        void FadeHandle(bool fadeIn, bool isImmediate = false)
        {
            if (DOTween.IsTweening(_handle))
            {
                _handle.DOKill();
            }

            float targetAlpha = Logic.BoolToFloat(fadeIn);
            if (isImmediate)
            {
                _handle.SetColorAlpha(targetAlpha);
                _borderGlow.SetColorAlpha(targetAlpha);
            }
            else
            {
                float startAlpha = Logic.BoolToFloat(!fadeIn);
                _handle.SetColorAlpha(startAlpha);
                _borderGlow.SetColorAlpha(startAlpha);
                _handle.DOFade(targetAlpha, HandleFadeDuration);
                _borderGlow.DOFade(targetAlpha, HandleFadeDuration);
            }
        }

        IEnumerator AnimateProgressBar(float endValue)
        {
            IsAnimating = true;

            float startValue = _progressBar.value;
            float speed = Mathf.Clamp(_minBarIncrementSpeed / (endValue - startValue), _minBarIncrementSpeed, _maxBarIncrementSpeed);
            float t = 0.0f;

            if (startValue < endValue)
            {
                FadeHandle(true);
                SoundEmitter barEmitter = AudioManager.PlaySoundUI(_barIncreaseCue, true);

                while (t < 1.0f)
                {
                    t += speed * Time.deltaTime;
                    _progressBar.value = Mathf.Lerp(startValue, endValue, t);
                    yield return null;
                }

                FadeHandle(false);
                barEmitter.Stop();
            }

            IsAnimating = false;
        }
    }
}
