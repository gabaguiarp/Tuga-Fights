using UnityEngine;

namespace MemeFight
{
    public enum RewardID
    {
        BONUS_FIGHTERS_BATATOON,
        BONUS_FIGHTERS_HERMAN_LILI
    }

    public class RewardSystem
    {
        /// <summary>
        /// Registers the reward immediately in the <see cref=" ResourcesManager"/>.
        /// </summary>
        public static void ClaimReward(RewardID id)
        {
            switch (id)
            {
                case RewardID.BONUS_FIGHTERS_BATATOON:
                    ResourcesManager.PersistentData.AddFighterBundle(FightersBundleID.BATATOON);
                    break;
            }
        }

        /// <summary>
        /// Unlocks the reward by registering it in the Persistent Data and claiming it, while also saving.
        /// </summary>
        /// <param name="id">The ID of the reward to unlock.</param>
        /// <param name="refreshResources">Whether the <see cref="ResourcesManager"/> should be refreshed, so any database
        /// changes caused by this reward can be taken into account, including, for instance, unlocked bonus fighters.</param>
        /// <param name="saveData">Whether to save the current persistent data to a file.</param>
        public static void UnlockReward(RewardID id, bool refreshResources = true, bool saveData = true)
        {
            ResourcesManager.PersistentData.UnlockReward(id);
            ClaimReward(id);

            Debug.Log("Reward unlocked: " + id.ToString());

            if (refreshResources)
                ResourcesManager.Refresh();

            if (saveData)
                SaveSystem.SaveData();
        }
    }
}
