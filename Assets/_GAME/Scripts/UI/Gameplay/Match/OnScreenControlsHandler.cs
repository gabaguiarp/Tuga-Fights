using UnityEngine;

namespace MemeFight.UI
{
    using Characters;
    using OnScreenControls;

    public class OnScreenControlsHandler : MonoBehaviour
    {
        [SerializeField] GameplayInputEventChannelSO _inputChannel;
        [SerializeField] PlayerController _playerFighter;

        [Header("Direction Buttons")]
        [SerializeField] OnScreenButtonCustom _leftButton;
        [SerializeField] OnScreenButtonCustom _rightButton;

        [Header("Action Buttons")]
        [SerializeField] OnScreenButtonCustom _blockButton;
        [SerializeField] OnScreenButtonUI _punchButton;
        [SerializeField] OnScreenButtonUI _kickButton;

        void OnEnable()
        {
            if (_playerFighter != null)
            {
                _playerFighter.OnPunchTimerUpdated += HandlePunchTimerUpdated;
                _playerFighter.OnKickTimerUpdated += HandleKickTimerUpdated;
            }
        }

        void OnDisable()
        {
            if (_playerFighter != null)
            {
                _playerFighter.OnPunchTimerUpdated -= HandlePunchTimerUpdated;
                _playerFighter.OnKickTimerUpdated -= HandleKickTimerUpdated;
            }
        }

        void Awake()
        {
            if (_leftButton != null)
            {
                _leftButton.OnHeldDown += HandleLeftButtonHeldDown;
                _leftButton.OnRelease += HandleDirectionButtonReleased;
            }

            if (_rightButton != null)
            {
                _rightButton.OnHeldDown += HandleRightButtonHeldDown;
                _rightButton.OnRelease += HandleDirectionButtonReleased;
            }

            // We are tracking this button in particular because there is an issue with Unity's Input System
            // that causes the button to be released early if the player moves the finger slightly on mobile.
            // This has to due to an unhandled issue, related to a fight between automatic touch and gamepad
            // input scheme switching
            _blockButton.OnHeldDown += HandleBlockButtonHeldDown;
            _blockButton.OnRelease += HandleBlockButtonReleased;
        }

        #region Direction Button Responders
        void HandleLeftButtonHeldDown()
        {
            _inputChannel.RaiseMoveEvent(Vector2.left);
        }

        void HandleRightButtonHeldDown()
        {
            _inputChannel.RaiseMoveEvent(Vector2.right);
        }

        void HandleDirectionButtonReleased()
        {
            _inputChannel.RaiseMoveEvent(Vector2.zero);
        }
        #endregion

        #region Action Buttons Responders
        void HandleBlockButtonHeldDown()
        {
            _inputChannel.RaiseBlockStartedEvent();
        }

        void HandleBlockButtonReleased()
        {
            _inputChannel.RaiseBlockCanceledEvent();
        }
        #endregion

        #region Action Buttons Fill Mask Handlers
        void HandlePunchTimerUpdated(float value)
        {
            UpdateButtonMask(_punchButton, value, _playerFighter.PunchCooldownTime);
        }

        void HandleKickTimerUpdated(float value)
        {
            UpdateButtonMask(_kickButton, value, _playerFighter.KickCooldownTime);
        }

        void UpdateButtonMask(OnScreenButtonUI button, float currentValue, float maxValue)
        {
            button.SetFillValue((maxValue - currentValue) / maxValue);
        }
        #endregion
    }
}
