using UnityEngine;

namespace MemeFight
{
    [CreateAssetMenu(menuName = MenuPaths.Quests + "Quest Line")]
    public class QuestLineSO : CollectionSO<QuestSO>
    {
        [Space(10)]
        [SerializeField] RewardID _reward;

        public RewardID Reward => _reward;
    }
}
