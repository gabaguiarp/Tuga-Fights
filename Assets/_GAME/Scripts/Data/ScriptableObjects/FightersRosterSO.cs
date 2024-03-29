using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;

namespace MemeFight
{
    [CreateAssetMenu(menuName = "Core/Data/Fighters Roster")]
    public class FightersRosterSO : ScriptableObject
    {
        [SerializeField] List<TeamDataSO> _teams;
        [SerializeField] List<FighterProfileSO> _fighters;
        [SerializeField] List<BonusFightersBundle> _bonusFighters;

        public TeamDataSO[] Teams => _teams.ToArray();
        public FighterProfileSO[] Fighters => _fighters.ToArray();
        public IReadOnlyList<BonusFightersBundle> BonusFighters => _bonusFighters.AsReadOnly();
        public int TotalFighters
        {
            get
            {
                int bonusCount = 0;
                foreach (var bundle in _bonusFighters)
                {
                    bonusCount += bundle.FightersCount;
                }

                return _bonusFighters.Count + bonusCount;
            }
        }

        public bool ContainsBonusID(FightersBundleID id) => _bonusFighters.Exists(b => b.ID.Equals(id));
        public IReadOnlyList<FighterProfileSO> GetBonusFighters(FightersBundleID bundleID)
        {
            try
            {
                var bundle = _bonusFighters.FirstOrDefault(b => b.ID.Equals(bundleID));
                return bundle.Fighters;
            }
            catch (Exception e)
            {
                throw new Exception($"Unable to get Bonus Fighters bundle with ID " + bundleID, e);
            }
        }

        [Serializable]
        public class BonusFightersBundle
        {
            [field: SerializeField]
            public FightersBundleID ID { get; private set; }
            [field: SerializeField]
            public LocalizedString UnlockMessageString { get; private set; }

            [SerializeField] List<FighterProfileSO> _fighters;

            public IReadOnlyList<FighterProfileSO> Fighters => _fighters.AsReadOnly();

            public int FightersCount => _fighters.Count;
        }
    }
}
