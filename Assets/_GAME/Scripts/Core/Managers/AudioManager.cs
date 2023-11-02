using MemeFight.Audio;
using System;
using UnityEngine;
using UnityEngine.Audio;

namespace MemeFight
{
    public class AudioManager : Singleton<AudioManager>
    {
        public enum SoundType { SFX, UI }

        [Serializable]
        struct ExposedParameters
        {
            public string musicVolume;
            public string sfxVolume;
        }

        [Header("Audio Mixer Settings")]
        [SerializeField] AudioMixer _mixer;
        [SerializeField] AudioMixerGroup _musicMixerGroup;
        [SerializeField] AudioMixerGroup _sfxMixerGroup;
        [SerializeField] AudioMixerGroup _uiMixerGroup;
        [SerializeField] ExposedParameters _parameters;

        [Header("Sound Emitters")]
        [SerializeField] SoundEmitter _musicEmitter;
        [SerializeField] SoundEmitterPool _emittersPool;

        const float MinVolume = -80f;

        public bool IsMusicEnabled => GetGroupVolume(_parameters.musicVolume, true) > 0;
        public bool IsSFXEnabled => GetGroupVolume(_parameters.sfxVolume, true) > 0;

        #region Value Converters
        /// <summary>
        /// Converts a [0 to 1] range value to the matching AudioMixer volume range [-80db to 0db].
        /// </summary>
        /// <param name="normalizedValue">The range value used for the conversion. If greater than 1 or smaller than 0, the value
        /// will be clamped.</param>
        /// <returns></returns>
        float NormalizedToMixerValue(float normalizedValue)
        {
            normalizedValue = Mathf.Clamp01(normalizedValue);
            return (normalizedValue - 1) * Mathf.Abs(MinVolume);
        }

        /// <summary>
        /// Converts an AudioMixer volume range [-80db to 0db] to a [0 to 1] value range.
        /// </summary>
        /// <param name="mixerValue">The AudioMixer volume value to convert.</param>
        float MixerToNormalizedValue(float mixerValue)
        {
            return 1f + (mixerValue / Mathf.Abs(MinVolume));
        }
        #endregion

        #region Volume Control
        void SetGroupVolume(string parameter, float normalizedValue)
        {
            bool volumeSet = _mixer.SetFloat(parameter, NormalizedToMixerValue(normalizedValue));
            if (!volumeSet)
                Debug.LogError($"Unable to set group volume because the '{parameter}' parameter was not found in the AudioMixer!");
        }

        /// <summary>
        /// Returns the group volume value for a specific AudioMixer group.
        /// </summary>
        /// <param name="parameter">The AudioMixer group parameter to get the volume value from.</param>
        /// <param name="normalize">Whether the returned value should be converted to a [0, 1] range.</param>
        /// <returns></returns>
        float GetGroupVolume(string parameter, bool normalize = false)
        {
            if (_mixer.GetFloat(parameter, out float value))
            {
                return normalize ? MixerToNormalizedValue(value) : value;
            }
            else
            {
                Debug.LogError($"Unable to get group volume because the '{parameter}' parameter was not found in the AudioMixer!");
                return 0f;
            }
        }

        public void EnableMusicOutput(bool enable)
        {
            SetGroupVolume(_parameters.musicVolume, Logic.BoolToFloat(enable));
        }

        public void EnableSFXOutput(bool enable)
        {
            SetGroupVolume(_parameters.sfxVolume, Logic.BoolToFloat(enable));
        }
        #endregion

        #region Sound Requests
        /// <summary>
        /// Plays a sound effect with the parameters defined in the <paramref name="audioCue"/>.
        /// </summary>
        /// <param name="autoRelease">Whether the sound emitter should automatically be returned to the pool upon finishing playing.</param>
        /// <returns>The <see cref="SoundEmitter"/> that will play the sound.</returns>
        public SoundEmitter PlaySoundEffect(AudioCueSO audioCue, bool autoRelease = true)
        {
            return PlaySound(audioCue, SoundType.SFX, autoRelease);
        }

        /// <summary>
        /// Plays a UI sound effect with the parameters defined in the <paramref name="audioCue"/>.
        /// </summary>
        /// <param name="autoRelease">Whether the sound emitter should automatically be returned to the pool upon finishing playing.</param>
        /// <returns>The <see cref="SoundEmitter"/> that will play the sound.</returns>
        public SoundEmitter PlaySoundUI(AudioCueSO audioCue, bool autoRelease = true)
        {
            return PlaySound(audioCue, SoundType.UI, autoRelease);
        }

        public void PlayMusicTrack(AudioCueSO audioCue)
        {
            _musicEmitter.Play(audioCue, _musicMixerGroup);
        }

        public void StopMusicTrack()
        {
            _musicEmitter.Stop();
        }

        SoundEmitter PlaySound(AudioCueSO audioCue, SoundType type, bool autoRelease)
        {
            SoundEmitter emitter = _emittersPool.Get();
            emitter.autoRelease = autoRelease;
            
            switch (type)
            {
                case SoundType.SFX:
                    emitter.Play(audioCue, _sfxMixerGroup);
                    break;

                case SoundType.UI:
                    emitter.Play(audioCue, _uiMixerGroup, true);
                    break;
            }

            return emitter;

        }
        #endregion
    }
}
