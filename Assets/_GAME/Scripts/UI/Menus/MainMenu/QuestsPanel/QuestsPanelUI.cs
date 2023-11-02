using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace MemeFight.UI
{
    public class QuestsPanelUI : MonoBehaviour
    {
        [SerializeField] QuestDisplayUI[] _questDisplays;
        [SerializeField] QuestsProgressBarUI _progressBar;

        [SerializeField, ReadOnly] bool _isAnimating = false;

        public event UnityAction OnQuestsAnimationComplete;
        public event UnityAction OnBellAnimationComplete;

        void Awake()
        {
            foreach (QuestDisplayUI display in _questDisplays)
            {
                display.Clear();
            }

            Debug.Log("Quest displays cleared");
        }

        #region Display Management
        public void SetupQuestDisplay(int displayIndex, QuestSystem.QuestData quest, bool isComplete, bool blink = false)
        {
            if (Mathf.Max(displayIndex, 0) >= _questDisplays.Length)
            {
                Debug.LogError("Unable to display quest because {displayIndex} is greater than the amount of displays available!");
                return;
            }

            _questDisplays[displayIndex].Setup(quest.DescriptionString, quest.CurrentValue, quest.TotalValue, isComplete, blink);
        }

        public void UpdateProgressBarValue(float value, bool isImmediate)
        {
            _progressBar.SetBarValue(value, isImmediate);
        }

        public void StopBlinkingAnimationForAllDisplays()
        {
            for (int i = 0; i < _questDisplays.Length; i++)
            {
                _questDisplays[i].SetBlinkActive(false);
            }
        }
        #endregion

        #region Bell Animation
        public void TriggerBellAnimation()
        {
            if (!_isAnimating)
            {
                StartCoroutine(BellAnimationRoutine());
            }
            else
            {
                Debug.LogWarning("The Quests Panel is already animating!");
            }
        }

        IEnumerator BellAnimationRoutine()
        {
            _isAnimating = true;

            _progressBar.EnableBellRingingAnimation(true);
            yield return CoroutineUtils.GetWaitTime(_progressBar.BellAnimationDuration);
            _progressBar.EnableBellRingingAnimation(false);

            _isAnimating = false;
            OnBellAnimationComplete?.Invoke();
        }
        #endregion

        #region Panel Animation
        public void AnimateQuestCompletion(bool[] newCompletedQuestsChecker, float progressBarValue)
        {
            if (!_isAnimating)
            {
                Debug.Log("Animating quest completion...");
                StartCoroutine(TriggerQuestCompletionAnimations(newCompletedQuestsChecker, progressBarValue));
            }
            else
            {
                Debug.LogWarning("The Quests Panel is already animating!");
            }
        }

        IEnumerator TriggerQuestCompletionAnimations(bool[] newCompletedQuestsChecker, float finalProgressBarValue)
        {
            _isAnimating = true;

            for (int i = 0; i < newCompletedQuestsChecker.Length; i++)
            {
                if (i < _questDisplays.Length)
                {
                    if (newCompletedQuestsChecker[i] == true)
                    {
                        _questDisplays[i].AnimateMedal();
                        yield return CoroutineUtils.GetWaitTime(1.0f);
                    }
                    else continue;
                }
                else
                {
                    Debug.LogWarning("More completed quests requested animations than there are quest displays available!");
                    break;
                }
            }

            // Trigger the progress bar animation and wait for it to complete before proceeding
            _progressBar.SetBarValue(finalProgressBarValue, false);
            yield return CoroutineUtils.WaitOneFrame;
            yield return new WaitUntil(() => _progressBar.IsAnimating == false);

            _isAnimating = false;
            OnQuestsAnimationComplete?.Invoke();
        }
        #endregion
    }
}
