using UnityEngine;

namespace MemeFight.UI.TextChat
{
    public class StatusBarUI : MonoBehaviour
    {
        [SerializeField] GameObject[] _bars;

        public void SetBarsValue(int value)
        {
            value = Mathf.Min(value, _bars.LastIndex());

            for (int i = 0; i < _bars.Length; i++)
            {
                _bars[i].SetActive(i < value);
            }
        }
    }
}
