using TMPro;
using UnityEngine;

namespace MemeFight.DebugSystem
{
    public class FPSCounter : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _counterText;

        int _lastFramIndex;
        float[] _frameDeltaTimeArray;

        const int FrameRateSamplesToAverage = 50;

        void Reset()
        {
            _counterText = GetComponent<TextMeshProUGUI>();
        }

        void Awake()
        {
            _frameDeltaTimeArray = new float[FrameRateSamplesToAverage];
        }

        void Update()
        {
            _frameDeltaTimeArray[_lastFramIndex] = Time.deltaTime;
            _lastFramIndex = (_lastFramIndex + 1) % _frameDeltaTimeArray.Length;
            _counterText.SetText(Mathf.RoundToInt(GetFPSAverage()) + " fps");
        }

        float GetFPSAverage()
        {
            float total = 0.0f;
            foreach (float deltaTime in _frameDeltaTimeArray)
            {
                total += deltaTime;
            }

            return _frameDeltaTimeArray.Length / total;
        }
    }
}
