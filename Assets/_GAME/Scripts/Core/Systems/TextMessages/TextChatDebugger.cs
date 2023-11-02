using UnityEngine;

namespace MemeFight
{
    using UI.TextChat;

    public class TextChatDebugger : MonoBehaviour
    {
        [SerializeField] TextChatSO _chat;
        [SerializeField] TextChatController _controller;

        void Start()
        {
            _controller.SetupChat(_chat);
        }
    }
}
