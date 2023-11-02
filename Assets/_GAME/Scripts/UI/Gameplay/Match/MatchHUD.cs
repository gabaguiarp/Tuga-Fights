using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MemeFight.UI
{
    public enum MatchPromptMessage
    {
        RoundNumber,
        Fight,
        ResultKO,
        ResultPerfect,
        Winner
    }

    public class MatchHUD : MonoBehaviour
    {
        [SerializeField] FighterStatsUI[] _statsUI = new FighterStatsUI[2];

        [Space(10)]
        [SerializeField] RingBellUI _bell;
        [SerializeField] LocalizedTextUI _promptText;
        [SerializeField] MatchMessages _messageStrings;

        bool _isBellAnimationPlaying = false;
        Dictionary<MatchPromptMessage, LocalizedStringVariableSO> _promptMessages;

        readonly float BellRingAnimationDuration = 0.5f;

        void Awake()
        {
            _promptMessages = new Dictionary<MatchPromptMessage, LocalizedStringVariableSO>()
            {
                { MatchPromptMessage.RoundNumber, _messageStrings.roundTextMsg },
                { MatchPromptMessage.Fight, _messageStrings.fightMsg },
                { MatchPromptMessage.ResultKO, _messageStrings.resultKOMsg },
                { MatchPromptMessage.ResultPerfect, _messageStrings.resultPerfectMsg },
                { MatchPromptMessage.Winner, _messageStrings.winnerMsg }
            };
        }

        #region Match Details Display
        public void SetFighterName(int fighterIndex, string fighterName)
        {
            _statsUI[fighterIndex].SetFighterName(fighterName);
        }

        public void SetNumberOfRounds(int numOfRounds)
        {
            foreach (var stats in _statsUI)
            {
                stats.ConfigureScorerDisplay(numOfRounds);
            }
        }
        #endregion

        #region Text Prompts
        public void DisplayPrompt(MatchPromptMessage message)
        {
            _promptText.UpdateTextString(_promptMessages[message].Value);
            _promptText.gameObject.SetActive(true);
        }

        public void HidePrompt()
        {
            _promptText.gameObject.SetActive(false);
        }
        #endregion

        #region Fighter Stats Display
        public void SetFighterMaxHealth(int fighterIndex, float maxHealth)
        {
            _statsUI[fighterIndex].SetMaxHealth(maxHealth);
        }

        public void UpdateFighterHealth(int fighterIndex, float health)
        {
            _statsUI[fighterIndex].UpdateHealth(health);
        }

        public void SetFighterMaxBlockingStrength(int fighterIndex, float maxStrength)
        {
            _statsUI[fighterIndex].SetMaxBlockingStrength(maxStrength);
        }

        public void UpdateFighterBlockingStrenght(int fighterIndex, float strength)
        {
            _statsUI[fighterIndex].UpdateBlockingStrength(strength);
        }

        public void DisplayWinForFighter(int fighterIndex)
        {
            _statsUI[fighterIndex].DisplayWin();
        }
        #endregion

        #region Bell Animation
        public void TriggerBellAnimation()
        {
            if (!_isBellAnimationPlaying)
                StartCoroutine(BellRingAnimatonTracker());
        }

        IEnumerator BellRingAnimatonTracker()
        {
            _isBellAnimationPlaying = true;
            _bell.SetRingAnimationState(true);

            yield return CoroutineUtils.GetWaitTime(BellRingAnimationDuration);

            _bell.SetRingAnimationState(false);
            _isBellAnimationPlaying = false;
        }
        #endregion

        [Serializable]
        struct MatchMessages
        {
            public LocalizedStringVariableSO roundTextMsg;
            public LocalizedStringVariableSO fightMsg;
            public LocalizedStringVariableSO resultKOMsg;
            public LocalizedStringVariableSO resultPerfectMsg;
            public LocalizedStringVariableSO winnerMsg;
        }
    }
}
