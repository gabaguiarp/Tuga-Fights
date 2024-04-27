using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MemeFight
{
    using Services;

    public class Initializer : MonoBehaviour
    {
        [SerializeField] SceneReferenceSO _managersScene;
        [Tooltip("The first scene to load after the initialization process is done.")]
        [SerializeField] SceneReferenceSO _startScene;

        void Awake()
        {
            StartCoroutine(SystemInitializationProcess());
        }

        IEnumerator SystemInitializationProcess()
        {
            // Load managers
            yield return SceneManager.LoadSceneAsync(_managersScene.Name, LoadSceneMode.Additive);

            // Wait one frame to ensure all Awake methods are called in the managers scene
            yield return CoroutineUtils.WaitOneFrame;

            // Initialize game
            GameManager.Init();

            // Wait for all resources to finish loading
            yield return new WaitUntil(() => ResourcesManager.ResourcesLoadingDone);

            // Initialize services
            if (ServicesManager.Instance != null)
            {
                var servicesTask = ServicesManager.Instance.InitializeServices();
                yield return new WaitUntil(() => servicesTask.IsCompleted);
            }
            else
            {
                Debug.LogWarning("No ServicesManager instance found! Services will not be initialised.");
            }

            // Load start scene
            SceneLoader.Instance.LoadScene(_startScene, false);
        }
    }
}
