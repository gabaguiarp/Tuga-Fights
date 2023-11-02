using UnityEngine;
using UnityEngine.EventSystems;

namespace MemeFight.Menus
{
    public class MenuSelectionHandler
    {
        public static void Select(GameObject selection)
        {
            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(selection);
            }
            else
            {
                Debug.LogWarning($"Unable to select '{selection.name}' because no EventSystem was found!");
            }
        }

        public static void ClearSelection()
        {
            if (EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
