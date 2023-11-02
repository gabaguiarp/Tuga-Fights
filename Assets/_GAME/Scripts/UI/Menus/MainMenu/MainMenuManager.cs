using System.Collections;
using UnityEngine;

namespace MemeFight.Menus
{
    using Mobile;
    using UI;

    public class MainMenuManager : ManagedBehaviour
    {
        [SerializeField] bool _openMainScreenOnStart = true;
        [SerializeField] MenuScreensGroupManager _menuScreensManager;
        [SerializeField] MainMenuUI _mainMenuScreen;
        [SerializeField] QuestChecker _questsChecker;

        [Header("Mobile Notifications")]
        [SerializeField] GameObject _notificationsHandlerPrefab;

        [Header("Debug")]
        [SerializeField, ReadOnly] bool _hasCheckedQuests = false;

        bool _isWaitingForQuestChecking = false;
        InputManager _inputManager;

        protected override void Awake()
        {
            base.Awake();
            _mainMenuScreen.OnOpened += _ => OnMainMenuScreenOpen();
            _questsChecker.OnQuestCheckingStarted += HandleWaitForQuestChecking;
        }

        protected override void OnSceneReady()
        {
            _inputManager = InputManager.Instance;

            // When playing on mobile, ensure an instance of the MobileNotificationsHandler singleton exists
            if (PlatformManager.IsMobile)
            {
                EnsureNotificationsHandler();
            }

            if (_openMainScreenOnStart)
            {
                _menuScreensManager.OpenMainScreen();
            }
            else
            {
                EnableInput();
            }
        }

        void OnMainMenuScreenOpen()
        {
            if (_hasCheckedQuests)
                return;

            // First we refresh the quests panel display with the last values registered in the persistent data.
            _questsChecker.RefreshDisplay();

            // Then we verify if the quest completion progress has been updated, in relation to the last registered value.
            // If so, we start the checking process.
            if (_questsChecker.WasQuestProgressUpdated())
            {
                StartCoroutine(CheckQuestsWithDelay());
            }
            else
            {
                EnableInput();
            }

            IEnumerator CheckQuestsWithDelay()
            {
                _inputManager.DisableInput();
                yield return CoroutineUtils.GetWaitRealtime(1.0f);
                _questsChecker.CheckQuests();
            }
        }

        void HandleWaitForQuestChecking()
        {
            if (!_isWaitingForQuestChecking)
                StartCoroutine(WaitForQuestCheckingProcess());

            IEnumerator WaitForQuestCheckingProcess()
            {
                _isWaitingForQuestChecking = true;

                if (_inputManager.IsInputEnabled)
                    _inputManager.DisableInput();

                while (_questsChecker.IsCheckingQuests)
                {
                    yield return null;
                }

                yield return CoroutineUtils.GetWaitRealtime(0.5f);

                EnableInput();
                _hasCheckedQuests = true;

                _isWaitingForQuestChecking = false;
            }
        }

        void EnableInput()
        {
            _inputManager.EnableMenusInput();
        }

        void EnsureNotificationsHandler()
        {
            if (MobileNotificationsHandler.Instance == null)
            {
                try
                {
                    var obj = Instantiate(_notificationsHandlerPrefab);
                    obj.name = nameof(MobileNotificationsHandler);
                    Debug.Log("MobileNotificationsHandler instantiated");
                }
                catch (System.Exception e)
                {
                    throw new System.Exception("Failed to instantiate MobileNotificationsHandler with exception: " + e);
                }
            }
        }
    }
}
