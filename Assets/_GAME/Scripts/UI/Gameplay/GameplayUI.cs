using UnityEngine;
using UnityEngine.Events;

namespace MemeFight.UI
{
    public class GameplayUI : MonoBehaviour
    {
        [SerializeField] ButtonUI _pauseButton;

        public event UnityAction OnPauseClicked;

        void Awake()
        {
            _pauseButton.OnClicked += PauseButtonClicked;
        }

        void PauseButtonClicked() => OnPauseClicked?.Invoke();
    }
}
