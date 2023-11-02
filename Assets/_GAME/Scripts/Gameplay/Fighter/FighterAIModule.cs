using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MemeFight.Characters.AI
{
    using DebugSystem;

    [RequireComponent(typeof(Fighter))]
    public class FighterAIModule : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] FighterStats _stats;

        [Header("Observers")]
        [SerializeField] LayerMask _hitDetectionLayers = Physics.AllLayers;
        [SerializeField] Transform _backObserverOrigin = default;
        [SerializeField] bool _displayObserverGizmos = true;
        [SerializeField] Color _observerGizmosColor = Color.yellow;

        [Header("Debug")]
        [SerializeField] bool _canAttack = true;
        [SerializeField] bool _canDefend = true;

        [Header("Info")]
        [SerializeField, ReadOnly] bool _wasInit;
        [SerializeField, ReadOnly] string _currentStateName;
        [SerializeField, ReadOnly] bool _isIdle;
        [SerializeField, ReadOnly] float _idleTime;
        [SerializeField, ReadOnly] FighterAIDecisions _latestDecision = FighterAIDecisions.Stop;
        [SerializeField, ReadOnly] string _latestCollision;

        AttackChain _attackChain = new AttackChain();
        ContactFilter2D _contactFilter = new ContactFilter2D();
        RaycastHit2D[] _raycastResults = new RaycastHit2D[1];
        IEnumerator _runningState;

        Fighter _fighter;
        FighterStateMachine _stateMachine;

        const float ObserverRaycastLength = 1f;
        const float AttackRange = 3.0f; // collider X size + 1.0f
        const float CloseRange = 4.0f;  // collider X size + 2.0f
        const float MaxStepBackDistance = 2.5f;
        const int CriticalHealthThreshold = 20;

        FighterAIConfigurationSO Configuration => _fighter.Profile.AIConfig;
        static FighterAIDecisions UpperAttackDecision => FighterAIDecisions.AttackPunch;
        static FighterAIDecisions LowerAttackDecision => FighterAIDecisions.AttackKick;

        public bool IsActive { get; private set; }

        public event UnityAction<FighterAIDecisions> OnDecisionEmmited;

        void Awake()
        {
            Init();
        }

        void Init()
        {
            if (_wasInit) return;

            _fighter = GetComponent<Fighter>();
            InitializeRaycastParameters();

            _wasInit = true;
        }

        #region Management Methods
        public void Activate(FighterStateMachine stateMachine, bool skipPrewarmState = false)
        {
            if (IsActive)
                return;

            if (!_wasInit)
            {
                Debug.LogWarning($"The AI module in '{name}' cannot be actiavted because it hasn't been initialized yet!");
                return;
            }

            _stateMachine = stateMachine;

            if (skipPrewarmState)
            {
                SwitchToDefaultAIState();
            }
            else
            {
                SwitchAIState(PrewarmState());
            }

            IsActive = true;
        }

        public void Shutdown()
        {
            if (!IsActive) return;

            AbortCurrentState();
            IsActive = false;
        }

        void InitializeRaycastParameters()
        {
            _contactFilter.SetLayerMask(_hitDetectionLayers);
        }

        void SwitchAIState(IEnumerator state)
        {
            if (_runningState != null)
            {
                AbortCurrentState();
            }
            else
            {
                // We make sure to stop the controller every time we switch states
                EmmitDecision(FighterAIDecisions.Stop);
            }

            _runningState = state;

            if (_runningState == null)
                throw new Exception("Failed to switch AI state! No running state was cached...");

            StartCoroutine(_runningState);
            _currentStateName = _runningState.ToString().TrimStart("MemeFight.Characters.AI.FighterAIModule+".ToCharArray());
        }

        void SwitchToDefaultAIState() => SwitchAIState(AlertState());

        void AbortCurrentState()
        {
            EmmitDecision(FighterAIDecisions.Stop);

            if (_runningState != null)
            {
                StopCoroutine(_runningState);
                _runningState = null;
            }

            _currentStateName = string.Empty;
        }

        void EmmitDecision(FighterAIDecisions decision)
        {
            _latestDecision = decision;
            OnDecisionEmmited?.Invoke(decision);
        }
        #endregion

        #region Calculation Methods
        float DistanceFromOponent() => Vector3.Distance(transform.position.Flat(), _fighter.Opponent.transform.position.Flat());
        Vector3 DirectionToOpponent() => (_fighter.Opponent.transform.position.Flat() - transform.position.Flat()).normalized;
        Vector3 DirectionAwayFromOpponent() => DirectionToOpponent().Flipped();
        int GetRandomAttackChainCapacity() => Randomizer.GetRandom(Configuration.MinConsecutiveAttacks, Configuration.MaxConsecutiveAttacks + 1);
        float GetRandomAttackInterval() => Randomizer.GetRandom(Configuration.MinAttackInterval, Configuration.MaxAttackInterval);
        FighterAIDecisions GetMoveDecisionFromDir(Vector2 dir) => dir.x > 0 ? FighterAIDecisions.MoveRight : FighterAIDecisions.MoveLeft;
        #endregion

        #region Condition Methods
        bool IsOpponentWithinAttackRange() => DistanceFromOponent() <= AttackRange;
        bool IsOpponentCloseToMe() => DistanceFromOponent() <= CloseRange;
        bool IsOpponentDefending() => _fighter.Opponent.Controller.IsDefending;
        bool ShouldMoveTowardsOpponent() => _idleTime >= Configuration.MaxIdleTime || Randomizer.GetProbability(Configuration.MoveProbability);
        bool ShouldStepBack() => !HasCollisionBehind() && Randomizer.GetProbability(Configuration.StepBackProbability);
        bool ShouldDefend() => _canDefend && !IsOpponentDefending() && Randomizer.GetProbability(Configuration.DefenseProbability);
        bool CanEffectivelyBlock() => _stats.BlockingStrength > 0;
        bool CanUseLowerAttack() => _fighter.Controller.CanKick;
        bool IsBlocking() => _stateMachine.CurrentState.Equals(FighterState.Blocking);
        bool IsStunned() => _stateMachine.CurrentState.Equals(FighterState.Stunned);
        bool IsHealthCritical() => _stats.CurrentHealth <= CriticalHealthThreshold;
        bool IsHostile() => _stats.ConsecutiveAttacksSuffered > Configuration.HostileThreshold || IsHealthCritical();
        #endregion

        #region Observer Conditions
        bool HasCollisionBehind() => HasHitAnything(_backObserverOrigin.position, _backObserverOrigin.up);
        bool HasHitAnything(Vector3 origin, Vector3 direction)
        {
            if (Physics2D.Raycast(origin, direction, _contactFilter, _raycastResults, ObserverRaycastLength) > 0)
            {
                _latestCollision = _raycastResults[0].collider.name;
                return true;
            }

            _latestCollision = string.Empty;
            return false;
        }
        #endregion

        #region Timers
        void Update()
        {
            if (_isIdle)
                _idleTime += Time.deltaTime;
        }
        #endregion

        #region AI States
        IEnumerator PrewarmState()
        {
            OnStateEnter(true);

            yield return CoroutineUtils.GetWaitTime(Configuration.PrewarmTime);
            SwitchToDefaultAIState();
        }

        IEnumerator AlertState()
        {
            OnStateEnter(true);

            float decisionFrequency;

            while (!IsOpponentWithinAttackRange())
            {
                decisionFrequency = IsHostile() ? 0.1f : Configuration.AlertStateDecisionFrequency;

                yield return CoroutineUtils.GetWaitTime(decisionFrequency);

                if (ShouldMoveTowardsOpponent())
                {
                    SwitchAIState(MoveTowardsOpponentState());
                    yield break;
                }
                else
                {
                    yield return null;
                }
            }

            var nextState = ShouldDefend() ? DefensiveState() : AttackState();
            SwitchAIState(nextState);
        }

        IEnumerator MoveTowardsOpponentState()
        {
            OnStateEnter(false);

            while (!IsOpponentWithinAttackRange())
            {
                MoveTowardsOpponent();
                yield return null;
            }

            if (_attackChain.Length > 0 || IsHostile() && Randomizer.GetProbability(1))
            {
                SwitchAIState(AttackState());
            }
            else
            {
                SwitchToDefaultAIState();
            }
        }

        IEnumerator AttackState()
        {
            OnStateEnter(false);

            while (_canAttack && IsOpponentCloseToMe())
            {
                if (_attackChain.IsEmpty)
                {
                    BuildAttackChain();
                }

                yield return CoroutineUtils.GetWaitTime(Randomizer.GetRandom(Configuration.MinChainAttackDelay,
                                                                             Configuration.MaxChainAttackDelay));

                int chainLength = _attackChain.Length;
                Logger.LogAIMessage($"Starting chain of {chainLength} attacks...");

                for (int i = 0; i < chainLength; i++)
                {
                    EmmitDecision(_attackChain.Chain.Dequeue());
                    yield return CoroutineUtils.GetWaitTime(_attackChain.AttackInterval);

                    while (!IsOpponentWithinAttackRange())
                    {
                        MoveTowardsOpponent();
                        yield return null;
                    }

                    EmmitDecision(FighterAIDecisions.Stop);

                    //if (!IsOpponentWithinAttackRange())
                    //{
                    //    SwitchAIState(MoveTowardsOpponentState());
                    //    //Logger.LogAIMessage("Chain attack broken because opponent stepped away!");
                    //    break;
                    //}
                }

                yield return CoroutineUtils.GetWaitTime(0.05f);

                // After finishing an attack chain, the AI will base its next decision on probability...
                // That decision can either be to step back, or to perform a new attack chain.
                if (ShouldStepBack())
                {
                    SwitchAIState(StepBackState());
                    yield break;
                }
                else if (ShouldDefend())
                {
                    SwitchAIState(DefensiveState());
                    yield break;
                }
                else
                {
                    yield return CoroutineUtils.GetWaitTime(Mathf.Max(0, Configuration.AttackChainCooldownTime - 0.15f));
                }
            }

            yield return CoroutineUtils.WaitOneFrame;

            SwitchToDefaultAIState();
        }

        IEnumerator DefensiveState()
        {
            OnStateEnter(true);

            Logger.LogAIMessage("Entered defensive state");

            CancelAttacks();
            float defenseDuration;

            while (IsOpponentCloseToMe())
            {
                if (CanEffectivelyBlock() && Randomizer.GetProbability(Configuration.BlockAttackProbability))
                {
                    if (!IsBlocking())
                        EmmitDecision(FighterAIDecisions.BlockStart);

                    defenseDuration = Randomizer.GetRandom(0.4f, 1.0f);
                }
                else
                {
                    EmmitDecision(FighterAIDecisions.Dodge);
                    defenseDuration = PlayerController.DodgeDuration;
                }

                yield return CoroutineUtils.GetWaitTime(defenseDuration);

                // Check if the AI should keep defending, or start attacking
                if (!Randomizer.GetProbability(Configuration.DefenseProbability))
                {
                    Logger.LogAIMessage("Switching from defensive to attack state");
                    CancelBlocking();
                    SwitchAIState(AttackState());
                    yield break;
                }

                Logger.LogAIMessage("Will continue defending...");
            }

            yield return CoroutineUtils.WaitOneFrame;

            CancelBlocking();
            SwitchToDefaultAIState();

            // ------ INTERNAL METHOD ------
            void CancelBlocking()
            {
                if (IsBlocking())
                {
                    EmmitDecision(FighterAIDecisions.BlockCancel);
                }
            }
        }

        IEnumerator StepBackState()
        {
            OnStateEnter(false);

            Logger.LogAIMessage("Stepping back...");
            CancelAttacks();

            Vector3 startPos = transform.position.Flat();
            Vector3 endPos = transform.position.Flat() + (DirectionAwayFromOpponent() * MaxStepBackDistance);
            float distanceTravelled = 0.0f;
            var moveDecision = GetMoveDecisionFromDir(DirectionAwayFromOpponent());

            while (distanceTravelled < Vector3.Distance(startPos, endPos))
            {
                if (HasCollisionBehind())
                {
                    Logger.LogAIMessage("Detected collision behind! Breaking out of step back loop");
                    break;
                }

                EmmitDecision(moveDecision);
                distanceTravelled = Vector3.Distance(startPos, transform.position.Flat());
                
                yield return new WaitForFixedUpdate();
            }

            SwitchToDefaultAIState();
        }

        IEnumerator StunnedState()
        {
            OnStateEnter(true);

            Logger.LogAIMessage(name + " entered stunned state");
            CancelAttacks();

            while (IsStunned())
            {
                yield return null;
            }

            Logger.LogAIMessage(name + " is not stunned anymore");

            if (IsHostile())
            {
                SwitchAIState(AttackState());
                yield break;
            }
            else if (IsHealthCritical() & ShouldStepBack())
            {
                SwitchAIState(StepBackState());
                yield break;
            }
            else if (ShouldDefend())
            {
                SwitchAIState(DefensiveState());
                yield break;
            }
            else
            {
                SwitchToDefaultAIState();
            }
        }
        #endregion

        #region State Callbacks
        void OnStateEnter(bool isStateIdle)
        {
            _isIdle = isStateIdle;

            if (!isStateIdle && _idleTime > 0.0f)
            {
                _idleTime = 0.0f;
            }
        }

        void MoveTowardsOpponent()
        {
            EmmitDecision(GetMoveDecisionFromDir(DirectionToOpponent()));
        }

        void BuildAttackChain()
        {
            int numOfAttacks;
            int lowerAttackProbability = -1;
            float attackInterval;

            // When the AI is hostile it is more likely to perform heavier and faster attacks
            numOfAttacks = IsHostile() ? Configuration.MaxConsecutiveAttacks : GetRandomAttackChainCapacity();
            attackInterval = IsHostile() ? Configuration.MinAttackInterval : GetRandomAttackInterval();

            if (CanUseLowerAttack())
            {
                lowerAttackProbability = IsHostile() ? 1 : Configuration.LowerAttackProbability;
            }

            _attackChain.Build(numOfAttacks, lowerAttackProbability, attackInterval);
        }

        void CancelAttacks()
        {
            if (!_attackChain.IsEmpty)
                _attackChain.Clear();
        }
        #endregion

        #region External Callbacks
        public void ProcessNotification(FighterAINotifications notification)
        {
            if (!IsActive)
                return;

            switch (notification)
            {
                case FighterAINotifications.GotStunned:
                    SwitchAIState(StunnedState());
                    break;
            }
        }
        #endregion

        #region Gizmos
        void OnDrawGizmos()
        {
            if (!_displayObserverGizmos) return;

            if (_backObserverOrigin != null)
            {
                Gizmos.color = _observerGizmosColor;
                Gizmos.DrawRay(_backObserverOrigin.position, _backObserverOrigin.up);
            }
        }
        #endregion

        class AttackChain
        {
            public AttackChain()
            {
                Chain = new Queue<FighterAIDecisions>();
                AttackInterval = 0.0f;
            }

            public Queue<FighterAIDecisions> Chain { get; private set; }
            public float AttackInterval { get; private set; }

            public bool IsEmpty => Chain == null || Chain.Count <= 0;
            public int Length => Chain.Count;

            public void Build(int capacity, int lowerAttackProbability, float attackInterval)
            {
                if (Chain.Count > 0)
                    Chain.Clear();

                bool isLowerAttack;

                for (int i = 0; i < capacity; i++)
                {
                    isLowerAttack = lowerAttackProbability >= 0 && Randomizer.GetProbability(lowerAttackProbability);
                    Chain.Enqueue(isLowerAttack ? LowerAttackDecision : UpperAttackDecision);
                }

                AttackInterval = attackInterval;
            }

            public void Clear()
            {
                Chain.Clear();
                AttackInterval = 0.0f;
            }
        }
    }

    public enum FighterAIDecisions
    {
        Stop,
        MoveLeft,
        MoveRight,
        AttackPunch,
        AttackKick,
        Dodge,
        BlockStart,
        BlockCancel
    }

    public enum FighterAINotifications
    {
        GotStunned
    }
}
