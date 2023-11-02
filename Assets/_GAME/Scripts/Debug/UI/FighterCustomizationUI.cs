using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MemeFight.DebugSystem.UI
{
    public class FighterCustomizationUI : MonoBehaviour
    {
        [SerializeField] Dropdown _fightersDropdown;

        public event UnityAction<int> OnFighterSelectionValidated;

        void Awake()
        {
            _fightersDropdown.onValueChanged.AddListener(HandleDropdownValueChanged);
        }

        void HandleDropdownValueChanged(int value) => OnFighterSelectionValidated?.Invoke(value);

        public void PopulateFightersDropdown(List<string> options, int currentOptionIndex = -1)
        {
            _fightersDropdown.ClearOptions();
            _fightersDropdown.AddOptions(options);

            if (currentOptionIndex >= 0 && _fightersDropdown.options.IsIndexValid(currentOptionIndex))
            {
                _fightersDropdown.SetValueWithoutNotify(currentOptionIndex);
            }
        }
    }
}
