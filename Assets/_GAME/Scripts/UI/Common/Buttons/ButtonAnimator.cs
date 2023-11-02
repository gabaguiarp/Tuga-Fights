using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace MemeFight.UI
{
    [RequireComponent(typeof(ButtonUI))]
    public class ButtonAnimator : MonoBehaviour
    {
        enum Animation { Punch, Bounce, Fade }
        enum TriggerEvent { PressAndRelease, Custom }

        [SerializeField] Animation _animation;
        [Tooltip("Defines when should the button animation be triggered.")]
        [SerializeField] TriggerEvent _triggerEvent;

        Vector3 _referenceScale;

        ButtonUI _button;
        RectTransform _rect;
        Image _image;

        const float FadeDuration = 0.6f;

        void Reset()
        {
            ConfigureButton();
        }

        void Awake()
        {
            ConfigureButton();
            _referenceScale = _rect.localScale;
        }

        void ConfigureButton()
        {
            _rect = GetComponent<RectTransform>();
            _image = GetComponent<Image>();

            if (_button == null && !TryGetComponent(out _button))
            {
                Debug.LogWarning(name + " has a MenuButtonUI component attached, but no ButtonUI component was found inside! Unable to " +
                    "configure button");
            }

            if (Application.isPlaying && _triggerEvent.Equals(TriggerEvent.PressAndRelease))
            {
                _button.OnButtonPressed += () => Animate(true);
                _button.OnButtonReleased += () => Animate(false);
            }
        }

        public void Animate(bool animateIn = true)
        {
            if (!gameObject.activeSelf || !_button.IsInteractable)
                return;

            KillTweenAnimation();

            switch (_animation)
            {
                case Animation.Punch:
                    PunchAnimation();
                    break;

                case Animation.Bounce:
                    BounceAnimation();
                    break;

                case Animation.Fade:
                    FadeAnimation();
                    break;
            }

            void PunchAnimation()
            {
                if (animateIn)
                    _rect.DOScale(_referenceScale * 0.9f, 0.2f).SetUpdate(true);
                else
                    _rect.DOScale(_referenceScale, 0.4f).SetEase(Ease.OutBounce).SetUpdate(true);
            }

            void BounceAnimation()
            {
                _rect.DOJumpAnchorPos(_rect.anchoredPosition, 60f, 1, 0.55f).SetEase(Ease.OutBounce).SetUpdate(true);
            }

            void FadeAnimation()
            {
                if (_image == null)
                {
                    Debug.LogError($"Unable to apply fade animation to {name} because no Image component was found!");
                    return;
                }

                Color startColor = _image.color;
                startColor.a = Logic.BoolToFloat(!animateIn);
                float targetValue = Logic.BoolToFloat(animateIn);

                _image.color = startColor;
                _image.DOFade(targetValue, FadeDuration).SetUpdate(true);
            }
        }

        void KillTweenAnimation()
        {
            if (DOTween.IsTweening(_rect))
            {
                _rect.DOKill(true);
            }
        }

        void OnDisable()
        {
            KillTweenAnimation();
        }

        void OnDestroy()
        {
            KillTweenAnimation();
        }
    }
}
