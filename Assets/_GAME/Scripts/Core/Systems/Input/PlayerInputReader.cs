using UnityEngine;
using UnityEngine.InputSystem;
using MemeFight.UI;

namespace MemeFight.Input
{
    public class PlayerInputReader : MonoBehaviour
    {
        [SerializeField] PlayerInputController _inputController;

        [Header("Broadcasting On")]
        public GameplayInputEventChannelSO gameplayChannel;
        public MenusInputEventChannelSO menusChannel;

        [Space(10)]
        [SerializeField, ReadOnly] bool _isPointerOverUI;

        void Update()
        {
            // NOTE: We never want to check if the pointer is over the UI when interacting with touchscreen in mobile platforms, because in those
            // cases we have on-screen controls. If we did this check, actions tied to certain on-screen buttons would not be triggered as a result.
            // This is only used for PC, to avoid triggering actions mapped to mouse buttons while interacting with UI elements of the game.
            if (PlatformManager.IsStandalone)
            {
                _isPointerOverUI = UIManager.IsPointerOverUI();
            }
        }

        #region Gameplay
        public void Move(InputAction.CallbackContext context)
        {
            gameplayChannel.RaiseMoveEvent(context.ReadValue<Vector2>());
        }

        public void Punch(InputAction.CallbackContext context)
        {
            if (context.started && !_isPointerOverUI)
                gameplayChannel.RaisePuchEvent();
        }

        public void Kick(InputAction.CallbackContext context)
        {
            if (context.started && !_isPointerOverUI)
                gameplayChannel.RaiseKickEvent();
        }

        public void Dodge(InputAction.CallbackContext context)
        {
            if (context.started)
                gameplayChannel.RaiseDodgeEvent();
        }

        public void Block(InputAction.CallbackContext context)
        {
            if (context.started)
                gameplayChannel.RaiseBlockStartedEvent();
            else if (context.canceled)
                gameplayChannel.RaiseBlockCanceledEvent();
        }
        #endregion

        #region Menus
        public void SwitchTeam(InputAction.CallbackContext context)
        {
            if (context.started)
                menusChannel.RaiseSwitchTeamEvent();
        }

        public void Back(InputAction.CallbackContext context)
        {
            if (context.started)
                menusChannel.RaiseBackEvent();
        }
        #endregion

        #region Common
        public void Pause(InputAction.CallbackContext context)
        {
            if (context.started)
                menusChannel.RaisePauseEvent();
        }
        #endregion
    }
}
