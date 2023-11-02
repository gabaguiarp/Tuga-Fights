using UnityEngine;
using UnityEngine.Localization;

namespace MemeFight.Menus
{
    [CreateAssetMenu(fileName = "TutorialPage", menuName = MenuPaths.Tutorial + "Tutorial Page")]
    public class TutorialPageSO : ScriptableObject
    {
        [field: SerializeField, SpritePreview]
        public Sprite Image { get; private set; }

        [field: SerializeField]
        public LocalizedString InstructionsString { get; private set; }
    }
}
