using UnityEngine;
using UnityEngine.Localization;

namespace MemeFight.Mobile
{
    [CreateAssetMenu(fileName = "MobileNotification", menuName = MenuPaths.Mobile + "Notification")]
    public class MobileNotificationSO : BaseSO
    {
        [Space(10)]
        [SerializeField] int _id;
        [SerializeField] string _name = "Notification";
        [SerializeField] LocalizedString _title;
        [SerializeField] LocalizedString _text;
        [field: SerializeField]
        public NotificationFireTime FireTime { get; private set; }

        public int ID => _id;
        public string Name => _name;
        public string Title => _title.GetLocalizedString();
        public string Text => _text.GetLocalizedString();
    }
}
