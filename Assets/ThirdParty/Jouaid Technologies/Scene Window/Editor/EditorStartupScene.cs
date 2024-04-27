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

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace JT.Utils
{
    public static class EditorStartupScene
    {
        private const string playFromFirstMenuStr = "Scenes/Start From 1st Build Scene";

        private static bool playFromFirstScene
        {
            get { return EditorPrefs.GetBool(playFromFirstMenuStr, true); }
            set { EditorPrefs.SetBool(playFromFirstMenuStr, value); }
        }

        [MenuItem(playFromFirstMenuStr, false, 150)]
        private static void PlayFromFirstSceneCheckMenu()
        {
            playFromFirstScene = !playFromFirstScene;
            Menu.SetChecked(playFromFirstMenuStr, playFromFirstScene);

            ShowNotifyOrLog(playFromFirstScene ? "Starting from 1st Build Scene" : "Starting from open scene");
        }

        // The menu won't be gray out, we use this validate method for update check state
        [MenuItem(playFromFirstMenuStr, true)]
        private static bool PlayFromFirstSceneCheckMenuValidate()
        {
            Menu.SetChecked(playFromFirstMenuStr, playFromFirstScene);
            return true;
        }

        // This method is called before any Awake. It's the perfect callback for this feature
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void LoadFirstSceneAtGameBegins()
        {
            if (!playFromFirstScene)
                return;

            if (EditorBuildSettings.scenes.Length <= 0)
            {
                Debug.LogWarning("The scene build list is empty. Can't play from 1st Build Scene.");
                return;
            }

            // Remove All Objects before loading the first scene
            Object[] allObjects = Object.FindObjectsOfType<Object>();
            foreach (Object obj in allObjects)
                Object.Destroy(obj);

            SceneManager.LoadScene(0);
        }

        private static void ShowNotifyOrLog(string msg)
        {
            if (Resources.FindObjectsOfTypeAll<SceneView>().Length > 0)
                EditorWindow.GetWindow<SceneView>().ShowNotification(new GUIContent(msg));
            else
                Debug.Log(msg); // When there's no scene view opened, we just print a log
        }
    }
}