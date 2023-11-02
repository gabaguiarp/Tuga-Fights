using UnityEngine;
using UnityEngine.Localization;

namespace MemeFight
{
    [CreateAssetMenu(fileName = "Quest", menuName = MenuPaths.Quests + "Quest")]
    public class QuestSO : ScriptableObject
    {
        [field: SerializeField]
        public string ID { get; private set; }

        [field: SerializeField]
        public LocalizedString DescriptionString { get; private set; }

        [field: SerializeField]
        public StatID StatID { get; private set; }

        [field: SerializeField]
        public int AmountRequired { get; private set; }
    }
}
