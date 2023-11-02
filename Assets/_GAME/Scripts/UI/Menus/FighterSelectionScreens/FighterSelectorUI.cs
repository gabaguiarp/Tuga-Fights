using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MemeFight.UI
{
    using Settings;

    public class FighterSelectorUI : MenuScreenUI
    {
        [SerializeField] ButtonUI _backButton;

        [Header("Display Settings")]
        [SerializeField] PlayerColorsSO _playerColors;
        [SerializeField] SelectionDisplay[] _selectionDisplays = new SelectionDisplay[2];

        [Space(10)]
        public UnityEvent onSelectionValidated;

        public event UnityAction<Team, int> OnFighterSelected;

        [System.Serializable]
        public struct SelectionDisplay
        {
            public FighterRosterPanelUI rosterPanel;

            [SerializeField] FighterSelectionDisplayUI _display;

            public void SetDisplayFrameColor(Color color) => _display.SetFrameColor(color);

            public void SetFighterDisplayData(Sprite avatar, string fighterName, bool animate = false)
            {
                _display.UpdateDisplay(avatar, fighterName, animate);
            }
        }

        protected override void Awake()
        {
            base.Awake();

            InputManager.OnJoiningEnabled += HandleJoiningEnabled;
            InputManager.OnJoiningDisabled += HandleJoiningDisabled;

            _selectionDisplays.ForEach(s => s.rosterPanel.OnSlotClicked += HandleFighterSelected);

            _backButton.OnClicked += Close;

            //ConfigureDisplays();
        }

        void ConfigureDisplays()
        {
            for (int i = 0; i < _selectionDisplays.Length; i++)
            {
                _selectionDisplays[i].SetDisplayFrameColor(_playerColors.GetColorForPlayer((Player)i+1));
            }
        }

        protected override void OnBackCommand()
        {
            _backButton.Click();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            InputManager.OnJoiningEnabled -= HandleJoiningEnabled;
            InputManager.OnJoiningDisabled -= HandleJoiningDisabled;
        }

        #region UI Elements Control
        public void PopulateSlots(Dictionary<Team, List<FighterProfileSO>> fighters)
        {
            foreach (SelectionDisplay display in _selectionDisplays)
            {
                display.rosterPanel.ClearSlots();
            }

            foreach (var kvp in fighters)
            {
                _selectionDisplays.ForEach(d => d.rosterPanel.PopulateSlotsForTeam(kvp.Key, kvp.Value));
            }
        }

        public void SetSlotsTeamForPlayer(Player player, TeamDataSO team)
        {
            GetDisplayForPlayer(player).rosterPanel.DisplaySlotsForTeam(team.Label);
            GetDisplayForPlayer(player).rosterPanel.SetTeamName(team.Name);
        }

        public void EnablePanelForPlayer(Player player, bool enable, bool selectFirstOnEnable = true)
        {
            SelectionDisplay display = GetDisplayForPlayer(player);
            display.rosterPanel.SetEnabled(enable);

            if (enable && selectFirstOnEnable)
                display.rosterPanel.SelectSlot(0);
        }

        public void DisableAllPanels()
        {
            _selectionDisplays.ForEach(d => d.rosterPanel.SetInteractable(false));
        }

        public void NotifySelectionValidated()
        {
            DisableAllPanels();
            _backButton.gameObject.SetActive(false);
            onSelectionValidated.Invoke();
        }
        #endregion

        #region Selection Control
        public void SetDisplayedFighterForPlayer(Player player, FighterProfileSO fighterProfile, bool animate = false)
        {
            GetDisplayForPlayer(player).SetFighterDisplayData(fighterProfile.Avatar, fighterProfile.Name, animate);
        }

        public void SetDisplayedFighterForPlayer(Player player, Team fighterTeam, int fighterIndex, bool animate = false)
        {
            FighterProfileSO profile = ResourcesManager.Fighters.GetFighterByIndex(fighterTeam, fighterIndex);
            SetDisplayedFighterForPlayer(player, profile, animate);
        }

        public int SelectRandomFighterForPlayer(Player player)
        {
            return GetDisplayForPlayer(player).rosterPanel.SelectRandom();
        }

        public void HighlightSlotForPlayer(Player player, int slotIndex, bool highlight = true)
        {
            GetDisplayForPlayer(player).rosterPanel.HighlightSlot(slotIndex, highlight);
        }

        public void ResetAllDisplays()
        {
            _selectionDisplays.ForEach(d => d.rosterPanel.TurnOffSlotsBlinking());
        }
        #endregion

        #region Event Responders
        void HandleJoiningEnabled()
        {
            _selectionDisplays.ForEach(d => d.rosterPanel.SetJoiningState(true));
        }

        void HandleJoiningDisabled()
        {
            _selectionDisplays.ForEach(d => d.rosterPanel.SetJoiningState(false));
        }

        void HandleFighterSelected(Team team, int slotIndex)
        {
            try
            {
                OnFighterSelected?.Invoke(team, slotIndex);
            }
            catch
            {
                Debug.LogError($"Unable to select fighter with index {slotIndex} because it is out of bounds!");
            }
        }
        #endregion

        SelectionDisplay GetDisplayForPlayer(Player player) => _selectionDisplays[(int)player - 1];
    }
}
