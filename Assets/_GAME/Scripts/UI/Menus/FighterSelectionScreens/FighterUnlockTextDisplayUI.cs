using UnityEngine;
using UnityEngine.Localization;

namespace MemeFight.UI
{
    public class FighterUnlockTextDisplayUI : MonoBehaviour
    {
        [SerializeField] LocalizedTextUI _textString;

        void Reset()
        {
            if (_textString == null)
                _textString = transform.GetComponentInChildren<LocalizedTextUI>();
        }

        public void SetEnabled(bool enabled)
        {
            gameObject.SetActive(enabled);
        }

        public void DisplayMessage(LocalizedString messageString, bool enableIfDisabled = true)
        {
            if (!gameObject.activeSelf && enableIfDisabled)
                SetEnabled(true);

            _textString.UpdateTextString(messageString);
        }
    }
}
