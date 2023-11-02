using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace MemeFight.UI
{
    [RequireComponent(typeof(Image))]
    public class SpinnerIconUI : MonoBehaviour
    {
        [SerializeField] float _spinSpeed = 280.0f;
        [SerializeField] float _fadeOutDuration = 1.0f;

        Image _icon = default;
        float _defaultAlpha = 1.0f;

        void Awake()
        {
            // Setup variables
            _icon = GetComponent<Image>();

            if (_icon.color.a > 0.0f)
                _defaultAlpha = _icon.color.a;

            // Hide the icon upon initialization
            Hide();

            // Subscribe to the SceneLoader events, so the icon can dinamically be displayed/hidden accordingly
            SceneLoader.OnLoadingStarted += Display;
            SceneLoader.OnLoadingComplete += Hide;
        }

        void Update()
        {
            _icon.rectTransform.Rotate(-Vector3.forward, _spinSpeed * Time.unscaledDeltaTime);
        }

        void SetIconAlpha(float alpha)
        {
            Color color = _icon.color;
            color.a = Mathf.Clamp01(alpha);
            _icon.color = color;
        }

        public void Display()
        {
            gameObject.SetActive(true);
            _icon.rectTransform.rotation = Quaternion.identity;
            SetIconAlpha(_defaultAlpha);
        }

        public void Hide()
        {
            _icon.DOFade(0.0f, _fadeOutDuration).SetUpdate(true).onComplete += OnFadeComplete;
        }

        void OnFadeComplete()
        {
            _icon.DOKill();
            gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            if (DOTween.IsTweening(_icon))
            {
                _icon.DOKill();
            }

            SceneLoader.OnLoadingStarted -= Display;
            SceneLoader.OnLoadingComplete -= Hide;
        }
    }
}
