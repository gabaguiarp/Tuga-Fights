using System.Collections.Generic;
using UnityEngine;

namespace MemeFight
{
    [CreateAssetMenu(fileName = "CampaignStream", menuName = MenuPaths.Data + "Campaign Stream")]
    public class CampaignStreamSO : BaseSO
    {
        [Space(10)]
        [SerializeField] List<MatchConfigurationSO> _matches = new List<MatchConfigurationSO>();
        [Space(10)]
        [SerializeField] List<BonusMatch> _bonusMatches = new List<BonusMatch>();

        public IReadOnlyList<MatchConfigurationSO> Matches => _matches.AsReadOnly();
        public IReadOnlyList<BonusMatch> BonusMatches => _bonusMatches.AsReadOnly();

        [System.Serializable]
        public struct BonusMatch
        {
            [field: SerializeField]
            public FightersBundleID RequiredBundle;

            [field: SerializeField]
            public MatchConfigurationSO Match;
        }
    }
}
