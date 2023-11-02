using UnityEngine;
using UnityEngine.UI;

namespace MemeFight.UI
{
    public class FightersRenderDisplayUI : MonoBehaviour
    {
        [SerializeField] Image _displayImage;
        [SerializeField] PersistentDataSO _persistentData;
        [SerializeField] Sprite[] _bacalhauTemRenders;
        [SerializeField] Sprite[] _azeiteTeamRenders;

        /// <summary>
        /// Displays a random fighter render from the player's current team roster.
        /// </summary>
        public void DisplayRandom()
        {
            var renders = GetRendersForCurrentTeam();
            DisplaySprite(renders[Randomizer.GetRandom(0, renders.Length, true)]);
        }

        /// <summary>
        /// Displays the first fighter render from the player's current team.
        /// </summary>
        public void DisplayFirstFighter()
        {
            var renders = GetRendersForCurrentTeam();
            if (renders.Length > 0)
            {
                DisplaySprite(renders[0]);
            }
            else
            {
                Debug.LogErrorFormat("Cannot display first fighter for team {0} because there are no render sprites assigned!",
                                     _persistentData.SelectedTeam);
            }
        }

        void DisplaySprite(Sprite sprite)
        {
            _displayImage.sprite = sprite;
        }

        Sprite[] GetRendersForCurrentTeam()
        {
            return _persistentData.SelectedTeam.Equals(Team.Bacalhau) ? _bacalhauTemRenders : _azeiteTeamRenders;
        }
    }
}
