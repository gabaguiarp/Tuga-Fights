using MemeFight.Characters;
using UnityEngine;

namespace MemeFight.DebugSystem
{
    public class FighterEnablerDebug : ManagedBehaviour
    {
        [SerializeField] Fighter _fighterToEnable;

        protected override void OnSceneReady()
        {
            if (_fighterToEnable == null)
            {
                Debug.LogError("No fighter to enable assigned!");
                return;
            }

            _fighterToEnable.Controller.controlMode = PlayerController.ControlMode.Input;
            _fighterToEnable.Controller.Activate();
        }
    }
}
