using UnityEngine;

namespace MemeFight.DebugSystem
{
    using Audio;

    public class AudioDebugger : MonoBehaviour
    {
        public AudioCueSO _musicToPlay;
        public AudioCueSO _soundToPlay;
        [Tooltip("Whether the Sound to Play emitter should be returned to the pool automatically upon finishing playing. If set to false, " +
            "you'll have to manually return it.")]
        public bool autoReleaseSound = true;

        SoundEmitter _sfxEmitter;

        void OnGUI()
        {
            if (GUILayout.Button("Play Music"))
            {
                AudioManager.Instance.PlayMusicTrack(_musicToPlay);
            }

            if (GUILayout.Button("Stop Music"))
            {
                AudioManager.Instance.StopMusicTrack();
            }

            if (GUILayout.Button("Play Sound"))
            {
                _sfxEmitter = AudioManager.Instance.PlaySoundEffect(_soundToPlay, autoReleaseSound);
            }

            if (GUILayout.Button("Stop Sound"))
            {
                if (_sfxEmitter != null)
                {
                    _sfxEmitter.Stop();
                    _sfxEmitter = null;
                }
            }
        }
    }
}
