using System;
using UnityEngine;

namespace MemeFight
{
    using UI.TextChat;

    public class TextChatController : MonoBehaviour
    {
        [SerializeField] TextChatPanelUI _textChatPanel;
        [SerializeField] bool _setTimeInUpdate = true;
        [SerializeField] bool _setBatteryLevelInUpdate = true;

        [Header("Broadcasting On")]
        [SerializeField] VoidEventSO _textChatFinishedEvent;

        [Header("Info")]
        [SerializeField, ReadOnly] TextChatSO _currentChat;
        [SerializeField, ReadOnly] int _currentMessageIndex = -1;
        
        int _currentHours, _currentMinutes;
        float _batteryLevel;
        Action _textChatAction;

        bool IsDisplayingLastMessage => _currentChat && _currentMessageIndex == _currentChat.Messages.LastIndex();

        void Awake()
        {
            _textChatPanel.OnActionButtonClicked += InvokeTextChatAction;

            _textChatPanel.ClearMessageText();
            _textChatPanel.ClearActionText();
            _textChatPanel.SetMessageIconVisibility(false);
        }

        #region Status Indicators Control
        void Update()
        {
            if (_setTimeInUpdate)
                SetCurrentTime();

            if (_setBatteryLevelInUpdate)
                SetBatteryLevel();
        }

        void SetCurrentTime()
        {
            _currentHours = DateTime.Now.Hour;
            _currentMinutes = DateTime.Now.Minute;
            _textChatPanel.SetClockTime(_currentHours, _currentMinutes);
        }

        void SetBatteryLevel()
        {
            _batteryLevel = SystemInfo.batteryLevel;
            _textChatPanel.SetBatteryLevel(_batteryLevel);
        }
        #endregion

        #region Chat Messages Control
        void OpenChat()
        {
            _textChatPanel.SetMessageIconVisibility(false);
            AdvanceMessage();
        }

        void AdvanceMessage()
        {
            if (_currentChat == null)
            {
                Debug.LogError("Unable to advance message because no text chat is set!");
                return;
            }

            if (_currentMessageIndex < _currentChat.Messages.Length)
            {
                _currentMessageIndex++;
                _textChatPanel.SetMessageText(_currentChat.Messages[_currentMessageIndex]);

                if (!IsDisplayingLastMessage)
                {
                    SetupTextChatAction(AdvanceMessage, "Next");
                }
                else
                {
                    SetupTextChatAction(EndChat, "Fight");
                }
            }
        }

        void EndChat()
        {
            Debug.Log("Chat ended");
            DisplayPanel(false);
            _textChatFinishedEvent.Raise();
        }
        #endregion

        #region Chat Action Control
        void SetupTextChatAction(Action action, string actionName)
        {
            _textChatAction = action;
            _textChatPanel.SetActionText(actionName);
        }

        void InvokeTextChatAction()
        {
            _textChatAction?.Invoke();
        }
        #endregion

        public void DisplayPanel(bool display)
        {
            _textChatPanel.gameObject.SetActive(display);
        }

        /// <summary>
        /// Sets the <paramref name="chat"/> to display.
        /// </summary>
        public void SetupChat(TextChatSO chat)
        {
            _currentChat = chat;
            _currentMessageIndex = -1;

            _textChatPanel.SetMessageIconVisibility(true);
            _textChatPanel.SetMessageText(chat.MessageCount + " Messages received");
            SetupTextChatAction(OpenChat, "Read");
        }
    }
}
