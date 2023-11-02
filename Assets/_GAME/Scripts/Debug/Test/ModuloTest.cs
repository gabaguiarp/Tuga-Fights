using System.Collections;
using TMPro;
using UnityEngine;

namespace MemeFight
{
    public class ModuloTest : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _displayText;
        [SerializeField] int _maxModuloValue = 3;

        [Header("Info")]
        [SerializeField, ReadOnly] int _currentValue;
        [SerializeField, ReadOnly] int _moduloValue;

        void Start()
        {
            UpdateDisplay();
            StartCoroutine(UpdateLoop());
        }

        void UpdateValues()
        {
            _currentValue++;
            _moduloValue = _currentValue % _maxModuloValue;
            UpdateDisplay();
        }

        void UpdateDisplay()
        {
            _displayText.SetText($"Value: {_currentValue}\nModulo: {_moduloValue}");
        }

        IEnumerator UpdateLoop()
        {
            while (true)
            {
                yield return CoroutineUtils.GetWaitTime(1f);
                UpdateValues();
            }
        }
    }
}
