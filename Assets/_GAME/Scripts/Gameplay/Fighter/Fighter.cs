using UnityEngine;
using UnityEngine.Events;

namespace MemeFight.Characters
{
    using ControlMode = PlayerController.ControlMode;

    public class Fighter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] GameObject _model;
        [SerializeField] FighterProfileSO _profile;

        [Header("Components")]
        [SerializeField] PlayerController _controller;
        [SerializeField] FighterStats _stats;

        [Header("Info")]
        [SerializeField, ReadOnly] Fighter _opponent;

        public string Name => _profile.Name;
        public FighterProfileSO Profile => _profile;
        public FighterStats Stats => _stats;
        public Fighter Opponent => _opponent;
        public PlayerController Controller => _controller;

        public static event UnityAction<Fighter> OnKnockedOut;

        void Awake()
        {
            _stats.OnDie += RaiseKnockedOutEvent;
        }

        void AssignProfileAndSetupModel(FighterProfileSO profile)
        {
            _profile = profile;

            // Setup the model
            foreach (var skin in _model.GetComponentsInChildren<FighterSkin>(true))
            {
                skin.SetEnabled(profile.Label.Equals(skin.ModelLabel));
            }
        }

        void RaiseKnockedOutEvent()
        {
            OnKnockedOut?.Invoke(this);
        }

        public void Configure(FighterProfileSO fighterProfile, Fighter opponent, ControlMode controlMode)
        {
            AssignProfileAndSetupModel(fighterProfile);
            _opponent = opponent;
            _controller.controlMode = controlMode;
        }

        public void ResetToDefault(Vector3? position = null)
        {
            _stats.ResetAllStats();
            _controller.Restore(position.Value);
        }
    }
}
