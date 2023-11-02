using System.Collections.Generic;
using UnityEngine;

namespace MemeFight.UI
{
    /// <summary>
    /// Manages the UI for menus with multiple screens, ensuring the previous screen is closed when a new one is opened.
    /// </summary>
    public class MenuScreensGroupManager : MonoBehaviour
    {
        [Tooltip("The menu landing screen. This is the screen we always return to when closing other ones.")]
        [SerializeField] MainMenuUI _mainScreen;

        [Space(10)]
        [Tooltip("The set of screens to manage, including the main one.")]
        [SerializeField] List<MenuScreenUI> _screensToManage = new List<MenuScreenUI>();

        void Awake()
        {
            foreach (var screen in _screensToManage)
            {
                if (screen == _mainScreen)
                {
                    continue;
                }
                else
                {
                    screen.OnOpened += CloseAllScreensExcept;
                    screen.OnClosed += HandleScreenClosed;
                }
            }
        }

        [ContextMenu("Open Main Screen")]
        public void OpenMainScreen()
        {
            _screensToManage.ForEach(s => s.ExclusiveClose());

            if (!_mainScreen.IsOpen)
                _mainScreen.Open();

#if UNITY_EDITOR
            if (!Application.isPlaying)
                UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
        }

        void CloseAllScreensExcept(MenuScreenUI screenToKeepOpen)
        {
            foreach (MenuScreenUI s in _screensToManage)
            {
                if (s == screenToKeepOpen)
                    continue;
                else if (s.IsOpen)
                    s.Close();
            }
        }

        void HandleScreenClosed(MenuScreenUI screen)
        {
            if (screen != _mainScreen)
            {
                OpenMainScreen();
            }
        }
    }
}
