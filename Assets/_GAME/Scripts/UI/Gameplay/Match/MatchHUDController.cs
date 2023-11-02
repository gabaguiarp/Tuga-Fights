using System.Collections;
using UnityEngine;

namespace MemeFight.UI
{
    using Characters;

    public class MatchHUDController : ManagedBehaviour
    {
        [SerializeField] MatchHUD _HUD;
        [SerializeField] AudioCueSO _promptMessageAudioRegular;
        [SerializeField] AudioCueSO _promptMessageAudioStrong;

        const float DefaultPromptMessageDuration = 2f;

        AudioManager _audioManager;

        protected override void Awake()
        {
            base.Awake();
            _HUD.HidePrompt();
        }

        protected override void OnSceneReady()
        {
            _audioManager = AudioManager.Instance;
        }

        public void ConfigureForMatch(int matchRounds)
        {
            _HUD.SetNumberOfRounds(matchRounds);
        }

        public void OnFighterConfigured(Fighter fighter, int fighterIndex)
        {
            fighter.Stats.OnHealthReset += (maxHealth) => HandleFighterHealthReset(fighterIndex, maxHealth);
            fighter.Stats.OnHealthUpdated += (currentHealth) => HandleFighterHealthUpdated(fighterIndex, currentHealth);
            fighter.Stats.OnBlockingStrengthReset += (maxStrength) => HandleFighterBlockingStrengthReset(fighterIndex, maxStrength);
            fighter.Stats.OnBlockingStrengthUpdated += (currentStrength) => HandleFighterBlockingUpdated(fighterIndex, currentStrength);

            // Since we are only subscribing to the stats events after they have been initialized, we need to update the HUD accordingly when the fighter
            // is configured, in order to display the correct stats information
            HandleFighterHealthReset(fighterIndex, fighter.Stats.MaxHealth);
            HandleFighterHealthUpdated(fighterIndex, fighter.Stats.CurrentHealth);
            HandleFighterBlockingStrengthReset(fighterIndex, fighter.Stats.MaxBlockingStrength);
            HandleFighterBlockingUpdated(fighterIndex, fighter.Stats.BlockingStrength);

            // Configure the HUD
            _HUD.SetFighterName(fighterIndex, fighter.Profile.Name);
        }

        public void RegisterWinForFighter(int fighterIndex)
        {
            _HUD.DisplayWinForFighter(fighterIndex);
        }

        public void TriggerBellAnimation() => _HUD.TriggerBellAnimation();

        #region Prompt Messages
        public void DisplayPromptMessage(MatchPromptMessage message, bool autoHide = false, float duration = 0, bool playSound = true)
        {
            StopCoroutine(HidePromptMessageDelayed(0));

            _HUD.DisplayPrompt(message);

            if (playSound)
            {
                bool isMessageCritical = message.Equals(MatchPromptMessage.Fight) || message.Equals(MatchPromptMessage.Winner);
                AudioCueSO audioCue = isMessageCritical ? _promptMessageAudioStrong : _promptMessageAudioRegular;
                _audioManager.PlaySoundUI(audioCue);
            }

            if (autoHide)
            {
                if (duration <= 0)
                    duration = DefaultPromptMessageDuration;

                StartCoroutine(HidePromptMessageDelayed(duration));
            }
        }

        public void HidePromptMessage()
        {
            StopCoroutine(HidePromptMessageDelayed(0));
            _HUD.HidePrompt();
        }

        IEnumerator HidePromptMessageDelayed(float delay)
        {
            yield return CoroutineUtils.GetWaitTime(delay);
            HidePromptMessage();
        }
        #endregion

        #region Event Responders
        void HandleFighterHealthReset(int fighterIndex, float maxHealth)
        {
            _HUD.SetFighterMaxHealth(fighterIndex, maxHealth);
        }

        void HandleFighterHealthUpdated(int fighterIndex, float currentHealth)
        {
            _HUD.UpdateFighterHealth(fighterIndex, currentHealth);
        }

        void HandleFighterBlockingStrengthReset(int fighterIndex, float maxStrength)
        {
            _HUD.SetFighterMaxBlockingStrength(fighterIndex, maxStrength);
        }

        void HandleFighterBlockingUpdated(int fighterIndex, float blockingStrength)
        {
            _HUD.UpdateFighterBlockingStrenght(fighterIndex, blockingStrength);
        }
        #endregion
    }
}
