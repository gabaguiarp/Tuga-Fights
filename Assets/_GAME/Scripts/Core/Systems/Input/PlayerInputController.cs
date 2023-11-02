using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace MemeFight.Input
{
    public class PlayerInputController : MonoBehaviour
    {
        [SerializeField] PlayerInput _input;
        [SerializeField] PlayerInputReader _inputReader;
        [SerializeField] MultiplayerEventSystem _eventSystem;

        public PlayerInput Input => _input;

        void Reset()
        {
            _input = GetComponent<PlayerInput>();
            _eventSystem = GetComponent<MultiplayerEventSystem>();
        }

        public void AssignInputChannel(GameplayInputEventChannelSO channel)
        {
            _inputReader.gameplayChannel = channel;
        }

        public void EnableMultiplayerEventSystem(bool enable)
        {
            if (_eventSystem)
                _eventSystem.enabled = enable;
        }

        public void SetUISelection(GameObject selection, GameObject rootGameObject = null)
        {
            if (_eventSystem)
            {
                _eventSystem.playerRoot = rootGameObject;
                _eventSystem.SetSelectedGameObject(selection);
            }
        }

        public void HandleDeviceLost(PlayerInput player)
        {
            Debug.Log("Device lost for " + name);
        }

        public void HandleDeviceRegained(PlayerInput player)
        {
            Debug.Log("Device regained for " + name);
        }

        public void HandleControlsChanged(PlayerInput player)
        {
            Debug.Log($"Controls changed for {name}: {player.currentControlScheme}");
        }

        [ContextMenu("Leave")]
        void Leave()
        {
            if (Application.isPlaying)
                Destroy(gameObject);
        }
    }
}
