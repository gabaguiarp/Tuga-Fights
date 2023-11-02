using System;
using UnityEngine;

namespace MemeFight.Characters
{
    using Animation;

    public enum FighterState
    {
        Idle,
        Moving,
        Attacking,
        Dodging,
        Blocking,
        Stunned,
        KnockedOut,
        Victorious
    }

    [Serializable]
    public class FighterStateMachine
    {
        [SerializeField, ReadOnly] FighterState _currentState;
        [SerializeField, ReadOnly] float _currentStateTime;
        [SerializeField, ReadOnly] bool _canMove;
        [SerializeField, ReadOnly] float _knockbackCountdown;

        PlayerController _controller;

        const float MinAttackStateDuration = 0.2f;
        const float KnockbackTime = 0.3f;

        public bool WasInit { get; private set; }
        public bool IsAllowedToMove => _canMove;
        public FighterState CurrentState => _currentState;

        public void Init(PlayerController controller, FighterState initialState = FighterState.Idle)
        {
            _controller = controller;

            if (_currentState != initialState)
            {
                SwitchState(initialState);
            }
            else
            {
                HandleCurrentStateEnter();
            }

            WasInit = true;
        }

        public void UpdateState()
        {
            _currentStateTime += Time.deltaTime;

            switch (_currentState)
            {
                case FighterState.Idle:
                    HandleIdleState();
                    _canMove = true;
                    break;

                case FighterState.Moving:
                    HandleMovingState();
                    _canMove = true;
                    break;

                case FighterState.Attacking:
                    HandleAttackingState();
                    _canMove = false;
                    break;

                case FighterState.Dodging:
                    HandleDodgeState();
                    _canMove = false;
                    break;

                case FighterState.Blocking:
                    HandleBlockingState();
                    _canMove = false;
                    break;

                case FighterState.Stunned:
                    HandleStunnedState();
                    _canMove = false;
                    break;

                case FighterState.KnockedOut:
                    HandleKnockedOutState();
                    _canMove = false;
                    break;

                case FighterState.Victorious:
                    HandleVictoriousState();
                    _canMove = false;
                    break;
            }
        }

        void HandleIdleState()
        {
            if (_controller.IsMoving)
                SwitchState(FighterState.Moving);
        }

        void HandleMovingState()
        {
            if (!_controller.IsMoving)
                SwitchState(FighterState.Idle);
        }

        void HandleAttackingState()
        {
            if (!HasBeenInCurrentStateForSeconds(MinAttackStateDuration)) return;

            if (!_controller.Animation.IsPlayingAttackAnimation())
                SwitchToDefaultState();
        }

        void HandleDodgeState()
        {
            if (HasBeenInCurrentStateForSeconds(PlayerController.DodgeDuration))
                SwitchToDefaultState();
        }

        void HandleBlockingState() { }

        void HandleStunnedState()
        {
            if (_knockbackCountdown > 0.0f)
            {
                _knockbackCountdown -= Time.deltaTime;
            }
            else
            {
                _controller.RemoveExternalMoveForce();
            }

            if (HasBeenInCurrentStateForSeconds(_controller.StunDuration))
                SwitchToDefaultState();
        }

        void HandleKnockedOutState() { }

        void HandleVictoriousState() { }

        bool HasBeenInCurrentStateForSeconds(float seconds) => _currentStateTime >= seconds;

        #region State Switching
        public void SwitchState(FighterState state)
        {
            Debug.Assert(_controller != null, "No PlayerController assigned in FighterStateMachine!");

            if (_currentState == state)
                return;

            HandleCurrentStateExit();

            _currentState = state;
            HandleCurrentStateEnter();
        }

        public void SwitchToDefaultState() => SwitchState(GetDefaultState());

        void HandleCurrentStateEnter()
        {
            _currentStateTime = 0.0f;

            switch (_currentState)
            {
                case FighterState.Dodging:
                    _controller.Animation.SetDodgeState(true);
                    _controller.HandleDodgeStarted();
                    break;

                case FighterState.Blocking:
                    _controller.Animation.SetBlockingState(true);
                    break;

                case FighterState.Stunned:
                    _knockbackCountdown = KnockbackTime;
                    break;

                case FighterState.KnockedOut:
                    _controller.Animation.SetKnockedOutState(true);
                    break;

                case FighterState.Victorious:
                    _controller.Animation.TriggerAnimation(AnimationTriggers.Victorious);
                    break;
            }
        }

        void HandleCurrentStateExit()
        {
            // This ensures that damagers are always disabled, especially if an attack is interrupted by a sudden state change
            _controller.HandleAttackEnded();

            switch (_currentState)
            {
                case FighterState.Dodging:
                    _controller.Animation.SetDodgeState(false);
                    _controller.HandleDodgeEnded();
                    break;

                case FighterState.Blocking:
                    _controller.Animation.SetBlockingState(false);
                    break;

                case FighterState.Stunned:
                    _knockbackCountdown = 0.0f;
                    _controller.RemoveExternalMoveForce();
                    break;

                case FighterState.KnockedOut:
                    _controller.Animation.SetKnockedOutState(false);
                    break;
            }
        }

        FighterState GetDefaultState() => !_controller.IsMoving ? FighterState.Idle : FighterState.Moving;
        #endregion
    }
}
