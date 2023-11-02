using System.Collections.Generic;
using UnityEngine;

namespace MemeFight
{
    public class CampaignDatabase
    {
        [SerializeField, ReadOnly] List<MatchConfigurationSO> _matches;

        public IReadOnlyList<MatchConfigurationSO> Matches => _matches.AsReadOnly();
        public int MatchCount => _matches.Count;
        public int LastMatchIndex => _matches.LastIndex();

        public CampaignDatabase(CampaignStreamSO campaignStream, PersistentDataSO persistentData)
        {
            Init(campaignStream, persistentData);
        }

        void Init(CampaignStreamSO campaignStream, PersistentDataSO persistentData)
        {
            _matches = new List<MatchConfigurationSO>();

            // Add default campaign matches
            campaignStream.Matches.ForEach(m => _matches.Add(m));

            // Check and add bonus matches
            foreach (var bonusMatch in campaignStream.BonusMatches)
            {
                if (persistentData.ContainsBundle(bonusMatch.RequiredBundle))
                {
                    _matches.Add(bonusMatch.Match);
                }
            }
        }
    }
}
