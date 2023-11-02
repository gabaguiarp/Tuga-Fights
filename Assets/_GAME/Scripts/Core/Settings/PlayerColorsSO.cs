using UnityEngine;

namespace MemeFight.Settings
{
    [CreateAssetMenu(fileName = "PlayerColors", menuName = MenuPaths.Settings + "Player Colors")]
    public class PlayerColorsSO : BaseSO
    {
        [Space(10)]
        [SerializeField] Color _playerOneColor = Color.blue;
        [SerializeField] Color _playerTwoColor = Color.red;

        public Color GetColorForPlayer(Player player)
        {
            switch (player)
            {
                default:
                case Player.One:
                    return _playerOneColor;

                case Player.Two:
                    return _playerTwoColor;
            }
        }
    }
}
