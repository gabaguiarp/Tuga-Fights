using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MemeFight.UI.TextChat
{
    public class TextChatPanelUI : MonoBehaviour
    {
        public event UnityAction OnActionButtonClicked;

        [Header("Status Bars")]
        [SerializeField] StatusBarUI _networkStatusDisplay;
        [SerializeField] StatusBarUI _batteryStatusDisplay;

        [Header("Top Bar")]
        [SerializeField] Image _messageIcon;
        [SerializeField] TextMeshProUGUI _clockText;

        [Header("Screen Content")]
        [SerializeField] TextMeshProUGUI _messageText;
        [SerializeField] TextMeshProUGUI _actionText;
        [SerializeField] ButtonUI _actionButton;

        int _batteryLevel;

        readonly float EmptyBatteryLevelThreshold = 0.05f;
        readonly float LowBatteryLevelThreshold = 0.16f;
        readonly float MidBatteryLevelThreshold = 0.51f;
        readonly float HighBatteryLevelThreshold = 0.9f;

        void Awake()
        {
            _actionButton.OnClicked += HandleActionButtonClicked;
        }

        void HandleActionButtonClicked()
        {
            OnActionButtonClicked?.Invoke();
        }

        public void SetMessageText(string message)
        {
            _messageText.SetText(message);
        }

        public void ClearMessageText()
        {
            _messageText.SetText(string.Empty);
        }

        public void SetActionText(string actionName)
        {
            _actionText.SetText(actionName);
        }

        public void ClearActionText()
        {
            _actionText.SetText(string.Empty);
        }

        public void SetNetworkValue(int value)
        {
            _networkStatusDisplay.SetBarsValue(value);
        }

        public void SetBatteryLevel(float value)
        {
            _batteryStatusDisplay.SetBarsValue(GetBatteryLevelInBarsFromValue(value));
        }

        public void SetMessageIconVisibility(bool visible)
        {
            _messageIcon.gameObject.SetActive(visible);
        }

        public void SetClockTime(int hour, int minutes)
        {
            _clockText.SetText(string.Format("{0}:{1}", hour.ToString("00"), minutes.ToString("00")));
        }

        int GetBatteryLevelInBarsFromValue(float value)
        {
            if (value >= 0 && value < EmptyBatteryLevelThreshold)
            {
                return 0;
            }
            else if (value >= EmptyBatteryLevelThreshold && value < LowBatteryLevelThreshold)
            {
                return 1;
            }
            else if (value >= LowBatteryLevelThreshold && value < MidBatteryLevelThreshold)
            {
                return 2;
            }
            else if (value >= MidBatteryLevelThreshold && value < HighBatteryLevelThreshold)
            {
                return 3;
            }
            else
            {
                return 4;
            }
        }
    }
}
