using System;
using UnityEditor;
using UnityEngine;

namespace MemeFight.Core
{
    public class GlobalGameSettings : ScriptableObject
    {
        // ENGINE SETTINGS
        [Min(0)]
        [SerializeField] int _targetFrameRate = 60;
        [SerializeField] bool _overrideForAllPlatforms = true;
        [SerializeField] FPSOptions _fpsOptions = new FPSOptions(60);

        // SAVE DATA SETTINGS
        [SerializeField] bool _loadData = true;
        [SerializeField] bool _saveData = true;

        // SERVICES
        [SerializeField] bool _analyticsEnabled = true;
        [SerializeField] bool _crashlyticsEnabled = true;
        [SerializeField] bool _cloudMessagingEnabled = true;

        // EXTERNAL LINKS
        [SerializeField] string _contactURL;
        [SerializeField] PrivacyPolicyLinks _privacyPolicyURLs;

        #region Public Members
        // ENGINE SETTINGS
        public int TargetFrameRateGlobal => _targetFrameRate;
        public bool UseGlobalTargetFrameRateForAllPlatforms => _overrideForAllPlatforms;
        public int TargetFrameRateStandalone => _fpsOptions.standalone;
        public int TargetFrameRateAndroid => _fpsOptions.android;
        public int TargetFrameRateIOS => _fpsOptions.iOS;

        // SAVE DATA SETTINGS
        public bool LoadData => _loadData;
        public bool SaveData => _saveData;

        // SERVICES
        public bool AnalyticsEnabled
        {
            get
            {
#if !DEVELOPMENT_BUILD
                return _analyticsEnabled;
#else
                return false;
#endif
            }
        }
        public bool CrashlyticsEnabled
        {
            get
            {
#if !DEVELOPMENT_BUILD
                return _crashlyticsEnabled;
#else
                return false;
#endif
            }
        }

        public bool CloudMessagingEnabled => _cloudMessagingEnabled;

        // EXTERNAL LINKS
        public string ContactURL => _contactURL;
        public PrivacyPolicyLinks PrivacyPolicyURLs => _privacyPolicyURLs;
#endregion

        public const string SettingsAssetName = "GlobalGameSettings";

        static GlobalGameSettings s_Instance;
        public static GlobalGameSettings Instance
        {
            get
            {
                if (s_Instance == null)
                    s_Instance = GetOrCreateSettings();

                return s_Instance;
            }
        }

        static GlobalGameSettings GetOrCreateSettings()
        {
            var settings = Resources.Load<GlobalGameSettings>(SettingsAssetName);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<GlobalGameSettings>();

#if UNITY_EDITOR
                AssetDatabase.CreateAsset(settings, $"Assets/Resources/{SettingsAssetName}.asset");
                AssetDatabase.SaveAssets();
#endif
            }

            return settings;
        }

#if UNITY_EDITOR
        public static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
#endif

        [Serializable]
        struct FPSOptions
        {
            public FPSOptions(int defaultFPS)
            {
                standalone = defaultFPS;
                android = defaultFPS;
                iOS = defaultFPS;
            }

            [Min(0)] public int standalone;
            [Min(0)] public int android;
            [Min(0)] public int iOS;
        }

        [Serializable]
        public struct PrivacyPolicyLinks
        {
            [field: SerializeField]
            public string Portuguese { get; private set; }
            [field: SerializeField]
            public string English { get; private set; }
        }
    }
}
