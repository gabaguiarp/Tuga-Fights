using TMPro;
using UnityEngine;

namespace MemeFight
{
    public class GameVersionUI : MonoBehaviour
    {
        [SerializeField] bool _updateInEditMode = true;
        [SerializeField] string _prefix;
        [SerializeField] TextMeshProUGUI _versionText;

        void Reset()
        {
            _versionText = GetComponent<TextMeshProUGUI>();
        }

        void OnValidate()
        {
            if (_updateInEditMode && !Application.isPlaying)
            {
                UpdateVersionDisplay();
            }
        }

        void Awake()
        {
            UpdateVersionDisplay();
        }

        void UpdateVersionDisplay()
        {
            if (_versionText != null)
                _versionText.SetText(_prefix + Application.version);
        }
    }
}
