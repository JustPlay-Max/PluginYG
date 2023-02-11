using System;
using UnityEngine;
using UnityEngine.Events;
using UnityToolbag;

namespace YG
{
    public class ViewingAdsYG : MonoBehaviour
    {
        public enum CursorVisible
        {
            [InspectorName("Show Cursor")] Show,
            [InspectorName("Hide Cursor")] Hide
        };

        [Serializable]
        public class OpeningADValues
        {
            [Tooltip("Значение временной шкалы при открытии рекламы")]
            public float timeScale;
        }

        [Serializable]
        public class ClosingADValues
        {
            [Tooltip("Значение временной шкалы при закрытии рекламы")]
            public float timeScale = 1;

            [Tooltip("Значение аудио паузы при закрытии рекламы")]
            public bool audioPause;

            [Tooltip("Показать или скрыть курсор при закрытии рекламы?")]
            public CursorVisible cursorVisible;

            [Tooltip("Выберите мод блокировки курсора при закрытии рекламы")]
            public CursorLockMode cursorLockMode;
        }

        [Serializable]
        public class CustomEvents
        {
            public UnityEvent OpenAd;
            public UnityEvent CloseAd;
        }

        public enum PauseType { AudioPause, TimeScalePause, CursorActivity, All, NothingToControl };
        [Tooltip("Данный скрипт будет ставить звук или верменную шкалу на паузу при просмотре рекламы взависимости от выбранной настройки Pause Type.\n •  Audio Pause - Ставить звук на паузу.\n •  Time Scale Pause - Останавливать время.\n •  Cursor Activity - Скрывать курсор.\n •  All - Ставить на паузу и звук и время.\n •  Nothing To Control - Не контролировать никакие параметры (подпишите свои методы в  Custom Events).")]
        public PauseType pauseType;

        public enum PauseMethod { RememberPreviousState, CustomState };
        [Tooltip("RememberPreviousState - Ставить паузу при открытии рекламы. После закрытия рекламы звук, временная шкала, курсор - придут в изначальное значение (до открытия рекламы).\n CustomState - Укажите свои значения, которые будут выставляться при открытии и закрытии рекламы")]
        public PauseMethod pauseMethod;

        [Tooltip("Установить значения при открытии рекламы")]
        [ConditionallyVisible(nameof(pauseMethod))]
        public OpeningADValues openingADValues;

        [Tooltip("Установить значения при закрытии рекламы")]
        [ConditionallyVisible(nameof(pauseMethod))]
        public ClosingADValues closingADValues;

        [Tooltip("Ивенты для кастомных методов")]
        public CustomEvents customEvents;

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
            if (pauseType != PauseType.NothingToControl)
                Pause(true);

            customEvents.OpenAd.Invoke();
        }

        void CloseFullAd()
        {
            if (pauseType != PauseType.NothingToControl)
                Pause(false);

            customEvents.CloseAd.Invoke();
        }

        void OpenRewAd()
        {
            if (pauseType != PauseType.NothingToControl)
                Pause(true);

            customEvents.OpenAd.Invoke();
        }

        void CloseRewAd()
        {
            if (pauseType != PauseType.NothingToControl)
                Pause(false);

            customEvents.CloseAd.Invoke();
        }

        void Pause(bool pause)
        {
            if (pauseType == PauseType.AudioPause || pauseType == PauseType.All)
            {
                if (pauseMethod == PauseMethod.CustomState)
                {
                    if (pause) AudioListener.pause = true;
                    else AudioListener.pause = closingADValues.audioPause;
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
                    if (pause) Time.timeScale = openingADValues.timeScale;
                    else Time.timeScale = closingADValues.timeScale;
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
                        if (closingADValues.cursorVisible == CursorVisible.Hide)
                            Cursor.visible = false;
                        else Cursor.visible = true;

                        Cursor.lockState = closingADValues.cursorLockMode;
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
