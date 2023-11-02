using TMPro;
using UnityEngine;

namespace MemeFight
{
    public class GameVersionUI : MonoBehaviour
    {
        [SerializeField] bool _updateInEditMode = true;
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
                _versionText.SetText(Application.version);
        }
    }
}
