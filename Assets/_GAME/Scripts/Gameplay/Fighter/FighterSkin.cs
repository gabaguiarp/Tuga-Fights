using UnityEngine;

namespace MemeFight.Characters
{
    public class FighterSkin : MonoBehaviour
    {
        [field: SerializeField]
        public string ModelLabel;

        void Reset()
        {
            ModelLabel = gameObject.name;
        }

        public void SetEnabled(bool enabled)
        {
            gameObject.SetActive(enabled);
        }
    }
}
