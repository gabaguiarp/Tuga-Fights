using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace MemeFight.UI.Popups
{
    public class MatchOverPopupHandler : MonoBehaviour
    {
        [SerializeField] ModalWindowTrigger _windowTrigger;
        [SerializeField] LocalizedString _attemptsString;

        readonly string AttemptsSmartStringKey = "attempts";
        readonly string RemainingMessageStringKey = "remainingAttempts";

        /// <summary>
        /// Shows the popup in game.
        /// </summary>
        /// <param name="remainingAttempts">If this value is greater than 0, a message with the amount of remaining attempts
        /// will be displayed.</param>
        /// <param name="canReplay">Whether to enable the 'Replay' button, even if <paramref name="remainingAttempts"/> is
        /// set to 0.</param>
        public void TriggerPopup(int remainingAttempts = 0, bool canReplay = false)
        {
            var messageArgs = new Dictionary<string, string>();
            if (remainingAttempts > 0)
            {
                var attemptArgs = new Dictionary<string, int> { [AttemptsSmartStringKey] = remainingAttempts };
                _attemptsString.Arguments = new object[] { attemptArgs };
            }

            string remainingMsg = remainingAttempts > 0 ? _attemptsString.GetLocalizedString() : string.Empty;
            messageArgs.Add(RemainingMessageStringKey, remainingMsg);

            _windowTrigger.displayTextString.Arguments = new object[] { messageArgs };
            _windowTrigger.confirmAction.isEnabled = remainingAttempts > 0 || canReplay;
            _windowTrigger.OpenWindow();
        }

#if UNITY_EDITOR
        [ContextMenu("Trigger 0 Attempts Popup")]
        void TriggerPopupNoAttempts() => ForceTriggerPopup(0);

        [ContextMenu("Trigger 1 Attempt Popup")]
        void TriggerPopupOneAttempt() => ForceTriggerPopup(1);

        [ContextMenu("Trigger 2 Attempts Popup")]
        void TriggerPopupTwoAttempts() => ForceTriggerPopup(2);

        void ForceTriggerPopup(int attempts)
        {
            if (Application.isPlaying)
            {
                TriggerPopup(attempts);
            }
        }
#endif
    }
}
