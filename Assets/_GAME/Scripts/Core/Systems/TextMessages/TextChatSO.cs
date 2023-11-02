using UnityEngine;

namespace MemeFight.UI.TextChat
{
    [CreateAssetMenu(fileName = "TextChat", menuName = MenuPaths.Chat + "Text Chat")]
    public class TextChatSO : ScriptableObject
    {
        [TextArea(2, 4)]
        public string[] Messages;

        public int MessageCount => Messages.Length;
    }
}
