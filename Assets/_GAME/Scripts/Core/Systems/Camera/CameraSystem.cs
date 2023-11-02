using Cinemachine;
using UnityEngine;

namespace MemeFight
{
    public class CameraSystem : MonoBehaviour
    {
        [SerializeField] CinemachineVirtualCamera _matchVcam;

        NoiseParameters _defaultNoiseParams;
        NoiseParameters _disabledNoiseParams = new NoiseParameters();

        CinemachineBasicMultiChannelPerlin _matchCamNoise;

        void Awake()
        {
            _matchCamNoise = _matchVcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            ConfigureCamera();

            SettingsManager.OnCameraNoiseUpdated += HandleCameraNoiseSettingsUpdated;
        }

        void OnDestroy()
        {
            SettingsManager.OnCameraNoiseUpdated -= HandleCameraNoiseSettingsUpdated;
        }

        void ConfigureCamera()
        {
            if (_matchCamNoise != null)
                _defaultNoiseParams = new NoiseParameters(_matchCamNoise.m_AmplitudeGain, _matchCamNoise.m_FrequencyGain);

            if (!SettingsManager.CameraNoiseEnabled)
                SetNoiseEnabled(false);
        }

        #region Camera Noise
        void ApplyNoiseToMatchCamera(NoiseParameters parameters)
        {
            if (_matchCamNoise == null)
            {
                Debug.LogWarning("Unable to apply camera noise parameters because no BasicMultiChannelPerlin component was found!");
                return;
            }

            _matchCamNoise.m_AmplitudeGain = parameters.Amplitude;
            _matchCamNoise.m_FrequencyGain = parameters.Frequency;
        }

        void HandleCameraNoiseSettingsUpdated() => SetNoiseEnabled(SettingsManager.CameraNoiseEnabled);

        public void SetNoiseEnabled(bool enable)
        {
            NoiseParameters targetParams = enable ? _defaultNoiseParams : _disabledNoiseParams;
            ApplyNoiseToMatchCamera(targetParams);
        }

        struct NoiseParameters
        {
            public NoiseParameters(float amplitude, float frequency)
            {
                Amplitude = amplitude;
                Frequency = frequency;
            }

            public float Amplitude { get; private set; }
            public float Frequency { get; private set; }
        }
        #endregion
    }
}
