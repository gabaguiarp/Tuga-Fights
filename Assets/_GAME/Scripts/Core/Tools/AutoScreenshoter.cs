using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MemeFight
{
    public class AutoScreenshoter : MonoBehaviour
    {
        [Tooltip("Capture screenshots every 'x' amount of seconds")]
        [SerializeField] float _captureRate = 2.0f;

        [Header("Automation Events")]
        [Tooltip("The event to listen to in order to start the screenshot capturing process")]
        [SerializeField] VoidEventSO _startEvent;
        [Tooltip("The event to listen to in order to stop the screenshot capturing process")]
        [SerializeField] VoidEventSO _stopEvent;

        [Space(10)]
        [SerializeField, ReadOnly] bool _isActive;

        float _nextCaptureTime;

        public bool IsActive => _isActive;

        void OnEnable()
        {
            if (_startEvent != null)
                _startEvent.OnRaised += StartCapture;

            if (_stopEvent != null)
                _stopEvent.OnRaised += StopCapture;
        }

        void OnDisable()
        {
            if (_startEvent != null)
                _startEvent.OnRaised -= StartCapture;

            if (_stopEvent != null)
                _stopEvent.OnRaised -= StopCapture;
        }

        void Update()
        {
            if (!_isActive)
                return;

            if (Time.time >= _nextCaptureTime)
            {
                CaptureScreenshot();
            }
        }

        void CaptureScreenshot()
        {
            Screenshots.Capture(outputPath: Screenshots.ScreenshotsDefaultPath + "Automated/");
            CalculateNextCaptureTime();
        }

        void CalculateNextCaptureTime()
        {
            _nextCaptureTime = Time.time + _captureRate;
        }

        public void StartCapture()
        {
            _isActive = true;
            CalculateNextCaptureTime();
            Debug.Log("Auto Screenshoter started");
        }

        public void StopCapture()
        {
            _isActive = false;
            Debug.Log("Auto Screenshoter stopped");
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(AutoScreenshoter))]
    public class AutoScreenshoterEditor : Editor
    {
        AutoScreenshoter _baseScript;

        readonly string StartCaptureLabel = "Start Capture";
        readonly string StopCaptureLabel = "Stop Capture";

        void OnEnable()
        {
            _baseScript = (AutoScreenshoter)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Application.isPlaying)
            {
                GUILayout.Space(10);

                string btnLabel = _baseScript.IsActive ? StopCaptureLabel : StartCaptureLabel;
                if (GUILayout.Button(btnLabel))
                {
                    ToggleCapture();
                }
            }
        }

        void ToggleCapture()
        {
            if (_baseScript.IsActive)
            {
                _baseScript.StopCapture();
            }
            else
            {
                _baseScript.StartCapture();
            }
        }
    }
#endif
}
