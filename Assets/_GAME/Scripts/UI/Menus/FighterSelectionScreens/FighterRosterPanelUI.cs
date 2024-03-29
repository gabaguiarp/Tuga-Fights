using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using DG.Tweening;
using FighterUnlockStateInfo = MemeFight.UI.FighterDisplayData.FighterUnlockStateInfo;

namespace MemeFight.UI
{
    using Menus;

    public class FighterRosterPanelUI : PlayerSelectionPanelUI
    {
        [Header("Dimensions")]
        [SerializeField] float _defaultWidth = 550f;
        [SerializeField] float _largeWidth = 680f;

        [Header("Core Components")]
        [SerializeField] FighterSlotUI _fighterSlotPrefab;
        [SerializeField] Transform _slotsRoot;
        [SerializeField] GameObject _teamSlotsGroupPrefab;
        [SerializeField] LocalizedTextSmartUI _teamNameLabel;

        [Header("Overlay")]
        [SerializeField] GameObject _disabledOverlay;
        [SerializeField] GameObject _joinPrompt;

        [Header("References")]
        [SerializeField] RectTransform _rect;
        [SerializeField] CanvasGroup _canvasGroup;
        [SerializeField] Sprite _lockedFighterIcon;

        [Header("Audio")]
        [SerializeField] AudioCueSO _slotClickCue;

        Vector3 _defaultScale;
        bool _wasAnySlotSelected = false;
        /// <summary>The team which the currently displayed slots belong to.</summary>
        Team _teamDisplayed;
        Dictionary<Team, SlotsGroup> _slotGroups = new Dictionary<Team, SlotsGroup>();

        AudioManager _audioManager;

        public bool IsPanelEnabled => !_disabledOverlay.activeSelf;
        public bool IsInteractable => _canvasGroup.interactable;

        public event UnityAction<Team, int> OnSlotHighlighted;
        public event UnityAction<Team, int, FighterUnlockStateInfo> OnSlotClicked;

        /// <summary>
        /// The maximum amount of slots accountable for the panel to retain the default size.
        /// </summary>
        const int MaxSlotsDefaultSize = 8;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (_audioManager == null)
                _audioManager = AudioManager.Instance;
        }

        void OnDisable()
        {
            _wasAnySlotSelected = false;
        }

        protected override void Awake()
        {
            base.Awake();
            _defaultScale = _rect.localScale;
        }

        void SetLargeSize(bool isLarge)
        {
            Vector2 adjustedSize = _rect.sizeDelta;
            adjustedSize.x = isLarge ? _largeWidth : _defaultWidth;
            _rect.sizeDelta = adjustedSize;
        }

        public void PopulateSlotsForTeam(Team team, List<FighterDisplayData> displayData, bool includeLocked = false)
        {
            SlotsGroup teamSlotsGroup = GetTeamSlotsGroup(team);
            Color slotColor = _playerColors.GetColorForPlayer(_player);

            // Cached variables
            Sprite fighterThumbnail = _lockedFighterIcon;

            int displayedSlotsCount = 0;
            for (int i = 0; i < displayData.Count; i++)
            {
                if (!displayData[i].State.IsUnlocked && !includeLocked)
                    continue;

                int slotIndex = i;  // We need to store the current index in a new variable, in order to pass it as an exclusive slot
                                    // index to the event

                var slot = Instantiate(_fighterSlotPrefab, teamSlotsGroup.Root);
                var state = displayData[i].State;
                fighterThumbnail = state.IsUnlocked ? displayData[i].Profile.Thumbnail : _lockedFighterIcon;
                slot.name = "Slot_" + displayData[i].Profile.Name;
                slot.Configure(fighterThumbnail, slotColor);
                slot.OnClicked += () => HandleSlotClickedEvent(team, slotIndex, state);

                teamSlotsGroup.Slots.Add(slot);

                displayedSlotsCount++;
            }

            if (displayedSlotsCount > MaxSlotsDefaultSize)
            {
                SetLargeSize(true);
            }
        }

        public void DisplaySlotsForTeam(Team team)
        {
            if (!_slotGroups.ContainsKey(team))
            {
                Debug.LogError("No slots available for team " + team);
                return;
            }

            foreach (var kvp in _slotGroups)
            {
                kvp.Value.SetActive(kvp.Key == team);
            }

            _teamDisplayed = team;
        }

        public void SetTeamName(LocalizedString nameString)
        {
            _teamNameLabel.UpdateSmartReferenceString(nameString);
        }

        public void SetEnabled(bool enabled)
        {
            _disabledOverlay.SetActive(!enabled);

            if (!enabled)
                _wasAnySlotSelected = false;
        }

        public void SetJoiningState(bool canJoin)
        {
            _joinPrompt.SetActive(canJoin);
        }

        public void SetInteractable(bool interactable)
        {
            _canvasGroup.interactable = interactable;
        }

        public void SelectSlot(int slotIndex)
        {
            SlotsGroup group = GetTeamSlotsGroup(_teamDisplayed);
            if (group.Slots.IsIndexValid(slotIndex))
            {
                var slot = group.Slots[slotIndex];
                slot.Select();
                MenuSelectionHandler.Select(slot.gameObject);

                _wasAnySlotSelected = true;
            }
        }

        public void HighlightSlot(int slotIndex, bool hightlight = true)
        {
            GetTeamSlotsGroup(_teamDisplayed).HighlightSlot(slotIndex, hightlight);
        }

        public void TurnOffSlotsBlinking()
        {
            foreach (var group in _slotGroups.Values)
            {
                group.TurnOffSlotsBlinking();
            }
        }

        public int SelectRandom()
        {
            return GetTeamSlotsGroup(_teamDisplayed).SelectRandom();
        }

        void HandleSlotClickedEvent(Team team, int slotIndex, FighterUnlockStateInfo unlockState)
        {
            // Prevent the sound from playing when automatically selecting a slot when the panel is enabled
            if (_audioManager != null && _wasAnySlotSelected && IsInteractable)
            {
                _audioManager.PlaySoundUI(_slotClickCue);
            }
            
            OnSlotClicked?.Invoke(team, slotIndex, unlockState);
        }

        /// <summary>
        /// Tries to get the slots group transform for the specified <paramref name="team"/>. If not found, the group will first be created,
        /// then returned.
        /// </summary>
        SlotsGroup GetTeamSlotsGroup(Team team)
        {
            if (!_slotGroups.ContainsKey(team))
            {
                GameObject groupRoot = Instantiate(_teamSlotsGroupPrefab, _slotsRoot);
                groupRoot.name = "Slots_" + team;
                _slotGroups.Add(team, new SlotsGroup(groupRoot.transform));
            }

            return _slotGroups[team];
        }

#if UNITY_EDITOR
        /// <summary>
        /// Adds an empty slot to the main slots root while in the editor.<br></br>
        /// Used for design/test purposes only, in order to preview the appearence of the panel with slots in it.
        /// </summary>
        [ContextMenu("Add Slot")]
        void AddEmptySlot()
        {
            if (Application.isPlaying)
                return;

            var root = _slotsRoot.childCount > 0 ? _slotsRoot.GetChild(0) : _slotsRoot;

            if (_fighterSlotPrefab != null)
            {
                Instantiate(_fighterSlotPrefab, root);
                UnityEditor.EditorUtility.SetDirty(gameObject);
            }
        }
#endif

        [ContextMenu("Clear Slots")]
        public void ClearSlots()
        {
            if (_slotsRoot.childCount == 0)
                return;

            var root = (Application.isPlaying || _slotsRoot.childCount == 0) ? _slotsRoot : _slotsRoot.GetChild(0);
            List<GameObject> slots = new List<GameObject>();

            for (int i = 0; i < root.childCount; i++)
            {
                slots.Add(root.GetChild(i).gameObject);
            }

            if (Application.isPlaying)
            {
                slots.ForEach(s => Destroy(s));
            }
#if UNITY_EDITOR
            else
            {
                slots.ForEach(s => DestroyImmediate(s));
                UnityEditor.EditorUtility.SetDirty(gameObject);
            }
#endif
        }

        [ContextMenu("Animate Display")]
        public void AnimateDisplay()
        {
            if (!Application.isPlaying || !gameObject.activeInHierarchy)
                return;

            if (DOTween.IsTweening(_rect))
            {
                _rect.DOKill(true);
            }

            _rect.localScale = _defaultScale;
            _rect.DOPunchScale(_defaultScale * 0.2f, 0.15f, 8, 0.5f);
        }

        struct SlotsGroup
        {
            public SlotsGroup(Transform groupRoot)
            {
                Root = groupRoot;
                Slots = new List<FighterSlotUI>();
            }

            public Transform Root { get; private set; }
            public List<FighterSlotUI> Slots { get; private set; }

            public void SetActive(bool active)
            {
                Root.gameObject.SetActive(active);
            }

            public int SelectRandom()
            {
                int randomIndex = Randomizer.GetRandom(0, Slots.Count);
                Slots[randomIndex].Select();
                return randomIndex;
            }

            public void HighlightSlot(int slotIndex, bool highlight)
            {
                if (Slots.IsIndexValid(slotIndex))
                {
                    Slots[slotIndex].SetHighlighted(highlight);
                }
                else
                {
                    Debug.LogError($"Unable to highlight slot with index {slotIndex} because it is out of bounds!");
                }
            }

            public void TurnOffSlotsBlinking()
            {
                foreach (var slot in Slots)
                {
                    slot.SetBlinking(false);
                }
            }
        }
    }
}
