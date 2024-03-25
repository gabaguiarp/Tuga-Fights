#region Copyright

// Copyright 2021, Jouaid Technologies, All rights reserved.
// Permission is hereby granted, to the person obtaining buying a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, 
// and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace JT.Utils
{
    struct SceneStruct
    {
        public string name;
        public string path;
    }

    public class EditorBuildScenesWindow : EditorWindow
    {
        private Vector2 scrollPosition = Vector2.zero;
        private readonly string[] dropOptions = new[] { "Build Scenes", "All Scenes" };

        private int selectedMode;
        private int oldSelectedMode;

        private int selectedScene;
        private int changedSelected;
        private List<SceneStruct> scenes = new List<SceneStruct>();
        private List<SceneStruct> cachedAllScenes = new List<SceneStruct>();
        private List<SceneStruct> cachedBuildScenes = new List<SceneStruct>();

        private FileInfo[] files;
        private Dictionary<string, bool> folds;

        [MenuItem("Scenes/Build Scenes Window")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(EditorBuildScenesWindow), false, "Scenes", true);
        }

        private void OnEnable()
        {
            GetScenes(true);
        }

        private void OnGUI()
        {
            if (Application.isPlaying)
                return;

            GUI.Label(new Rect(0, 0, 300, 50), GUI.tooltip);
            
            EditorGUILayout.Space();
            
            scenes.Clear();
            selectedMode = EditorGUI.Popup(new Rect(0, 10, position.width - 35, 30), selectedMode, dropOptions);

            GUIContent refreshIcon = EditorGUIUtility.IconContent("d_Refresh", "|Refresh The List Of Scenes");
            if (EditorGUI.DropdownButton(new Rect(position.width - 30, 10, 30, 30), refreshIcon,
                    FocusType.Passive, GUIStyle.none))
                GetScenes(true);

            RenderAllTheScene();
        }

        private void GetScenes(bool fresh)
        {
            if (selectedMode == 0)
                GetBuildScenes(fresh);
            else
                GetAllScenes(fresh);
        }

        private void RenderAllTheScene()
        {
            GUIContent addIcon = EditorGUIUtility.IconContent("CreateAddNew", "|Add Scene To The Build Settings");
            GUIContent removeIcon =
                EditorGUIUtility.IconContent("Toolbar Minus", "|Remove Scene From The Build Settings");

            if (selectedMode != oldSelectedMode)
            {
                oldSelectedMode = selectedMode;
                GetScenes(true);
            }
            else
                GetScenes(false);

            selectedScene = changedSelected;

            EditorGUILayout.Space(40f);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            for (int i = 0; i < scenes.Count; i++)
            {
                GUILayout.BeginHorizontal("box");
                if (GUILayout.Button(new GUIContent($"{scenes[i].name}", "Load Scene In Single Mode"),
                        GUILayout.Height(20)))
                {
                    selectedScene = changedSelected = i;
                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                    EditorSceneManager.OpenScene(scenes[i].path, OpenSceneMode.Single);
                }

                if (GUILayout.Button(new GUIContent($"Additive", "Load Scene In Additive Mode"), GUILayout.Height(20),
                        GUILayout.Width(70)))
                {
                    EditorSceneManager.OpenScene(scenes[i].path, OpenSceneMode.Additive);
                }

                if (selectedMode == 1 && GUILayout.Button(addIcon, GUILayout.Width(30)))
                {
                    List<EditorBuildSettingsScene> sceneAsset =
                        new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes.ToArray());
                    sceneAsset.Add(new EditorBuildSettingsScene(scenes[i].path, true));

                    EditorBuildSettings.scenes = sceneAsset.ToArray();
                    GetScenes(true);
                }

                if (selectedMode == 0 && GUILayout.Button(removeIcon, GUILayout.Width(30)))
                {
                    List<EditorBuildSettingsScene> sceneAsset =
                        new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes.ToArray());
                    int indexToRemoveAt = sceneAsset.FindIndex(x => x.path == scenes[i].path);
                    sceneAsset.RemoveAt(indexToRemoveAt);

                    EditorBuildSettings.scenes = sceneAsset.ToArray();
                    GetScenes(true);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }

        /// <summary>
        /// Update the scene list to include all the scenes in the build settings
        /// </summary>
        /// <param name="fresh">True = Get all the scenes from scratch ||
        /// False = Returned the cached list</param>
        private void GetBuildScenes(bool fresh)
        {
            if (!fresh)
            {
                scenes = new List<SceneStruct>(cachedBuildScenes);
                return;
            }

            cachedBuildScenes.Clear();
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                EditorBuildSettingsScene scene = EditorBuildSettings.scenes[i];

                string sceneName = Path.GetFileNameWithoutExtension(scene.path);
                SceneStruct sceneStruct = new SceneStruct()
                {
                    name = sceneName,
                    path = scene.path
                };

                scenes.Add(sceneStruct);
                cachedBuildScenes.Add(sceneStruct);
                if (scene.path == EditorSceneManager.GetActiveScene().path)
                    selectedScene = changedSelected = i;
            }

            SelectScene();
        }


        /// <summary>
        /// Updated the scene list to include all the scenes in the project
        /// </summary>
        /// <param name="fresh">True = Get all the scene from scratch ||
        /// False = Returned the cached list</param>
        private void GetAllScenes(bool fresh)
        {
            if (!fresh)
            {
                scenes = new List<SceneStruct>(cachedAllScenes);
                return;
            }

            cachedAllScenes.Clear();
            DirectoryInfo directory = new DirectoryInfo(Application.dataPath);
            files = directory.GetFiles("*.unity", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++)
            {
                FileInfo f = files[i];
                string scenePath = GetAssetPath(f.FullName);

                // Pathing fix if you are using a different OS
                //scenePath = scenePath.Replace("\\", "/");

                string sceneName = Path.GetFileNameWithoutExtension(scenePath);

                SceneStruct scene = new SceneStruct()
                {
                    name = sceneName,
                    path = scenePath
                };

                scenes.Add(scene);
                cachedAllScenes.Add(scene);

                if (scene.path == EditorSceneManager.GetActiveScene().path)
                    selectedScene = changedSelected = i;
            }

            // Selection
            SelectScene();
        }

        /// <summary>
        /// Set the selected scene as the active one
        /// </summary>
        private void SelectScene()
        {
            if (selectedScene != changedSelected)
            {
                selectedScene = changedSelected;

                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                EditorSceneManager.OpenScene(EditorBuildSettings.scenes[changedSelected].path);
            }
        }

        /// <summary>
        /// Return asset path without the previous directories
        /// </summary>
        /// <param name="text">text / path to trim</param>
        /// <param name="stopAt">Key word to stop at</param>
        /// <returns></returns>
        private string GetAssetPath(string text, string stopAt = "Assets")
        {
            return "Assets" + text.Substring(text.IndexOf(stopAt) + stopAt.Length);
        }
    }
}