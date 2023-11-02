using UnityEngine;

namespace MemeFight.Audio
{
    public class MatchMusicController : ManagedBehaviour
    {
        [SerializeField] AudioCueSO _matchIntroCue;
        [SerializeField] AudioCueSO _matchMusicLoop;

        AudioManager _audioManager;

        protected override void OnSceneReady()
        {
            _audioManager = AudioManager.Instance;
        }

        public void PlayMatchIntroCue()
        {
            _audioManager.PlayMusicTrack(_matchIntroCue);
        }

        public void PlayMatchMusic()
        {
            _audioManager.PlayMusicTrack(_matchMusicLoop);
        }

        public void StopMatchMusic()
        {
            _audioManager.StopMusicTrack();
        }
    }
}
