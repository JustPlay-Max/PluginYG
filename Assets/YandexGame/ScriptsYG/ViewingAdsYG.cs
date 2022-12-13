using System;
using UnityEngine;
using UnityToolbag;
using static YG.ViewingAdsYG;

namespace YG
{
    public class ViewingAdsYG : MonoBehaviour
    {
        //[Serializable]
        //public class AudioPause
        //{
        //    public bool audioPauseActive;
            
        //    public enum PauseMethod { RememberPreviousState, CustomState };
        //    [Tooltip("RememberPreviousState - Ставить звук на паузу при открытии рекламы. После закрытия рекламы звук перейдёт в изначальное значение (до открытия рекламы).\n CustomState - Укажите свои значения, которые будут выставляться при открытии и закрытии рекламы")]
        //    public PauseMethod pauseMethod;

        //    [ConditionallyVisible(nameof(audioPauseActive))]
        //    public bool AudioPauseByOpenAd = true, AudioPauseByCloseAd;
        //}

        //[Serializable]
        //public class TimeScalePause
        //{
        //    public enum PauseMethod { RememberPreviousState, CustomState };
        //    [Tooltip("RememberPreviousState - Ставить паузу при открытии рекламы. После закрытия рекламы временная шкала перейдёт в изначальное значение (до открытия рекламы).\n CustomState - Укажите свои значения, которые будут выставляться при открытии и закрытии рекламы")]
        //    public PauseMethod pauseMethod;

        //    [ConditionallyVisible(nameof(pauseMethod))]
        //    public float timeScaleByOpenAd, TimeScaleByCloseAd = 1;
        //}

        //[Serializable]
        //public class CursorVisible
        //{
        //    public enum PauseMethod { RememberPreviousState, CustomState };
        //    [Tooltip("RememberPreviousState - Ставить паузу при открытии рекламы. После закрытия рекламы курсор перейдёт в изначальное значение (до открытия рекламы).\n CustomState - Укажите свои значения, которые будут выставляться при закрытии рекламы")]
        //    public PauseMethod pauseMethod;

        //    public enum CursorVisibleByCloseAd { ShowCursor, HideCursor };
        //    [ConditionallyVisible(nameof(pauseMethod))]
        //    public CursorVisibleByCloseAd cursorVisibleByCloseAd;
        //}

        ////public bool audioPauseActive;
        ////[ConditionallyVisible(nameof(audioPauseActive))]
        //public AudioPause audioPause;

        //public bool timeScalePauseActive;
        //[ConditionallyVisible(nameof(timeScalePauseActive))]
        //public TimeScalePause timeScalePause;

        //public bool cursorVisibleActive;
        //[ConditionallyVisible(nameof(cursorVisibleActive))]
        //public CursorVisible cursorVisible;


        public enum PauseType { AudioPause, TimeScalePause, CursorActivity, All };
        [Tooltip("Данный скрипт будет ставить звук или верменную шкалу на паузу при просмотре рекламы взависимости от выбранной настройки PauseType.\n AudioPause - Ставить звук на паузу.\n TimeScalePause - Останавливать время.\n All - Ставить на паузу и звук и время.")]
        public PauseType pauseType;

        public enum PauseMethod { RememberPreviousState, CustomState };
        [Tooltip("RememberPreviousState - Ставить паузу при открытии рекламы. После закрытия рекламы звук и/или временная шкала придут в изначальное значение (до открытия рекламы).\n CustomState - Укажите свои значения, которые будут выставляться при открытии и закрытии рекламы")]
        public PauseMethod pauseMethod;

        [Header("Значения при открытии рекламы")]
        [ConditionallyVisible(nameof(pauseMethod))]
        public bool openAudioPause = true;
        [ConditionallyVisible(nameof(pauseMethod))]
        public float openTimeScale;

        [Header("Значения при закрытии рекламы")]
        [ConditionallyVisible(nameof(pauseMethod))]
        public bool closeAudioPause;
        [ConditionallyVisible(nameof(pauseMethod))]
        public float closeTimeScale = 1;
        public enum CursorVisible
        {
            [InspectorName("Show Cursor")] Show,
            [InspectorName("Hide Cursor")] Hide
        };
        [Tooltip("Показать или скрыть курсор при закрытии рекламы?")]
        [ConditionallyVisible(nameof(pauseMethod))]
        public CursorVisible cursorVisible;
        [ConditionallyVisible(nameof(pauseMethod))]
        public CursorLockMode cursorLockMode;


        //[Space(7)]
        //public bool cursorActivate;
        //public enum CursorAfterViewingAdMethod { RememberPreviousState, CustomState };
        //public PauseMethod pauseMethod;

        static bool audioPauseOnAd;
        static float timeScaleOnAd;
        static bool cursorVisibleOnAd;
        static CursorLockMode cursorLockModeOnAd;
        static bool start;

        private void Start()
        {
            if (!start)
            {
                start = true;
                audioPauseOnAd = AudioListener.pause;
                timeScaleOnAd = Time.timeScale;
                cursorVisibleOnAd = Cursor.visible;
                cursorLockModeOnAd = Cursor.lockState;
            }
        }

        private void OnEnable()
        {
            YandexGame.OpenFullAdEvent += OpenFullAd;
            YandexGame.CloseFullAdEvent += CloseFullAd;
            YandexGame.OpenVideoEvent += OpenRewAd;
            YandexGame.CloseVideoEvent += CloseRewAd;
        }
        private void OnDisable()
        {
            YandexGame.OpenFullAdEvent -= OpenFullAd;
            YandexGame.CloseFullAdEvent -= CloseFullAd;
            YandexGame.OpenVideoEvent -= OpenRewAd;
            YandexGame.CloseVideoEvent -= CloseRewAd;
        }

        void OpenFullAd()
        {
            Pause(true);
        }

        void CloseFullAd()
        {
            Pause(false);
        }

        void OpenRewAd()
        {
            Pause(true);
        }

        void CloseRewAd()
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

            if (pauseType == PauseType.CursorActivity || pauseType == PauseType.All)
            {
                if (pause)
                {
                    cursorVisibleOnAd = Cursor.visible;
                    cursorLockModeOnAd = Cursor.lockState;

                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
                else
                {
                    if (pauseMethod == PauseMethod.CustomState)
                    {
                        if (cursorVisible == CursorVisible.Hide)
                            Cursor.visible = false;
                        else Cursor.visible = true;

                        Cursor.lockState = cursorLockMode;
                    }
                    else
                    {
                        Cursor.visible = cursorVisibleOnAd;
                        Cursor.lockState = cursorLockModeOnAd;
                    }
                }
            }
        }
    }
}
