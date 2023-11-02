using UnityEngine;

namespace MemeFight
{
    /// <summary>
    /// A <see cref="Singleton{T}"/> class that manages core functionality and needs to be initialized as soon as the game starts. Contains a
    /// dedicated <i>OnInitialize</i> method to handle all the initialization logic.<br></br>
    /// <br></br>
    /// NOTE: Core Managers can only be initialized once.
    /// </summary>
    public abstract class CoreManager<T> : Singleton<T> where T : MonoBehaviour
    {
        public static bool WasInit { get; private set; } = false;

        /// <summary>
        /// Initializes the manager if an instance of it is found.
        /// </summary>
        public static void Init()
        {
            if (WasInit) return;

            if (Instance == null)
            {
                Debug.LogError($"Unable to initialize {typeof(T)} because no instance was found!");
                return;
            }

            var manager = Instance as CoreManager<T>;
            manager.OnInitialize();

            WasInit = true;
        }

        /// <summary>
        /// Used to handle all the initialization logic.
        /// </summary>
        protected abstract void OnInitialize();

        protected override void OnDestroy()
        {
            base.OnDestroy();
            WasInit = false;
        }
    }
}
