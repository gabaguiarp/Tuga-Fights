using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MemeFight.UI
{
    public class EffectsPanelUI : MonoBehaviour
    {
        [SerializeField] float _flashEffectDuration = 0.02f;

        [Header("References")]
        [SerializeField] Image _flashEffectImage;

        bool _isPlayingFlashEffect = false;

        public float FlashEffectDuration => _flashEffectDuration;

        public void TriggerFlashEffect()
        {
            if (_isPlayingFlashEffect)
                return;

            StartCoroutine(FlashEffectRoutine());
        }

        IEnumerator FlashEffectRoutine()
        {
            _isPlayingFlashEffect = true;

            _flashEffectImage.gameObject.SetActive(true);
            yield return CoroutineUtils.GetWaitTime(_flashEffectDuration);
            _flashEffectImage.gameObject.SetActive(false);

            _isPlayingFlashEffect = false;
        }
    }
}
