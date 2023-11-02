using UnityEngine;

namespace MemeFight
{
    /// <summary>
    /// Applies specific modifications when the current platform is mobile.
    /// </summary>
    public class MobileModifier : MonoBehaviour
    {
        [SerializeField] GameObject[] _objectsToDisable;
        [SerializeField] GameObject[] _objectsToEnable;

#if UNITY_ANDROID || UNITY_IOS
        void Awake()
        {
            if (!PlatformManager.IsMobile)
                return;

            DisableObjects();
            EnableObjects();
        }

        void DisableObjects()
        {
            _objectsToDisable.ForEach(o => o.SetActive(false));
        }

        void EnableObjects()
        {
            _objectsToEnable.ForEach(o => o.SetActive(true));
        }
#endif
    }
}