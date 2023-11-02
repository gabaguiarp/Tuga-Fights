using UnityEditor;
using UnityEngine;

namespace MemeFight.EditorTools
{
    // Docs: https://docs.unity3d.com/ScriptReference/MenuItem.html
    public class CustomMenuItems
    {
        const string GameObjectMenuPath = "GameObject/";
        const string ManagerMenuPath = GameObjectMenuPath + "Manager/";
        const string EditorStartupPrefabPath = "Assets/_GAME/Prefabs/Core/EditorStartup.prefab";

        [MenuItem(ManagerMenuPath + "Editor Startup", priority = 10)]
        static void AddEditorStartup()
        {
            InstantiatePrefab<EditorStartup>(EditorStartupPrefabPath);
        }

        [MenuItem(GameObjectMenuPath + "Audio/Music Player", priority = 10)]
        static void AddMusicPlayer()
        {
            CreateObject<Audio.MusicPlayer>("MusicPlayer");
        }

        /// <summary>
        /// Instantiates the prefab designated at <paramref name="prefabPath"/>.
        /// </summary>
        /// <typeparam name="T">The type expected for the prefab.</typeparam>
        /// <param name="isExclusive">Indicates if only one instance of this prefab should be available in the current scene, preventing the creation of
        /// duplicates.</param>
        /// <param name="actionName">The name to override the 'Undo' action with, when instantiating the prefab.</param>
        static void InstantiatePrefab<T>(string prefabPath, bool isExclusive = true, string actionName = null) where T : Object
        {
            if (isExclusive && Object.FindObjectOfType<T>(true))
            {
                Debug.LogWarning($"A {typeof(T)} prefab already exists in this scene!");
                return;
            }

            try
            {
                Object prefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));

                if (string.IsNullOrEmpty(actionName))
                    actionName = $"Create {typeof(T)}";

                var go = PrefabUtility.InstantiatePrefab(prefab);
                Undo.RegisterCreatedObjectUndo(go, actionName);
            }
            catch
            {
                Debug.LogError("Unable to instantiate prefab because it could not be found in the Asset Database path: " + prefabPath);
            }
        }

        static void CreateObject<T>(string name)
        {
            Transform parent = Selection.activeTransform;

            GameObject obj = new GameObject(name, typeof(T));
            if (parent != null)
                obj.transform.SetParent(parent);

            Undo.RegisterCreatedObjectUndo(obj, $"Create {name}");

            Selection.activeObject = obj;
        }
    }
}
