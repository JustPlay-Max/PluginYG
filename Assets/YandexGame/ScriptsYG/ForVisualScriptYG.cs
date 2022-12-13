using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace YG
{
    public class ForVisualScriptYG : MonoBehaviour
    {
        public YandexGame yandexGame;
        public UnityEvent GetDataEvent;

        [HideInInspector] public bool SDKEnabled;
        [HideInInspector] public bool auth;
        [HideInInspector] public string playerName;
        [HideInInspector] public string playerId;
        [HideInInspector] public bool initializedLB;
        [HideInInspector] public string playerPhoto;
        [HideInInspector] public string photoSize;
        [HideInInspector] public string language;
        [HideInInspector] public string domain;
        [HideInInspector] public string deviceType;
        [HideInInspector] public bool isMobile;
        [HideInInspector] public bool isDesktop;
        [HideInInspector] public bool isTablet;
        [HideInInspector] public bool isTV;
        [HideInInspector] public string appID;
        [HideInInspector] public string browserLang;
        [HideInInspector] public string payload;
        [HideInInspector] public bool promptCanShow;
        [HideInInspector] public bool reviewCanShow;
        [HideInInspector] public bool isFirstSession;
        [HideInInspector] public string languageSaves;
        [HideInInspector] public bool promptDone;

        private void Start()
        {
            if (YandexGame.SDKEnabled)
                GetData();

            else StartCoroutine(Initialization());
        }

        IEnumerator Initialization()
        {
            yield return YandexGame.SDKEnabled;
            yield return new WaitForSecondsRealtime(0.1f);
            GetData();
        }

        void GetData()
        {
            SDKEnabled = YandexGame.SDKEnabled;
            auth = YandexGame.auth;
            playerName = YandexGame.playerName;
            playerId = YandexGame.playerId;
            initializedLB = YandexGame.initializedLB;
            playerPhoto = YandexGame.playerPhoto;
            photoSize = YandexGame.photoSize;
            language = YandexGame.EnvironmentData.language;
            domain = YandexGame.EnvironmentData.domain;
            deviceType = YandexGame.EnvironmentData.deviceType;
            isMobile = YandexGame.EnvironmentData.isMobile;
            isDesktop = YandexGame.EnvironmentData.isDesktop;
            isTablet = YandexGame.EnvironmentData.isTablet;
            isTV = YandexGame.EnvironmentData.isTV;
            appID = YandexGame.EnvironmentData.appID;
            browserLang = YandexGame.EnvironmentData.browserLang;
            payload = YandexGame.EnvironmentData.payload;
            promptCanShow = YandexGame.EnvironmentData.promptCanShow;
            reviewCanShow = YandexGame.EnvironmentData.reviewCanShow;
            isFirstSession = YandexGame.savesData.isFirstSession;
            languageSaves = YandexGame.savesData.language;
            promptDone = YandexGame.savesData.promptDone;

            GetDataEvent.Invoke();
        }
    }
}
