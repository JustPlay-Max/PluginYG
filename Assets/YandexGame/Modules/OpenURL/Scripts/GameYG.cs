using UnityEngine;
using UnityEngine.UI;
using UnityToolbag;
using YG.Utils.OpenURL;
#if YG_TEXT_MESH_PRO
using TMPro;
#endif

namespace YG
{
    public class GameYG : MonoBehaviour
    {
        public bool loadByAppID;
        [ConditionallyVisible(nameof(loadByAppID))]
        public int appID;
        [ConditionallyVisible(nameof(loadByAppID))]
        public bool deleteObjIfNull = true;
        [ConditionallyVisible(nameof(loadByAppID))]
        public GameInfo.ImageURL imageURLType;

        public ImageLoadYG imageLoad;
        public Text title;
#if YG_TEXT_MESH_PRO
        public TextMeshProUGUI titleTMP;
#endif
        private string url;

        private void Start()
        {
            if (loadByAppID)
            {
                GameInfo data = YandexGame.GetGameByID(appID);

                if (data != null)
                    Setup(data, imageURLType);
                else if (deleteObjIfNull)
                    Destroy(gameObject);
            }
        }

        public void Setup(GameInfo data, GameInfo.ImageURL imageURL)
        {
            appID = data.appID;
            url = data.url;

            if (title)
                title.text = data.title;
#if YG_TEXT_MESH_PRO
            if (titleTMP)
                titleTMP.text = data.title;
#endif
            if (imageLoad)
            {
                if (imageURL == GameInfo.ImageURL.Icon)
                    imageLoad.Load(data.iconURL);
                else if (imageURL == GameInfo.ImageURL.Cover)
                    imageLoad.Load(data.coverURL);
            }
        }

        public void OnGameURL()
        {
            YandexGame.OnURL(url);
        }
    }
}