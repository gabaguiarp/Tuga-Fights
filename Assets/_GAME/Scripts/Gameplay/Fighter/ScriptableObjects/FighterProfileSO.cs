using UnityEngine;

namespace MemeFight
{
    [CreateAssetMenu(menuName = "Gameplay/Fighter/Profile", order = 0)]
    public class FighterProfileSO : ScriptableObject
    {
        [SerializeField] string _name = "Name";
        [Tooltip("The label used for sprite swaping when configuring the fighter model at the beginning of a match. It needs to match any " +
            "of the labels configured inside the model.")]
        [SerializeField] string _modelLabel = "Template";
        [SerializeField] Team _team;
        [Tooltip("The stat to register when this fighter wins a match in Free Fight mode.")]
        [SerializeField] StatID _winStat;
        [Tooltip("The configuration to use for the AI module when the fighter is controlled by the CPU.")]
        [SerializeField] FighterAIConfigurationSO _configuration;

        [Space(20)]
        [SerializeField, SpritePreview] Sprite _avatar;
        [SerializeField, SpritePreview] Sprite _thumbnail;

        public string Name => _name;
        public string Label => _modelLabel;
        public Sprite Avatar => _avatar;
        public Sprite Thumbnail => _thumbnail;
        public Team Team => _team;
        public StatID WinStat => _winStat;
        public FighterAIConfigurationSO AIConfig => _configuration;
    }
}
