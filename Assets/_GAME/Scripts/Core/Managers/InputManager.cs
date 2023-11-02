using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace MemeFight
{
    using Input;

    public enum Player { One = 1, Two = 2 }

    /// <summary>
    /// The InputManager provides custom player joining methods, instead of relying on the default ones from the <see cref="PlayerInputManager"/>,
    /// mainly due to the fact that the latter does not natively support a split-keyboard layout.<br></br>
    /// Therefore, some of those properties were replicated and slightly modified here so any player can join either by using a new device
    /// (i.e.: Gamepad), or sharing the same keyboard.
    /// </summary>
    [RequireComponent(typeof(PlayerInputManager))]
    public class InputManager : Singleton<InputManager>
    {
        public enum InputMap
        {
            NONE,
            Gameplay,
            Menus
        }

        [Header("Settings")]
        [SerializeField] InputMap _initialInputMap;
        [SerializeField] EventSystem _mobileEventSystem;

        [Header("Joining")]
        [SerializeField] PlayerInputController _playerPrefab;
        [SerializeField] bool _joinFirstPlayerOnStart = true;
        [SerializeField] bool _joiningEnabled = true;
        [SerializeField] InputActionProperty _joinAction;

        [Space(10)]
        [SerializeField] GameplayInputEventChannelSO[] _inputChannels = new GameplayInputEventChannelSO[2];

        [Header("Info")]
        [SerializeField, ReadOnly] InputMap _currentInputMap;
        [SerializeField, ReadOnly] InputMap _previousInputMap;
        [SerializeField, ReadOnly] List<PlayerInputController> _joinedPlayers = new List<PlayerInputController>();

        bool _joiningDelegateHooked;
        PlayerInputController[] _playerSlots;

        PlayerInputManager _playersManager;

        public int ActivePlayers => _joinedPlayers.Count;
        public bool IsJoiningEnabled => _joiningEnabled;
        public bool IsSinglePlayerMode => ActivePlayers <= 1;
        public bool IsInputEnabled => _currentInputMap != InputMap.NONE;

        public static event UnityAction OnJoiningEnabled;
        public static event UnityAction OnJoiningDisabled;
        public static event UnityAction<int> OnPlayerJoined;
        public static event UnityAction<int> OnPlayerLeft;

        void OnEnable()
        {
            // [Copied from the PlayerInputManager class]
            // If the join action is a reference, clone it so we don't run into problems with the action being disabled by
            // PlayerInput when devices are assigned to individual players
            if (_joinAction.reference != null && _joinAction.action?.actionMap?.asset != null)
            {
                var inputActionAsset = Instantiate(_joinAction.action.actionMap.asset);
                var inputActionReference = InputActionReference.Create(inputActionAsset.FindAction(_joinAction.action.name));
                _joinAction = new InputActionProperty(inputActionReference);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            _playersManager = GetComponent<PlayerInputManager>();

            PopulatePlayerSlots();
        }

        void Start()
        {
            _currentInputMap = _initialInputMap;

            if (_joiningEnabled)
                EnableJoining();
            else
                DisableJoining();

            // Join player 1
            if (_joinFirstPlayerOnStart)
                JoinPlayer();

            // Enable mobile event system when the current platform is mobile (used for UI interaction and on-screen controls)
            //_mobileEventSystem.gameObject.SetActive(PlatformManager.IsMobile);
        }

        #region Join Actions
        [ContextMenu("Join Player")]
        public void JoinPlayer()
        {
            if (CanJoinPlayer())
                _playersManager.JoinPlayer();

            // NOTE: The new player will be added to the JoinedPlayers list through a specific PlayerInputManager event, handled bellow
        }

        void JoinPlayerFromAction(InputAction.CallbackContext context)
        {
            if (!CanJoinPlayer())
                return;

            if (PlatformManager.IsStandalone && Keyboard.current != null && context.control.device == Keyboard.current)
            {
                var deviceUser = PlayerInput.FindFirstPairedToDevice(context.control.device);
                if (deviceUser != null)
                {
                    bool isPlayerOne = _playerSlots[0] == null;

                    if (InputDevicesManager.TryShareDeviceWithPlayer(context.control.device, deviceUser, out string controlScheme, isPlayerOne))
                    {
                        JoinPlayerWithDevice(context.control.device, controlScheme);
                    }
                    else
                    {
                        Debug.LogError($"The device {context.control.device.name} is already being used by {deviceUser.name} and cannot be shared! " +
                            $"Unable to join player.");
                    }

                    return;
                }
            }

            JoinPlayer();
        }

        void JoinPlayerWithDevice(InputDevice device, string controlScheme)
        {
            if (CanJoinPlayer())
            {
                var player = PlayerInput.Instantiate(_playerPrefab.gameObject, controlScheme: controlScheme, pairWithDevice: device);
                player.SwitchCurrentControlScheme(controlScheme, device);
            }
        }

        bool CanJoinPlayer()
        {
            if (_playersManager.maxPlayerCount > 0 && ActivePlayers == _playersManager.maxPlayerCount)
            {
                Debug.LogWarning("Unable to join more players because the player limit was reached!");
                return false;
            }

            return true;
        }
        #endregion

        #region Players Indexation
        void PopulatePlayerSlots()
        {
            _playerSlots = new PlayerInputController[_playersManager.maxPlayerCount];
        }

        int GetFirstEmptySlot()
        {
            for (int i = 0; i < _playerSlots.Length; i++)
            {
                if (_playerSlots[i] == null)
                {
                    return i + 1;
                }
            }

            return -1;
        }

        int GetSlotIndexForPlayer(PlayerInputController player)
        {
            return _playerSlots.IndexOf(player) + 1;
        }

        void AddPlayerToSlot(PlayerInputController player, int slotIndex)
        {
            if (_playerSlots.IsIndexValid(slotIndex))
            {
                _playerSlots[slotIndex] = player;
            }
            else
            {
                Debug.LogError(slotIndex + " is not a valid slot index! Unable to add player to slot.");
            }
        }

        void RemovePlayerFromSlot(int slotIndex)
        {
            if (_playerSlots.IsIndexValid(slotIndex))
            {
                _playerSlots[slotIndex] = null;
            }
            else
            {
                Debug.LogError(slotIndex + " is not a valid slot index! Unable to remove player from slot.");
            }
        }

        int PlayerToSlotIndex(Player player) => (int)player - 1;
        int PlayerToSlotIndex(int playerIndex) => playerIndex - 1;

        public bool IsPlayerJoined(Player player)
        {
            if (_playerSlots.IsIndexValid(PlayerToSlotIndex(player)))
                return _playerSlots[PlayerToSlotIndex(player)] != null;

            return false;
        }
        #endregion

        #region Player UI Selection
        public void SetUISelectionForPlayer(Player player, GameObject selection, GameObject rootGameObject = null)
        {
            if (_playerSlots[PlayerToSlotIndex(player)] != null)
                _playerSlots[PlayerToSlotIndex(player)].SetUISelection(selection, rootGameObject);
        }
        #endregion

        #region Joining Control
        [ContextMenu("Enable Joining")]
        public void EnableJoining()
        {
            if (!Application.isPlaying || _joiningEnabled) return;

            if (PlatformManager.IsMobile)
            {
                Debug.LogWarning("Players joining functionality is not available on mobile! Cannot enable joining.");
                return;
            }

            if (!_joiningDelegateHooked)
            {
                _joinAction.action.started += JoinPlayerFromAction;
                _joiningDelegateHooked = true;
            }

            _playersManager.EnableJoining();
            _joinAction.action.Enable();
            _joiningEnabled = true;

            OnJoiningEnabled?.Invoke();

            Debug.Log("PLAYERS JOINING ENABLED");
        }

        [ContextMenu("Disable Joining")]
        public void DisableJoining()
        {
            if (!Application.isPlaying || !_joiningEnabled) return;

            if (_joiningDelegateHooked)
            {
                _joinAction.action.started -= JoinPlayerFromAction;
                _joiningDelegateHooked = false;
            }

            _playersManager.DisableJoining();
            _joinAction.action.Disable();
            _joiningEnabled = false;

            OnJoiningDisabled?.Invoke();

            Debug.Log("PLAYERS JOINING DISABLED");
        }
        #endregion

        #region Input Map Control
        public void EnableInputMap(InputMap map) => SetActionMapForAllPlayers(map);
        public void EnableGameplayInput() => SetActionMapForAllPlayers(InputMap.Gameplay);
        public void EnableMenusInput() => SetActionMapForAllPlayers(InputMap.Menus);
        public void SwitchToPreviousInput() => SetActionMapForAllPlayers(_previousInputMap);
        public void DisableInput() => SetActionMapForAllPlayers(InputMap.NONE);

        void SetActionMapForAllPlayers(InputMap map)
        {
            _previousInputMap = _currentInputMap;
            _currentInputMap = map;

            _joinedPlayers.ForEach(p => ApplyCurrentActionMapToPlayer(p.Input));

            if (_currentInputMap != InputMap.NONE)
            {
                Debug.Log(map.ToString().ToUpper() + " INPUT ENABLED");
            }
            else
            {
                Debug.Log("ALL INPUT DISABLED");
            }

            if (PlatformManager.IsMobile)
            {
                // Enable or disable the mobile event system, depending on whether the new map is set to something other than NONE
                _mobileEventSystem.gameObject.SetActive(map != InputMap.NONE);
            }
        }

        /// <summary>
        /// Assigns the current action map to <paramref name="player"/>.
        /// </summary>
        void ApplyCurrentActionMapToPlayer(PlayerInput player)
        {
            if (_currentInputMap != InputMap.NONE)
            {
                if (!player.inputIsActive)
                    player.ActivateInput();

                player.SwitchCurrentActionMap(_currentInputMap.ToString());
            }
            else
            {
                player.DeactivateInput();
            }
        }
        #endregion

        #region Players Manager Event Handlers
        public void HandlePlayerJoined(PlayerInput player)
        {
            int playerIndex = GetFirstEmptySlot();

            if (playerIndex <= 0)
            {
                Debug.LogError("Invalid result!");
                return;
            }

            ConfigurePlayer(player, playerIndex);

            var controller = player.GetComponent<PlayerInputController>();
            if (!_joinedPlayers.Contains(controller))
                _joinedPlayers.Add(controller);

            AddPlayerToSlot(controller, PlayerToSlotIndex(playerIndex));

            OnPlayerJoined?.Invoke(playerIndex);
            Debug.Log($"Player {playerIndex} joined!");
        }

        public void HandlePlayerLeft(PlayerInput player)
        {
            var controller = player.GetComponent<PlayerInputController>();
            int playerIndex = GetSlotIndexForPlayer(controller);

            if (playerIndex <= 0)
            {
                Debug.LogError("Invalid result!");
                return;
            }

            // If this player was sharing the keyboard with another one, make the latter regain full control of the device
            if (PlatformManager.IsStandalone && Keyboard.current != null && player.devices.Contains(Keyboard.current) && IsPlayerSharingKeyboard(player))
            {
                var keyboardUser = PlayerInput.FindFirstPairedToDevice(Keyboard.current);
                keyboardUser.SwitchCurrentControlScheme("Keyboard&Mouse", InputDevicesManager.GetKeyboardAndMouseSchemeDevices());
            }

            if (_joinedPlayers.Contains(controller))
                _joinedPlayers.Remove(controller);

            RemovePlayerFromSlot(PlayerToSlotIndex(playerIndex));

            OnPlayerLeft?.Invoke(playerIndex);
            Debug.Log($"Player {playerIndex} left!");
        }

        void ConfigurePlayer(PlayerInput player, int playerIndex)
        {
            if (player.TryGetComponent(out PlayerInputController input))
            {
                input.AssignInputChannel(_inputChannels[PlayerToSlotIndex(playerIndex)]);
                input.EnableMultiplayerEventSystem(PlatformManager.IsStandalone);   // The multiplayer event system should only be enabled for PC
            }

            player.transform.SetParent(transform);
            player.name = "PlayerInput" + playerIndex;

            ApplyCurrentActionMapToPlayer(player);
        }

        bool IsPlayerSharingKeyboard(PlayerInput player)
        {
            return player.currentControlScheme == "KeyboardLeft" || player.currentControlScheme == "KeyboardRight";
        }
        #endregion
    }
}
