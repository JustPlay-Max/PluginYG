using UnityEngine;
#if YG_TEXT_MESH_PRO
using TMPro;
#endif

namespace YG.Utils.Lang
{
    public class LangMethods : MonoBehaviour
    {
        public static bool[] LangArr(InfoYG inf)
        {
            bool[] b = new bool[27];

            b[0] = inf.languages.ru;
            b[1] = inf.languages.en;
            b[2] = inf.languages.tr;
            b[3] = inf.languages.az;
            b[4] = inf.languages.be;
            b[5] = inf.languages.he;
            b[6] = inf.languages.hy;
            b[7] = inf.languages.ka;
            b[8] = inf.languages.et;
            b[9] = inf.languages.fr;
            b[10] = inf.languages.kk;
            b[11] = inf.languages.ky;
            b[12] = inf.languages.lt;
            b[13] = inf.languages.lv;
            b[14] = inf.languages.ro;
            b[15] = inf.languages.tg;
            b[16] = inf.languages.tk;
            b[17] = inf.languages.uk;
            b[18] = inf.languages.uz;
            b[19] = inf.languages.es;
            b[20] = inf.languages.pt;
            b[21] = inf.languages.ar;
            b[22] = inf.languages.id;
            b[23] = inf.languages.ja;
            b[24] = inf.languages.it;
            b[25] = inf.languages.de;
            b[26] = inf.languages.hi;

            return b;
        }

        public static string LangName(int i)
        {
            if (i == 0) return "ru";
            else if (i == 1) return "en";
            else if (i == 2) return "tr";
            else if (i == 3) return "az";
            else if (i == 4) return "be";
            else if (i == 5) return "he";
            else if (i == 6) return "hy";
            else if (i == 7) return "ka";
            else if (i == 8) return "et";
            else if (i == 9) return "fr";
            else if (i == 10) return "kk";
            else if (i == 11) return "ky";
            else if (i == 12) return "lt";
            else if (i == 13) return "lv";
            else if (i == 14) return "ro";
            else if (i == 15) return "tg";
            else if (i == 16) return "tk";
            else if (i == 17) return "uk";
            else if (i == 18) return "uz";
            else if (i == 19) return "es";
            else if (i == 20) return "pt";
            else if (i == 21) return "ar";
            else if (i == 22) return "id";
            else if (i == 23) return "ja";
            else if (i == 24) return "it";
            else if (i == 25) return "de";
            else if (i == 26) return "hi";
            else return null;
        }

        public static Font[] GetFont(int i, InfoYG inf)
        {
            if (i == 0) return inf.fonts.ru;
            else if (i == 1) return inf.fonts.en;
            else if (i == 2) return inf.fonts.tr;
            else if (i == 3) return inf.fonts.az;
            else if (i == 4) return inf.fonts.be;
            else if (i == 5) return inf.fonts.he;
            else if (i == 6) return inf.fonts.hy;
            else if (i == 7) return inf.fonts.ka;
            else if (i == 8) return inf.fonts.et;
            else if (i == 9) return inf.fonts.fr;
            else if (i == 10) return inf.fonts.kk;
            else if (i == 11) return inf.fonts.ky;
            else if (i == 12) return inf.fonts.lt;
            else if (i == 13) return inf.fonts.lv;
            else if (i == 14) return inf.fonts.ro;
            else if (i == 15) return inf.fonts.tg;
            else if (i == 16) return inf.fonts.tk;
            else if (i == 17) return inf.fonts.uk;
            else if (i == 18) return inf.fonts.uz;
            else if (i == 19) return inf.fonts.es;
            else if (i == 20) return inf.fonts.pt;
            else if (i == 21) return inf.fonts.ar;
            else if (i == 22) return inf.fonts.id;
            else if (i == 23) return inf.fonts.ja;
            else if (i == 24) return inf.fonts.it;
            else if (i == 25) return inf.fonts.de;
            else if (i == 26) return inf.fonts.hi;
            else return null;
        }

#if YG_TEXT_MESH_PRO
        public static TMP_FontAsset[] GetFontTMP(int i, InfoYG inf)
        {
            if (i == 0) return inf.fontsTMP.ru;
            else if (i == 1) return inf.fontsTMP.en;
            else if (i == 2) return inf.fontsTMP.tr;
            else if (i == 3) return inf.fontsTMP.az;
            else if (i == 4) return inf.fontsTMP.be;
            else if (i == 5) return inf.fontsTMP.he;
            else if (i == 6) return inf.fontsTMP.hy;
            else if (i == 7) return inf.fontsTMP.ka;
            else if (i == 8) return inf.fontsTMP.et;
            else if (i == 9) return inf.fontsTMP.fr;
            else if (i == 10) return inf.fontsTMP.kk;
            else if (i == 11) return inf.fontsTMP.ky;
            else if (i == 12) return inf.fontsTMP.lt;
            else if (i == 13) return inf.fontsTMP.lv;
            else if (i == 14) return inf.fontsTMP.ro;
            else if (i == 15) return inf.fontsTMP.tg;
            else if (i == 16) return inf.fontsTMP.tk;
            else if (i == 17) return inf.fontsTMP.uk;
            else if (i == 18) return inf.fontsTMP.uz;
            else if (i == 19) return inf.fontsTMP.es;
            else if (i == 20) return inf.fontsTMP.pt;
            else if (i == 21) return inf.fontsTMP.ar;
            else if (i == 22) return inf.fontsTMP.id;
            else if (i == 23) return inf.fontsTMP.ja;
            else if (i == 24) return inf.fontsTMP.it;
            else if (i == 25) return inf.fontsTMP.de;
            else if (i == 26) return inf.fontsTMP.hi;
            else return null;
        }
#endif

        public static int[] GetFontSize(int i, InfoYG inf)
        {
            if (i == 0) return inf.fontsSizeCorrect.ru;
            else if (i == 1) return inf.fontsSizeCorrect.en;
            else if (i == 2) return inf.fontsSizeCorrect.tr;
            else if (i == 3) return inf.fontsSizeCorrect.az;
            else if (i == 4) return inf.fontsSizeCorrect.be;
            else if (i == 5) return inf.fontsSizeCorrect.he;
            else if (i == 6) return inf.fontsSizeCorrect.hy;
            else if (i == 7) return inf.fontsSizeCorrect.ka;
            else if (i == 8) return inf.fontsSizeCorrect.et;
            else if (i == 9) return inf.fontsSizeCorrect.fr;
            else if (i == 10) return inf.fontsSizeCorrect.kk;
            else if (i == 11) return inf.fontsSizeCorrect.ky;
            else if (i == 12) return inf.fontsSizeCorrect.lt;
            else if (i == 13) return inf.fontsSizeCorrect.lv;
            else if (i == 14) return inf.fontsSizeCorrect.ro;
            else if (i == 15) return inf.fontsSizeCorrect.tg;
            else if (i == 16) return inf.fontsSizeCorrect.tk;
            else if (i == 17) return inf.fontsSizeCorrect.uk;
            else if (i == 18) return inf.fontsSizeCorrect.uz;
            else if (i == 19) return inf.fontsSizeCorrect.es;
            else if (i == 20) return inf.fontsSizeCorrect.pt;
            else if (i == 21) return inf.fontsSizeCorrect.ar;
            else if (i == 22) return inf.fontsSizeCorrect.id;
            else if (i == 23) return inf.fontsSizeCorrect.ja;
            else if (i == 24) return inf.fontsSizeCorrect.it;
            else if (i == 25) return inf.fontsSizeCorrect.de;
            else if (i == 26) return inf.fontsSizeCorrect.hi;
            else return null;
        }

        public static string UnauthorizedTextTranslate(InfoYG inf)
        {
            string lang = YandexGame.EnvironmentData.language;
            if (inf.LocalizationEnable)
                lang = YandexGame.savesData.language;

            return UnauthorizedTextTranslate(lang);
        }

        public static string UnauthorizedTextTranslate(string languageTranslate)
        {
            string name;

            switch (languageTranslate)
            {
                case "ru":
                    name = "неавторизованный";
                    break;
                case "en":
                    name = "unauthorized";
                    break;
                case "tr":
                    name = "yetkisiz";
                    break;
                case "az":
                    name = "icazəsiz";
                    break;
                case "be":
                    name = "неаўтарызаваны";
                    break;
                case "he":
                    name = "---";
                    break;
                case "hy":
                    name = "---";
                    break;
                case "ka":
                    name = "---";
                    break;
                case "et":
                    name = "loata";
                    break;
                case "fr":
                    name = "non autorisé";
                    break;
                case "kk":
                    name = "рұқсат етілмеген";
                    break;
                case "ky":
                    name = "уруксатсыз";
                    break;
                case "lt":
                    name = "neleistinas";
                    break;
                case "lv":
                    name = "neleistinas";
                    break;
                case "ro":
                    name = "neautorizat";
                    break;
                case "tg":
                    name = "беиҷозат";
                    break;
                case "tk":
                    name = "yetkisiz";
                    break;
                case "uk":
                    name = "несанкціонований";
                    break;
                case "uz":
                    name = "ruxsatsiz";
                    break;
                case "es":
                    name = "autorizado";
                    break;
                case "pt":
                    name = "autorizado";
                    break;
                case "ar":
                    name = "---";
                    break;
                case "id":
                    name = "tidak sah";
                    break;
                case "ja":
                    name = "---";
                    break;
                case "it":
                    name = "autorizzato";
                    break;
                case "de":
                    name = "unerlaubter";
                    break;
                case "hi":
                    name = "---";
                    break;
                default:
                    name = "unauthorized";
                    break;

            }
            return name;
        }

        public static string IsHiddenTextTranslate(InfoYG inf)
        {
            string lang = YandexGame.EnvironmentData.language;
            if (inf.LocalizationEnable)
                lang = YandexGame.savesData.language;

            return IsHiddenTextTranslate(lang);
        }

        public static string IsHiddenTextTranslate(string languageTranslate)
        {
            string name;

            switch (languageTranslate)
            {
                case "ru":
                    name = "скрыт";
                    break;
                case "en":
                    name = "is hidden";
                    break;
                case "tr":
                    name = "gizli";
                    break;
                case "az":
                    name = "gizlidir";
                    break;
                case "be":
                    name = "скрыты";
                    break;
                case "he":
                    name = "---";
                    break;
                case "hy":
                    name = "---";
                    break;
                case "ka":
                    name = "---";
                    break;
                case "et":
                    name = "on peidetud";
                    break;
                case "fr":
                    name = "est caché";
                    break;
                case "kk":
                    name = "жасырылған";
                    break;
                case "ky":
                    name = "жашыруун";
                    break;
                case "lt":
                    name = "yra paslėpta";
                    break;
                case "lv":
                    name = "ir paslēpts";
                    break;
                case "ro":
                    name = "este ascuns";
                    break;
                case "tg":
                    name = "пинҳон аст";
                    break;
                case "tk":
                    name = "gizlenendir";
                    break;
                case "uk":
                    name = "прихований";
                    break;
                case "uz":
                    name = "yashiringan";
                    break;
                case "es":
                    name = "Está oculto";
                    break;
                case "pt":
                    name = "está escondido";
                    break;
                case "ar":
                    name = "---";
                    break;
                case "id":
                    name = "tersembunyi";
                    break;
                case "ja":
                    name = "---";
                    break;
                case "it":
                    name = "è nascosto";
                    break;
                case "de":
                    name = "ist versteckt";
                    break;
                case "hi":
                    name = "---";
                    break;
                default:
                    name = "is hidden";
                    break;
            }
            return name;
        }
    }
}