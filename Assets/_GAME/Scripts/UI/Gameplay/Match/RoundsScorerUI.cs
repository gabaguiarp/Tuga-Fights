using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace MemeFight.UI
{
    public class RoundsScorerUI : MonoBehaviour
    {
        public enum Arrangement { LeftToRight, RightToLeft }

        [SerializeField] WinIconUI _winIconPrefab;

        [Header("Info")]
        [SerializeField] List<WinIconUI> _icons = new List<WinIconUI>();

        public Arrangement arrangement
        {
            get => _arrangement;
            set
            {
                _arrangement = value;

                if (TryGetComponent(out HorizontalLayoutGroup layourGroup))
                {
                    layourGroup.reverseArrangement = _arrangement == Arrangement.RightToLeft;
                }
            }
        }

        public RectTransform rectTransform
        {
            get
            {
                if (_rect == null)
                    _rect = GetComponent<RectTransform>();

                return _rect;
            }
        }

        Arrangement _arrangement;
        RectTransform _rect;

        public void PopulateWinIcons(int numOfRounds)
        {
            _icons.Clear();

            // Get the icons that already exist and add them to the icons list
            foreach (var icon in transform.GetComponentsInChildren<WinIconUI>())
            {
                icon.SetHighlightOn(false);
                _icons.Add(icon);
            }

            // Based on the number of pre-existing icons, determine if we need to add or remove some in order to match the number of rounds
            if (_icons.Count < numOfRounds)
            {
                int remaining = numOfRounds - _icons.Count;
                CreateWinIcons(remaining);
            }
            else if (_icons.Count > numOfRounds)
            {
                int spare = _icons.Count - numOfRounds;
                RemoveWinIcons(spare);
            }

            //Debug.Log("Win Icons populated!");
        }

        public void HighlightNewIcon()
        {
            try
            {
                var icon = _icons.FirstOrDefault(i => i.IsOn == false);
                icon.SetHighlightOn(true);
            }
            catch
            {
                Debug.Log($"Unable to highlight new icon in {name} because all icons are already highlighted!");
            }
        }

        public void Restore()
        {
            _icons.ForEach(i => i.SetHighlightOn(false));
        }

        void CreateWinIcons(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                var icon = Instantiate(_winIconPrefab, transform);
                icon.SetHighlightOn(false);
                _icons.Add(icon);
            }

            Debug.Log($"Created {amount} new win icons.");
        }

        void RemoveWinIcons(int amount)
        {
            for (int i = _icons.LastIndex(); i >= _icons.Count - amount; i--)
            {
                Destroy(_icons[i].gameObject);
            }

            _icons.Trim(_icons.Count - amount);
            Debug.Log($"Removed {amount} win icons.");
        }
    }
}
