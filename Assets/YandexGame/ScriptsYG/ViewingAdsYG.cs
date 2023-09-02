using System;
using UnityEngine;
using UnityEngine.Events;
using UnityToolbag;

namespace YG
{
    [HelpURL("https://www.notion.so/PluginYG-d457b23eee604b7aa6076116aab647ed#facf33554b8f478d9b03656f789cc38a")]
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

        private static bool audioPauseOnAd;
        private static float timeScaleOnAd;
        private static bool cursorVisibleOnAd;
        private static CursorLockMode cursorLockModeOnAd;
        private static bool start;

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
            YandexGame.OpenFullAdEvent += OpenFullscreenAd;
            YandexGame.CloseFullAdEvent += CloseFullscreenAd;
            YandexGame.OpenVideoEvent += OpenRewardedAd;
            YandexGame.CloseVideoEvent += CloseRewardedAd;
        }

        private void OnDisable()
        {
            YandexGame.OpenFullAdEvent -= OpenFullscreenAd;
            YandexGame.CloseFullAdEvent -= CloseFullscreenAd;
            YandexGame.OpenVideoEvent -= OpenRewardedAd;
            YandexGame.CloseVideoEvent -= CloseRewardedAd;
        }

        private void OpenFullscreenAd() => Pause(true);
        private void CloseFullscreenAd() => Pause(false);
        private void OpenRewardedAd() => Pause(true);
        private void CloseRewardedAd() => Pause(false);

        private void Pause(bool pause)
        {
            if (pauseType != PauseType.NothingToControl)
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

            if (pause) customEvents.OpenAd.Invoke();
            else customEvents.CloseAd.Invoke();
        }
    }
}
