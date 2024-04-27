using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace MemeFight
{
    public class QuestSystem
    {
        public class QuestData
        {
            public QuestData(string id, LocalizedString description, int current, int total, bool isComplete)
            {
                ID = id;
                DescriptionString = description;
                CurrentValue = current;
                TotalValue = total;
                IsComplete = isComplete;
            }

            public string ID { get; private set; }
            public LocalizedString DescriptionString { get; private set; }
            public int CurrentValue { get; private set; }
            public int TotalValue { get; private set; }
            public bool IsComplete { get; private set; }
        }

        public static void Init()
        {
            ResourcesManager.PersistentData.QuestProgressPercentage = GetCompletionPercentage();
        }

        public static QuestData[] GetAllQuests()
        {
            QuestLineSO gameQuests = ResourcesManager.ActiveQuestline;

            if (gameQuests == null)
            {
                throw new System.Exception("Failed to get game quests, because they were not assigned in the ResourcesManager! " +
                    "Make sure you only call this function after the ResourcesManager is correclty initialized.");
            }

            QuestData[] quests = new QuestData[gameQuests.Total];

            for (int i = 0; i < quests.Length; i++)
            {
                QuestSO quest = gameQuests.items[i];
                quests[i] = new QuestData(quest.ID, quest.DescriptionString, GetQuestValue(quest), quest.AmountRequired,
                                          IsQuestComplete(quest));
            }

            return quests;
        }

        public static bool IsQuestComplete(QuestSO quest)
        {
            if (ResourcesManager.PersistentData.GetStat(quest.StatID, out StatData stat))
            {
                return stat.Value >= quest.AmountRequired;
            }

            return false;
        }

        public static int GetQuestValue(QuestSO quest)
        {
            if (ResourcesManager.PersistentData.GetStat(quest.StatID, out StatData stat))
            {
                return stat.Value;
            }

            return 0;
        }

        public static float GetCompletionPercentage()
        {
            int totalAmount = 0;
            int completedAmount = 0;

            foreach (QuestSO quest in ResourcesManager.ActiveQuestline.items)
            {
                totalAmount += quest.AmountRequired;
                completedAmount += Mathf.Clamp(GetQuestValue(quest), 0, quest.AmountRequired);
                // NOTE: We avoid adding values that exceed the required amount for completing the quest
            }

            if (totalAmount == 0)
                return 0;
            
            return (float)completedAmount / (float)totalAmount;
        }

        public static List<RewardID> GetUnlockedBonusRewards()
        {
            List<RewardID> unlockedRewards = new List<RewardID>();

            foreach (var questline in ResourcesManager.BonusQuestLines)
            {
                if (ResourcesManager.PersistentData.WasRewardUnlocked(questline.Reward))
                    continue;

                int completedQuests = 0;
                foreach (var item in questline.items)
                {
                    if (GetQuestValue(item) >= item.AmountRequired)
                        completedQuests++;
                }

                if (completedQuests >= questline.items.Length)
                {
                    unlockedRewards.Add(questline.Reward);
                }
            }

            return unlockedRewards;
        }
    }
}
