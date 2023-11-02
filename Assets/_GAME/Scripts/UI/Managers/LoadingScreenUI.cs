using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MemeFight.UI
{
    public class LoadingScreenUI : MonoBehaviour
    {
        [SerializeField] CanvasGroup _display;
        [SerializeField] Slider _loadingBar;
        [Range(0.0f, 1.5f)]
        [SerializeField] float _fadeDuration = 0.8f;

        public event UnityAction OnFadeComplete;

        public WaitForSecondsRealtime WaitForFade => CoroutineUtils.GetWaitRealtime(_fadeDuration);

        void Awake()
        {
            ShowLoadingScreen(false, false);
        }

        void ResetLoadingBar()
        {
            if (_loadingBar != null)
                _loadingBar.value = 0.0f;
        }

        void Fade(bool fadeIn)
        {
            if (DOTween.IsTweening(_display))
                _display.DOKill();

            _display.alpha = Logic.BoolToFloat(!fadeIn);
            _display.DOFade(Logic.BoolToFloat(fadeIn), _fadeDuration).SetUpdate(true).onComplete += NotifyFadeComplete;
        }

        void NotifyFadeComplete()
        {
            OnFadeComplete?.Invoke();

            if (_display.alpha <= 0)
                _display.gameObject.SetActive(false);
        }

        public void ShowLoadingScreen(bool show, bool fade = true)
        {
            if (show)
            {
                _display.gameObject.SetActive(true);
                if (fade)
                    Fade(true);
            }
            else
            {
                if (fade)
                {
                    Fade(false);
                }
                else
                {
                    _display.gameObject.SetActive(false);
                }
            }

            ResetLoadingBar();
        }

        public void SetLoadingBarValue(float value)
        {
            if (_loadingBar != null)
                _loadingBar.value = Mathf.Clamp01(value);
        }
    }
}
