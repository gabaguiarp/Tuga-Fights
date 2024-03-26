using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static MemeFight.FightersRosterSO;

namespace MemeFight
{
    public enum Team { Azeite, Bacalhau }

    public class FightersDatabase
    {
        /// <summary>Contains references to the fighters, separated by team.</summary>
        public Dictionary<Team, List<FighterProfileSO>> Roster { get; private set; }

        Dictionary<Team, TeamDataSO> _teamsIndexer;

        public FightersDatabase(FightersRosterSO roster, PersistentDataSO persistentData)
        {
            Init(roster, persistentData);
        }

        void Init(FightersRosterSO roster, PersistentDataSO persistentData)
        {
            // Register main fighters
            Roster = new Dictionary<Team, List<FighterProfileSO>>();
            roster.Fighters.ForEach(f => RegisterFighter(f));

            // Register bonus fighters
            foreach (var bundle in roster.BonusFighters)
            {
                if (persistentData.ContainsBundle(bundle.ID))
                {
                    var fighters = roster.GetBonusFighters(bundle.ID);
                    fighters.ForEach(f => RegisterFighter(f));
                }
            }

            // Populate the teams indexer with the data containers for each team
            _teamsIndexer = new Dictionary<Team, TeamDataSO>();
            foreach (Team value in Enum.GetValues(typeof(Team)))
            {
                TeamDataSO data = roster.Teams.FirstOrDefault(t => t.Label.Equals(value));
                if (data != null)
                    _teamsIndexer.Add(value, data);
            }

            Debug.Log("Fighters Database generated");
        }

        void RegisterFighter(FighterProfileSO fighter)
        {
            // Register each fighter, while pairing them with the corresponding team
            if (!Roster.ContainsKey(fighter.Team))
                Roster.Add(fighter.Team, new List<FighterProfileSO>());

            Roster[fighter.Team].Add(fighter);
        }

        public FighterProfileSO[] GetFightersForTeam(Team team)
        {
            if (Roster.ContainsKey(team))
                return Roster[team].ToArray();
            else
                throw new Exception($"Team {team} not found in the database. Failed to get fighters!");
        }

        public TeamDataSO GetTeamData(Team team)
        {
            if (_teamsIndexer.ContainsKey(team))
                return _teamsIndexer[team];
            else
                throw new Exception($"Unable to get data for team {team} because it wasn't registered in the database!");
        }

        public FighterProfileSO GetFighterByIndex(Team fighterTeam, int index)
        {
            try
            {
                return Roster[fighterTeam][index];
            }
            catch (Exception e)
            {
                throw new Exception($"Unable to get fighter with index {index} for team {fighterTeam}: " + e);
            }
        }

        public FighterProfileSO GetRandomFighterForTeam(Team team)
        {
            int randomIndex = Randomizer.GetRandom(0, Roster[team].Count, true);
            return Roster[team][randomIndex];
        }

        public int GetTotalFightersForTeam(Team team) => Roster[team].Count;

        /// <summary>
        /// Returns the team that acts as an opponent of <paramref name="myTeam"/>.
        /// </summary>
        public static Team GetOpposingTeam(Team myTeam)
        {
            return myTeam == Team.Azeite ? Team.Bacalhau : Team.Azeite;
        }
    }

    public enum FightersBundleID
    {
        BATATOON,
        HERMAN_LILI,
    }
}
