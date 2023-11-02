using UnityEngine;

namespace MemeFight.UI
{
    using Menus;

    public class MenuUIBase : ManagedBehaviour
    {
        [SerializeField] protected GameObject _defaultSelection;
        [SerializeField] protected bool _registerMenuCommands = true;
        [Tooltip("The buttons or button groups that the menu should track clicks of, so it can store them as the previous menu selection. " +
            "This also looks for ButtonUI elements in the children of the assigned references, when no button is found in the assigned " +
            "root GameObject.")]
        [SerializeField] protected GameObject[] _buttonsToTrack;

        private GameObject _previousSelection;

        public GameObject PreviousSelection
        {
            get
            {
                if (_previousSelection == null)
                    return _defaultSelection;

                return _previousSelection;
            }
        }

        public bool IsOpen => gameObject.activeInHierarchy;

        protected virtual void OnEnable()
        {
            SelectPreviousIfEnabled();
        }

        protected override void Awake()
        {
            base.Awake();
            TrackAssignedButtons();
        }

        protected override void OnSceneReady()
        {
            SelectPreviousIfEnabled();
        }

        protected virtual void SelectPreviousIfEnabled()
        {
            if (gameObject.activeInHierarchy)
                SelectPrevious();
        }

        private void TrackAssignedButtons()
        {
            // Subscribe selection update callback to all button click events
            if (_buttonsToTrack == null)
                return;

            foreach (GameObject obj in _buttonsToTrack)
            {
                if (obj.TryGetComponent(out ButtonUI rootBtn))
                {
                    ListenToButtonClick(rootBtn);
                }
                else
                {
                    foreach (ButtonUI btn in obj.GetComponentsInChildren<ButtonUI>())
                    {
                        ListenToButtonClick(btn);
                    }
                }
            }

            void ListenToButtonClick(ButtonUI button)
            {
                button.OnClicked += () => SelectionUpdated(button.gameObject);
            }
        }

        #region Overridable Callbacks
        protected virtual void OnOpen() { }
        protected virtual void OnClose() { }
        protected virtual void OnBackCommand() { }
        #endregion

        public void Open()
        {
            if (IsOpen)
                return;

            gameObject.SetActive(true);
            OnOpen();
            RegisterMenuCommands();
        }

        public void Close()
        {
            if (!IsOpen)
                return;

            ClearMenuCommands();
            OnClose();
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Closes the UI element without invoking the <i>OnClose</i> method.
        /// </summary>
        public void ExclusiveClose()
        {
            if (IsOpen)
            {
                ClearMenuCommands();
                gameObject.SetActive(false);
            }
        }

        public void SelectPrevious()
        {
            if (PreviousSelection)
                MenuSelectionHandler.Select(PreviousSelection);
        }

        public void SelectionUpdated(GameObject selection)
        {
            _previousSelection = selection;
        }

        public void RegisterMenuCommands()
        {
            MenuCommandsSystem.Register(OnBackCommand);
        }

        public void ClearMenuCommands()
        {
            MenuCommandsSystem.Clear();
        }
    }
}
