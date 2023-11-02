using UnityEngine;

namespace MemeFight.Audio
{
    [AddComponentMenu("Audio/Music Player")]
    public class MusicPlayer : ManagedBehaviour
    {
        [SerializeField] AudioCueSO _musicTrack;
        [Tooltip("Starts playing the music track as soon as the scene is done loading.")]
        [SerializeField] bool _playImmediately = true;

        AudioManager _audioManager;

        protected override void OnSceneReady()
        {
            _audioManager = AudioManager.Instance;

            if (_playImmediately)
                Play();
        }

        public void Play()
        {
            try
            {
                _audioManager.PlayMusicTrack(_musicTrack);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to play music track with exception: " + e);
            }
        }
    }
}
