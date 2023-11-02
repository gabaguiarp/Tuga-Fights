using UnityEngine;

namespace MemeFight.DebugSystem
{
    public class DebugMenuUIBase : MonoBehaviour
    {
        [SerializeField] protected GameObject _debugPanel;

        protected virtual void Start()
        {
            _debugPanel.SetActive(ShouldEnable());
        }

        protected virtual bool ShouldEnable()
        {
            return GameManager.IsDebugMode;
        }
    }
}
