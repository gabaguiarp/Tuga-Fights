using Firebase;
using Firebase.Analytics;
using Firebase.Crashlytics;
using System.Threading.Tasks;
using UnityEngine;
#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

namespace MemeFight.Services
{
    public class ServicesManager : Singleton<ServicesManager>
    {
        [Header("Services Status")]
        [SerializeField, ReadOnly] bool _firebaseInit = false;
        [SerializeField, ReadOnly] bool _googlePlayGamesInit = false;

        bool _isAuthenticatingToGooglePlayGames = false;
#if UNITY_ANDROID
        PlayGamesPlatform _playGamesPlatform;
#endif

        #region Initialization
        protected override void Awake()
        {
            base.Awake();
            Stats.OnStatUpdated += HandleStatUpdated;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Stats.OnStatUpdated -= HandleStatUpdated;
        }

        public async Task InitializeServices()
        {
            Debug.Log("[Services] Initializing services...");

            await InitializeFirebase();
#if UNITY_ANDROID
            await InitializeGooglePlayGames();
#endif
        }
        #endregion

        #region Firebase Callbacks
        async Task InitializeFirebase()
        {
            if (_firebaseInit)
                return;

            Debug.Log("[Firebase] Initializing...");

            await FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    FirebaseAnalytics.SetAnalyticsCollectionEnabled(GameManager.GlobalSettings.AnalyticsEnabled);
                    Crashlytics.IsCrashlyticsCollectionEnabled = GameManager.GlobalSettings.CrashlyticsEnabled;

                    if (GameManager.IsDebugMode)
                    {
                        FirebaseApp.LogLevel = LogLevel.Debug;
                    }

                    _firebaseInit = true;
                    Debug.Log("[Firebase] Firebase is ready to use!");
                }
                else
                {
                    // Firebase Unity SDK is not safe to use here.
                    Debug.LogErrorFormat("[Firebase] Could not resolve all Firebase dependencies: {0}", dependencyStatus);
                }
            });
        }
        #endregion

        #region Google Play Games Callbacks
#if UNITY_ANDROID
        bool IsPlayGamesServicesAvailable()
        {
            return _playGamesPlatform != null && _playGamesPlatform.IsAuthenticated();
        }

        async Task InitializeGooglePlayGames()
        {
            if (!_googlePlayGamesInit)
            {
                Debug.Log("[Google Play Games] Initializing...");
                PlayGamesPlatform.DebugLogEnabled = true;
                await AuthenticateToGooglePlayGamesAsync();
            }
        }

        async Task AuthenticateToGooglePlayGamesAsync()
        {
            // NOTE: Calling 'Activate' in the PlayGamesPlatform, will make it the default platform for Social callbacks.
            // If you wish to use Unity's Social features with other platforms, do not call 'Activate' here.
            _playGamesPlatform = PlayGamesPlatform.Activate();

            if (_playGamesPlatform == null)
            {
                Debug.LogError("Failed to get a valid instance of PlayGamesPlatform! Unable to authenticate to Google Play Games.");
            }
            else
            {
                // Perform authentication
                _isAuthenticatingToGooglePlayGames = true;

                _playGamesPlatform.Authenticate(status =>
                {
                    if (status == SignInStatus.Success)
                    {
                        // Continue with Play Games Services
                        Debug.Log("[Google Play Games] Successfully authenticated to Google Play Games!");
                        _googlePlayGamesInit = true;
                        Analytics.RegisterEvent(Analytics.Event.LOGIN);
                    }
                    else
                    {
                        // Disable your integration with Play Games Services or show a login button
                        // to ask users to sign-in. Clicking it should call
                        // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
                        Debug.LogWarning("[Google Play Games] The platform is not available at the moment.");
                    }

                    _isAuthenticatingToGooglePlayGames = false;
                });

                while (_isAuthenticatingToGooglePlayGames)
                {
                    await Task.Yield();
                }
            }
        }
#endif
        #endregion

        #region Achievements Callbacks
        async Task ShowAchievementsAsync()
        {
#if UNITY_ANDROID
            if (!IsPlayGamesServicesAvailable())
            {
                await AuthenticateToGooglePlayGamesAsync();
            }

            if (_playGamesPlatform == null)
            {
                Debug.LogError("[Services] Unable to show Achievements because no PlayGamesPlatform instance exists!");
            }
            else
            {
                _playGamesPlatform.ShowAchievementsUI();
            }
#endif
        }

        public async void ShowAchievements()
        {
            Debug.Log("[Services] ShowAchievements called");

            if (_isAuthenticatingToGooglePlayGames)
            {
                Debug.LogWarning("[Services] Unable to show achievements because the game is waiting for Google Play Games " +
                    "authentication to complete.");
                return;
            }

            await ShowAchievementsAsync();
        }
        #endregion

        #region Event Responders
        void HandleStatUpdated(StatID statID, int amount)
        {
            switch (statID)
            {
                case StatID.AGATA_WIN:
                    Achievements.Increment(Achievement.AGATA_WINS, amount);
                    break;

                case StatID.ANAMALHOA_WIN:
                    Achievements.Increment(Achievement.ANAMALHOA_WINS, amount);
                    break;

                case StatID.BAIAO_WIN:
                    Achievements.Increment(Achievement.BAIAO_WINS, amount);
                    break;

                case StatID.BATATINHA_WIN:
                    Achievements.Increment(Achievement.BATATINHA_WINS, amount);
                    break;

                case StatID.CARLOSCOSTA_WIN:
                    Achievements.Increment(Achievement.CARLOSCOSTA_WINS, amount);
                    break;

                case StatID.CLAUDIORAMOS_WIN:
                    Achievements.Increment(Achievement.CLAUDIORAMOS_WINS, amount);
                    break;

                case StatID.COMPANHIA_WIN:
                    Achievements.Increment(Achievement.COMPANHIA_WINS, amount);
                    break;

                case StatID.GISELA_WIN:
                    Achievements.Increment(Achievement.GISELA_WINS, amount);
                    break;

                case StatID.GOUCHA_WIN:
                    Achievements.Increment(Achievement.GOUCHA_WINS, amount);
                    break;

                case StatID.JCB_WIN:
                    Achievements.Increment(Achievement.JCB_WINS, amount);
                    break;

                case StatID.JULIAPINHEIRO_WIN:
                    Achievements.Increment(Achievement.JULIAPINHEIRO_WINS, amount);
                    break;

                case StatID.LUCY_WIN:
                    Achievements.Increment(Achievement.LUCY_WINS, amount);
                    break;

                case StatID.MALATO_WIN:
                    Achievements.Increment(Achievement.MALATO_WINS, amount);
                    break;

                case StatID.MARIALEAL_WIN:
                    Achievements.Increment(Achievement.MARIALEAL_WINS, amount);
                    break;

                case StatID.MARIAVIEIRA_WIN:
                    Achievements.Increment(Achievement.MARIAVIEIRA_WINS, amount);
                    break;

                case StatID.SANDRA_WIN:
                    Achievements.Increment(Achievement.SANDRA_WINS, amount);
                    break;

                case StatID.TERESAGUILHERME_WIN:
                    Achievements.Increment(Achievement.TERESAGUILHERME_WINS, amount);
                    break;

                case StatID.TOY_WIN:
                    Achievements.Increment(Achievement.TOY_WINS, amount);
                    break;
            }
        }
        #endregion
    }
}
