using UnityEngine.Events;

namespace MemeFight
{
    public enum StatID
    {
        BACALAHU_TEAM_WIN,
        AZEITE_TEAM_WIN,
        FREE_FIGHT_WIN,
        MATCH_WIN,
        AGATA_WIN,
        ANAMALHOA_WIN,
        BAIAO_WIN,
        CARLOSCOSTA_WIN,
        JCB_WIN,
        CLAUDIORAMOS_WIN,
        GISELA_WIN,
        GOUCHA_WIN,
        JULIAPINHEIRO_WIN,
        LUCY_WIN,
        MALATO_WIN,
        MARIALEAL_WIN,
        MARIAVIEIRA_WIN,
        SANDRA_WIN,
        TERESAGUILHERME_WIN,
        TOY_WIN,
        BATATINHA_WIN,
        COMPANHIA_WIN
    }

    public class Stats
    {
        /// <summary>
        /// Informs that a stat has been updated, while broadcasting the <see cref="StatID"/> and the amount by which
        /// the stat was updated.
        /// </summary>
        public static event UnityAction<StatID, int> OnStatUpdated;

        public static void RegisterStat(StatID statID, int amount)
        {
            int value = ResourcesManager.PersistentData.AddStat(statID, amount);
            OnStatUpdated?.Invoke(statID, amount);
        }
    }
}
