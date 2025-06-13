using System;
using System.Runtime.InteropServices;
using UnityEngine;
using YG.Utils.OpenURL;

namespace YG
{
    public partial class YandexGame
    {
        public static string developerURL;
        public static GameInfo[] allGames = new GameInfo[0];

        class JsonAllGames
        {
            public int[] appID;
            public string[] title;
            public string[] url;
            public string[] coverURL;
            public string[] iconURL;
            public string developerURL;
        }

        [DllImport("__Internal")]
        private static extern string GetAllGames_js();

        [InitYG]
        public static void GetAllGamesInit()
        {
#if UNITY_EDITOR
            allGames = Instance.infoYG.OpenURL.allGames;
            developerURL = Instance.infoYG.OpenURL.developerURL;
#else
            string jsonAllGamesStr = GetAllGames_js();
            if (jsonAllGamesStr == "no data")
                return;

            JsonAllGames jsonAllGames = JsonUtility.FromJson<JsonAllGames>(jsonAllGamesStr);

            allGames = new GameInfo[jsonAllGames.appID.Length];

            for (int i = 0; i < jsonAllGames.appID.Length; i++)
            {
                allGames[i] = new GameInfo();
                allGames[i].appID = jsonAllGames.appID[i];
                allGames[i].title = jsonAllGames.title[i];
                allGames[i].url = jsonAllGames.url[i];
                allGames[i].coverURL = jsonAllGames.coverURL[i];
                allGames[i].iconURL = jsonAllGames.iconURL[i];
            }
            developerURL = jsonAllGames.developerURL;
#endif
        }

        public static GameInfo GetGameByID(int appID)
        {
            for (int i = 0; i < allGames.Length; i++)
            {
                if (allGames[i].appID == appID)
                    return allGames[i];
            }
            return null;
        }

        public static void OnDeveloperURL()
        {
            OnURL(developerURL);
        }

        public static void OnGameURL(int appID)
        {
            OnURL(GetGameByID(appID).url);
        }
    }
}

namespace YG.Utils.OpenURL
{
    [Serializable]
    public class GameInfo
    {
        public int appID = 0;
        public string title = string.Empty;
        public string url = string.Empty;
        public string coverURL = string.Empty;
        public string iconURL = string.Empty;
        public enum ImageURL { Icon, Cover }
    }
}