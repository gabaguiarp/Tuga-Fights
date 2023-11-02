using UnityEngine;

namespace MemeFight.UI
{
    using Settings;

    public class PlayerSelectionPanelUI : MenuUIBase
    {
        [Tooltip("The player that is allowed to interact with the content inside this panel.")]
        [SerializeField] protected Player _player = Player.One;
        [SerializeField] protected PlayerColorsSO _playerColors;

        protected override void SelectPreviousIfEnabled()
        {
            if (!gameObject.activeInHierarchy || _defaultSelection == null) return;

            if (InputManager.Instance != null)
            {
                //InputManager.Instance.SetUISelectionForPlayer(_player, _defaultSelection, gameObject);
                //Debug.Log("Selected default item for player " + (int)_player);
            }
        }
    }
}
