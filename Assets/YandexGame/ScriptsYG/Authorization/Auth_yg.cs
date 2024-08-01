using System.Runtime.InteropServices;
using UnityEngine;

namespace YG
{
    public partial class YandexGame
    {
        private static string _playerId;
        public static string playerId { get => _playerId; }

        public static string playerName = "unauthorized";
        public static string playerPhoto;
        public static string photoSize;
        public static string payingStatus;

        JsonAuth jsonAuth = new JsonAuth();


        [DllImport("__Internal")]
        private static extern string InitPlayer_js();

        [InitYG]
        public static void InitializationGame()
        {
            photoSize = Instance.infoYG.GetPlayerPhotoSize();
#if !UNITY_EDITOR
            Debug.Log("Init Auth inGame");
            string playerData = InitPlayer_js();
            Instance.SetInitializationSDK(playerData);
#else
            InitPlayerForEditor();
#endif
        }

#if UNITY_EDITOR
        private static void InitPlayerForEditor()
        {
            string auth = "resolved";
            string name = Instance.infoYG.playerInfoSimulation.name;

            if (!Instance.infoYG.playerInfoSimulation.authorized)
            {
                auth = "rejected";
                name = "unauthorized";
            }
            else
            {
                if (!Instance.infoYG.scopes)
                    name = "anonymous";
            }

            JsonAuth playerDataSimulation = new JsonAuth()
            {
                playerAuth = auth,
                playerName = name,
                playerId = Instance.infoYG.playerInfoSimulation.uniqueID,
                playerPhoto = Instance.infoYG.playerInfoSimulation.photo,
                payingStatus = Instance.infoYG.playerInfoSimulation.payingStatus
            };

            string json = JsonUtility.ToJson(playerDataSimulation);
            Instance.SetInitializationSDK(json);
        }
#endif

        [DllImport("__Internal")]
        public static extern void RequestAuth_js(bool sendback);
        public static void RequestAuth(bool sendback = true)
        {
#if !UNITY_EDITOR
            RequestAuth_js(sendback);
#else
            InitPlayerForEditor();
#endif
        }

        public void _RequestAuth() => RequestAuth(true);


        public void SetInitializationSDK(string data)
        {
            if (data == "noData" || data == "" || data == null)
            {
                _playerId = null;
                playerName = "unauthorized";
                playerPhoto = null;
                payingStatus = null;

                RejectedAuthorization.Invoke();
                Debug.LogError("Failed init player data");
                GetDataInvoke();
                return;
            }

            jsonAuth = JsonUtility.FromJson<JsonAuth>(data);

            if (jsonAuth.playerAuth.ToString() == "resolved")
            {
                ResolvedAuthorization.Invoke();
                _auth = true;
            }
            else if (jsonAuth.playerAuth.ToString() == "rejected")
            {
                RejectedAuthorization.Invoke();
                _auth = false;
            }

            _playerId = jsonAuth.playerId.ToString();
            playerName = jsonAuth.playerName.ToString();
            playerPhoto = jsonAuth.playerPhoto.ToString();
            payingStatus = jsonAuth.payingStatus.ToString();

            Message("Authorization - " + _auth);
            GetDataInvoke();
        }


        [DllImport("__Internal")]
        private static extern void OpenAuthDialog();

        public static void AuthDialog()
        {
            if (auth)
                Message("Open Auth Dialog");
            else
                Message("SDK Яндекс Игр предлагает войти в аккаунт только тем пользователям, которые еще не вошли.");

#if !UNITY_EDITOR
            OpenAuthDialog();
#endif
        }
        public void _OpenAuthDialog() => AuthDialog();


        public class JsonAuth
        {
            public string playerAuth;
            public string playerName;
            public string playerId;
            public string playerPhoto;
            public string payingStatus;
        }
    }
}
