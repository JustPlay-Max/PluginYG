using UnityEngine;
using UnityToolbag;

namespace YG
{
    public class ViewingAdsYG : MonoBehaviour
    {
        public enum PauseType { AudioPause, TimeScalePause, All };
        [Tooltip("Данный скрипт будет ставить звук или верменную шкалу на паузу при просмотре рекламы взависимости от выбранной настройки PauseType.\n AudioPause - Ставить звук на паузу.\n TimeScalePause - Останавливать время.\n All - Ставить на паузу и звук и время.")]
        public PauseType pauseType;

        public enum PauseMethod { RememberPreviousState, CustomState };
        [Tooltip("RememberPreviousState - Ставить паузу при открытии рекламы. После закрытия рекламы звук и/или временная шкала придут в изначальное значение (до открытия рекламы).\n CustomState - Укажите свои значения, которые будут выставляться при открытии и закрытии рекламы")]
        public PauseMethod pauseMethod;

        [ConditionallyVisible(nameof(pauseMethod))]
        public bool openAudioPause = true, closeAudioPause;
        [ConditionallyVisible(nameof(pauseMethod))]
        public float openTimeScale, closeTimeScale = 1;

        static bool audioPauseOnAd;
        static float timeScaleOnAd;
        static bool start;

        private void Start()
        {
            if (!start)
            {
                start = true;
                audioPauseOnAd = AudioListener.pause;
                timeScaleOnAd = Time.timeScale;
            }
        }

        private void OnEnable()
        {
            YandexGame.OpenFullAdEvent += OpenFullAd;
            YandexGame.CloseFullAdEvent += CloseFullAd;
            YandexGame.OpenVideoEvent += OpenRewAd;
            YandexGame.CloseVideoEvent += CloseRewAd;
            YandexGame.CheaterVideoEvent += CloseRewAdError;
        }
        private void OnDisable()
        {
            YandexGame.OpenFullAdEvent -= OpenFullAd;
            YandexGame.CloseFullAdEvent -= CloseFullAd;
            YandexGame.OpenVideoEvent -= OpenRewAd;
            YandexGame.CloseVideoEvent -= CloseRewAd;
            YandexGame.CheaterVideoEvent -= CloseRewAdError;
        }

        void OpenFullAd()
        {
            Pause(true);
        }

        void CloseFullAd()
        {
            Pause(false);
        }

        void OpenRewAd(int ID)
        {
            Pause(true);
        }

        void CloseRewAd(int ID)
        {
            Pause(false);
        }

        void CloseRewAdError()
        {
            Pause(false);
        }

        void Pause(bool pause)
        {
            if (pauseType == PauseType.AudioPause || pauseType == PauseType.All)
            {
                if (pauseMethod == PauseMethod.CustomState)
                {
                    if (pause) AudioListener.pause = openAudioPause;
                    else AudioListener.pause = closeAudioPause;
                }
                else
                {
                    if (pause)
                    {
                        audioPauseOnAd = AudioListener.pause;
                        AudioListener.pause = true;
                    }
                    else AudioListener.pause = audioPauseOnAd;
                }
            }

            if (pauseType == PauseType.TimeScalePause || pauseType == PauseType.All)
            {
                if (pauseMethod == PauseMethod.CustomState)
                {
                    if (pause) Time.timeScale = openTimeScale;
                    else Time.timeScale = closeTimeScale;
                }
                else
                {
                    if (pause)
                    {
                        timeScaleOnAd = Time.timeScale;
                        Time.timeScale = 0;
                    }
                    else Time.timeScale = timeScaleOnAd;
                }
            }
        }
    }
}
