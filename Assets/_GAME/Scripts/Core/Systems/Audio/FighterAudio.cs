using MemeFight.Characters;
using MemeFight.DamageSystem;
using UnityEngine;

namespace MemeFight.Audio
{
    public class FighterAudio : ManagedBehaviour
    {
        public enum SoundEffect
        {
            Punch,
            Kick,
            GroundHit,
            Step,
            Block,
            Dodge
        }

        [SerializeField] AudioCueBankSO _punchCueBank;
        [SerializeField] AudioCueBankSO _kickCueBank;
        [SerializeField] AudioCueSO _groundHitCue;
        [SerializeField] AudioCueBankSO _stepCueBank;
        [SerializeField] AudioCueSO _blockCue;
        [SerializeField] AudioCueSO _dodgeCue;

        AudioManager _audioManager;

        protected override void Awake()
        {
            base.Awake();
            if (TryGetComponent(out DamageController damageController))
            {
                damageController.OnUpperAttackHit += HandleUpperAttackHit;
                damageController.OnLowerAttackHit += HandleLowerAttackHit;
                damageController.OnAttackBlocked += HandleAttackBlocked;
            }
        }

        protected override void OnSceneReady()
        {
            _audioManager = AudioManager.Instance;
        }

        #region Event Responders
        void HandleUpperAttackHit() => PlaySound(SoundEffect.Punch);
        void HandleLowerAttackHit() => PlaySound(SoundEffect.Kick);
        void HandleAttackBlocked() => PlaySound(SoundEffect.Block);

        // Called via UnityEvents from the FighterAnimationEvents script in the FighterModel
        public void HandleStep() => PlaySound(SoundEffect.Step);
        public void HandleGroundHit() => PlaySound(SoundEffect.GroundHit);
        public void HandleDodge() => PlaySound(SoundEffect.Dodge);
        #endregion

        public void PlaySound(SoundEffect sound)
        {
            switch (sound)
            {
                case SoundEffect.Punch:
                    _audioManager.PlaySoundEffect(_punchCueBank.GetRandom());
                    break;

                case SoundEffect.Kick:
                    _audioManager.PlaySoundEffect(_kickCueBank.GetRandom());
                    break;

                case SoundEffect.GroundHit:
                    _audioManager.PlaySoundEffect(_groundHitCue);
                    break;

                case SoundEffect.Step:
                    _audioManager.PlaySoundEffect(_stepCueBank.GetRandom());
                    break;

                case SoundEffect.Block:
                    _audioManager.PlaySoundEffect(_blockCue);
                    break;

                case SoundEffect.Dodge:
                    _audioManager.PlaySoundEffect(_dodgeCue);
                    break;
            }
        }
    }
}
