using System;
using System.Collections.Generic;
using UnityEngine;

namespace MemeFight
{
    /// <summary>
    /// Contains all the data that can be serialized onto a save file, so it can be carried through multiple game sessions.
    /// </summary>
    [Serializable]
    public class PlayerData
    {
        [Serializable]
        public class QuestProgress
        {
            public QuestProgress(string id, bool isComplete)
            {
                ID = id;
                IsComplete = isComplete;
            }

            public string ID;
            public bool IsComplete;
        }

        public Profile profile = new Profile();
        [Space(10)]
        public Settings settings = new Settings();
        [Space(10)]
        public List<StatData> stats = new List<StatData>();

        [Serializable]
        public class Profile
        {
            [Tooltip("The team selected by the main player. In a multiplayer scenario this still applies, since player 2 will " +
                "automatically be assigned the opposing team.")]
            public Team selectedTeam = Team.Bacalhau;
            public List<QuestProgress> questProgress = new List<QuestProgress>();
            public List<RewardID> unlockedRewards = new List<RewardID>();
            public int activeQuestlineIndex = 0;
        }

        [Serializable]
        public class Settings
        {
            public bool music = true;
            public bool sfx = true;
            public bool cameraNoise = true;
        }
    }
}
