using UnityEngine;

namespace MemeFight.DamageSystem
{
    public class Damageable : MonoBehaviour
    {
        [SerializeField] DamageController _damageController;

        GameObject _owner;

        public GameObject Owner
        {
            get => _owner == null ? this.gameObject : _owner;
            private set => _owner = value;
        }

        public bool DealDamage(float amount, DamagerRegion region)
        {
            if (enabled && _damageController.HandleIncomingDamage(amount, region))
                return true;

            return false;
        }

        public void SetOwner(GameObject owner) => _owner = owner;
    }
}
