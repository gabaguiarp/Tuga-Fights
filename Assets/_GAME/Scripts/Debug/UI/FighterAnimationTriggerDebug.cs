using UnityEngine;
using UnityEngine.Events;

namespace MemeFight.DebugSystem
{
    using Animation;
    using UI;

    public class FighterAnimationTriggerDebug : MonoBehaviour
    {   
        [SerializeField] FighterAnimation _fighterAnimation;
        [SerializeField] AnimationStatesUI _animationStatesUI;

        void Awake()
        {
            _animationStatesUI.OnIdleButtonClicked += TriggerIdleAnimation;
            _animationStatesUI.OnWalkingButtonClicked += TriggerWalkingAnimation;
            _animationStatesUI.OnPunchButtonClicked += TriggerPuchAnimation;
            _animationStatesUI.OnKickButtonClicked += TriggerKickAnimation;
            _animationStatesUI.OnBlockButtonClicked += TriggerBlockAnimation;
            _animationStatesUI.OnDodgeButtonClicked += TriggerDodgeAnimation;
            _animationStatesUI.OnDamagedButtonClicked += TriggerDamagedAnimation;
            _animationStatesUI.OnKnockedOutButtonClicked += TriggerKnockedOutAnimation;
        }

        void TriggerIdleAnimation() => EnterAnimationState(() => _fighterAnimation.SetMovingState(false));

        void TriggerWalkingAnimation() => EnterAnimationState(() => _fighterAnimation.SetMovingState(true));

        void TriggerPuchAnimation() => EnterAnimationState(() => _fighterAnimation.TriggerAnimation(AnimationTriggers.Punch));

        void TriggerKickAnimation() => EnterAnimationState(() => _fighterAnimation.TriggerAnimation(AnimationTriggers.Kick));

        void TriggerBlockAnimation() => EnterAnimationState(() => _fighterAnimation.SetBlockingState(true));

        void TriggerDodgeAnimation() => EnterAnimationState(() => _fighterAnimation.SetDodgeState(true));

        void TriggerDamagedAnimation() => EnterAnimationState(() => _fighterAnimation.TriggerAnimation(AnimationTriggers.Damaged));

        void TriggerKnockedOutAnimation() => EnterAnimationState(() => _fighterAnimation.SetKnockedOutState(true));

        void EnterAnimationState(UnityAction stateCallback)
        {
            _fighterAnimation.SetMovingState(false);
            _fighterAnimation.SetDodgeState(false);
            _fighterAnimation.SetBlockingState(false);
            _fighterAnimation.SetKnockedOutState(false);

            stateCallback.Invoke();
        }
    }
}
