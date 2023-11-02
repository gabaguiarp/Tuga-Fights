using UnityEngine;

namespace MemeFight.DebugSystem
{
    public enum MessageType { Default, Warning, Error }

    public class Logger
    {
        public static void LogAIMessage(string message, MessageType messageType = MessageType.Default)
        {
            LogMessage("[AI LOG] " + message, messageType);
        }

        public static void LogMessage(string message, MessageType messageType = MessageType.Default)
        {
#if DEBUG_MODE
            switch (messageType)
            {
                case MessageType.Default:
                    Debug.Log(message);
                    break;

                case MessageType.Warning:
                    Debug.LogWarning(message);
                    break;

                case MessageType.Error:
                    Debug.LogError(message);
                    break;
            }
#endif
        }

        public static void LogMessageFormat(string message, MessageType messageType = MessageType.Default, params object[] args)
        {
#if DEBUG_MODE
            switch (messageType)
            {
                case MessageType.Default:
                    Debug.LogFormat(message, args);
                    break;

                case MessageType.Warning:
                    Debug.LogWarningFormat(message, args);
                    break;

                case MessageType.Error:
                    Debug.LogErrorFormat(message, args);
                    break;
            }
#endif
        }
    }
}
