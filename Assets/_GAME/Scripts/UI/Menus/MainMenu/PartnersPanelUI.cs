using UnityEngine;
using UnityEngine.Events;

namespace MemeFight.UI
{
    public class PartnersPanelUI : MonoBehaviour
    {
        public event UnityAction<string> OnButtonClicked;

        [SerializeField] GameObject _buttonsGroup;

        void Awake()
        {
            foreach (var button in _buttonsGroup.GetComponentsInChildren<PartnerButtonUI>())
            {
                button.OnClicked += OpenPartnerURL;
            }
        }

        void OpenPartnerURL(string url)
        {
            if (url.Equals(string.Empty))
            {
                Debug.LogWarning("Partner URL is empty!");
                return;
            }

            GameManager.OpenURL(url);
        }
    }
}
