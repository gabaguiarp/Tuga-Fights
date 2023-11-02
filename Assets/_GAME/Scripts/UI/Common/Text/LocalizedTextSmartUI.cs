using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

namespace MemeFight.UI
{
    [RequireComponent(typeof(LocalizeStringEvent))]
    public class LocalizedTextSmartUI : LocalizedTextUI
    {
        [Tooltip("The reference look for when calling 'UpdateSmartReferenceString'")]
        [SerializeField] string _smartReference;
        
        public void UpdateSmartReferenceString(LocalizedString str)
        {
            (_localizeEvent.StringReference[_smartReference] as LocalizedString).SetReference(str.TableReference, str.TableEntryReference);
        }
    }
}
