using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace MemeFight.UI
{
    public class FighterSelectionDisplayUI : MonoBehaviour
    {
        [SerializeField] RectTransform _displayGroup;
        [SerializeField] Image _avatar;
        [SerializeField] Image _frame;
        [SerializeField] TextMeshProUGUI _fighterName;

        public void UpdateDisplay(Sprite avatar, string fighterName, bool animate = false)
        {
            _avatar.sprite = avatar;
            _fighterName.SetText(fighterName);

            if (animate)
                AnimateDisplay();
        }

        public void SetFrameColor(Color color) => _frame.SetColor(color);

        [ContextMenu("Animate Display")]
        void AnimateDisplay()
        {
            if (!Application.isPlaying)
                return;

            if (DOTween.IsTweening(_displayGroup))
            {
                _displayGroup.DOKill(true);
            }

            _displayGroup.localScale = Vector3.one;
            _displayGroup.DOPunchScale(Vector3.one * 0.2f, 0.15f, 8, 0.5f);
        }
    }
}
