using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MemeFight.DebugSystem.UI
{
    public class AnimationStatesUI : MonoBehaviour
    {
        public event UnityAction OnIdleButtonClicked;
        public event UnityAction OnWalkingButtonClicked;
        public event UnityAction OnPunchButtonClicked;
        public event UnityAction OnKickButtonClicked;
        public event UnityAction OnBlockButtonClicked;
        public event UnityAction OnDodgeButtonClicked;
        public event UnityAction OnDamagedButtonClicked;
        public event UnityAction OnKnockedOutButtonClicked;

        [SerializeField] Button _idleButton;
        [SerializeField] Button _walkingButton;
        [SerializeField] Button _punchButton;
        [SerializeField] Button _kickButton;
        [SerializeField] Button _blockButton;
        [SerializeField] Button _dodgeButton;
        [SerializeField] Button _damageButton;
        [SerializeField] Button _knockedOutButton;

        void Awake()
        {
            _idleButton.onClick.AddListener(() => OnIdleButtonClicked?.Invoke());
            _walkingButton.onClick.AddListener(() => OnWalkingButtonClicked?.Invoke());
            _punchButton.onClick.AddListener(() => OnPunchButtonClicked?.Invoke());
            _kickButton.onClick.AddListener(() => OnKickButtonClicked?.Invoke());
            _blockButton.onClick.AddListener(() => OnBlockButtonClicked?.Invoke());
            _dodgeButton.onClick.AddListener(() => OnDodgeButtonClicked?.Invoke());
            _damageButton.onClick.AddListener(() => OnDamagedButtonClicked?.Invoke());
            _knockedOutButton.onClick.AddListener(() => OnKnockedOutButtonClicked?.Invoke());
        }
    }
}
