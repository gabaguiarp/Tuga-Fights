using AYellowpaper.SerializedCollections;
using MemeFight.UI.Popups;
using System;
using UnityEngine;
using UnityEngine.Localization;

namespace MemeFight.UI
{
    public class BonusRewardsPopupTrigger : MonoBehaviour
    {
        [Serializable]
        struct RewardPopupDisplayConfiguration
        {
            public LocalizedString textString;
            public Sprite displayImage;
        }

        [SerializeField]
        SerializedDictionary<RewardID, RewardPopupDisplayConfiguration> _rewardDisplayConfigurations = new SerializedDictionary<RewardID, RewardPopupDisplayConfiguration>();

        [Space(10)]
        [SerializeField] BonusModalWindowTrigger _modalTrigger;

        [field: SerializeField, ReadOnly]
        public bool IsPopupOpen { get; private set; } = false;

        void Reset()
        {
            if (_modalTrigger == null && !TryGetComponent(out _modalTrigger))
            {
                _modalTrigger = gameObject.AddComponent<BonusModalWindowTrigger>();
            }
        }

        void Awake()
        {
            _modalTrigger.OnPopupClosed += HandlePopupClosed;
        }

        void HandlePopupClosed() => IsPopupOpen = false;

        public void TriggerPopupForReward(RewardID reward)
        {
            if (_rewardDisplayConfigurations.ContainsKey(reward))
            {
                _modalTrigger.displayTextString = _rewardDisplayConfigurations[reward].textString;
                _modalTrigger.imageToDisplay = _rewardDisplayConfigurations[reward].displayImage;

                if (_modalTrigger.OpenWindow())
                {
                    IsPopupOpen = true;
                }
            }
            else
            {
                Debug.LogError("Unable to find a valid key to display reward popup!");
            }
        }
    }
}
