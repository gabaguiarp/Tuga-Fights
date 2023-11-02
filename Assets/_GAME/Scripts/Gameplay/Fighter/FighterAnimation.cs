using UnityEngine;

namespace MemeFight.Animation
{
    public enum AnimationTriggers
    {
        Punch,
        Kick,
        Damaged,
        Victorious,
        Idle
    }

    [RequireComponent(typeof(Animator))]
    public class FighterAnimation : MonoBehaviour
    {
        Animator _animator;

        readonly int IdleHash = Animator.StringToHash("Idle");
        readonly int MovingHash = Animator.StringToHash("IsMoving");
        readonly int BlockingHash = Animator.StringToHash("IsBlocking");
        readonly int DodgingHash = Animator.StringToHash("IsDodging");
        readonly int PunchHash = Animator.StringToHash("Punch");
        readonly int KickHash = Animator.StringToHash("Kick");
        readonly int DamagedHash = Animator.StringToHash("Damaged");
        readonly int KnockedOutHash = Animator.StringToHash("IsKnockedOut");
        readonly int VictoriousHash = Animator.StringToHash("Victorious");

        readonly string AttackAnimTag = "Attack";

        void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        void SetAnimatorTrigger(int triggerHash)
        {
            _animator.ResetTrigger(triggerHash);
            _animator.SetTrigger(triggerHash);
        }

        public void TriggerAnimation(AnimationTriggers animation)
        {
            switch (animation)
            {
                case AnimationTriggers.Punch:
                    SetAnimatorTrigger(PunchHash);
                    break;

                case AnimationTriggers.Kick:
                    SetAnimatorTrigger(KickHash);
                    break;

                case AnimationTriggers.Damaged:
                    SetAnimatorTrigger(DamagedHash);
                    break;

                case AnimationTriggers.Victorious:
                    SetAnimatorTrigger(VictoriousHash);
                    break;

                case AnimationTriggers.Idle:
                    SetAnimatorTrigger(IdleHash);
                    break;
            }
        }

        public void SetMovingState(bool isMoving) => _animator.SetBool(MovingHash, isMoving);
        public void SetDodgeState(bool active) => _animator.SetBool(DodgingHash, active);
        public void SetBlockingState(bool active) => _animator.SetBool(BlockingHash, active);
        public void SetKnockedOutState(bool isKnockedOut) => _animator.SetBool(KnockedOutHash, isKnockedOut);

        public bool IsPlayingAttackAnimation()
        {
            return _animator.IsInTransition(0) || _animator.GetCurrentAnimatorStateInfo(0).IsTag(AttackAnimTag);
        }
    }
}
