using Firebase.Messaging;
using UnityEngine;

namespace MemeFight.Services
{
    public class FirebaseCloudMessagingServiceHanlder : MonoBehaviour
    {
        [SerializeField, ReadOnly] bool _isRunning = false;

        public bool IsRunning => _isRunning;

        public void Init()
        {
            if (_isRunning)
                return;

            FirebaseMessaging.TokenReceived += OnTokenReceived;
            FirebaseMessaging.MessageReceived += OnMessageReceived;

            Debug.Log("[Services] Firebase Cloud Messaging initialised.");

            _isRunning = true;
        }

        void OnTokenReceived(object sender, TokenReceivedEventArgs token)
        {
            Debug.Log("Received Registration Token: " + token.Token);
        }

        void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Debug.Log("Received a new message from: " + e.Message.From);
            Debug.Log("Message ID: " + e.Message.MessageId);
        }

        public void Shutdown()
        {
            FirebaseMessaging.TokenReceived -= OnTokenReceived;
            FirebaseMessaging.MessageReceived -= OnMessageReceived;

            Debug.Log("[Services] Firebase Cloud Messaging shutdown.");

            _isRunning = false;
        }
    }
}
