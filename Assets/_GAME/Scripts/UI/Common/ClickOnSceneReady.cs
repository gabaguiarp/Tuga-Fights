using UnityEngine;

namespace MemeFight.UI
{
    /// <summary>
    /// Triggers a click on the specified button as soon as the scene is activated.
    /// </summary>
    public class ClickOnSceneReady : ManagedBehaviour
    {
        [SerializeField] ButtonUI _buttonToClick;

        protected override void OnSceneReady()
        {
            _buttonToClick.Click();
        }
    }
}
