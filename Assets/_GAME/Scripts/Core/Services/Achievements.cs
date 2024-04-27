#if UNITY_ANDROID
using GooglePlayGames;
#endif
using UnityEngine;

namespace MemeFight.Services
{
    public enum Achievement
    {
        CAMPAIGN_BACALHAU_WIN,
        CAMPAIGN_AZEITE_WIN,
        GISELA_WINS,
        LUCY_WINS,
        CARLOSCOSTA_WINS,
        JCB_WINS,
        AGATA_WINS,
        MALATO_WINS,
        GOUCHA_WINS,
        JULIAPINHEIRO_WINS,
        SANDRA_WINS,
        ANAMALHOA_WINS,
        CLAUDIORAMOS_WINS,
        MARIALEAL_WINS,
        TOY_WINS,
        MARIAVIEIRA_WINS,
        BAIAO_WINS,
        TERESAGUILHERME_WINS,
        PERFECT_WIN,
        QUESTLINE_COMPLETE,
        BATATOON_MATCH,
        BATATINHA_WINS,
        COMPANHIA_WINS,
        HERMAN_WINS,
        LILI_WINS
    }

    public class Achievements
    {
        /// <summary>
        /// Unlocks the given <paramref name="achievement"/><br></br>
        /// This will be ignored if an achievement marked as "Incremental" in the Google Play Console
        /// is passed, since it is only possible to unlock an incremental achievement by making its
        /// count reach the "steps required" amount.
        /// </summary>
        /// <param name="achievement"></param>
        public static void Unlock(Achievement achievement)
        {
            UnlockAchievementInternal(GetAchievementID(achievement));
        }

        /// <summary>
        /// Increments the given <paramref name="achievement"/>'s count by <paramref name="amount"/>.
        /// This only applies to achievements marked as "Incremental" in the Google Play Console.<br></br>
        /// Note that, once the count reaches the "steps required" amount, the achievement will be
        /// automatically unlocked.
        /// </summary>
        public static void Increment(Achievement achievement, int amount)
        {
            IncrementAchievementInternal(GetAchievementID(achievement), amount);
        }

        /// <summary>
        /// Reveals a previously hidden achievement.
        /// </summary>
        public static void Reveal(Achievement achievement)
        {
            RevealAchievementInternal(GetAchievementID(achievement));
        }

        #region Internal Callbacks
        static void UnlockAchievementInternal(string achievementId)
        {
            try
            {
#if UNITY_ANDROID || UNITY_IOS
                PlayGamesPlatform.Instance.UnlockAchievement(achievementId, (isSuccess) =>
                                                             HandleAchievementResult(isSuccess, achievementId));
#endif
            }
            catch (System.Exception e)
            {
                Debug.LogError("Unable to unlock achievement! " + e);
            }
        }

        static void IncrementAchievementInternal(string achievementId, int amount)
        {
            try
            {
#if UNITY_ANDROID || UNITY_IOS
                PlayGamesPlatform.Instance.IncrementAchievement(achievementId, amount, (isSuccess) =>
                                                                HandleAchievementIncrementResult(isSuccess, achievementId, amount));
#endif
            }
            catch (System.Exception e)
            {
                Debug.LogError("Unable to increment achievement! " + e);
            }
        }

        static void RevealAchievementInternal(string achievementId)
        {
            try
            {
#if UNITY_ANDROID || UNITY_IOS
                PlayGamesPlatform.Instance.RevealAchievement(achievementId, (isSuccess) =>
                                                             HandleAchievementRevealResult(isSuccess, achievementId));
#endif
            }
            catch (System.Exception e)
            {
                Debug.LogError("Unable to reveal achievement! " + e);
            }
        }
        #endregion

        #region Result Callbacks
        static void HandleAchievementResult(bool isSuccess, string achievementID)
        {
            if (isSuccess)
            {
                Debug.LogFormat("Achievement '{0}' successfully unlocked!", achievementID);
            }
            else
            {
                Debug.LogWarningFormat("Unable to unlock achievement '{0}'", achievementID);
            }
        }

        static void HandleAchievementIncrementResult(bool isSuccess, string achievementID, int amount)
        {
            if (isSuccess)
            {
                Debug.LogFormat("Achievement '{0}' successfully incremented by {1}!", achievementID, amount);
            }
            else
            {
                Debug.LogWarningFormat("Unable to increment achievement '{0}'", achievementID);
            }
        }

        static void HandleAchievementRevealResult(bool isSuccess, string achievementID)
        {
            if (isSuccess)
            {
                Debug.LogFormat("Achievement '{0}' successfully revealed!", achievementID);
            }
            else
            {
                Debug.LogWarningFormat("Unable to reveal achievement '{0}'", achievementID);
            }
        }
        #endregion

        static string GetAchievementID(Achievement achievement)
        {
            switch (achievement)
            {
                case Achievement.CAMPAIGN_BACALHAU_WIN:
                    return GPGSIds.achievement_o_bacalhau_quer_alho;
                case Achievement.CAMPAIGN_AZEITE_WIN:
                    return GPGSIds.achievement_oliveirinha_da_serra;
                case Achievement.GISELA_WINS:
                    return GPGSIds.achievement_deite_na_cara_e_doute_na_tromba_toda;
                case Achievement.LUCY_WINS:
                    return GPGSIds.achievement_nossa_que_biolncia;
                case Achievement.CARLOSCOSTA_WINS:
                    return GPGSIds.achievement_mentira;
                case Achievement.JCB_WINS:
                    return GPGSIds.achievement_olha_a_parva_olha_a_pindrica;
                case Achievement.AGATA_WINS:
                    return GPGSIds.achievement_sai_da_minha_vida;
                case Achievement.MALATO_WINS:
                    return GPGSIds.achievement_malat;
                case Achievement.GOUCHA_WINS:
                    return GPGSIds.achievement_uma_salva_de_palmas;
                case Achievement.JULIAPINHEIRO_WINS:
                    return GPGSIds.achievement_querida_jlia;
                case Achievement.SANDRA_WINS:
                    return GPGSIds.achievement_tens_cara_de_estpida_e_de_parva;
                case Achievement.ANAMALHOA_WINS:
                    return GPGSIds.achievement_turbinada;
                case Achievement.CLAUDIORAMOS_WINS:
                    return GPGSIds.achievement_que_prca_p;
                case Achievement.MARIALEAL_WINS:
                    return GPGSIds.achievement_aqui_s_pra_ti;
                case Achievement.TOY_WINS:
                    return GPGSIds.achievement_toda_a_noite;
                case Achievement.MARIAVIEIRA_WINS:
                    return GPGSIds.achievement_seus_vendidos;
                case Achievement.BAIAO_WINS:
                    return GPGSIds.achievement_um_grande_beijinho;
                case Achievement.TERESAGUILHERME_WINS:
                    return GPGSIds.achievement_ol_a_em_casa;
                case Achievement.PERFECT_WIN:
                    return GPGSIds.achievement_intocvel;
                case Achievement.QUESTLINE_COMPLETE:
                    return GPGSIds.achievement_conquistador;
                case Achievement.BATATOON_MATCH:
                    return GPGSIds.achievement_beijinhos_abraos_e_muitos_palhaos;
                case Achievement.BATATINHA_WINS:
                    return GPGSIds.achievement_chama_o_batatinha;
                case Achievement.COMPANHIA_WINS:
                    return GPGSIds.achievement_convida_o_companhia;
                case Achievement.HERMAN_WINS:
                    return GPGSIds.achievement_no_havia_necessidade;
                case Achievement.LILI_WINS:
                    return GPGSIds.achievement_no_me_chamem_maria_alice;

                default:
                    Debug.LogWarningFormat("Achievement {0} does not have a matching ID!", achievement);
                    return string.Empty;
            }
        }
    }
}
