using System;
using UnityEngine;

namespace MemeFight.Menus
{
    public class MenuCommandsSystem : MonoBehaviour
    {
        [SerializeField] MenusInputEventChannelSO _menusInput;

        Action _backAction;

        static MenuCommandsSystem s_Current = null;
        public static MenuCommandsSystem Current { get { return s_Current; } }

        void OnEnable()
        {
            _menusInput.OnBack += ProcessBackInput;
        }

        void OnDisable()
        {
            _menusInput.OnBack -= ProcessBackInput;
        }

        void Awake()
        {
            if (s_Current == null)
            {
                // Set current instance reference to this one
                s_Current = this;
            }
            else
            {
                Debug.LogWarning("There seems to be more than one MenuCommandsSystem in the hierarchy! Only one instance is allowed.");
            }
        }

        void OnDestroy()
        {
            if (s_Current == this)
                s_Current = null;
        }

        #region Internal Callbacks
        void RegisterInternal(Action backAction)
        {
            _backAction = backAction;
            Debug.LogFormat("Commands registered in the current MenuCommandsSystem. Back: {0}", backAction.Method.Name.ToString());
        }

        void ClearInternal()
        {
            _backAction = null;
            Debug.Log("All commands cleared in the current MenuCommandsSystem");
        }
        #endregion

        #region Input Event Responders
        void ProcessBackInput()
        {
            if (_backAction != null)
                _backAction.Invoke();
        }
        #endregion

        #region External Static Callbacks
        public static void Register(Action backAction = null)
        {
            if (s_Current == null)
            {
                Debug.LogWarning("Unable to setup menu commands because no MenuCommandsSystem instance is currently available!");
                return;
            }

            s_Current.RegisterInternal(backAction);
        }

        public static void Clear()
        {
            if (s_Current == null)
                return;

            s_Current.ClearInternal();
        }
        #endregion
    }
}
