using MemeFight.Characters;
using UnityEngine;

namespace MemeFight
{
    public class AITestConfiguration : ManagedBehaviour
    {
        [SerializeField] Fighter _aiFighter;
        [SerializeField] Fighter _opponent;
        [Tooltip("The profile to use for the AI fighter")]
        [SerializeField] FighterProfileSO _profileToUse;

        protected override void OnSceneReady()
        {
            ConfigureAIFighter();
        }

        void ConfigureAIFighter()
        {
            _aiFighter.Configure(_profileToUse, _opponent, PlayerController.ControlMode.AI);
            _aiFighter.Controller.Activate();
        }
    }
}
