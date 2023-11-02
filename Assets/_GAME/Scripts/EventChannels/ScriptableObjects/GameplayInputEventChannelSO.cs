using UnityEngine;
using UnityEngine.Events;

namespace MemeFight
{
    [CreateAssetMenu(fileName = "GameplayInputChannel", menuName = MenuPaths.InputEventChannels + "Gameplay Input Event Channel")]
    public class GameplayInputEventChannelSO : BaseSO
    {
        public event UnityAction<Vector2> OnMove;
        public event UnityAction OnPunch;
        public event UnityAction OnKick;
        public event UnityAction OnDodge;
        public event UnityAction<bool> OnBlock;

        public void RaiseMoveEvent(Vector2 moveInput) => OnMove?.Invoke(moveInput);
        public void RaisePuchEvent() => OnPunch?.Invoke();
        public void RaiseKickEvent() => OnKick?.Invoke();
        public void RaiseDodgeEvent() => OnDodge?.Invoke();
        public void RaiseBlockStartedEvent() => OnBlock?.Invoke(true);
        public void RaiseBlockCanceledEvent() => OnBlock?.Invoke(false);
    }
}
