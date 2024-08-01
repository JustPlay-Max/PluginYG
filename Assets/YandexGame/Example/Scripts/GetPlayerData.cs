using UnityEngine;
using UnityEngine.UI;

namespace YG.Example
{
    public class GetPlayerData : MonoBehaviour
    {
        [SerializeField] ImageLoadYG imageLoad;
        [SerializeField] Text textPlayerData;
        [SerializeField] Text textEnvirData;

        private void OnEnable() => YandexGame.GetDataEvent += DebugData;
        private void OnDisable() => YandexGame.GetDataEvent -= DebugData;

        private void Awake()
        {
            if (YandexGame.SDKEnabled)
            {
                DebugData();
            }
        }

        void DebugData()
        {
            string playerId = YandexGame.playerId;
            if (playerId.Length > 7)
                playerId = playerId.Remove(7) + "...";

            textPlayerData.text = "playerName - " + YandexGame.playerName +
                "\nplayerId - " + playerId +
                "\nphotoSize - " + YandexGame.photoSize +
                "\nauth - " + YandexGame.auth +
                "\npayingStatus - " + YandexGame.payingStatus;

            if (imageLoad != null && YandexGame.auth)
                imageLoad.Load(YandexGame.playerPhoto);

            textEnvirData.text = "domain - " + YandexGame.EnvironmentData.domain +
                "\ndeviceType - " + YandexGame.EnvironmentData.deviceType +
                "\nisMobile - " + YandexGame.EnvironmentData.isMobile +
                "\nisDesktop - " + YandexGame.EnvironmentData.isDesktop +
                "\nisTablet - " + YandexGame.EnvironmentData.isTablet +
                "\nisTV - " + YandexGame.EnvironmentData.isTV +
                "\nisTablet - " + YandexGame.EnvironmentData.isTablet +
                "\nappID - " + YandexGame.EnvironmentData.appID +
                "\nlang (sort) - " + YandexGame.lang +
                "\nenvir lang - " + YandexGame.EnvironmentData.language +
                "\nbrowserLang - " + YandexGame.EnvironmentData.browserLang +
                "\npayload - " + YandexGame.EnvironmentData.payload +
                "\npromptCanShow - " + YandexGame.EnvironmentData.promptCanShow +
                "\nreviewCanShow - " + YandexGame.EnvironmentData.reviewCanShow +
                "\nplatform - " + YandexGame.EnvironmentData.platform +
                "\nbrowser - " + YandexGame.EnvironmentData.browser +
                "\nisFullscreen - " + YandexGame.isFullscreen;
        }
    }
}