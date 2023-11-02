using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MemeFight.UI
{
    public class RowToggle : MonoBehaviour, IPointerClickHandler
    {
        [Serializable]
        public struct RowToggleOption
        {
            public RowToggleOption(Image graphic, TextMeshProUGUI labelText)
            {
                _graphic = graphic;
                _label = labelText;
            }

            [SerializeField] Image _graphic;
            [SerializeField] TextMeshProUGUI _label;

            public void SetGraphicColor(Color color)
            {
                if (_graphic != null)
                    _graphic.color = color;
            }

            public void SetSprite(Sprite sprite)
            {
                if (_graphic != null)
                    _graphic.sprite = sprite;
            }

            public void SetLabelText(string text)
            {
                if (_label != null)
                    _label.SetText(text);
            }
        }

        [Serializable]
        public class RowToggleEvent : UnityEvent<int> { }

        [SerializeField] Color _selectedColor = Color.white;
        [SerializeField] Color _deselectedColor = Color.gray;
        [SerializeField] Sprite _selectedSprite = default;
        [SerializeField] Sprite _deselectedSprite = default;

        [Space(10)]
        [SerializeField] RowToggleOption[] _options = new RowToggleOption[2];

        [Space(10)]
        public RowToggleEvent onValueChanged = new RowToggleEvent();

        int _selectedOption = 0;

        void Reset()
        {
            _options = new RowToggleOption[2];

            Transform child;
            for (int i = 0; i < transform.childCount; i++)
            {
                child = transform.GetChild(i);
                if (child.TryGetComponent(out Image img))
                {
                    _options[i] = new RowToggleOption(img, child.GetComponentInChildren<TextMeshProUGUI>());
                }
            }
        }

        void OnValidate()
        {
            ValidateSelection();
        }

        void Awake()
        {
            ValidateSelection();
        }

        void ValidateSelection()
        {
            if (_options.Length < 2) return;

            int deselectedOption = GetDeselectedOption();

            // Color tint
            _options[_selectedOption].SetGraphicColor(_selectedColor);
            _options[deselectedOption].SetGraphicColor(_deselectedColor);

            // Swap sprites
            if (_selectedSprite != null)
                _options[_selectedOption].SetSprite(_selectedSprite);

            if (_deselectedSprite != null)
                _options[deselectedOption].SetSprite(_deselectedSprite);
        }

        int GetDeselectedOption() => _selectedOption > 0 ? 0 : 1;

        public void Setup(string label1, string label2)
        {
            _options[0].SetLabelText(label1);
            _options[1].SetLabelText(label2);
        }

        public void SelectOption(bool option1)
        {
            SelectOptionWithoutNotify(option1);
            onValueChanged?.Invoke(_selectedOption);
        }

        public void SelectOptionWithoutNotify(bool option1)
        {
            _selectedOption = Logic.BoolToInt(!option1);
            ValidateSelection();
        }

        public void ToggleSelection() => SelectOption(_selectedOption == 1);

        public void OnPointerClick(PointerEventData eventData)
        {
            ToggleSelection();
        }
    }
}
