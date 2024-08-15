using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using MemeFight.UI;

namespace MemeFight
{
    public class SceneLoader : Singleton<SceneLoader>
    {
        [Header("Settings")]
        [SerializeField] bool _stopMusicWhenSwitchingScenes = true;
        [Range(0, 2)]
        [Tooltip("The minimum amout of time for which the loading screen must be displayed, even if everything is loaded faster.")]
        [SerializeField] float _minDisplayTime = 1.0f;

        [Header("References")]
        [SerializeField] SceneReferenceSO _managersScene;
        [SerializeField] SceneReferenceSO _mainMenuScene;
        [SerializeField] LoadingScreenUI _loadingScreen;

        [Header("Listening On")]
        [SerializeField] VoidEventSO _restartSceneEvent;

        [Header("Info")]
        [SerializeField, ReadOnly] bool _isLoading;
        [SerializeField, ReadOnly] float _loadingPct;

        Scene _lastLoadedScene;
        List<Scene> _scenesToUnload = new List<Scene>();
        List<AsyncOperation> _currentOperations = new List<AsyncOperation>();
        WaitUntil _waitForOperations;
        bool _showLoadingScreen;

        public static event UnityAction OnLoadingStarted;
        public static event UnityAction OnLoadingComplete;

        #region Initialization
        void OnEnable()
        {
            _restartSceneEvent.OnRaised += ReloadCurrentScene;
        }

        void OnDisable()
        {
            _restartSceneEvent.OnRaised -= ReloadCurrentScene;
        }
        #endregion

        #region Load Requests
        public void LoadScene(SceneReferenceSO scene, bool showLoadingScreen = true)
        {
            LoadScene(scene.Name, showLoadingScreen);
        }

        public void ReloadCurrentScene()
        {
            Debug.Log("Will reload current scene...");
            LoadScene(GetCurrentScene().name);
        }

        public void LoadMainMenu() => LoadScene(_mainMenuScene);
        #endregion

        void LoadScene(string sceneName, bool showLoadingScreen = true)
        {
            if (_isLoading)
            {
                Debug.LogWarning("Unable to load scenes because a loading process is already in course. Wait for it to finish in order to " +
                    "make new scene loading requests.");
                return;
            }

            // Setup and start loading process
            _showLoadingScreen = showLoadingScreen;
            StartCoroutine(SceneAsyncLoading(sceneName));
        }

        IEnumerator SceneAsyncLoading(string sceneToLoad)
        {
            // Prepare scene loading process
            SetAllLoadedScenesForUnloading();

            _isLoading = true;
            _loadingPct = 0.0f;
            OnLoadingStarted?.Invoke();

            float loadingScreenDisplayStartTime = Time.time;

            // Show loading screen
            if (_showLoadingScreen)
            {
                _loadingScreen.ShowLoadingScreen(true);
                yield return _loadingScreen.WaitForFade;
            }

            if (_stopMusicWhenSwitchingScenes)
                AudioManager.Instance.StopMusicTrack();

            // Set time scale to normal and clear previous operations
            GameManager.SetTimeScale(TimeScale.Normal);
            _currentOperations.Clear();

            // Unload current scenes
            if (_scenesToUnload.Count > 0)
            {
                _scenesToUnload.ForEach(s => _currentOperations.Add(SceneManager.UnloadSceneAsync(s)));
                yield return WaitUnitlAllOperationsAreDone();
            }

            _currentOperations.Clear();

            // Load new scene
            _currentOperations.Add(SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive));
            Debug.Log($"Loading {sceneToLoad}...");

            while (!AreAllOperationsDone())
            {
                _loadingPct = GetOperationsAveragePercentage();
                _loadingScreen.SetLoadingBarValue(_loadingPct);
                yield return null;
            }

            // Check if loading screen has been displayed for the defined minimum time
            if (_showLoadingScreen)
            {
                while (Time.time < loadingScreenDisplayStartTime + _minDisplayTime)
                {
                    yield return null;
                }
            }

            // Activate the loaded scene
            _lastLoadedScene = SceneManager.GetSceneByName(sceneToLoad);
            SceneManager.SetActiveScene(_lastLoadedScene);

            // Wait one frame to let the Awake methods be called in the new scene
            yield return CoroutineUtils.WaitOneFrame;

            // Hide loading screen
            if (_showLoadingScreen)
                _loadingScreen.ShowLoadingScreen(false);

            // Finish the process by raising the event that notifies other instances about the new scene being ready
            _isLoading = false;
            OnLoadingComplete?.Invoke();
        }

        /// <summary>
        /// Keeps a reference of all the currently loaded scenes, excluding the managers.
        /// </summary>
        void SetAllLoadedScenesForUnloading()
        {
            _scenesToUnload.Clear();

            Scene loadedScene;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                loadedScene = SceneManager.GetSceneAt(i);

                if (loadedScene.name == _managersScene.Name)
                    continue;
                else
                    _scenesToUnload.Add(loadedScene);
            }
        }

        WaitUntil WaitUnitlAllOperationsAreDone()
        {
            if (_waitForOperations == null)
                _waitForOperations = new WaitUntil(() => AreAllOperationsDone());

            return _waitForOperations;
        }

        bool AreAllOperationsDone() => _currentOperations.TrueForAll(op => op.isDone);

        float GetOperationsAveragePercentage()
        {
            float overallProgress = 0.0f;

            _currentOperations.ForEach(op => overallProgress += op.progress);
            overallProgress /= _currentOperations.Count;

            // We divide the overall progress by 0.9 because that's the maximum value the "progress" parameter from AsyncOperations gives us.
            // The other 0.1 are reserved for activation.
            return Mathf.Clamp01(overallProgress / 0.9f);
        }

        Scene GetCurrentScene()
        {
            if (!_lastLoadedScene.IsValid())
                return SceneManager.GetActiveScene();

            return _lastLoadedScene;
        }

#if UNITY_EDITOR
        public static void NotifyEditorStartupFinished()
        {
            OnLoadingComplete?.Invoke();
            Debug.Log("EDITOR STARTUP COMPLETE");
        }
#endif
    }
}
