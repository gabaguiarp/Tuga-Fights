using UnityEngine;

namespace MemeFight
{
    /// <summary>
    /// A custom type of <see cref="MonoBehaviour"/> that invokes a special <i>OnSceneReady</i> method when the <see cref="SceneLoader"/>
    /// finishes loading a new scene. Since this method is always invoked after <i>Awake</i>, singleton instances and managers are allowed
    /// to be initialized first, therefore they can be safely accessed through <i>OnSceneReady</i> without the risk of getting any null
    /// exceptions.
    /// </summary>
    public abstract class ManagedBehaviour : MonoBehaviour
    {
        protected virtual void Awake()
        {
            SceneLoader.OnLoadingComplete += OnSceneReady;
        }

        protected virtual void OnDestroy()
        {
            SceneLoader.OnLoadingComplete -= OnSceneReady;
        }

        /// <summary>
        /// Called when a scene has been loaded and initialized. This method is always called after <i>Awake</i>, when the scene
        /// gains focus.
        /// </summary>
        protected abstract void OnSceneReady();
    }
}
