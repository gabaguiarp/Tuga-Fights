using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Audio;

namespace MemeFight.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundEmitter : MonoBehaviour, IPoolable<SoundEmitter>
    {
        [SerializeField] AudioSource _source;
        [Tooltip("Whether this sound emitter will automatically be returned to the pool when it finishes playing.")]
        [ReadOnly] public bool autoRelease = true;
        [SerializeField, ReadOnly] bool _isPlaying;

        PoolInstance<SoundEmitter> _pool;

        public event UnityAction OnSoundFinishedPlaying;

        public bool IsPlaying => _isPlaying;

        void Reset()
        {
            if (_source == null)
                _source = GetComponent<AudioSource>();
        }

        void Awake()
        {
            _source.playOnAwake = false;
        }

        /// <summary>
        /// Plays an audio cue though this sound emitter.
        /// </summary>
        /// <param name="audioCue">The sound to play.</param>
        /// <param name="outputGroup">The <see cref="AudioMixerGroup"/> to which the sound will be outputed. By default, all sounds are
        /// being sent to the Master. Leave this parameter <i>null</i> to keep the output used previously for this emitter.</param>
        /// <param name="ignorePause">Whether the sound to play should ignore the game's time scale, allowing it to keep playing even
        /// when the game is paused.</param>
        public void Play(AudioCueSO audioCue, AudioMixerGroup outputGroup = null, bool ignorePause = false)
        {
            if (_source.isPlaying)
                _source.Stop();

            if (outputGroup != null)
                _source.outputAudioMixerGroup = outputGroup;

            _source.clip = audioCue.Clip;
            _source.loop = audioCue.Loop;
            _source.volume = audioCue.Volume;
            _source.pitch = audioCue.Pitch;
            _source.ignoreListenerPause = ignorePause;

            _source.Play();
            _isPlaying = true;

            if (!_source.loop)
                StartCoroutine(TrackSourcePlaytime());
        }

        [ContextMenu("Stop")]
        public void Stop()
        {
            if (_isPlaying)
            {
                _source.Stop();
                StopCoroutine(TrackSourcePlaytime());
                HandleSoundFinishedPlaying();
            }
        }

        public void OnGetFromPool(PoolInstance<SoundEmitter> pool)
        {
            _pool = pool;
        }

        public void ReturnToPool() => _pool.Return(this);

        IEnumerator TrackSourcePlaytime()
        {
            yield return CoroutineUtils.GetWaitTime(_source.clip.length);
            HandleSoundFinishedPlaying();
        }

        void HandleSoundFinishedPlaying()
        {
            OnSoundFinishedPlaying?.Invoke();

            if (_pool && autoRelease)
                ReturnToPool();

            _isPlaying = false;
        }
    }
}
