using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace MemeFight.UI
{
    using Settings;

    public class MatchIntroUI : MonoBehaviour
    {
        [SerializeField] GameObject _introPanel;
        [SerializeField] FighterSelectionDisplayUI _fighterOneDisplay;
        [SerializeField] FighterSelectionDisplayUI _fighterTwoDisplay;
        [SerializeField] PlayerColorsSO _playerColors;

        [Header("Animation")]
        [SerializeField] Animator _animator;
        [SerializeField] string _matchIntroAnimName = "MatchIntro";

        public event UnityAction OnAnimationComplete;

        const float kPostAnimationDelay = 2.5f;

        void Awake()
        {
            // Configure displays
            //_fighterOneDisplay.SetFrameColor(_playerColors.GetColorForPlayer(Player.One));
            //_fighterTwoDisplay.SetFrameColor(_playerColors.GetColorForPlayer(Player.Two));
        }

        public void Populate(FighterProfileSO fighterOneProfile, FighterProfileSO fighterTwoProfile)
        {
            _fighterOneDisplay.UpdateDisplay(fighterOneProfile.Avatar, fighterOneProfile.Name);
            _fighterTwoDisplay.UpdateDisplay(fighterTwoProfile.Avatar, fighterTwoProfile.Name);
        }

        public void PlayAnimation()
        {
            _introPanel.SetActive(true);

            StartCoroutine(AnimationRoutine());

            IEnumerator AnimationRoutine()
            {
                _animator.Play(_matchIntroAnimName);

                yield return CoroutineUtils.WaitOneFrame;

                while (IsPlayingIntroAnimation())
                {
                    yield return null;
                }

                yield return CoroutineUtils.GetWaitTime(kPostAnimationDelay);

                OnAnimationComplete?.Invoke();

                HidePanel();
            }
        }

        bool IsPlayingIntroAnimation() => _animator.GetCurrentAnimatorStateInfo(0).IsName(_matchIntroAnimName);

        public void HidePanel()
        {
            _introPanel.SetActive(false);
        }
    }
}
