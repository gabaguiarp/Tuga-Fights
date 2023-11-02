using System;
using Unity.Notifications.Android;
using UnityEngine.Android;

namespace MemeFight.Mobile
{
    using DebugSystem;

    public enum NotificationFireTime
    {
        After1Minute,
        After1Hour,
        After3Hours,
        After6Hours,
        After12Hours,
        After24Hours
    }

    // Docs: https://docs.unity3d.com/Packages/com.unity.mobile.notifications@1.4/manual/Android.html
    public class MobileNotifications
    {
        static AndroidNotificationChannel ChannelAndroidMain;

        readonly static string MainChannelID = "main";
        readonly static string MainChannelName = "General";
        readonly static string MainChannelDescription = "Generic notifications";
        readonly static Importance MainChannelImportanceAndroid = Importance.Default;
        readonly static string SmallIconID = "icon_tray";
        readonly static string LargeIconID = "icon_full";

        /// <summary>
        /// Whether notifications in the main channel are currently enabled.
        /// </summary>
        public static bool IsEnabled
        {
            get
            {
#if UNITY_ANDROID
                return ChannelAndroidMain.Importance != Importance.None;
#else
                return false;
#endif
            }
        }

        public static void Init()
        {
#if UNITY_ANDROID
            InitializeMainNotificationChannelAndroid();
#endif
            ClearAllDisplayedNotifications();
        }

        public static void SendNotification(MobileNotificationSO notification)
        {
            // Before sending a notification, we check if it has already been sent before. If that's the case, we cancel
            // the previous one first, then replace it with the new one.
            if (IsNotificationScheduled(notification))
                CancelScheduledNotification(notification);

#if UNITY_ANDROID
            SendNotificationAndroid(notification);
#endif
        }

        public static void CancelScheduledNotification(MobileNotificationSO notification)
        {
            if (IsNotificationScheduled(notification))
            {
#if UNITY_ANDROID
                AndroidNotificationCenter.CancelNotification(notification.ID);
#endif
                Logger.LogMessageFormat("Notification '{0}' cancelled", MessageType.Default, notification.Name);
            }
        }

        public static void CancelAllScheduledNotifications()
        {
#if UNITY_ANDROID
            AndroidNotificationCenter.CancelAllScheduledNotifications();
#endif
        }

        /// <summary>
        /// Removes all notifications sent by this app from the system tray.
        /// </summary>
        public static void ClearAllDisplayedNotifications()
        {
#if UNITY_ANDROID
            AndroidNotificationCenter.CancelAllDisplayedNotifications();
#endif
        }

        public static bool IsNotificationScheduled(MobileNotificationSO notification)
        {
#if UNITY_ANDROID
            return AndroidNotificationCenter.CheckScheduledNotificationStatus(notification.ID) == NotificationStatus.Scheduled;
#else
            return false;
#endif
        }

        public static bool TryGetLastNotificationIntentData(out string data)
        {
#if UNITY_ANDROID
            var notificationData = AndroidNotificationCenter.GetLastNotificationIntent();
            if (notificationData != null)
            {
                data = notificationData.Notification.IntentData;
                return true;
            }
#endif
            data = null;
            return false;
        }

        #region Internal Methods
        static void InitializeMainNotificationChannelAndroid()
        {
            // Create the main notification channel
            ChannelAndroidMain = new AndroidNotificationChannel()
            {
                Id = MainChannelID,
                Name = MainChannelName,
                Importance = MainChannelImportanceAndroid,
                Description = MainChannelDescription
            };

            AndroidNotificationCenter.RegisterNotificationChannel(ChannelAndroidMain);

            // Request user permission to show notifications (SDK 33+)
            if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
            {
                Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
            }
        }

        static void SendNotificationAndroid(MobileNotificationSO notif)
        {
            var notification = new AndroidNotification()
            {
                Title = notif.Title,
                Text = notif.Text,
                IntentData = notif.Name,
                SmallIcon = SmallIconID,
                FireTime = GetDateTimeFromFireTime(notif.FireTime),
                ShouldAutoCancel = true
            };

            AndroidNotificationCenter.SendNotificationWithExplicitID(notification, MainChannelID, notif.ID);
            Logger.LogMessageFormat("Android notification '{0}' sent. Will fire {1}", MessageType.Default,
                                    notif.Name, notif.FireTime.ToString());
        }

        static DateTime GetDateTimeFromFireTime(NotificationFireTime fireTime)
        {
            switch (fireTime)
            {
                case NotificationFireTime.After1Hour:
                    return DateTime.Now.AddHours(1);
                case NotificationFireTime.After3Hours:
                    return DateTime.Now.AddHours(3);
                case NotificationFireTime.After6Hours:
                    return DateTime.Now.AddHours(6);
                case NotificationFireTime.After12Hours:
                    return DateTime.Now.AddHours(12);
                case NotificationFireTime.After24Hours:
                    return DateTime.Now.AddHours(24);
                default:
                    return DateTime.Now.AddMinutes(1);
            }
        }
        #endregion
    }
}
