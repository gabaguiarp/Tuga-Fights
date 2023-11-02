#if UNITY_EDITOR
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MemeFight.EditorTools
{
    /// <summary>
    /// Performs a quick initialization, ensuring that the core game components are loaded when entering playmode from a random scene while
    /// testing inside the editor, avoiding that we have to start from the Startup scene every time we want to test.
    /// </summary>
    public class EditorStartup : MonoBehaviour
    {
        [SerializeField] SceneReferenceSO _managersScene;

        void Awake()
        {
            if (!SceneManager.GetSceneByName(_managersScene.Name).isLoaded)
            {
                StartCoroutine(EditorInitialization());
            }
        }

        IEnumerator EditorInitialization()
        {
            yield return SceneManager.LoadSceneAsync(_managersScene.Name, LoadSceneMode.Additive);

            yield return CoroutineUtils.WaitOneFrame;

            GameManager.Init();

            SceneLoader.NotifyEditorStartupFinished();
        }
    }
}
#endif
