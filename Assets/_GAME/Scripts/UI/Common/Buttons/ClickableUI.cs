using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MemeFight.UI
{
    public class ClickableUI<T> : MonoBehaviour where T : Selectable
    {
        T _clickable;

        protected T Clickable
        {
            get
            {
                if (_clickable == null)
                    ConfigureClickable();

                return _clickable;
            }
        }

        public event UnityAction OnClicked;
        public event UnityAction OnSelected;

        protected virtual void Reset()
        {
            ConfigureClickable();
        }

        protected virtual void Awake()
        {
            ConfigureClickable();
        }

        void ConfigureClickable()
        {
            if (_clickable == null && !gameObject.TryGetComponent(out _clickable))
            {
                _clickable = gameObject.AddComponent(typeof(T)) as T;
            }
        }

        protected void RaiseClickedEvent() => OnClicked?.Invoke();
    }
}
