using DG.Tweening;
using UnityEngine;

namespace MemeFight.UI
{
    public class BumperSequenceUI : MonoBehaviour
    {
        [SerializeField] CanvasGroup _group;
        [Range(0.1f, 2.0f)]
        [SerializeField] float _fadeDuration = 1.0f;

        public WaitForSecondsRealtime WaitForFade => CoroutineUtils.GetWaitRealtime(_fadeDuration);

        void Reset()
        {
            if (_group == null)
                _group = GetComponent<CanvasGroup>();
        }

        public void Fade(bool fadeIn, bool isImmediate = false)
        {
            if (DOTween.IsTweening(_group))
                _group.DOKill();

            float targetAlpha = Logic.BoolToFloat(fadeIn);
            _group.alpha = isImmediate ? targetAlpha : Logic.BoolToFloat(!fadeIn);

            if (!isImmediate)
            {
                _group.DOFade(targetAlpha, _fadeDuration).SetUpdate(true);
            }
        }

        void OnDestroy()
        {
            if (DOTween.IsTweening(_group))
                _group.DOKill();
        }
    }
}
