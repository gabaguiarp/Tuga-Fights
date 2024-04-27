using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

namespace MemeFight.UI.Popups
{
    public class ModalWindowTrigger : MonoBehaviour
    {
        public LocalizedString displayTextString;
        [Tooltip("The audio cue to play when this popup is displayed.")]
        public AudioCueSO popupAudioCue;

        [Header("Actions")]
        public ButtonConfig confirmAction;
        public ButtonConfig declineAction;

        public event UnityAction OnPopupClosed;

        [ContextMenu("Open Window")]
        public bool OpenWindow()
        {
            if (Application.isPlaying)
            {
                OnOpenWindowCallback();
                return true;
            }

            return false;
        }

        [ContextMenu("Close Window")]
        public void CloseWindow()
        {
            if (Application.isPlaying)
            {
                PopupsManager.Instance.CloseCurrentWindow();
                OnPopupClosed?.Invoke();
            }
        }

        protected virtual void OnOpenWindowCallback()
        {
            PopupsManager.Instance.DisplayWindow(this);
        }
    }

    [Serializable]
    public class ButtonConfig
    {
        public ButtonConfig(LocalizedString nameString, UnityAction callback, Sprite icon = null)
        {
            displayNameString = nameString;
            this.icon = icon;
            this.callback.RemoveAllListeners();
            this.callback.AddListener(callback);
            isEnabled = true;
        }

        public LocalizedString displayNameString;
        public Sprite icon;
        public UnityEvent callback;
        public bool isEnabled = true;

        public bool HasCallback => callback.GetPersistentEventCount() > 0;
    }
}
