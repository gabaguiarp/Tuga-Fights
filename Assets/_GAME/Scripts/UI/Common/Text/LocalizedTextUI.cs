using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using TMPro;

namespace MemeFight.UI
{
    [RequireComponent(typeof(LocalizeStringEvent))]
    public class LocalizedTextUI : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI _text;
        [SerializeField] protected LocalizeStringEvent _localizeEvent;

        #region Setup
        void Reset()
        {
            GetRequiredComponents();
        }

        void GetRequiredComponents()
        {
            if (_text == null)
                _text = GetComponent<TextMeshProUGUI>();

            if (_localizeEvent == null)
                _localizeEvent = GetComponent<LocalizeStringEvent>();
        }
        #endregion

        public void UpdateText(string s)
        {
            if (_text != null)
                _text.SetText(s);
        }

        public void UpdateTextString(LocalizedString textString)
        {
            if (!textString.IsEmpty)
                _localizeEvent.StringReference = textString;
        }
    }
}
