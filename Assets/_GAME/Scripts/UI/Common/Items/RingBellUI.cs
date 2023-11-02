using UnityEngine;

namespace MemeFight.UI
{
    public class RingBellUI : MonoBehaviour
    {
        [SerializeField] Animator _animator;

        readonly int BellRingAnimationHash = Animator.StringToHash("Ringing");

        public void SetRingAnimationState(bool active)
        {
            _animator.SetBool(BellRingAnimationHash, active);
        }
    }
}
