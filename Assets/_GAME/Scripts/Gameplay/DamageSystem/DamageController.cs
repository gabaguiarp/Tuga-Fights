using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MemeFight.DamageSystem
{
    using Characters;

    public enum DamagerRegion { UpperAttack, LowerAttack }
    public enum DamageableRegion { UpperBody, LowerBody }

    [RequireComponent(typeof(PlayerController))]
    public class DamageController : MonoBehaviour
    {
        [SerializeField] FighterStats _stats;

        [Header("Damagers")]
        [SerializeField] Damager _upperAttackDamager;
        [SerializeField] Damager _lowerAttackDamager;

        [Header("Damageables")]
        [SerializeField] Damageable _damageable;
        [Space(10)]
        [SerializeField] List<Collider2D> _upperBodyHitboxes = new List<Collider2D>();
        [SerializeField] List<Collider2D> _lowerBodyHitboxes = new List<Collider2D>();

        [Header("Debug")]
        [SerializeField] bool _isInvincible;

        PlayerController _controller;

        public event UnityAction OnUpperAttackHit;
        public event UnityAction OnLowerAttackHit;
        public event UnityAction OnAttackBlocked;

        void Reset()
        {
            if (_stats == null)
                _stats = GetComponent<FighterStats>();
        }

        void Awake()
        {
            _controller = GetComponent<PlayerController>();
        }

        void Start()
        {
            SetupDamagers();
            SetupForDamage();
        }

        void SetupDamagers()
        {
            _upperAttackDamager.SetOwner(gameObject);
            _upperAttackDamager.OnDamageDelt += () => HandleDamageDelt(DamagerRegion.UpperAttack);

            _lowerAttackDamager.SetOwner(gameObject);
            _lowerAttackDamager.OnDamageDelt += () => HandleDamageDelt(DamagerRegion.LowerAttack);
        }

        void SetupForDamage()
        {
            _damageable.SetOwner(gameObject);
        }

        void HandleDamageDelt(DamagerRegion region)
        {
            _stats.RegisterSuccessfulAttack();

            if (region.Equals(DamagerRegion.UpperAttack))
            {
                OnUpperAttackHit?.Invoke();
            }
            else
            {
                OnLowerAttackHit?.Invoke();
            }
        }

        bool IsAttacking()
        {
            return _upperAttackDamager.IsEnabled || _lowerAttackDamager.IsEnabled;
        }

        public bool HandleIncomingDamage(float damageAmount, DamagerRegion region)
        {
            // No incoming damage is accountable under the following conditions:
            // - The player controller is not active
            // - The fighter is currently invincible
            // - The fighter's health is totally depleated
            if (!_controller.IsActive || _isInvincible || _stats.CurrentHealth <= 0)
            {
                return false;
            }

            // When the fighter is in a defensive state, they only take half of the incoming damage amount
            if ((_controller.IsBlocking && _stats.BlockingStrength > 0) || _controller.IsDodging)
            {
                damageAmount /= 2;
            }

            // Blocking is not effective against lower attacks (kicks)
            if (_controller.IsBlocking && region.Equals(DamagerRegion.UpperAttack))
            {
                OnAttackBlocked?.Invoke();
                return _stats.UpdateBlockingStrength(-1, damageAmount);
            }
            else
            {
                _stats.TakeDamage(damageAmount);
                return true;
            }
        }

        public void ActivateDamager(DamagerRegion region, bool activate)
        {
            switch (region)
            {
                case DamagerRegion.UpperAttack:
                    _upperAttackDamager.SetEnabled(activate);
                    break;

                case DamagerRegion.LowerAttack:
                    _lowerAttackDamager.SetEnabled(activate);
                    break;
            }
        }

        public void DeactivateAllDamagers()
        {
            _upperAttackDamager.SetEnabled(false);
            _lowerAttackDamager.SetEnabled(false);
        }

        public void SetDamageableRegionEnabled(DamageableRegion region, bool enabled)
        {
            switch (region)
            {
                case DamageableRegion.UpperBody:
                    _upperBodyHitboxes.ForEach(d => d.gameObject.SetActive(enabled));
                    break;

                case DamageableRegion.LowerBody:
                    _lowerBodyHitboxes.ForEach(d => d.gameObject.SetActive(enabled));
                    break;
            }
        }

        // ANIMATION EVENTS CALLBACKS
        public void ActivateUpperAttackDamager() => ActivateDamager(DamagerRegion.UpperAttack, true);
        public void DeactivateUpperAttackDamager() => ActivateDamager(DamagerRegion.UpperAttack, false);
        public void ActivateLowerAttackDamager() => ActivateDamager(DamagerRegion.LowerAttack, true);
        public void DeactivateLowerAttackDamager() => ActivateDamager(DamagerRegion.LowerAttack, false);
    }
}
