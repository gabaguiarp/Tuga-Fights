using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace MemeFight.EditorTools
{
    /// <summary>
    /// This script is executed right before a new build is started, in order to run some required behaviour.
    /// </summary>
    public class GameBuildProcessor : IPreprocessBuildWithReport
    {
        public int callbackOrder { get { return 0; } }

        public void OnPreprocessBuild(BuildReport report)
        {
            Debug.Log("OnPreprocessBuild called");
            ClearPersistentData();
        }

        void ClearPersistentData()
        {
            var data = Resources.Load("PersistentData") as PersistentDataSO;
            if (data != null)
            {
                data.Clear();
            }
            else
            {
                Debug.LogError("Persistent Data could not be found!");
            }
        }
    }
}
