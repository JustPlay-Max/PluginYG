using UnityEngine;
#if YG_TEXT_MESH_PRO
using TMPro;
#endif

namespace YG
{
    public class LangYGAdditionalText : MonoBehaviour
    {
        public enum Side { Left, Right };
        public Side side;

        public string additionalText
        {
            get => _additionalText;
            set
            {
                _additionalText = value;
                AssignAdditionalText();
            }
        }

        [SerializeField] private string _additionalText;
        private LanguageYG langYG;

        public void AssignAdditionalText(LanguageYG languageYG)
        {
            langYG = languageYG;
            DoAssignAdditionalText();
        }

        public void AssignAdditionalText()
        {
            if (langYG)
                DoAssignAdditionalText();
        }

        private void DoAssignAdditionalText()
        {
            if (side == Side.Left)
            {
                if (langYG.textLComponent)
                    langYG.textLComponent.text = _additionalText + langYG.textLComponent.text;
#if YG_TEXT_MESH_PRO
                else if (langYG.textMPComponent)
                    langYG.textMPComponent.text = _additionalText + langYG.textMPComponent.text;
#endif
            }
            else if (side == Side.Right)
            {
                if (langYG.textLComponent)
                    langYG.textLComponent.text += _additionalText;
#if YG_TEXT_MESH_PRO
                else if (langYG.textMPComponent)
                    langYG.textMPComponent.text += _additionalText;
#endif
            }
        }
    }
}