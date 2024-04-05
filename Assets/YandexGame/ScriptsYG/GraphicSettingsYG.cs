using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

namespace YG
{
    public class GraphicSettingsYG : MonoBehaviour
    {
        [SerializeField] InfoYG infoYG;
        [SerializeField] Dropdown dropdown;
        [SerializeField] Text labelText;
        [SerializeField] Text itemText;
        [SerializeField] int fontNumber;
        [Space(5)]
        [Header("Translate")]
        [SerializeField] string[] ru = new string[6];
        [SerializeField] string[] en = new string[6];
        [SerializeField] string[] tr = new string[6];
        [SerializeField] string[] az = new string[6];
        [SerializeField] string[] be = new string[6];
        [SerializeField] string[] he = new string[6];
        [SerializeField] string[] hy = new string[6];
        [SerializeField] string[] ka = new string[6];
        [SerializeField] string[] et = new string[6];
        [SerializeField] string[] fr = new string[6];
        [SerializeField] string[] kk = new string[6];
        [SerializeField] string[] ky = new string[6];
        [SerializeField] string[] lt = new string[6];
        [SerializeField] string[] lv = new string[6];
        [SerializeField] string[] ro = new string[6];
        [SerializeField] string[] tg = new string[6];
        [SerializeField] string[] tk = new string[6];
        [SerializeField] string[] uk = new string[6];
        [SerializeField] string[] uz = new string[6];
        [SerializeField] string[] es = new string[6];
        [SerializeField] string[] pt = new string[6];
        [SerializeField] string[] ar = new string[6];
        [SerializeField] string[] id = new string[6];
        [SerializeField] string[] ja = new string[6];
        [SerializeField] string[] it = new string[6];
        [SerializeField] string[] de = new string[6];
        [SerializeField] string[] hi = new string[6];

        int labelBaseFontSize, itemBaseFontSize;

        void Awake()
        {
            labelBaseFontSize = labelText.fontSize;
            itemBaseFontSize = itemText.fontSize;

            dropdown.ClearOptions();
            dropdown.AddOptions(QualitySettings.names.ToList());
            dropdown.value = QualitySettings.GetQualityLevel();
        }

        private void Start()
        {
            SwitchLanguage(YandexGame.lang);
        }

        private void OnEnable() => YandexGame.SwitchLangEvent += SwitchLanguage;
        private void OnDisable() => YandexGame.SwitchLangEvent -= SwitchLanguage;

        public static Action onQualityChange;

        public void SetQuality()
        {
            QualitySettings.SetQualityLevel(dropdown.value);
            onQualityChange?.Invoke();
        }

        void SwitchLanguage(string lang)
        {
            switch (lang)
            {
                case "ru":
                    labelText.text = ru[QualitySettings.GetQualityLevel()];
                    SwithFont(infoYG.fonts.ru);
                    FontSizeCorrect(infoYG.fontsSizeCorrect.ru);
                    for (int i = 0; i < ru.Length; i++)
                        dropdown.options[i].text = ru[i];
                    break;
                case "tr":
                    labelText.text = tr[QualitySettings.GetQualityLevel()];
                    SwithFont(infoYG.fonts.tr);
                    FontSizeCorrect(infoYG.fontsSizeCorrect.tr);
                    for (int i = 0; i < tr.Length; i++)
                        dropdown.options[i].text = tr[i];
                    break;
                case "en":
                    labelText.text = en[QualitySettings.GetQualityLevel()];
                    SwithFont(infoYG.fonts.en);
                    FontSizeCorrect(infoYG.fontsSizeCorrect.en);
                    for (int i = 0; i < en.Length; i++)
                        dropdown.options[i].text = en[i];
                    break;
                case "az":
                    labelText.text = az[QualitySettings.GetQualityLevel()];
                    SwithFont(infoYG.fonts.az);
                    FontSizeCorrect(infoYG.fontsSizeCorrect.az);
                    for (int i = 0; i < az.Length; i++)
                        dropdown.options[i].text = az[i];
                    break;
                case "be":
                    labelText.text = be[QualitySettings.GetQualityLevel()];
                    SwithFont(infoYG.fonts.be);
                    FontSizeCorrect(infoYG.fontsSizeCorrect.be);
                    for (int i = 0; i < be.Length; i++)
                        dropdown.options[i].text = be[i];
                    break;
                case "he":
                    labelText.text = he[QualitySettings.GetQualityLevel()];
                    SwithFont(infoYG.fonts.he);
                    FontSizeCorrect(infoYG.fontsSizeCorrect.he);
                    for (int i = 0; i < he.Length; i++)
                        dropdown.options[i].text = he[i];
                    break;
                case "hy":
                    labelText.text = hy[QualitySettings.GetQualityLevel()];
                    SwithFont(infoYG.fonts.hy);
                    FontSizeCorrect(infoYG.fontsSizeCorrect.hy);
                    for (int i = 0; i < hy.Length; i++)
                        dropdown.options[i].text = hy[i];
                    break;
                case "ka":
                    labelText.text = ka[QualitySettings.GetQualityLevel()];
                    SwithFont(infoYG.fonts.ka);
                    FontSizeCorrect(infoYG.fontsSizeCorrect.ka);
                    for (int i = 0; i < ka.Length; i++)
                        dropdown.options[i].text = ka[i];
                    break;
                case "et":
                    labelText.text = et[QualitySettings.GetQualityLevel()];
                    SwithFont(infoYG.fonts.et);
                    FontSizeCorrect(infoYG.fontsSizeCorrect.et);
                    for (int i = 0; i < et.Length; i++)
                        dropdown.options[i].text = et[i];
                    break;
                case "fr":
                    labelText.text = fr[QualitySettings.GetQualityLevel()];
                    SwithFont(infoYG.fonts.fr);
                    FontSizeCorrect(infoYG.fontsSizeCorrect.fr);
                    for (int i = 0; i < fr.Length; i++)
                        dropdown.options[i].text = fr[i];
                    break;
                case "kk":
                    labelText.text = kk[QualitySettings.GetQualityLevel()];
                    SwithFont(infoYG.fonts.kk);
                    FontSizeCorrect(infoYG.fontsSizeCorrect.kk);
                    for (int i = 0; i < kk.Length; i++)
                        dropdown.options[i].text = kk[i];
                    break;
                case "ky":
                    labelText.text = ky[QualitySettings.GetQualityLevel()];
                    SwithFont(infoYG.fonts.ky);
                    FontSizeCorrect(infoYG.fontsSizeCorrect.ky);
                    for (int i = 0; i < ky.Length; i++)
                        dropdown.options[i].text = ky[i];
                    break;
                case "lt":
                    labelText.text = lt[QualitySettings.GetQualityLevel()];
                    SwithFont(infoYG.fonts.lt);
                    FontSizeCorrect(infoYG.fontsSizeCorrect.lt);
                    for (int i = 0; i < lt.Length; i++)
                        dropdown.options[i].text = lt[i];
                    break;
                case "lv":
                    labelText.text = lv[QualitySettings.GetQualityLevel()];
                    SwithFont(infoYG.fonts.lv);
                    FontSizeCorrect(infoYG.fontsSizeCorrect.lv);
                    for (int i = 0; i < lv.Length; i++)
                        dropdown.options[i].text = lv[i];
                    break;
                case "ro":
                    labelText.text = ro[QualitySettings.GetQualityLevel()];
                    SwithFont(infoYG.fonts.ro);
                    FontSizeCorrect(infoYG.fontsSizeCorrect.ro);
                    for (int i = 0; i < ro.Length; i++)
                        dropdown.options[i].text = ro[i];
                    break;
                case "tg":
                    labelText.text = tg[QualitySettings.GetQualityLevel()];
                    SwithFont(infoYG.fonts.tg);
                    FontSizeCorrect(infoYG.fontsSizeCorrect.tg);
                    for (int i = 0; i < tg.Length; i++)
                        dropdown.options[i].text = tg[i];
                    break;
                case "tk":
                    labelText.text = tk[QualitySettings.GetQualityLevel()];
                    SwithFont(infoYG.fonts.tk);
                    FontSizeCorrect(infoYG.fontsSizeCorrect.tk);
                    for (int i = 0; i < tk.Length; i++)
                        dropdown.options[i].text = tk[i];
                    break;
                case "uk":
                    labelText.text = uk[QualitySettings.GetQualityLevel()];
                    SwithFont(infoYG.fonts.uk);
                    FontSizeCorrect(infoYG.fontsSizeCorrect.uk);
                    for (int i = 0; i < uk.Length; i++)
                        dropdown.options[i].text = uk[i];
                    break;
                case "uz":
                    labelText.text = uz[QualitySettings.GetQualityLevel()];
                    SwithFont(infoYG.fonts.uz);
                    FontSizeCorrect(infoYG.fontsSizeCorrect.uz);
                    for (int i = 0; i < uz.Length; i++)
                        dropdown.options[i].text = uz[i];
                    break;
                case "es":
                    labelText.text = es[QualitySettings.GetQualityLevel()];
                    SwithFont(infoYG.fonts.es);
                    FontSizeCorrect(infoYG.fontsSizeCorrect.es);
                    for (int i = 0; i < es.Length; i++)
                        dropdown.options[i].text = es[i];
                    break;
                case "pt":
                    labelText.text = pt[QualitySettings.GetQualityLevel()];
                    SwithFont(infoYG.fonts.pt);
                    FontSizeCorrect(infoYG.fontsSizeCorrect.pt);
                    for (int i = 0; i < pt.Length; i++)
                        dropdown.options[i].text = pt[i];
                    break;
                case "ar":
                    labelText.text = ar[QualitySettings.GetQualityLevel()];
                    SwithFont(infoYG.fonts.ar);
                    FontSizeCorrect(infoYG.fontsSizeCorrect.ar);
                    for (int i = 0; i < ar.Length; i++)
                        dropdown.options[i].text = ar[i];
                    break;
                case "id":
                    labelText.text = id[QualitySettings.GetQualityLevel()];
                    SwithFont(infoYG.fonts.id);
                    FontSizeCorrect(infoYG.fontsSizeCorrect.id);
                    for (int i = 0; i < id.Length; i++)
                        dropdown.options[i].text = id[i];
                    break;
                case "ja":
                    labelText.text = ja[QualitySettings.GetQualityLevel()];
                    SwithFont(infoYG.fonts.ja);
                    FontSizeCorrect(infoYG.fontsSizeCorrect.ja);
                    for (int i = 0; i < ja.Length; i++)
                        dropdown.options[i].text = ja[i];
                    break;
                case "it":
                    labelText.text = it[QualitySettings.GetQualityLevel()];
                    SwithFont(infoYG.fonts.it);
                    FontSizeCorrect(infoYG.fontsSizeCorrect.it);
                    for (int i = 0; i < it.Length; i++)
                        dropdown.options[i].text = it[i];
                    break;
                case "de":
                    labelText.text = de[QualitySettings.GetQualityLevel()];
                    SwithFont(infoYG.fonts.de);
                    FontSizeCorrect(infoYG.fontsSizeCorrect.en);
                    for (int i = 0; i < de.Length; i++)
                        dropdown.options[i].text = de[i];
                    break;
                case "hi":
                    labelText.text = hi[QualitySettings.GetQualityLevel()];
                    SwithFont(infoYG.fonts.hi);
                    FontSizeCorrect(infoYG.fontsSizeCorrect.hi);
                    for (int i = 0; i < hi.Length; i++)
                        dropdown.options[i].text = hi[i];
                    break;
                default:
                    labelText.text = en[QualitySettings.GetQualityLevel()];
                    SwithFont(infoYG.fonts.en);
                    FontSizeCorrect(infoYG.fontsSizeCorrect.en);
                    for (int i = 0; i < en.Length; i++)
                        dropdown.options[i].text = en[i];
                    break;
            }
        }

        void SwithFont(Font[] fontArray)
        {
            Font font = labelText.font;

            if (fontArray.Length >= fontNumber + 1 && fontArray[fontNumber])
            {
                font = fontArray[fontNumber];
            }
            else
            {
                if (infoYG.fonts.defaultFont.Length >= fontNumber + 1 && infoYG.fonts.defaultFont[fontNumber])
                {
                    font = infoYG.fonts.defaultFont[fontNumber];
                }
                else if (infoYG.fonts.defaultFont.Length >= 1 && infoYG.fonts.defaultFont[0])
                {
                    font = infoYG.fonts.defaultFont[0];
                }
            }

            labelText.font = font;
            itemText.font = font;
        }

        void FontSizeCorrect(int[] fontSizeArray)
        {
            labelText.fontSize = labelBaseFontSize;
            itemText.fontSize = itemBaseFontSize;

            if (fontSizeArray.Length != 0 && fontSizeArray.Length >= fontNumber - 1)
            {
                labelText.fontSize += fontSizeArray[fontNumber];
                itemText.fontSize += fontSizeArray[fontNumber];
            }
        }
    }
}
