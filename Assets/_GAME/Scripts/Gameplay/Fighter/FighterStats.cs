using UnityEngine;
using UnityEngine.Events;

namespace MemeFight.Characters
{
    public class FighterStats : MonoBehaviour
    {
        [SerializeField] int _maxHealth = 100;
        [SerializeField] int _maxBlockingStrength = 5;

        [Header("Info")]
        [SerializeField, ReadOnly] bool _isBlocking;
        [SerializeField, ReadOnly] float _healthPoints;
        [SerializeField, ReadOnly] float _blockingStrength;
        /// <summary>How many consecutive attacks successfully damaged the opponent.</summary>
        [SerializeField, ReadOnly] int _attackComboCount;
        /// <summary>How many consecutive attacks were suffered.</summary>
        [SerializeField, ReadOnly] int _incomingAttacksChain;

        [Space(10)]
        [SerializeField, ReadOnly] Timer _comboCooldownTimer;
        [SerializeField, ReadOnly] Timer _incomingAttacksCooldownTimer;
        /// <summary>
        /// Controls how many seconds left before recovering one blocking strength point. The fighter must not suffer any attacks in
        /// the meantime, otherwise the timer will be reset.
        /// </summary>
        [SerializeField, ReadOnly] Timer _blockingRecoveryTimer;

        bool _wasPreviouslyBlocking;

        const float AttackComboTimeout = 1.2f;
        const float IncomingAttackChainTimeout = 0.8f;
        const float BlockingRecoveryTime = 3.0f;
        const int CombosRequiredToGainBlockingStrength = 3;

        public float CurrentHealth => _healthPoints;
        public float MaxHealth => _maxHealth;
        public float BlockingStrength => _blockingStrength;
        public float MaxBlockingStrength => _maxBlockingStrength;
        public bool HasSufferedDamage => CurrentHealth != MaxHealth;
        public int ConsecutiveAttacksSuffered => _incomingAttacksChain;

        public event UnityAction<float> OnHealthReset;
        public event UnityAction<float> OnHealthUpdated;
        public event UnityAction<float> OnDamaged;
        public event UnityAction OnDie;

        public event UnityAction<float> OnBlockingStrengthReset;
        public event UnityAction<float> OnBlockingStrengthUpdated;
        public event UnityAction OnBlockingBroken;

        #region Management
        void Awake()
        {
            _comboCooldownTimer = new Timer(AttackComboTimeout);
            _comboCooldownTimer.OnTimerEnd += ResetAttacksCombo;
            _incomingAttacksCooldownTimer = new Timer(IncomingAttackChainTimeout);
            _incomingAttacksCooldownTimer.OnTimerEnd += ResetIncomingAttacksChain;
            _blockingRecoveryTimer = new Timer(BlockingRecoveryTime);
            _blockingRecoveryTimer.OnTimerEnd += RegainBlockingStrength;

            ResetAllStats();
        }

        public void ResetAllStats()
        {
            ResetHealth();
            ResetBlockingStrength();
            ResetAttacksCombo();
            ResetIncomingAttacksChain();
        }
        #endregion

        #region Timers
        void Update()
        {
            HandleComboTimer();
            HandleIncomingAttacksTimer();
            HandleBlockingRecoveryTimer();
        }

        void HandleComboTimer()
        {
            if (_comboCooldownTimer == null) return;

            if (_comboCooldownTimer.HasTimeLeft && _attackComboCount > 0)
            {
                _comboCooldownTimer.Tick(Time.deltaTime);
            }
        }

        void HandleIncomingAttacksTimer()
        {
            if (_incomingAttacksCooldownTimer == null) return;

            if (_incomingAttacksCooldownTimer.HasTimeLeft && _incomingAttacksChain > 0)
            {
                _incomingAttacksCooldownTimer.Tick(Time.deltaTime);
            }
        }

        void HandleBlockingRecoveryTimer()
        {
            if (_blockingRecoveryTimer == null || _isBlocking) return;

            if (_blockingRecoveryTimer.HasTimeLeft && _blockingStrength < _maxBlockingStrength)
            {
                _blockingRecoveryTimer.Tick(Time.deltaTime);
            }
        }
        #endregion

        #region Damage
        void RegisterIncomingAttack()
        {
            _incomingAttacksChain++;
            _incomingAttacksCooldownTimer.Restart();
            _blockingRecoveryTimer.Restart();
        }

        void ResetIncomingAttacksChain() => _incomingAttacksChain = 0;

        void UpdateHealth(float amount)
        {
            _healthPoints = Mathf.Clamp(_healthPoints + amount, 0, _maxHealth);
            OnHealthUpdated?.Invoke(_healthPoints);
        }

        void Die()
        {
            OnDie?.Invoke();
        }

        public void TakeDamage(float damageAmount)
        {
            // When the fighter takes damage the current attack combo is broken
            ResetAttacksCombo();

            UpdateHealth(-damageAmount);

            if (_healthPoints > 0)
            {
                RegisterIncomingAttack();
                OnDamaged?.Invoke(damageAmount);
            }
            else
            {
                Die();
            }
        }

        public void ResetHealth()
        {
            _healthPoints = _maxHealth;
            OnHealthReset?.Invoke(_maxHealth);
            OnHealthUpdated?.Invoke(_healthPoints);
        }
        #endregion

        #region Blocking
        void BreakBlocking()
        {
            OnBlockingBroken?.Invoke();
        }

        void RegainBlockingStrength()
        {
            if (_blockingStrength >= _maxBlockingStrength)
                return;
            
            UpdateBlockingStrength(1);
            _blockingRecoveryTimer.Restart();
        }

        /// <summary>
        /// Updates the value of the blocking strength by <paramref name="amount"/>. If the value gets to 0, it means blocking got
        /// broken and the fighter will take some damage, defined by <paramref name="damageAmount"/>.
        /// </summary>
        /// <param name="amount">The amount to add or take from the blocking strength.</param>
        /// <param name="damageAmount">The amount of damage to take if the blocking strength gets to 0 in this call.</param>
        /// <returns>Whether blocking was broken and the fighter damaged as a result.</returns>
        public bool UpdateBlockingStrength(float amount, float damageAmount = 0)
        {
            _blockingStrength = Mathf.Clamp(_blockingStrength + amount, 0, _maxBlockingStrength);
            OnBlockingStrengthUpdated?.Invoke(_blockingStrength);

            if (_blockingStrength <= 0)
            {
                BreakBlocking();
                TakeDamage(damageAmount);
                return true;
            }

            return false;
        }

        public void ResetBlockingStrength()
        {
            _blockingStrength = _maxBlockingStrength;
            OnBlockingStrengthReset?.Invoke(_maxBlockingStrength);
            OnBlockingStrengthUpdated?.Invoke(_blockingStrength);
        }

        public void SetBlockingState(bool state)
        {
            _wasPreviouslyBlocking = _isBlocking;
            _isBlocking = state;

            if (!_isBlocking && _wasPreviouslyBlocking)
                _blockingRecoveryTimer.Restart();
        }
        #endregion

        #region Combos
        void ResetAttacksCombo() => _attackComboCount = 0;

        void CheckCombos()
        {
            // When the modulo operation of the current attack combo count returns 0, we increase the blocking strength.
            // This means that the fighter will have to chain the same number of consecutive attacks indicated by the
            // constant value, in order to gain blocking strength.
            if (_attackComboCount % CombosRequiredToGainBlockingStrength == 0)
                UpdateBlockingStrength(1);
        }

        public void RegisterSuccessfulAttack()
        {
            _attackComboCount++;
            _comboCooldownTimer.Restart();

            CheckCombos();
        }
        #endregion
    }
}
