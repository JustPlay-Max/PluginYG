using UnityEngine;
using UnityEngine.UI;

namespace YG
{
    public class GetPlayerYG : MonoBehaviour
    {
        public Text textPlayerName;
        public ImageLoadYG imageLoadPlayerPhoto;

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
                    textPlayerName.text = YandexGame.Instance.infoYG.UnauthorizedTextTranslate();
                else if (YandexGame.playerName == "anonymous")
                    textPlayerName.text = YandexGame.Instance.infoYG.IsHiddenTextTranslate();
                else textPlayerName.text = YandexGame.playerName;
            }
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
                    textPlayerName.text = YandexGame.Instance.infoYG.UnauthorizedTextTranslate(lang);
                else if (YandexGame.playerName == "anonymous")
                    textPlayerName.text = YandexGame.Instance.infoYG.IsHiddenTextTranslate(lang);
                else textPlayerName.text = YandexGame.playerName;
            }
        }
    }
}
