using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

namespace MemeFight.UI.Popups
{
    public class ModalWindowTrigger : MonoBehaviour
    {
        public LocalizedString displayTextString;

        [Space(10)]
        public ButtonConfig confirmAction;
        public ButtonConfig declineAction;

        [ContextMenu("Open Window")]
        public void OpenWindow()
        {
            if (Application.isPlaying)
                PopupsManager.Instance.DisplayWindow(this);
        }

        [ContextMenu("Close Window")]
        public void CloseWindow()
        {
            if (Application.isPlaying)
                PopupsManager.Instance.CloseCurrentWindow();
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
