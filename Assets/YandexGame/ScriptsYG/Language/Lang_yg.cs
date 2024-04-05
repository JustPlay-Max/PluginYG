using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace YG
{
    public partial class YandexGame
    {
        public static string lang = "ru";
        public static Action<string> SwitchLangEvent;

        [InitYG]
        public static void InitLang()
        {
#if !UNITY_EDITOR
            Debug.Log("Init Lang inGame");
#endif
            if (!Instance.infoYG.LocalizationEnable ||
                Instance.infoYG.callingLanguageCheck == InfoYG.CallingLanguageCheck.DoNotChangeLanguageStartup)
            {
                return;
            }

            if (!HasKeyLang())
            {
                LanguageRequest();
            }
            else
            {
                if (Instance.infoYG.callingLanguageCheck == InfoYG.CallingLanguageCheck.EveryGameLaunch)
                {
                    LanguageRequest();
                }
                else if (Instance.infoYG.callingLanguageCheck == InfoYG.CallingLanguageCheck.FirstLaunchOnly)
                {
                    lang = LoadKeyLang();
                    SwitchLangEvent?.Invoke(lang);
                }
            }
        }

        public static bool LangEnable()
        {
            return Instance.infoYG.LocalizationEnable;
        }


        [DllImport("__Internal")]
        private static extern string LangRequest_js();

        public void _LanguageRequest()
        {
#if !UNITY_EDITOR
            SetLanguage(LangRequest_js());
#else
            string langSimulate = Instance.infoYG.playerInfoSimulation.language;
            if (langSimulate != null && langSimulate != "")
            {
                lang = langSimulate;
            }
            SetLanguage(lang);
#endif
        }
        public static void LanguageRequest() => Instance._LanguageRequest();


        public static void SwitchLanguage(string language)
        {
            savesData.language = language;
            lang = language;
            SwitchLangEvent?.Invoke(language);
            SaveKeyLang();
        }

        public void _SwitchLanguage(string language)
        {
            SwitchLanguage(language);
            SaveProgress();
        }

        private static bool HasKeyLang()
        {
#if UNITY_EDITOR
            return PlayerPrefs.HasKey("langYG");
#else
#if PLATFORM_WEBGL
            return HasKey("langYG");
#else
            return PlayerPrefs.HasKey("langYG");
#endif
#endif
        }

        private static string LoadKeyLang()
        {
#if UNITY_EDITOR
            return PlayerPrefs.GetString("langYG");
#else
#if PLATFORM_WEBGL
            return LoadFromLocalStorage("langYG");
#else
            return PlayerPrefs.GetString("langYG");
#endif
#endif
        }

        private static void SaveKeyLang()
        {
#if UNITY_EDITOR
            PlayerPrefs.SetString("langYG", lang);
            PlayerPrefs.Save();
#else
#if PLATFORM_WEBGL
            SaveToLocalStorage("langYG", lang);
#else
            PlayerPrefs.SetString("langYG", lang);
            PlayerPrefs.Save();
#endif
#endif
        }


        public void SetLanguage(string sendLang)
        {
            string _lang = "en";

            switch (sendLang)
            {
                case "ru":
                    if (infoYG.languages.ru)
                        _lang = sendLang;
                    break;
                case "en":
                    if (infoYG.languages.en)
                        _lang = sendLang;
                    break;
                case "tr":
                    if (infoYG.languages.tr)
                        _lang = sendLang;
                    else _lang = "ru";
                    break;
                case "az":
                    if (infoYG.languages.az)
                        _lang = sendLang;
                    else _lang = "en";
                    break;
                case "be":
                    if (infoYG.languages.be)
                        _lang = sendLang;
                    else _lang = "ru";
                    break;
                case "he":
                    if (infoYG.languages.he)
                        _lang = sendLang;
                    else _lang = "en";
                    break;
                case "hy":
                    if (infoYG.languages.hy)
                        _lang = sendLang;
                    else _lang = "en";
                    break;
                case "ka":
                    if (infoYG.languages.ka)
                        _lang = sendLang;
                    else _lang = "en";
                    break;
                case "et":
                    if (infoYG.languages.et)
                        _lang = sendLang;
                    else _lang = "en";
                    break;
                case "fr":
                    if (infoYG.languages.fr)
                        _lang = sendLang;
                    else _lang = "en";
                    break;
                case "kk":
                    if (infoYG.languages.kk)
                        _lang = sendLang;
                    else _lang = "ru";
                    break;
                case "ky":
                    if (infoYG.languages.ky)
                        _lang = sendLang;
                    else _lang = "en";
                    break;
                case "lt":
                    if (infoYG.languages.lt)
                        _lang = sendLang;
                    else _lang = "en";
                    break;
                case "lv":
                    if (infoYG.languages.lv)
                        _lang = sendLang;
                    else _lang = "en";
                    break;
                case "ro":
                    if (infoYG.languages.ro)
                        _lang = sendLang;
                    else _lang = "en";
                    break;
                case "tg":
                    if (infoYG.languages.tg)
                        _lang = sendLang;
                    else _lang = "en";
                    break;
                case "tk":
                    if (infoYG.languages.tk)
                        _lang = sendLang;
                    else _lang = "en";
                    break;
                case "uk":
                    if (infoYG.languages.uk)
                        _lang = sendLang;
                    else _lang = "ru";
                    break;
                case "uz":
                    if (infoYG.languages.uz)
                        _lang = sendLang;
                    else _lang = "ru";
                    break;
                case "es":
                    if (infoYG.languages.es)
                        _lang = sendLang;
                    else _lang = "en";
                    break;
                case "pt":
                    if (infoYG.languages.pt)
                        _lang = sendLang;
                    else _lang = "en";
                    break;
                case "ar":
                    if (infoYG.languages.ar)
                        _lang = sendLang;
                    else _lang = "en";
                    break;
                case "id":
                    if (infoYG.languages.id)
                        _lang = sendLang;
                    else _lang = "en";
                    break;
                case "ja":
                    if (infoYG.languages.ja)
                        _lang = sendLang;
                    else _lang = "en";
                    break;
                case "it":
                    if (infoYG.languages.it)
                        _lang = sendLang;
                    else _lang = "en";
                    break;
                case "de":
                    if (infoYG.languages.de)
                        _lang = sendLang;
                    else _lang = "en";
                    break;
                case "hi":
                    if (infoYG.languages.hi)
                        _lang = sendLang;
                    else _lang = "en";
                    break;
                default:
                    _lang = "en";
                    break;
            }

            if (_lang == "en" && !infoYG.languages.en)
                _lang = "ru";
            else if (_lang == "ru" && !infoYG.languages.ru)
                _lang = "en";

            savesData.language = _lang;
            lang = _lang;
            SwitchLangEvent?.Invoke(_lang);
        }
    }
}
