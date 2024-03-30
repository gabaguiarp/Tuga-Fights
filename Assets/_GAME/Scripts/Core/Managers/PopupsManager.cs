using UnityEngine;

namespace MemeFight
{
    using UI.Popups;

    public class PopupsManager : Singleton<PopupsManager>
    {
        [Header("References")]
        [SerializeField] ModalWindowUI _modalWindow;
        [SerializeField] ModalWindowUI _bonusModalWindow;

        public bool IsPopupOpen => _modalWindow.IsOpen || _bonusModalWindow.IsOpen;

        protected override void Awake()
        {
            base.Awake();
            _modalWindow.Close();
            _bonusModalWindow.Close();
        }

        public void DisplayWindow(ModalWindowTrigger window)
        {
            if (_bonusModalWindow.IsOpen)
                _bonusModalWindow.Close();

            _modalWindow.Configure(window.displayTextString, window.confirmAction, window.declineAction);
            _modalWindow.Open();
        }

        public void DisplayBonusWindow(BonusModalWindowTrigger window)
        {
            if (_modalWindow.IsOpen)
                _modalWindow.Close();

            _bonusModalWindow.Configure(window.displayTextString, window.imageToDisplay, window.confirmAction, window.declineAction);
            _bonusModalWindow.Open();
        }

        public void CloseCurrentWindow()
        {
            _modalWindow.Close();
            _bonusModalWindow.Close();
        }
    }
}
