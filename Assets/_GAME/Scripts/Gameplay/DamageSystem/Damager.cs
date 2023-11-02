using UnityEngine;
using UnityEngine.Events;

namespace MemeFight.DamageSystem
{
    public class Damager : MonoBehaviour
    {
        [SerializeField] FloatVariableSO _damageAmount;
        [SerializeField] DamagerRegion _region;
        [SerializeField] bool _startEnabled = false;

        GameObject _owner;
        Collider2D _collider;

        public bool IsEnabled => _collider && _collider.enabled;

        public event UnityAction OnDamageDelt;

        void Awake()
        {
            _collider = GetComponent<Collider2D>();
            SetEnabled(_startEnabled);
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Damageable damageable) && !IsDamageableMine(damageable))
            {
                if (damageable.DealDamage(_damageAmount.Value, _region))
                {
                    OnDamageDelt?.Invoke();
                }
            }
        }

        bool IsDamageableMine(Damageable damageable) => _owner != null && _owner == damageable.Owner;

        public void SetOwner(GameObject owner) => _owner = owner;

        public void SetEnabled(bool enabled)
        {
            if (_collider == null)
            {
                Debug.LogError($"Unable to change the enabled state for the damager in {name} because no collider was found!");
                return;
            }

            _collider.enabled = enabled;
        }
    }
}
