using System.Runtime.InteropServices;
using UnityEngine;

namespace YG
{
    public partial class YandexGame
    {
        public static JsonEnvironmentData EnvironmentData = new JsonEnvironmentData();

        // Initialization

        [DllImport("__Internal")]
        private static extern string InitEnvironmentData_js();

        [InitBaisYG]
        public static void InitEnvirData()
        {
#if !UNITY_EDITOR
            Debug.Log("Init Envir inGame");
            string data = InitEnvironmentData_js();
            if (data != "null")
                EnvironmentData = JsonUtility.FromJson<JsonEnvironmentData>(data);
#else
            InitEnvirForEditor();
#endif
        }

        // Requesting Data

        [DllImport("__Internal")]
        private static extern string RequestingEnvironmentData_js(bool sendback);

        public static void RequesEnvirData(bool sendback = true)
        {
#if !UNITY_EDITOR
            RequestingEnvironmentData_js(sendback);
#else
            InitEnvirForEditor();
            GetDataInvoke();
#endif
        }
        public void _RequesEnvirData() => RequesEnvirData(true);

        public void SetEnvirData(string data) 
        {
            EnvironmentData = JsonUtility.FromJson<JsonEnvironmentData>(data);
            GetDataInvoke();
        }


#if UNITY_EDITOR
        private static void InitEnvirForEditor()
        {
            if (Instance.infoYG.playerInfoSimulation.isMobile)
            {
                EnvironmentData.deviceType = "mobile";
                EnvironmentData.isMobile = true;
                EnvironmentData.isTablet = false;
                EnvironmentData.isDesktop = false;
                EnvironmentData.isTV = false;
            }
            else
            {
                EnvironmentData.deviceType = "desktop";
                EnvironmentData.isMobile = false;
                EnvironmentData.isTablet = false;
                EnvironmentData.isDesktop = true;
                EnvironmentData.isTV = false;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ResetStaticEnvir()
        {
            EnvironmentData = new JsonEnvironmentData();
        }
#endif


        public class JsonEnvironmentData
        {
            public string language = "ru";
            public string domain = "ru";
            public string deviceType = "desktop";
            public bool isMobile;
            public bool isDesktop = true;
            public bool isTablet;
            public bool isTV;
            public string appID;
            public string browserLang;
            public string payload;
            public bool promptCanShow;
            public bool reviewCanShow;
            public string platform = "Win32";
            public string browser = "Other";
        }
    }
}
