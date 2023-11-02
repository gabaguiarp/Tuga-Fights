using UnityEngine;
using UnityEngine.Localization;

namespace MemeFight
{
    [CreateAssetMenu(menuName = "Gameplay/Team Data")]
    public class TeamDataSO : ScriptableObject
    {
        [SerializeField] Team _label;
        [SerializeField] LocalizedString _nameString;

        public Team Label => _label;
        public LocalizedString Name => _nameString;
    }
}
