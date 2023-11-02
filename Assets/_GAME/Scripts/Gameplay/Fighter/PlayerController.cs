using UnityEngine;
using UnityEngine.Events;

namespace MemeFight.Characters
{
    using AI;
    using Animation;
    using DamageSystem;
    using DebugSystem;

    [RequireComponent(typeof(CharacterMovement))]
    public class PlayerController : MonoBehaviour
    {
        [System.Serializable]
        struct AttackSettings
        {
            public AttackSettings(float cooldownTime, bool startDepleted = false)
            {
                this.cooldownTime = cooldownTime;
                this.startDepleted = startDepleted;
            }

            public float cooldownTime;
            public bool startDepleted;
        }

        public enum ControlMode { Input, AI, NONE }

        [Header("Control")]
        public ControlMode controlMode;
        [SerializeField] GameplayInputEventChannelSO _inputChannel;

        [Header("Settings")]
        [SerializeField] float _moveSpeed = 4f;
        [SerializeField] float _stunDuration = 0.3f;
        [SerializeField] AttackSettings _punchSettings = new AttackSettings(0.4f, false);
        [SerializeField] AttackSettings _kickSettings = new AttackSettings(1.4f, true);
        //[SerializeField] float _punchCooldownTime = 0.6f;
        //[SerializeField] float _kickCooldownTime = 1.0f;

        [Header("Components")]
        [SerializeField] FighterAIModule _ai;
        [SerializeField] FighterAnimation _animation;
        [SerializeField] FighterStats _stats;
        [SerializeField] DamageController _damage;

        [Header("Info")]
        [SerializeField, ReadOnly] bool _isActive;
        /// <summary>The vector received from player input.</summary>
        [SerializeField, ReadOnly] Vector2 _moveInput;
        /// <summary>The movement vector resulting from move input multiplied by move speed.</summary>
        [SerializeField, ReadOnly] Vector2 _moveVector;
        [SerializeField, ReadOnly] Vector2 _externalMoveForce;
        [SerializeField, ReadOnly] Timer _punchTimer;
        [SerializeField, ReadOnly] Timer _kickTimer;
        [SerializeField, ReadOnly] FighterStateMachine _stateMachine = new FighterStateMachine();

        CharacterMovement _movement;

        // CONSTANTS
        public const float DodgeDuration = 0.8f;
        private const float KnockbackForceFactor = 0.7f;

        // EXPOSED PARAMETERS
        public bool IsActive => _isActive;
        public bool IsFacingRight => transform.localScale.x > 0;
        public bool IsBlocking => _stateMachine.CurrentState == FighterState.Blocking;
        public bool IsDodging => _stateMachine.CurrentState == FighterState.Dodging;
        public bool IsDefending => IsBlocking || IsDodging;
        public bool CanPunch => _punchTimer != null && _punchTimer.RemainingSeconds <= 0.0f;
        public bool CanKick => _kickTimer != null && _kickTimer.RemainingSeconds <= 0.0f;
        public float PunchCooldownTime => _punchSettings.cooldownTime;
        public float KickCooldownTime => _kickSettings.cooldownTime;

        // STATE MACHINE PAREMETERS
        public bool IsMoving => _moveVector.sqrMagnitude > 0.0f;
        public float StunDuration => _stunDuration;
        public Vector2 Back => -transform.right * transform.localScale.x;
        public FighterAnimation Animation => _animation;

        // EVENTS
        public event UnityAction<PlayerController> OnKnockedOut;
        public event UnityAction<float> OnPunchTimerUpdated;
        public event UnityAction<float> OnKickTimerUpdated;

        #region Core Loop
        void OnEnable()
        {
            //Activate();
        }

        void OnDisable()
        {
            Deactivate();
        }

        void Awake()
        {
            _movement = GetComponent<CharacterMovement>();
            _punchTimer = new Timer(_punchSettings.cooldownTime);
            _kickTimer = new Timer(_kickSettings.cooldownTime);

        }

        void Start()
        {
            _stateMachine.Init(this);

            _stats.OnDamaged += HandleDamage;
            _stats.OnDie += HandleDeath;
            _stats.OnBlockingBroken += HandleBlockingBroken;

            // Setup punch attack start value
            if (_punchSettings.startDepleted)
            {
                _punchTimer.Restart();
            }

            OnPunchTimerUpdated?.Invoke(_punchTimer.RemainingSeconds);

            // Setup kick attack start value
            if (_kickSettings.startDepleted)
            {
                _kickTimer.Restart();
            }

            OnKickTimerUpdated?.Invoke(_kickTimer.RemainingSeconds);
        }

        void Update()
        {
            if (_stateMachine.WasInit)
                _stateMachine.UpdateState();

            if (_isActive)
            {
                _stats.SetBlockingState(IsBlocking);
                HandleTimers();
            }
        }

        void HandleTimers()
        {
            if (_punchTimer.HasTimeLeft)
            {
                _punchTimer.Tick(Time.deltaTime);
                OnPunchTimerUpdated?.Invoke(_punchTimer.RemainingSeconds);
            }

            if (_kickTimer.HasTimeLeft)
            {
                _kickTimer.Tick(Time.deltaTime);
                OnKickTimerUpdated?.Invoke(_kickTimer.RemainingSeconds);
            }
        }

        void ResetTimers()
        {
            if (_punchSettings.startDepleted)
            {
                _punchTimer.Restart();
            }
            else
            {
                _punchTimer.End();
            }

            if (_kickSettings.startDepleted)
            {
                _kickTimer.Restart();
            }
            else
            {
                _kickTimer.End();
            }

            OnPunchTimerUpdated?.Invoke(_punchTimer.RemainingSeconds);
            OnKickTimerUpdated?.Invoke(_kickTimer.RemainingSeconds);
        }

        void FixedUpdate()
        {
            if (_movement.enabled)
            {
                if (_stateMachine.IsAllowedToMove)
                {
                    CalculateMovement();
                }
                else if (_externalMoveForce.sqrMagnitude > 0.0f)
                {
                    _moveVector = _externalMoveForce;
                }
                else if (_moveVector.sqrMagnitude > 0.0f)
                {
                    _moveVector = Vector2.zero;
                }

                ApplyMovement();
            }

            if (_animation != null)
                _animation.SetMovingState(IsMoving);
        }
        #endregion

        #region Movement
        void CalculateMovement()
        {
            _moveVector = _moveInput * _moveSpeed;
        }

        void ApplyMovement()
        {
            _movement.MoveRigidbody(_moveVector * Time.fixedDeltaTime);
        }

        void AbortMovement()
        {
            _moveVector = Vector2.zero;
            ApplyMovement();
        }

        public void SetExternalMoveForce(Vector3 force) => _externalMoveForce = force;

        /// <summary>Resets the exteral move force value to zero.</summary>
        public void RemoveExternalMoveForce() => _externalMoveForce = Vector3.zero;
        #endregion

        #region Actions
        void Punch()
        {
            if (!CanPunch)
                return;

            _animation.TriggerAnimation(AnimationTriggers.Punch);
            _stateMachine.SwitchState(FighterState.Attacking);
            _punchTimer.Restart();
        }

        void Kick()
        {
            if (!CanKick)
                return;

            _animation.TriggerAnimation(AnimationTriggers.Kick);
            _stateMachine.SwitchState(FighterState.Attacking);
            _kickTimer.Restart();
        }

        void Dodge()
        {
            _stateMachine.SwitchState(FighterState.Dodging);
        }

        void BlockStart()
        {
            _stateMachine.SwitchState(FighterState.Blocking);
        }

        void BlockCancel()
        {
            _stateMachine.SwitchToDefaultState();
        }
        #endregion

        #region Event Responders
        void HandleDamage(float amount)
        {
            if (_animation != null)
                _animation.TriggerAnimation(AnimationTriggers.Damaged);

            // Knockback
            float knockbackForce = KnockbackForceFactor * amount;
            _externalMoveForce = Back * knockbackForce;
            _stateMachine.SwitchState(FighterState.Stunned);

            if (controlMode == ControlMode.AI && _ai != null)
            {
                _ai.ProcessNotification(FighterAINotifications.GotStunned);
            }
        }

        void HandleDeath()
        {
            _stateMachine.SwitchState(FighterState.KnockedOut);
            _movement.SetEnabled(false);

            //_animation.SetKnockedOutState(true);
            OnKnockedOut?.Invoke(this);

            Deactivate();
        }

        void HandleBlockingBroken()
        {
            Logger.LogMessage(name + "'s blocking was broken!");
        }
        #endregion

        #region State Machine Callbacks
        public void HandleAttackEnded()
        {
            _damage.DeactivateAllDamagers();
        }

        public void HandleDodgeStarted()
        {
            _damage.SetDamageableRegionEnabled(DamageableRegion.UpperBody, false);
        }

        public void HandleDodgeEnded()
        {
            _damage.SetDamageableRegionEnabled(DamageableRegion.UpperBody, true);
        }
        #endregion

        #region External Callbacks
        public void SetVictorious()
        {
            // The fighter will enter the victorios state and trigger the matching animation.
            // It will then automatically exit the state when switching to the default one.
            Debug.LogFormat("Setting {0} victorious", gameObject.name);
            _stateMachine.SwitchState(FighterState.Victorious);
        }
        #endregion

        #region Control Callbacks
        public void Activate()
        {
            if (_isActive || controlMode == ControlMode.NONE)
                return;

            if (controlMode == ControlMode.Input)
            {
                EnableInput();
            }
            else if (controlMode == ControlMode.AI)
            {
                EnableAI();
            }

            _stats.enabled = true;
            _damage.DeactivateAllDamagers();
            _isActive = true;

            Debug.Log(name + " actiavted with control mode: " + controlMode);
        }

        public void Deactivate()
        {
            if (!_isActive)
                return;

            DisableInput();
            DisableAI();

            AbortMovement();

            _stats.enabled = false;
            _isActive = false;

            Debug.Log(name + " was deactivated");
        }

        public void Restore(Vector3? newPos = null)
        {
            Revive();
            ResetTimers();
            RemoveExternalMoveForce();

            if (newPos.HasValue)
            {
                TeleportTo(newPos.Value);
            }
            else
            {
                _movement.SetEnabled(true);
            }
        }

        public void TeleportTo(Vector3 destination)
        {
            _movement.SetEnabled(false);
            transform.position = destination;
            _movement.SetEnabled(true);
        }

        void Revive()
        {
            _stateMachine.SwitchToDefaultState();
            _animation.TriggerAnimation(AnimationTriggers.Idle);
        }
        #endregion

        #region Input Events
        void EnableInput()
        {
            if (_inputChannel != null)
            {
                _inputChannel.OnMove += HandleMoveInput;
                _inputChannel.OnPunch += Punch;
                _inputChannel.OnKick += Kick;
                _inputChannel.OnDodge += Dodge;
                _inputChannel.OnBlock += HandleBlockInput;
            }
        }

        void DisableInput()
        {
            if (_inputChannel != null)
            {
                _inputChannel.OnMove -= HandleMoveInput;
                _inputChannel.OnPunch -= Punch;
                _inputChannel.OnKick -= Kick;
                _inputChannel.OnDodge -= Dodge;
                _inputChannel.OnBlock -= HandleBlockInput;
            }

            _moveInput = Vector2.zero;
        }

        void HandleMoveInput(Vector2 moveInput) => _moveInput = moveInput;

        void HandleBlockInput(bool isBlocking)
        {
            if (isBlocking)
                BlockStart();
            else
                BlockCancel();
        }
        #endregion

        #region AI Callbacks
        void HandleAIDecision(FighterAIDecisions decision)
        {
            switch (decision)
            {
                case FighterAIDecisions.Stop:
                    HandleMoveInput(Vector2.zero);
                    break;

                case FighterAIDecisions.MoveLeft:
                    HandleMoveInput(Vector2.left);
                    break;

                case FighterAIDecisions.MoveRight:
                    HandleMoveInput(Vector2.right);
                    break;

                case FighterAIDecisions.AttackPunch:
                    Punch();
                    break;

                case FighterAIDecisions.AttackKick:
                    Kick();
                    break;

                case FighterAIDecisions.Dodge:
                    Dodge();
                    break;

                case FighterAIDecisions.BlockStart:
                    BlockStart();
                    break;

                case FighterAIDecisions.BlockCancel:
                    BlockCancel();
                    break;
            }
        }

        void EnableAI()
        {
            if (_ai == null)
            {
                Debug.LogWarning($"Cannot enable AI for {name}, because no AIModule component was found!");
                return;
            }

            _ai.OnDecisionEmmited += HandleAIDecision;
            _ai.Activate(_stateMachine);
        }

        void DisableAI()
        {
            if (_ai != null)
            {
                _ai.OnDecisionEmmited -= HandleAIDecision;
                _ai.Shutdown();
            }

            _moveInput = Vector2.zero;
            AbortMovement();
        }
        #endregion

        #region Editor
        [ContextMenu("Flip")]
        void Flip()
        {
            float xScale = transform.localScale.x * -1;
            transform.localScale = new Vector3(xScale, transform.localScale.y, transform.localScale.z);
        }
        #endregion
    }
}
