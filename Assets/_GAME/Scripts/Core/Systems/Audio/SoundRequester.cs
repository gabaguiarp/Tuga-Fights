using UnityEngine;

namespace MemeFight.Audio
{
    public class SoundRequester : MonoBehaviour
    {
        [SerializeField] AudioCueSO _soundToPlay;
        [SerializeField] AudioManager.SoundType _soundType;
        [SerializeField] bool _autoRelease = true;

        AudioManager _audioManager;

        AudioManager Manager
        {
            get
            {
                if (_audioManager == null)
                    _audioManager = AudioManager.Instance;

                return _audioManager;
            }
        }

        public void Play()
        {
            switch (_soundType)
            {
                case AudioManager.SoundType.SFX:
                    Manager.PlaySoundEffect(_soundToPlay, _autoRelease);
                    break;

                case AudioManager.SoundType.UI:
                    Manager.PlaySoundUI(_soundToPlay, _autoRelease);
                    break;
            }
        }
    }
}
