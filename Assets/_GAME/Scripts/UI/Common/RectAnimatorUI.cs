using DG.Tweening;
using UnityEngine;

namespace MemeFight.UI
{
    public class RectAnimatorUI : MonoBehaviour
    {
        enum Animation { Punch, Bounce }

        [SerializeField] RectTransform _target;
        [SerializeField] Animation _animation;

        Vector3 _referenceScale;

        void Awake()
        {
            _referenceScale = _target.localScale;
        }

        public void Animate()
        {
            if (!gameObject.activeSelf)
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
            }

            void PunchAnimation()
            {
                _target.localScale = _referenceScale;
                _target.DOPunchScale(_referenceScale * 0.1f, 0.55f, 10, 0.6f).SetUpdate(true);
            }

            void BounceAnimation()
            {
                _target.DOJumpAnchorPos(_target.anchoredPosition, 60f, 1, 0.55f).SetEase(Ease.OutBounce).SetUpdate(true);
            }
        }

        void KillTweenAnimation()
        {
            if (DOTween.IsTweening(_target))
            {
                _target.DOKill(true);
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
