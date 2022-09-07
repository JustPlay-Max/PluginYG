using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace YG
{
    public class GraphicSettingsYG : MonoBehaviour
    {
        [SerializeField] Dropdown dropdown;
        [SerializeField] Text labelText;
        [Space(5)]
        [Header("Перевод")]
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

        void Start()
        {
            dropdown.ClearOptions();
            dropdown.AddOptions(QualitySettings.names.ToList());
            dropdown.value = QualitySettings.GetQualityLevel();

            SwitchLanguage(YandexGame.savesData.language);
        }

        private void OnEnable() => YandexGame.SwitchLangEvent += SwitchLanguage;
        private void OnDisable() => YandexGame.SwitchLangEvent -= SwitchLanguage;

        public delegate void QualitySwitchDelegate(int qualityLevel);
        public event QualitySwitchDelegate QualitySwitchEvent;

        public void SetQuality()
        {
            QualitySettings.SetQualityLevel(dropdown.value);
            QualitySwitchEvent?.Invoke(dropdown.value);
        }

        void SwitchLanguage(string lang)
        {
            switch (lang)
            {
                case "ru":
                    labelText.text = ru[QualitySettings.GetQualityLevel()];
                    for (int i = 0; i < ru.Length; i++)
                        dropdown.options[i].text = ru[i];
                    break;
                case "tr":
                    labelText.text = tr[QualitySettings.GetQualityLevel()];
                    for (int i = 0; i < tr.Length; i++)
                        dropdown.options[i].text = tr[i];
                    break;
                case "en":
                    labelText.text = en[QualitySettings.GetQualityLevel()];
                    for (int i = 0; i < en.Length; i++)
                        dropdown.options[i].text = en[i];
                    break;
                case "az":
                    labelText.text = az[QualitySettings.GetQualityLevel()];
                    for (int i = 0; i < az.Length; i++)
                        dropdown.options[i].text = az[i];
                    break;
                case "be":
                    labelText.text = be[QualitySettings.GetQualityLevel()];
                    for (int i = 0; i < be.Length; i++)
                        dropdown.options[i].text = be[i];
                    break;
                case "he":
                    labelText.text = he[QualitySettings.GetQualityLevel()];
                    for (int i = 0; i < he.Length; i++)
                        dropdown.options[i].text = he[i];
                    break;
                case "hy":
                    labelText.text = hy[QualitySettings.GetQualityLevel()];
                    for (int i = 0; i < hy.Length; i++)
                        dropdown.options[i].text = hy[i];
                    break;
                case "ka":
                    labelText.text = ka[QualitySettings.GetQualityLevel()];
                    for (int i = 0; i < ka.Length; i++)
                        dropdown.options[i].text = ka[i];
                    break;
                case "et":
                    labelText.text = et[QualitySettings.GetQualityLevel()];
                    for (int i = 0; i < et.Length; i++)
                        dropdown.options[i].text = et[i];
                    break;
                case "fr":
                    labelText.text = fr[QualitySettings.GetQualityLevel()];
                    for (int i = 0; i < fr.Length; i++)
                        dropdown.options[i].text = fr[i];
                    break;
                case "kk":
                    labelText.text = kk[QualitySettings.GetQualityLevel()];
                    for (int i = 0; i < kk.Length; i++)
                        dropdown.options[i].text = kk[i];
                    break;
                case "ky":
                    labelText.text = ky[QualitySettings.GetQualityLevel()];
                    for (int i = 0; i < ky.Length; i++)
                        dropdown.options[i].text = ky[i];
                    break;
                case "lt":
                    labelText.text = lt[QualitySettings.GetQualityLevel()];
                    for (int i = 0; i < lt.Length; i++)
                        dropdown.options[i].text = lt[i];
                    break;
                case "lv":
                    labelText.text = lv[QualitySettings.GetQualityLevel()];
                    for (int i = 0; i < lv.Length; i++)
                        dropdown.options[i].text = lv[i];
                    break;
                case "ro":
                    labelText.text = ro[QualitySettings.GetQualityLevel()];
                    for (int i = 0; i < ro.Length; i++)
                        dropdown.options[i].text = ro[i];
                    break;
                case "tg":
                    labelText.text = tg[QualitySettings.GetQualityLevel()];
                    for (int i = 0; i < tg.Length; i++)
                        dropdown.options[i].text = tg[i];
                    break;
                case "tk":
                    labelText.text = tk[QualitySettings.GetQualityLevel()];
                    for (int i = 0; i < tk.Length; i++)
                        dropdown.options[i].text = tk[i];
                    break;
                case "uk":
                    labelText.text = uk[QualitySettings.GetQualityLevel()];
                    for (int i = 0; i < uk.Length; i++)
                        dropdown.options[i].text = uk[i];
                    break;
                case "uz":
                    labelText.text = uz[QualitySettings.GetQualityLevel()];
                    for (int i = 0; i < uz.Length; i++)
                        dropdown.options[i].text = uz[i];
                    break;
                case "es":
                    labelText.text = es[QualitySettings.GetQualityLevel()];
                    for (int i = 0; i < es.Length; i++)
                        dropdown.options[i].text = es[i];
                    break;
                case "pt":
                    labelText.text = pt[QualitySettings.GetQualityLevel()];
                    for (int i = 0; i < pt.Length; i++)
                        dropdown.options[i].text = pt[i];
                    break;
                case "ar":
                    labelText.text = ar[QualitySettings.GetQualityLevel()];
                    for (int i = 0; i < ar.Length; i++)
                        dropdown.options[i].text = ar[i];
                    break;
                case "id":
                    labelText.text = id[QualitySettings.GetQualityLevel()];
                    for (int i = 0; i < id.Length; i++)
                        dropdown.options[i].text = id[i];
                    break;
                case "ja":
                    labelText.text = ja[QualitySettings.GetQualityLevel()];
                    for (int i = 0; i < ja.Length; i++)
                        dropdown.options[i].text = ja[i];
                    break;
                case "it":
                    labelText.text = it[QualitySettings.GetQualityLevel()];
                    for (int i = 0; i < it.Length; i++)
                        dropdown.options[i].text = it[i];
                    break;
                case "de":
                    labelText.text = de[QualitySettings.GetQualityLevel()];
                    for (int i = 0; i < de.Length; i++)
                        dropdown.options[i].text = de[i];
                    break;
                case "hi":
                    labelText.text = hi[QualitySettings.GetQualityLevel()];
                    for (int i = 0; i < hi.Length; i++)
                        dropdown.options[i].text = hi[i];
                    break;
                default:
                    labelText.text = en[QualitySettings.GetQualityLevel()];
                    for (int i = 0; i < en.Length; i++)
                        dropdown.options[i].text = en[i];
                    break;
            }
        }
    }
}
