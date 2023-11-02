using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace MemeFight.UI.Popups
{
    public class CampaignWinnerPopupHandler : MonoBehaviour
    {
        [SerializeField] TeamDataSO _bacalhauTeamData;
        [SerializeField] TeamDataSO _azeiteTeamData;
        [SerializeField] ModalWindowTrigger _windowTrigger;

        readonly string WinnerTeamStringKey = "winnerTeam";

        /// <summary>
        /// Shows the popup in game.
        /// </summary>
        public void TriggerPopup(Team winnerTeam)
        {
            var args = new Dictionary<string, string> { [WinnerTeamStringKey] = GetStringForTeam(winnerTeam).GetLocalizedString() };
            _windowTrigger.displayTextString.Arguments = new object[] { args };
            _windowTrigger.OpenWindow();
        }

        LocalizedString GetStringForTeam(Team team)
        {
            return team.Equals(Team.Bacalhau) ? _bacalhauTeamData.Name : _azeiteTeamData.Name;
        }

#if UNITY_EDITOR
        [ContextMenu("Trigger Bacalhau Team Win Popup")]
        void TriggerPopupNoAttempts() => ForceTriggerPopup(Team.Bacalhau);

        [ContextMenu("Trigger Azeite Team Win Popup")]
        void TriggerPopupOneAttempt() => ForceTriggerPopup(Team.Azeite);

        void ForceTriggerPopup(Team winnerTeam)
        {
            if (Application.isPlaying)
            {
                TriggerPopup(winnerTeam);
            }
        }
#endif
    }
}
