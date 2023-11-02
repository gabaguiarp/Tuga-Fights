using UnityEngine;
using UnityEngine.Events;

namespace MemeFight.UI
{
    public class MenuScreenUI : MenuUIBase
    {
        [System.Serializable]
        public struct ScreenEvents
        {
            public UnityEvent OnScreenOpen;
            public UnityEvent OnScreenClose;
        }

        [SerializeField] ScreenEvents _screenEvents;

        public event UnityAction<MenuScreenUI> OnOpened;
        public event UnityAction<MenuScreenUI> OnClosed;

        public ScreenEvents Events => _screenEvents;

        protected override void OnOpen()
        {
            _screenEvents.OnScreenOpen.Invoke();
            OnOpened?.Invoke(this);
        }

        protected override void OnClose()
        {
            _screenEvents.OnScreenClose.Invoke();
            OnClosed?.Invoke(this);
        }
    }
}
