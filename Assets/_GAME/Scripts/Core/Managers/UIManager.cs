using UnityEngine;
using UnityEngine.EventSystems;

namespace MemeFight.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] GameplayUI _gameplayUI;
        [SerializeField] PauseMenuUI _pauseMenu;

        [Header("Listening On")]
        [SerializeField] MenusInputEventChannelSO _menusInputChannel;

        void OnEnable()
        {
            _menusInputChannel.OnPause += TogglePauseMenu;
        }

        void OnDisable()
        {
            _menusInputChannel.OnPause -= TogglePauseMenu;
        }

        void Awake()
        {
            _gameplayUI.OnPauseClicked += OpenPauseMenu;
            _pauseMenu.OnOpened += _ => HandlePauseMenuOpen();
            _pauseMenu.OnClosed += _ => HandlePauseMenuClosed();
        }

        void OpenPauseMenu()
        {
            _pauseMenu.Open();
        }

        void TogglePauseMenu()
        {
            if (!_pauseMenu.IsOpen)
                _pauseMenu.Open();
            else
                _pauseMenu.Close();
        }

        void HandlePauseMenuOpen()
        {
            GameManager.PauseGame();
            InputManager.Instance.EnableMenusInput();
        }

        void HandlePauseMenuClosed()
        {
            GameManager.PauseGame(false);
            InputManager.Instance.SwitchToPreviousInput();
        }

        public static bool IsPointerOverUI()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }
    }
}
