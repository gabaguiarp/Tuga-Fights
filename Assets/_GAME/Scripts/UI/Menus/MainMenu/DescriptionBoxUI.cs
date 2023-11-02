using UnityEngine;

namespace MemeFight.UI
{
    public class DescriptionBoxUI : MonoBehaviour
    {
        [SerializeField] LocalizedTextUI _descriptionText;

        public void UpdateDescription(LocalizedStringVariableSO textString)
        {
            _descriptionText.UpdateTextString(textString.Value);
        }
    }
}
