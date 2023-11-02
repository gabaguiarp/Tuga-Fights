using System.Text;
using TMPro;
using UnityEngine;

namespace MemeFight
{
    public class CreditsGenerator : MonoBehaviour
    {
        public GameCreditsSO credits;
        [SerializeField] TextMeshProUGUI _targetText;

        [Header("Settings")]
        [SerializeField] float _companySectionNameSize = 35f;
        [SerializeField] float _roleTitleSize = 26f;
        [SerializeField] float _creditNameSize = 35f;

        readonly string FormatString = "<size={0}>{1}</size>";

        [ContextMenu("Generate Credits")]
        public void GenerateCredits()
        {
            if (credits == null)
            {
                Debug.LogError("Unable to generate credits because no credits asset was assigned!");
                return;
            }

            if (_targetText == null)
            {
                Debug.LogError("Failed to generate credits because no target text was assigned!");
                return;
            }

            _targetText.text = GetCreditsText();
        }

        string GetCreditsText()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < credits.roles.Count; i++)
            {
                // Role Title
                var role = credits.roles[i];

                if (i > 0)
                {
                    sb.AppendLine();
                }

                sb.AppendFormat(FormatString, _roleTitleSize, role.title);

                // Names
                for (int j = 0; j < role.names.Count; j++)
                {
                    sb.AppendLine();
                    sb.AppendFormat(FormatString, _creditNameSize, role.names[j]);
                }

                if (i < credits.roles.LastIndex())
                {
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }
    }
}
