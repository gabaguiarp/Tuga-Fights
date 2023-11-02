using UnityEngine;

namespace MemeFight
{
    public class InputEnabler : ManagedBehaviour
    {
        [SerializeField] InputManager.InputMap _initialInputMap;

        protected override void OnSceneReady()
        {
            InputManager.Instance.EnableInputMap(_initialInputMap);
        }
    }
}
