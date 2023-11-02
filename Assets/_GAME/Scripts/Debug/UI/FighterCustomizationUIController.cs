using MemeFight.Characters;
using System.Collections.Generic;
using UnityEngine;

namespace MemeFight.DebugSystem.UI
{
    public class FighterCustomizationUIController : ManagedBehaviour
    {
        [Header("Settings")]
        [Tooltip("Whether the fighter should be automatically configured to the first one from the roster as soon as the controller " +
            "is initialized.")]
        [SerializeField] bool _configureOnInit = true;

        [Header("References")]
        [SerializeField] FightersRosterSO _roster;
        [SerializeField] Fighter _fighter;

        [Header("Components")]
        [SerializeField] FighterCustomizationUI _customizationUI;

        List<FighterProfileSO> _profiles = new List<FighterProfileSO>();

        protected override void OnSceneReady()
        {
            Initialize();
            _customizationUI.OnFighterSelectionValidated += HandleFighterSelected;

            // Select first fighter, if enabled
            if (_configureOnInit)
                HandleFighterSelected(0);
        }

        void Initialize()
        {
            if (_roster == null)
                return;

            if (_roster.Fighters.Length <= 0)
            {
                Debug.LogError("No fighter profiles retrieved from the roster! Unable to populate UI dropdown!");
                return;
            }

            // Gather main and bonus fighter profiles
            for (int i = 0; i < _roster.Fighters.Length; i++)
            {
                _profiles.Add(_roster.Fighters[i]);
            }

            for (int j = 0; j < _roster.BonusFighters.Count; j++)
            {
                for (int k = 0; k < _roster.BonusFighters[j].FightersCount; k++)
                {
                    _profiles.Add(_roster.BonusFighters[j].Fighters[k]);
                }
            }

            // Populate the names dropdown
            List<string> fighterNames = new List<string>();
            _profiles.ForEach(p => fighterNames.Add(p.Name));

            _customizationUI.PopulateFightersDropdown(fighterNames);
        }

        void HandleFighterSelected(int fighterIndex)
        {
            var profile = GetProfileFromIndex(fighterIndex);

            if (profile == null)
            {
                Debug.LogError($"Unable to get fighter profile with index {fighterIndex}!");
                return;
            }

            _fighter.Configure(profile, null, PlayerController.ControlMode.Input);

            Debug.Log("Fighter successfully configured: " + profile.Name);
        }

        FighterProfileSO GetProfileFromIndex(int index)
        {
            try
            {
                return _profiles[index];
            }
            catch
            {
                throw new System.Exception(index + " is not a valid fighter profile index!");
            }
        }
    }
}
