using UnityEngine;
using UnityEngine.Events;

namespace MemeFight.Animation
{
    public class FighterAnimationEvents : MonoBehaviour
    {
        [SerializeField] UnityEvent _onPunchStarted;
        [SerializeField] UnityEvent _onPunchEnded;
        [SerializeField] UnityEvent _onKickStarted;
        [SerializeField] UnityEvent _onKickEnded;
        [SerializeField] UnityEvent _onStep;
        [SerializeField] UnityEvent _onGroundHit;
        [SerializeField] UnityEvent _onDodge;

        public void PunchStart() => _onPunchStarted.Invoke();
        public void PunchEnd() => _onPunchEnded.Invoke();
        public void KickStart() => _onKickStarted.Invoke();
        public void KickEnd() => _onKickEnded.Invoke();
        public void Step() => _onStep.Invoke();
        public void GroundHit() => _onGroundHit.Invoke();
        public void Dodge() => _onDodge.Invoke();
    }
}
