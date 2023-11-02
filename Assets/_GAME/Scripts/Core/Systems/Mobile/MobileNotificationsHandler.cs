using MemeFight.Services;
using UnityEngine;

namespace MemeFight.Mobile
{
    public class MobileNotificationsHandler : Singleton<MobileNotificationsHandler>
    {
        [Header("Notifications")]
        [SerializeField] MobileNotificationSO _questsNotification;
        [SerializeField] MobileNotificationSO _testNotification;

        [Header("Listening On")]
        [SerializeField] VoidEventSO _questlineCompleteEvent;

        void OnEnable()
        {
            _questlineCompleteEvent.OnRaised += OnQuestlineComplete;
        }

        void OnDisable()
        {
            _questlineCompleteEvent.OnRaised -= OnQuestlineComplete;
        }

        protected override void Awake()
        {
            base.Awake();

            // Check if the app was opened through a notification
            if (MobileNotifications.TryGetLastNotificationIntentData(out string data))
            {
                Debug.LogFormat("App was opened via notification '{0}'", data);
                Analytics.RegisterEvent(Analytics.Event.NOTIFICATION_TAPPED);
            }
        }

        void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                // When the application is paused, a notification is sent to the player reminding them that there are still
                // quests to complete (in case the active questline hasn't been completed yet).
                if (!ResourcesManager.PersistentData.WasCurrentQuestlineComplete)
                {
                    MobileNotifications.SendNotification(_questsNotification);
                }
            }
            else
            {
                // When the application is unpaused, all delevered notifications are cleared from the tray.
                // All scheduled notifications are cancelled as well, to avoid them being delivered while the game is running.
                MobileNotifications.ClearAllDisplayedNotifications();
                MobileNotifications.CancelAllScheduledNotifications();
            }
        }

        void OnQuestlineComplete()
        {
            MobileNotifications.CancelScheduledNotification(_questsNotification);
        }
    }
}
