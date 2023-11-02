using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MemeFight.EditorTools
{
    using Core;

    [CustomEditor(typeof(GlobalGameSettings))]
    public class GlobalGameSettingsEditor : Editor
    {
        internal class Styles
        {
            public static readonly GUIContent FPSSettingsLabel = new GUIContent("Frame Rate");
            public static readonly GUIContent TargetFPSLabel = new GUIContent("Target Frame Rate");
            public static readonly GUIContent OverrideFPSLabel = new GUIContent("Override For All Platforms", "Whether to use the same target frame rate for all platforms");
            public static readonly GUIContent FPSOptionsLabel = new GUIContent("Platform Frame Rates");

            public static readonly GUIContent SaveDataSettingsLabel = new GUIContent("Save Data Settings");
            public static readonly GUIContent LoadDataLabel = new GUIContent("Load Data", "Is the game allowed to load data from an existing save file?");
            public static readonly GUIContent SaveDataLabel = new GUIContent("Save Data", "Is the game allowed to save data onto a file?");

            public static readonly GUIContent ServicesLabel = new GUIContent("Services");
            public static readonly GUIContent AnalyticsEnabledLabel = new GUIContent("Analytics Enabled");
            public static readonly GUIContent CrashlyticsEnabledLabel = new GUIContent("Crashlytics Enabled");

            public static readonly GUIContent ExternalLinksLabel = new GUIContent("External Links");
            public static readonly GUIContent ContactURL_Label = new GUIContent("Contact URL");
            public static readonly GUIContent PrivacyPolicyURLsLabel = new GUIContent("Privacy Policy URLs");
        }

        SerializedProperty _propTargetFPS;
        SerializedProperty _propOverrideFPS;
        SerializedProperty _propFPSOptions;

        SerializedProperty _propLoadData;
        SerializedProperty _propSaveData;

        SerializedProperty _propAnalyticsEnabled;
        SerializedProperty _propCrashlyticsEnabled;

        SerializedProperty _propContactURL;
        SerializedProperty _propPrivacyPolicyURLs;

        readonly string ServicesNoticeMessage = "Services are always disabled in Development Builds.";

        void OnEnable()
        {
            if (target == null)
                return;

            _propTargetFPS = serializedObject.FindProperty("_targetFrameRate");
            _propOverrideFPS = serializedObject.FindProperty("_overrideForAllPlatforms");
            _propFPSOptions = serializedObject.FindProperty("_fpsOptions");

            _propLoadData = serializedObject.FindProperty("_loadData");
            _propSaveData = serializedObject.FindProperty("_saveData");

            _propAnalyticsEnabled = serializedObject.FindProperty("_analyticsEnabled");
            _propCrashlyticsEnabled = serializedObject.FindProperty("_crashlyticsEnabled");

            _propContactURL = serializedObject.FindProperty("_contactURL");
            _propPrivacyPolicyURLs = serializedObject.FindProperty("_privacyPolicyURLs");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // ENGINE
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label(Styles.FPSSettingsLabel, EditorStyles.boldLabel);
            EditorGUI.indentLevel = 1;

            EditorGUILayout.PropertyField(_propOverrideFPS, Styles.OverrideFPSLabel);

            if (_propOverrideFPS.boolValue == true)
            {
                EditorGUILayout.PropertyField(_propTargetFPS, Styles.TargetFPSLabel);
            }
            else
            {
                EditorGUILayout.PropertyField(_propFPSOptions, Styles.FPSOptionsLabel);
            }

            EditorGUI.indentLevel = 0;

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            // SAVE DATA
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label(Styles.SaveDataSettingsLabel, EditorStyles.boldLabel);
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(_propLoadData, Styles.LoadDataLabel);
            EditorGUILayout.PropertyField(_propSaveData, Styles.SaveDataLabel);
            EditorGUI.indentLevel = 0;

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            // SERVICES
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label(Styles.ServicesLabel, EditorStyles.boldLabel);
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(_propAnalyticsEnabled, Styles.AnalyticsEnabledLabel);
            EditorGUILayout.PropertyField(_propCrashlyticsEnabled, Styles.CrashlyticsEnabledLabel);
            EditorGUI.indentLevel = 0;

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(ServicesNoticeMessage, MessageType.Info);
            EditorGUILayout.EndVertical();

            // EXTERNAL LINKS
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label(Styles.ExternalLinksLabel, EditorStyles.boldLabel);
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(_propContactURL, Styles.ContactURL_Label);
            EditorGUILayout.PropertyField(_propPrivacyPolicyURLs, Styles.PrivacyPolicyURLsLabel);
            EditorGUI.indentLevel = 0;

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }

    class GlobalGameSettingsProvider : SettingsProvider
    {
        private SerializedObject _globalSettings;

        public GlobalGameSettingsProvider(string path, SettingsScope scope) : base(path, scope) { }

        // This function is called when the user clicks on the Global Game Settings element in the Settings window.
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            _globalSettings = GlobalGameSettings.GetSerializedSettings();
        }

        static Object GetGlobalSettings()
        {
            return GlobalGameSettings.Instance;
        }

        // Register the SettingsProvider
        [SettingsProvider]
        public static SettingsProvider CreateGlobalGameSettingsProvider()
        {
            if (GlobalGameSettings.Instance != null)
            {
                // This will use the reference ScriptableObject as the provider.
                // This way, the custom editor format will be kept and therefore we don't need to override the OnGUI method.
                var provider = new AssetSettingsProvider("Project/Global Game Settings", GetGlobalSettings);

                // Automatically extract all keywords from the Styles.
                provider.PopulateSearchKeywordsFromGUIContentProperties<GlobalGameSettingsEditor.Styles>();
                return provider;
            }

            // Settings asset doesn't exist yet; no need to display anything in the Settings window.
            return null;
        }
    }
}
