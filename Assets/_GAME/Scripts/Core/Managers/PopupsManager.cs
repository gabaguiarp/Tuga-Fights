using UnityEngine;

namespace MemeFight
{
    using UI.Popups;

    public class PopupsManager : Singleton<PopupsManager>
    {
        [Header("References")]
        [SerializeField] ModalWindowUI _modalWindow;

        public bool IsPopupOpen => _modalWindow.IsOpen;

        protected override void Awake()
        {
            base.Awake();
            _modalWindow.Close();
        }

        public void DisplayWindow(ModalWindowTrigger window)
        {
            _modalWindow.Configure(window.displayTextString, window.confirmAction, window.declineAction);
            _modalWindow.Open();
        }

        public void CloseCurrentWindow()
        {
            _modalWindow.Close();
        }
    }
}
