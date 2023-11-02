using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace MemeFight.UI
{
    [RequireComponent(typeof(Image))]
    public class BlinkOverlayGraphicUI : MonoBehaviour
    {
        [Range(0.0f, 1.0f)]
        [SerializeField] float _alphaMax = 0.3f;
        [Range(0.0f, 1.0f)]
        [SerializeField] float _alphaMin = 0.0f;
        [SerializeField] float _blinkingDuration = 0.8f;

        Image _image;

        void OnEnable()
        {
            SetBlinkingEnabled(true);
        }

        void OnDisable()
        {
            SetBlinkingEnabled(false);
        }

        void GetImageIfNull()
        {
            if (_image == null)
                _image = GetComponent<Image>();
        }

        public void SetBlinkingEnabled(bool enabled)
        {
            GetImageIfNull();

            if (DOTween.IsTweening(_image))
            {
                _image.DOKill();
            }

            if (enabled)
            {
                _image.color = GraphicHelpers.GetColorWithAlpha(_image.color, _alphaMin);
                _image.DOFade(_alphaMax, _blinkingDuration).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
            }
        }
    }
}
