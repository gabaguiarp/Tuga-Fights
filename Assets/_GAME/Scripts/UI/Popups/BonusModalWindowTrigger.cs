using UnityEngine;

namespace MemeFight.UI.Popups
{
    public class BonusModalWindowTrigger : ModalWindowTrigger
    {
        public Sprite imageToDisplay;

        protected override void OnOpenWindowCallback()
        {
            PopupsManager.Instance.DisplayBonusWindow(this);
        }
    }
}
