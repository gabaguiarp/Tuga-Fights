using UnityEngine;

namespace MemeFight
{
    [CreateAssetMenu(fileName = "MatchConfiguration", menuName = MenuPaths.Data + "Match Configuration")]
    public class MatchConfigurationSO : BaseSO
    {
        [Space(10)]
        [SerializeField] FighterProfileSO _fighterOne;
        [SerializeField] FighterProfileSO _fighterTwo;

        public FighterProfileSO FighterOne => _fighterOne;
        public FighterProfileSO FighterTwo => _fighterTwo;

        public void SetFighterForPlayer(Player player, FighterProfileSO fighter)
        {
            if (player == Player.One)
                _fighterOne = fighter;
            else if (player == Player.Two)
                _fighterTwo = fighter;
        }

        public FighterProfileSO GetFighterForTeam(Team team)
        {
            if (_fighterOne.Team == team)
                return _fighterOne;
            else if (_fighterTwo.Team == team)
                return _fighterTwo;
            else
                return null;
        }

        [ContextMenu("Clear")]
        public void Clear()
        {
            _fighterOne = null;
            _fighterTwo = null;
        }
    }
}
