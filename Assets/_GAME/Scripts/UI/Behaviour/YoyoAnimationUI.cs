using DG.Tweening;
using UnityEngine;

namespace MemeFight
{
    public class YoyoAnimationUI : MonoBehaviour
    {
        [SerializeField] RectTransform _rect;
        [SerializeField] bool _triggerOnStart = true;
        [SerializeField] float _targetAnchorYPosition = -10f;
        [SerializeField] float _duration = 0.5f;
        [SerializeField] Ease _ease = Ease.Unset;

        Vector2 _startAnchorPos;
        bool _isAnimating = false;

        void Reset()
        {
            if (_rect == null)
                _rect = GetComponent<RectTransform>();
        }

        void Start()
        {
            _startAnchorPos = _rect.anchoredPosition;

            if (_triggerOnStart)
                TriggerAnimation();
        }

        public void TriggerAnimation()
        {
            if (_rect == null)
            {
                Debug.LogError($"Cannot start animation on '{name}' because no RectTransform was assigned!");
                return;
            }

            if (_isAnimating)
                return;

            _rect.DOAnchorPosY(_targetAnchorYPosition, _duration).SetEase(_ease).SetLoops(-1, LoopType.Yoyo);

            _isAnimating = true;
        }

        public void StopAnimation(bool reset = true)
        {
            if (!_isAnimating)
                return;
            
            _rect.DOKill();

            if (reset)
                _rect.anchoredPosition = _startAnchorPos;

            _isAnimating = false;
        }
    }
}
