using System.Collections.Generic;
using UnityEngine;

namespace MemeFight.UI
{
    public class TeamSelectorUI : MenuScreenUI
    {
        [SerializeField] FighterRosterPanelUI _rosterPanel;
        [SerializeField] ButtonUI _backButton;

        protected override void Awake()
        {
            base.Awake();
            _rosterPanel.SetInteractable(false);
            _backButton.OnClicked += Close;
        }

        protected override void OnBackCommand()
        {
            _backButton.Click();
        }

        public void PopulateSlots(Dictionary<Team, List<FighterDisplayData>> fightersDisplayData)
        {
            _rosterPanel.ClearSlots();

            foreach (var kvp in fightersDisplayData)
            {
                _rosterPanel.PopulateSlotsForTeam(kvp.Key, kvp.Value);
            }
        }

        public void SetSlotsTeam(TeamDataSO team, bool animate = false)
        {
            _rosterPanel.DisplaySlotsForTeam(team.Label);
            _rosterPanel.SetTeamName(team.Name);

            if (animate)
            {
                _rosterPanel.AnimateDisplay();
            }
        }
    }
}
