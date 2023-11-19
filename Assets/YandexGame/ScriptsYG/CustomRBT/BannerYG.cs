using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityToolbag;

namespace YG
{
    [HelpURL("https://www.notion.so/PluginYG-d457b23eee604b7aa6076116aab647ed#e53839c6ec8842ef98cedcc2344cbd85")]
    public class BannerYG : MonoBehaviour
    {
        public enum RTBNumber { One, Two };
        [Tooltip("Всего доступно два баннера. Выберите номер данного баннера.")]
        public RTBNumber RTB_Number;
        public enum Device { desktopAndMobile, onlyDesktop, onlyMobile };
        [Tooltip(" Desktop And Mobile - Отображение баннера на всех устройствах.\n Only Desktop - Отображение баннера только на компьютере.\n Only Mobile - Отображение баннера только на мобильных устройствах (телефонах и планшетах).")]
        public Device device;
        [Tooltip(" Минимальный размер блока. RBT-блок не будет меньше установленного значения.\n X - минимальная ширина.\n Y - минимальная высота.")]
        public Vector2 minSize = new Vector2(20, 20);
        [Tooltip("Выполнять код каждые 31сек.")]
        public bool codeExecutionTimer31Sec;

        [HideInInspector]
        public RectTransform rt;
        CanvasScaler scaler;

#if UNITY_EDITOR
        public enum ScaleMode { ConstantPixelSize, ScaleWithScreenSize };
        [Tooltip("Режим масштабирования блоков.\nНастраивайте масштаб здесь, не изменяйте параметры компонентов Canvas'а!")]
        public ScaleMode UIScaleMode;

        [ConditionallyVisible(nameof(UIScaleMode))]
        public Vector2 referenceResolution = new Vector2(1920, 1080);
        public enum MatchMode { Width, Height };
        [ConditionallyVisible(nameof(UIScaleMode))]
        public MatchMode Match;
#else
        static float timerRBT1 = 31;
        static float timerRBT2 = 31;
        bool focus = true;

        void OnApplicationFocus(bool hasFocus)
        {
            focus = hasFocus;
            if (YandexGame.SDKEnabled)
            {
                ActivateRTB();
            }
        }

        void OnApplicationPause(bool isPaused) => focus = !isPaused;

        private void Awake()
        {
            rt = (RectTransform)transform.GetChild(0);
            rt.GetComponent<RawImage>().color = Color.clear;
            rt.pivot = new Vector2(0, 1);
        }

        private void OnEnable()
        {
            YandexGame.GetDataEvent += ActivateRTB;
            YandexGame.OpenFullAdEvent += DeactivateRTB;
            YandexGame.CloseFullAdEvent += ActivateRTB;
            YandexGame.OpenVideoEvent += DeactivateRTB;
            YandexGame.CloseVideoEvent += ActivateRTB;

            DebuggingModeYG.onRBTActivity += ActivityRTB;
            DebuggingModeYG.onRBTRecalculate += RecalculateRect;
            DebuggingModeYG.onRBTExecuteCode += ExecuteCodeRBT;

            focus = true;

            if (YandexGame.SDKEnabled)
                ActivateRTB();
        }

        private void OnDisable()
        {
            YandexGame.GetDataEvent -= ActivateRTB;
            YandexGame.OpenFullAdEvent -= DeactivateRTB;
            YandexGame.CloseFullAdEvent -= ActivateRTB;
            YandexGame.OpenVideoEvent -= DeactivateRTB;
            YandexGame.CloseVideoEvent -= ActivateRTB;

            DebuggingModeYG.onRBTActivity -= ActivityRTB;
            DebuggingModeYG.onRBTRecalculate -= RecalculateRect;
            DebuggingModeYG.onRBTExecuteCode -= ExecuteCodeRBT;

            DeactivateRTB();
        }

        private void OnRectTransformDimensionsChange()
        {
            if (CheckDevice())
            {
                RecalculateRect();
                CancelInvoke("RecalculateRect");
                Invoke("RecalculateRect", 0.3f);
            }
        }

        private void Update()
        {
            if (CheckDevice())
            {
                // Обновление RTB-блоков
                if (codeExecutionTimer31Sec && YandexGame.SDKEnabled && focus && NoAds())
                {
                    if (RTB_Number == RTBNumber.One)
                    {
                        timerRBT1 += Time.unscaledDeltaTime;

                        if (timerRBT1 >= 31)
                        {
                            timerRBT1 = 0;
                            RecalculateRect();
                            ExecuteCodeRTB1();
                        }
                    }
                    else if (RTB_Number == RTBNumber.Two)
                    {
                        timerRBT2 += Time.unscaledDeltaTime;

                        if (timerRBT2 >= 31)
                        {
                            timerRBT2 = 0;
                            RecalculateRect();
                            ExecuteCodeRTB2();
                        }
                    }
                }
            }
        }
#endif
        public void RecalculateRect()
        {
            if (CheckDevice())
            {
                if (!rt)
                    rt = transform.GetChild(0).GetComponent<RectTransform>();

                if (!scaler)
                    scaler = GetComponent<CanvasScaler>();

                float width = rt.rect.width;
                float height = rt.rect.height;

                float left = rt.localPosition.x;
                float top = -rt.localPosition.y;

                if (scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
                {
                    Vector2 multResolution = new Vector2(Screen.width / scaler.referenceResolution.x, Screen.height / scaler.referenceResolution.y);

                    if (scaler.matchWidthOrHeight == 0)
                    {
                        width *= multResolution.x;
                        height *= multResolution.x;
                        left *= multResolution.x;
                        top *= multResolution.x;
                    }
                    else if (scaler.matchWidthOrHeight == 1)
                    {
                        width *= multResolution.y;
                        height *= multResolution.y;
                        left *= multResolution.y;
                        top *= multResolution.y;
                    }
                }

                if (width < minSize.x) width = minSize.x;
                if (height < minSize.y) height = minSize.y;

                width = 100 * width / Screen.width;
                height = 100 * height / Screen.height;
                left = 100 * (Screen.width / 2 + left) / Screen.width;
                top = 100 * (Screen.height / 2 + top) / Screen.height;

                left = Mathf.Clamp(left, 0, 100);
                top = Mathf.Clamp(top, 0, 100);

                string _width = width.ToString().Replace(",", ".") + "%";
                string _height = height.ToString().Replace(",", ".") + "%";
                string _left = left.ToString().Replace(",", ".") + "%";
                string _top = top.ToString().Replace(",", ".") + "%";

                if (RTB_Number == RTBNumber.One) RecalculateRTB1(_width, _height, _left, _top);
                else if (RTB_Number == RTBNumber.Two) RecalculateRTB2(_width, _height, _left, _top);
            }
        }

        public void ActivityRTB(bool state)
        {
            if (CheckDevice())
            {
                if (state)
                    RecalculateRect();

                if (RTB_Number == RTBNumber.One)
                {
                    ActivityRTB1(state);
                    if (PaintBlock())
                        PaintRBT("RTB1");
                }
                else if (RTB_Number == RTBNumber.Two)
                {
                    ActivityRTB2(state);
                    if (PaintBlock())
                        PaintRBT("RTB2");
                }
            }
        }

        bool PaintBlock()
        {
            if (YandexGame.EnvironmentData.payload != "" &&
                YandexGame.EnvironmentData.payload != null &&
                YandexGame.EnvironmentData.payload == DebuggingModeYG.Instance.payloadPassword)
            {
                return true;
            }
            else return false;
        }

        void PaintRBT(string rbt)
        {
            rt.GetComponent<RawImage>().color = Color.blue;
            PaintRBTInternal(rbt);
        }

        void ActivateRTB()
        {
            if (NoAds()) ActivityRTB(true);
        }
        void ActivateRTB(int id) => ActivateRTB();
        void DeactivateRTB() => ActivityRTB(false);
        void DeactivateRTB(int id) => DeactivateRTB();

        public void ExecuteCodeRBT()
        {
            if (RTB_Number == RTBNumber.One) ExecuteCodeRTB1();
            else if (RTB_Number == RTBNumber.Two) ExecuteCodeRTB2();
        }

        [DllImport("__Internal")]
        private static extern void RecalculateRTB1(
            string width,
            string height,
            string left,
            string top);

        [DllImport("__Internal")]
        private static extern void RecalculateRTB2(
            string width,
            string height,
            string left,
            string top);


        [DllImport("__Internal")]
        private static extern void ActivityRTB1(bool state);

        [DllImport("__Internal")]
        private static extern void ActivityRTB2(bool state);


        [DllImport("__Internal")]
        private static extern void ExecuteCodeRTB1();

        [DllImport("__Internal")]
        private static extern void ExecuteCodeRTB2();


        [DllImport("__Internal")]
        private static extern void PaintRBTInternal(string rbt);


        static bool? allowDevice;
        bool CheckDevice()
        {
            if (allowDevice == null)
            {
                bool result = true;

                if (device == Device.onlyDesktop)
                {
                    if (YandexGame.EnvironmentData.isMobile || YandexGame.EnvironmentData.isTablet)
                        result = false;
                }
                else if (device == Device.onlyMobile)
                {
                    if (!YandexGame.EnvironmentData.isMobile && !YandexGame.EnvironmentData.isTablet)
                        result = false;
                }

                allowDevice = result;
                return result;
            }
            else
            {
                return allowDevice.Value;
            }
        }

        bool NoAds()
        {
            if (!YandexGame.nowFullAd && !YandexGame.nowVideoAd)
                return true;
            else return false;
        }
    }
}
