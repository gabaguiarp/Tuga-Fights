using UnityEngine;
using Random = UnityEngine.Random;

namespace MemeFight
{
    [CreateAssetMenu(fileName = "AudioCueBank", menuName = MenuPaths.Audio + "Audio Cue Bank")]
    public class AudioCueBankSO : CollectionSO<AudioCueSO>
    {
        public AudioCueSO GetRandom()
        {
            return items[Random.Range(0, items.Length)];
        }
    }
}
