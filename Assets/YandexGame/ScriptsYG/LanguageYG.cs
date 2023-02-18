using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
#endif

namespace YG
{
    public class LanguageYG : MonoBehaviour
    {
        public Text textUIComponent;
        public TextMesh textMeshComponent;
        public InfoYG infoYG;
        [Space(10)]
        public string text;
        [Tooltip("RUSSIAN")]
        public string ru, en, tr, az, be, he, hy, ka, et, fr, kk, ky, lt, lv, ro, tg, tk, uk, uz, es, pt, ar, id, ja, it, de, hi;
        public int fontNumber;
        public Font uniqueFont;
        int baseFontSize;

        private void Awake()
        {
            // Раскомментируйте нижнюю строку, если вы получаете какие-либо ошибки связанные с InfoYG. В каких то случаях, это может помочь.
            // Uncomment the bottom line if you get any errors related to infoYG. In some cases, it may help.
            //Serialize();

            if (textUIComponent)
                baseFontSize = textUIComponent.fontSize;
            else if (textMeshComponent)
                baseFontSize = textMeshComponent.fontSize;
        }

        [ContextMenu("Reserialize")]
        public void Serialize()
        {
            textUIComponent = GetComponent<Text>();
            textMeshComponent = GetComponent<TextMesh>();

            infoYG = GetInfoYG();
        }

        public InfoYG GetInfoYG()
        {
            YandexGame yg = (YandexGame)GameObject.FindObjectOfType<YandexGame>();

            if (yg)
            {
                return yg.infoYG;
            }
            else
            {
#if UNITY_EDITOR
                InfoYG infoYGFromConfig = Insides.ConfigYG.GetInfoYG();
                return infoYGFromConfig;
#else
                return null;
#endif
            }

        }

        private void OnEnable()
        {
            YandexGame.SwitchLangEvent += SwitchLanguage;
            SwitchLanguage(YandexGame.savesData.language);
        }

        private void OnDisable() => YandexGame.SwitchLangEvent -= SwitchLanguage;

        public void SwitchLanguage(string lang)
        {
            for (int i = 0; i < languages.Length; i++)
            {
                if (lang == infoYG.LangName(i))
                {
                    AssignTranslate(languages[i]);
                    ChangeFont(infoYG.GetFont(i));
                    FontSizeCorrect(infoYG.GetFontSize(i));
                }
            }
        }

        void AssignTranslate(string translation)
        {
            if (textUIComponent)
                textUIComponent.text = translation;
            else if (textMeshComponent)
                textMeshComponent.text = translation;
        }

        public void ChangeFont(Font[] fontArray)
        {
            Font font;

            if (fontArray.Length >= fontNumber + 1 && fontArray[fontNumber])
            {
                font = fontArray[fontNumber];
            }
            else font = null;

            if (uniqueFont)
            {
                font = uniqueFont;
            }
            else if (font == null)
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

            if (font != null)
            {
                if (textUIComponent)
                    textUIComponent.font = font;
                else if (textMeshComponent)
                    textMeshComponent.font = font;
            }
        }

        void FontSizeCorrect(int[] fontSizeArray)
        {
            if (textUIComponent)
                textUIComponent.fontSize = baseFontSize;
            else if (textMeshComponent)
                textMeshComponent.fontSize = baseFontSize;

            if (fontSizeArray.Length != 0 && fontSizeArray.Length >= fontNumber - 1)
            {
                if (textUIComponent)
                    textUIComponent.fontSize += fontSizeArray[fontNumber];
                else if (textMeshComponent)
                    textMeshComponent.fontSize += fontSizeArray[fontNumber];
            }
        }

        public string[] languages
        {
            get
            {
                string[] s = new string[27];

                s[0] = ru;
                s[1] = en;
                s[2] = tr;
                s[3] = az;
                s[4] = be;
                s[5] = he;
                s[6] = hy;
                s[7] = ka;
                s[8] = et;
                s[9] = fr;
                s[10] = kk;
                s[11] = ky;
                s[12] = lt;
                s[13] = lv;
                s[14] = ro;
                s[15] = tg;
                s[16] = tk;
                s[17] = uk;
                s[18] = uz;
                s[19] = es;
                s[20] = pt;
                s[21] = ar;
                s[22] = id;
                s[23] = ja;
                s[24] = it;
                s[25] = de;
                s[26] = hi;

                return s;
            }
            set
            {
                ru = value[0];
                en = value[1];
                tr = value[2];
                az = value[3];
                be = value[4];
                he = value[5];
                hy = value[6];
                ka = value[7];
                et = value[8];
                fr = value[9];
                kk = value[10];
                ky = value[11];
                lt = value[12];
                lv = value[13];
                ro = value[14];
                tg = value[15];
                tk = value[16];
                uk = value[17];
                uz = value[18];
                es = value[19];
                pt = value[20];
                ar = value[21];
                id = value[22];
                ja = value[23];
                it = value[24];
                de = value[25];
                hi = value[26];
            }
        }

#if UNITY_EDITOR
        public float textHeight = 20f;
        public string processTranslateLabel;
        public bool componentTextField;

        public void SetLang(int index, string text)
        {
            string[] str = languages;
            str[index] = text;

            languages = str;
        }

        public void Translate(int countLangAvailable)
        {
            StartCoroutine(TranslateEmptyFields(countLangAvailable));
        }

        string TranslateGoogle(string translationTo = "en")
        {
            string text;

            if (!componentTextField)
                text = this.text;
            else if (textUIComponent)
                text = textUIComponent.text;
            else if (textMeshComponent)
                text = textMeshComponent.text;
            else
            {
                Debug.LogError("(ruСообщение)Текст для перевода не найден!\n(enMessage)The text for translation was not found!");
                return null;
            }

            var url = String.Format("http://translate.google." + infoYG.domainAutoLocalization + "/translate_a/single?client=gtx&dt=t&sl={0}&tl={1}&q={2}",
                "auto", translationTo, WebUtility.UrlEncode(text));
            UnityWebRequest www = UnityWebRequest.Get(url);
            www.SendWebRequest();
            while (!www.isDone)
            {

            }
            string response = www.downloadHandler.text;

            try
            {
                JArray jsonArray = JArray.Parse(response);
                response = jsonArray[0][0][0].ToString();
            }
            catch
            {
                response = "process error";
                StopAllCoroutines();
                processTranslateLabel = processTranslateLabel + " error";

                Debug.LogError("(ruСообщение) Процесс не завершён! Вероятно, Вы делали слишком много запросов. В таком случае, API Google Translate блокирует доступ к переводу на некоторое время.  Пожалуйста, попробуйте позже. Не переводите текст слишком часто, чтобы Google не посчитал Ваши действия за спам" +
                            "\n" + "(enMessage) The process is not completed! Most likely, you made too many requests. In this case, the Google Translate API blocks access to the translation for a while.  Please try again later. Do not translate the text too often, so that Google does not consider your actions as spam");
            }

            return response;
        }

        public int countLang = 0;
        IEnumerator TranslateEmptyFields(int countLangAvailable)
        {
            countLang = 0;
            processTranslateLabel = "processing... 0/" + countLangAvailable;

            for (int i = 0; i < languages.Length; i++)
            {
                if (infoYG.LangArr()[i] && (languages[i] == null || languages[i] == ""))
                {
                    bool complete = false;
                    SetLang(i, TranslateGoogle(infoYG.LangName(i)));

                    if (processTranslateLabel.Contains("error"))
                        processTranslateLabel = countLang + "/" + countLangAvailable + " error";
                    else
                    {
                        complete = true;
                        processTranslateLabel = countLang + "/" + countLangAvailable;
                    }

                    yield return complete == true;
                    countLang++;
                }
            }

            processTranslateLabel = countLang + "/" + countLangAvailable + " completed";
        }
#endif
    }
}