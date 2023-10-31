using UnityEngine;
using UnityEngine.UI;
using YG.Utils.Lang;
#if YG_TEXT_MESH_PRO
using TMPro;
#endif

namespace YG
{
    public class GetPlayerYG : MonoBehaviour
    {
        public Text textPlayerName;
#if YG_TEXT_MESH_PRO
        public TMP_Text TMPPlayerName;
#endif
        public ImageLoadYG imageLoadPlayerPhoto;

        private InfoYG info { get => YandexGame.Instance.infoYG; }

        private void OnEnable()
        {
            YandexGame.GetDataEvent += GetPlayerData;
            YandexGame.SwitchLangEvent += GetName;

            if (YandexGame.SDKEnabled == true)
            {
                GetPlayerData();
            }
        }

        private void OnDisable()
        {
            YandexGame.GetDataEvent -= GetPlayerData;
            YandexGame.SwitchLangEvent -= GetName;
        }

        void GetPlayerData()
        {
            GetPhoto();

            if (textPlayerName != null)
            {
                if (YandexGame.playerName == "unauthorized")
                    textPlayerName.text = LangMethods.UnauthorizedTextTranslate(info);
                else if (YandexGame.playerName == "anonymous")
                    textPlayerName.text = LangMethods.IsHiddenTextTranslate(info);
                else textPlayerName.text = YandexGame.playerName;
            }
#if YG_TEXT_MESH_PRO
            else if (TMPPlayerName != null)
            {
                if (YandexGame.playerName == "unauthorized")
                    TMPPlayerName.text = LangMethods.UnauthorizedTextTranslate(info);
                else if (YandexGame.playerName == "anonymous")
                    TMPPlayerName.text = LangMethods.IsHiddenTextTranslate(info);
                else TMPPlayerName.text = YandexGame.playerName;
            }
#endif
        }

        void GetPhoto()
        {
            if (imageLoadPlayerPhoto != null && YandexGame.auth)
                imageLoadPlayerPhoto.Load(YandexGame.playerPhoto);
        }

        public void GetName(string lang)
        {
            if (textPlayerName != null)
            {
                if (YandexGame.playerName == "unauthorized")
                    textPlayerName.text = LangMethods.UnauthorizedTextTranslate(lang);
                else if (YandexGame.playerName == "anonymous")
                    textPlayerName.text = LangMethods.IsHiddenTextTranslate(lang);
                else textPlayerName.text = YandexGame.playerName;
            }
#if YG_TEXT_MESH_PRO
            if (TMPPlayerName != null)
            {
                if (YandexGame.playerName == "unauthorized")
                    TMPPlayerName.text = LangMethods.UnauthorizedTextTranslate(lang);
                else if (YandexGame.playerName == "anonymous")
                    TMPPlayerName.text = LangMethods.IsHiddenTextTranslate(lang);
                else TMPPlayerName.text = YandexGame.playerName;
            }
#endif
        }
    }
}
