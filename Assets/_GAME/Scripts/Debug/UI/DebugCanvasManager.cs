using UnityEngine;

namespace MemeFight.DebugSystem
{
    public class DebugCanvasManager : MonoBehaviour
    {
        [SerializeField] Canvas _debugCanvas;

        void Awake()
        {
            _debugCanvas.gameObject.SetActive(GameManager.IsDebugMode);
        }
    }
}
