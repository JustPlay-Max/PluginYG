using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.Events;
using System;
using UnityEngine.SceneManagement;

namespace YG
{
    [HelpURL("https://ash-message-bf4.notion.site/PluginYG-d457b23eee604b7aa6076116aab647ed")]
    [DefaultExecutionOrder(-100)]
    public partial class YandexGame : MonoBehaviour
    {
        public InfoYG infoYG;
        [Tooltip("Объект YandexGame не будет удаляться при смене сцены. При выборе опции singleton, объект YandexGame необходимо поместить только на одну сцену, которая первая загружается при запуске игры.")]
        public bool singleton;
        [Space(10)]
        public UnityEvent ResolvedAuthorization;
        public UnityEvent RejectedAuthorization;
        [Space(30)]
        public UnityEvent OpenFullscreenAd;
        public UnityEvent CloseFullscreenAd;
        public UnityEvent ErrorFullscreenAd;
        [Space(30)]
        public UnityEvent OpenVideoAd;
        public UnityEvent CloseVideoAd;
        public UnityEvent RewardVideoAd;
        public UnityEvent ErrorVideoAd;
        [Space(30)]
        public UnityEvent PurchaseSuccess;
        public UnityEvent PurchaseFailed;
        [Space(30)]
        public UnityEvent PromptDo;
        public UnityEvent PromptFail;
        public UnityEvent ReviewDo;

        #region Data Fields
        public static bool auth { get => _auth; }
        public static bool SDKEnabled { get => _SDKEnabled; }

        public static bool nowAdsShow
        {
            get
            {
                if (nowFullAd || nowVideoAd)
                    return true;
                else
                    return false;
            }
        }

        private static bool _auth;
        private static bool _SDKEnabled;

        public static bool nowFullAd;
        public static bool nowVideoAd;
        public static YandexGame Instance;
        public static Action onAdNotification;
        public static Action GetDataEvent;
        #endregion Data Fields

        #region Methods
        private void OnEnable()
        {
            if (singleton)
                SceneManager.sceneLoaded += OnSceneLoaded;
#if UNITY_EDITOR
            Application.focusChanged += OnVisibilityGameWindow;
#endif
        }
        private void OnDisable()
        {
            if (singleton)
                SceneManager.sceneLoaded -= OnSceneLoaded;
#if UNITY_EDITOR
            Application.focusChanged -= OnVisibilityGameWindow;
#endif
        }

        private void Awake()
        {
            transform.SetParent(null);
            gameObject.name = "YandexGame";

            if (singleton)
            {
                if (Instance != null)
                {
                    Destroy(gameObject);
                }
                else
                {
                    Instance = this;
                    DontDestroyOnLoad(gameObject);
                }
            }
            else
            {
                Instance = this;
            }

            if (!_SDKEnabled)
            {
                CallInitBaisYG();
                CallInitYG();
                GetPayments();
            }
        }

        [DllImport("__Internal")]
        private static extern void InitGame_js();

        private void Start()
        {
            if (infoYG.AdWhenLoadingScene)
                FullscreenShow();

            if (!_SDKEnabled)
            {
                CallStartYG();
                _SDKEnabled = true;
                GetDataInvoke();
#if !UNITY_EDITOR
                InitGame_js();
#endif
            }
        }

        private static void Message(string message)
        {
#if UNITY_EDITOR
            if (Instance.infoYG.debug)
#endif
                Debug.Log(message);
        }

        public static void GetDataInvoke()
        {
            if (_SDKEnabled)
                GetDataEvent?.Invoke();
        }

        private static bool firstSceneLoad = true;
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (firstSceneLoad)
                firstSceneLoad = false;
            else if (infoYG.AdWhenLoadingScene)
                _FullscreenShow();
        }

        #region For ECS
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ResetStatic()
        {
            _SDKEnabled = false;
            _auth = false;
            playerName = "unauthorized";
            _playerId = null;
            playerPhoto = null;
            photoSize = "medium";
            nowFullAd = false;
            nowVideoAd = false;
            savesData = new SavesYG();
            Instance = null;
            timerShowAd = 0;
            GetDataEvent = null;
            onResetProgress = null;
            SwitchLangEvent = null;
            OpenFullAdEvent = null;
            CloseFullAdEvent = null;
            ErrorFullAdEvent = null;
            OpenVideoEvent = null;
            CloseVideoEvent = null;
            RewardVideoEvent = null;
            ErrorVideoEvent = null;
            onGetLeaderboard = null;
            ReviewSentEvent = null;
            PromptSuccessEvent = null;
            PromptFailEvent = null;
            onAdNotification = null;
        }
#endif
        #endregion For ECS

        #endregion Methods


        // Sending messages

        #region Fullscren Ad Show
        [DllImport("__Internal")]
        private static extern void FullAdShow();

        public void _FullscreenShow()
        {
            if (!nowAdsShow && timerShowAd >= infoYG.fullscreenAdInterval)
            {
                timerShowAd = 0;
                onAdNotification?.Invoke();
#if !UNITY_EDITOR
                FullAdShow();
#else
                Message("Fullscren Ad");
                FullAdInEditor();
#endif
            }
            else
            {
                if (nowAdsShow)
                    Message($"Реклама не может быть открыта во время показа другой рекламы!");
                else
                    Message($"До запроса к показу рекламы в середине игры {(infoYG.fullscreenAdInterval - timerShowAd).ToString("00.0")} сек.");
            }
        }

        public static void FullscreenShow() => Instance._FullscreenShow();

#if UNITY_EDITOR
        private void FullAdInEditor()
        {
            GameObject obj = new GameObject { name = "TestFullAd" };
            DontDestroyOnLoad(obj);
            Insides.CallingAnEvent call = obj.AddComponent(typeof(Insides.CallingAnEvent)) as Insides.CallingAnEvent;
            call.StartCoroutine(call.CallingAd(infoYG.durationOfAdSimulation));
        }
#endif
        #endregion Fullscren Ad Show

        #region Rewarded Video Show
        [DllImport("__Internal")]
        private static extern void RewardedShow(int id);

        public void _RewardedShow(int id)
        {
            Message("Rewarded Ad Show");

            if (!nowFullAd && !nowVideoAd)
            {
                onAdNotification?.Invoke();
#if !UNITY_EDITOR
                RewardedShow(id);
#else
                AdRewardInEditor(id);
#endif
            }
        }

        public static void RewVideoShow(int id) => Instance._RewardedShow(id);

#if UNITY_EDITOR
        private void AdRewardInEditor(int id)
        {
            GameObject obj = new GameObject { name = "TestVideoAd" };
            DontDestroyOnLoad(obj);
            Insides.CallingAnEvent call = obj.AddComponent(typeof(Insides.CallingAnEvent)) as Insides.CallingAnEvent;
            call.StartCoroutine(call.CallingAd(infoYG.durationOfAdSimulation, id));
        }
#endif
        #endregion Rewarded Video Show

        #region URL
        [DllImport("__Internal")]
        private static extern void OpenURL(string url);

        public static void OnURL(string url)
        {
            try
            {
                OpenURL(url);
            }
            catch (Exception error)
            {
                Debug.LogError("The first method of following the link failed! Error:\n" + error + "\nInstead of the first method, let's try to call the second method 'Application.OpenURL'");
                Application.OpenURL(url);
            }
        }

        public void _OnURL_Yandex_DefineDomain(string url)
        {
            url = "https://yandex." + EnvironmentData.domain + "/games/" + url;
            Message("URL Transition (yandexGames.DefineDomain) url: " + url);
#if !UNITY_EDITOR
            if (EnvironmentData.domain != null && EnvironmentData.domain != "")
            {
                OnURL(url);
            }
            else Debug.LogError("OnURL_Yandex_DefineDomain: Domain not defined!");
#else
            Application.OpenURL(url);
#endif
        }

        public void _OnAnyURL(string url)
        {
            Message("Any URL Transition. url: " + url);
#if !UNITY_EDITOR
            OnURL(url);
#else
            Application.OpenURL(url);
#endif
        }
        #endregion URL

        #region Review Show
        [DllImport("__Internal")]
        private static extern void ReviewInternal();

        public void _ReviewShow(bool authDialog)
        {
            Message("Review");
#if !UNITY_EDITOR
            if (authDialog)
            {
                if (_auth) ReviewInternal();
                else _OpenAuthDialog();
            }
            else ReviewInternal();
#else
            ReviewSent("true");
#endif
        }

        public static void ReviewShow(bool authDialog)
        {
            Instance._ReviewShow(authDialog);
        }
        #endregion Review Show

        #region Prompt
        [DllImport("__Internal")]
        private static extern void PromptShowInternal();

        public static void PromptShow()
        {
#if !UNITY_EDITOR
            if (EnvironmentData.promptCanShow)
                PromptShowInternal();
#else
            savesData.promptDone = true;
            SaveProgress();

            Instance.PromptDo?.Invoke();
            PromptSuccessEvent?.Invoke();
#endif
        }
        public void _PromptShow() => PromptShow();
        #endregion Prompt

        #region Sticky Ad
        [DllImport("__Internal")]
        private static extern void StickyAdActivityInternal(bool activity);

        public static void StickyAdActivity(bool activity)
        {
            if (activity) Message("Sticky Ad Show");
            else Message("Sticky Ad Hide");
#if !UNITY_EDITOR
            StickyAdActivityInternal(activity);
#endif
        }

        public void _StickyAdActivity(bool activity) => StickyAdActivity(activity);
        #endregion Sticky Ad

        #region Gameplay Start/Stop
        private static bool gamePlaying;
        public static bool isGamePlaying { get { return gamePlaying; } }
        private static bool saveGameplayState;

        [DllImport("__Internal")]
        private static extern void GameplayStart_js();

        public static void GameplayStart(bool useSaveGameplayState = false)
        {
            if (useSaveGameplayState && (!saveGameplayState || nowAdsShow || !isVisibilityWindowGame))
                return;

            if (!gamePlaying)
            {
                gamePlaying = true;
                Message("Gameplay Start");
#if !UNITY_EDITOR
                GameplayStart_js();
#endif
            }
        }
        public void _GameplayStart() => GameplayStart();

        [DllImport("__Internal")]
        private static extern void GameplayStop_js();

        public static void GameplayStop(bool useSaveGameplayState = false)
        {
            if (useSaveGameplayState && !nowAdsShow && isVisibilityWindowGame)
                saveGameplayState = gamePlaying;

            if (gamePlaying)
            {
                gamePlaying = false;
                Message("Gameplay Stop");
#if !UNITY_EDITOR
                GameplayStop_js();
#endif
            }
        }
        public void _GameplayStop() => GameplayStop();
        #endregion Gameplay Start/Stop

        #region Visibility Window Game
        public static bool isVisibilityWindowGame { get { return visibilityWindowGame; } }
        private static bool visibilityWindowGame = true;

        public static Action<bool> onVisibilityWindowGame;
        public static Action onShowWindowGame, onHideWindowGame;

        public void OnVisibilityGameWindow(string visible)
        {
            if (visible == "true")
            {
                visibilityWindowGame = true;
                GameplayStart(true);

                onVisibilityWindowGame?.Invoke(true);
                onShowWindowGame?.Invoke();
            }
            else
            {
                onVisibilityWindowGame?.Invoke(false);
                onHideWindowGame?.Invoke();

                GameplayStop(true);
                visibilityWindowGame = false;
            }
        }
        public void OnVisibilityGameWindow(bool visible) => OnVisibilityGameWindow(visible ? "true" : "false");
        #endregion Visibility Window Game

        #region Server Time

        [DllImport("__Internal")]
        private static extern IntPtr ServerTime_js();

        public static long ServerTime()
        {
#if UNITY_EDITOR
            return Instance.infoYG.playerInfoSimulation.serverTime;
#else
            IntPtr serverTimePtr = ServerTime_js();
            string serverTimeStr = Marshal.PtrToStringAuto(serverTimePtr);
            if (long.TryParse(serverTimeStr, out long serverTime))
            {
                return serverTime;
            }
            return 0;
#endif
        }
        #endregion Server Time

        #region Fullscreen
#if UNITY_EDITOR
        private static bool isFullscreenEditor;
#endif
        [DllImport("__Internal")]
        private static extern long SetFullscreen_js(bool fullscreen);
        public static void SetFullscreen(bool fullscreen)
        {
            if (isFullscreen != fullscreen)
            {
                Message("Set Fullscreen: " + fullscreen);
#if UNITY_EDITOR
                isFullscreenEditor = fullscreen;
#else
                SetFullscreen_js(fullscreen);
#endif
            }
        }
        public void _SetFullscreen(bool fullscreen) => SetFullscreen(fullscreen);

        [DllImport("__Internal")]
        private static extern bool IsFullscreen_js();
        public static bool isFullscreen
        {
            get
            {
#if UNITY_EDITOR
                return isFullscreenEditor;
#else
                return IsFullscreen_js();
#endif
            }
        }

        #endregion Fullscreen


        // Receiving messages

        #region Fullscren Ad
        public static Action OpenFullAdEvent;
        public void OpenFullAd()
        {
            GameplayStop(true);
            OpenFullscreenAd.Invoke();
            OpenFullAdEvent?.Invoke();
            nowFullAd = true;
        }

        public static Action CloseFullAdEvent;
        public void CloseFullAd(string wasShown)
        {
            nowFullAd = false;
            CloseFullscreenAd.Invoke();
            CloseFullAdEvent?.Invoke();
            timerShowAd = 0;
#if !UNITY_EDITOR
            if (wasShown == "true")
            {
                Message("Closed Ad Interstitial");
            }
            else
            {
                if (infoYG.adDisplayCalls == InfoYG.AdCallsMode.until)
                {
                    Message("Реклама не была показана. Ждём следующего запроса.");
                    ResetTimerFullAd();
                }
                else Message("Реклама не была показана. Следующий запрос через: " + infoYG.fullscreenAdInterval);
            }
#endif
            GameplayStart(true);
        }
        public void CloseFullAd() => CloseFullAd("true");

        public void ResetTimerFullAd()
        {
            timerShowAd = infoYG.fullscreenAdInterval;
        }

        public static Action ErrorFullAdEvent;
        public void ErrorFullAd()
        {
            ErrorFullscreenAd.Invoke();
            ErrorFullAdEvent?.Invoke();
        }
        #endregion Fullscren Ad

        #region Rewarded Video
        private float timeOnOpenRewardedAds;

        public static Action OpenVideoEvent;
        public void OpenVideo()
        {
            GameplayStop(true);
            OpenVideoEvent?.Invoke();
            OpenVideoAd.Invoke();
            nowVideoAd = true;
            timeOnOpenRewardedAds = Time.realtimeSinceStartup;
        }

        public static Action CloseVideoEvent;
        public void CloseVideo()
        {
            nowVideoAd = false;
            CloseVideoAd.Invoke();
            CloseVideoEvent?.Invoke();

            if (rewardAdResult == RewardAdResult.Success)
            {
                RewardVideoAd.Invoke();
                RewardVideoEvent?.Invoke(lastRewardAdID);
            }
            else if (rewardAdResult == RewardAdResult.Error)
            {
                ErrorVideo();
            }

            rewardAdResult = RewardAdResult.None;
            GameplayStart(true);
        }

        public static Action<int> RewardVideoEvent;
        private enum RewardAdResult { None, Success, Error };
        private static RewardAdResult rewardAdResult = RewardAdResult.None;
        private static int lastRewardAdID;

        public void RewardVideo(int id)
        {
            lastRewardAdID = id;
#if UNITY_EDITOR
            if (Instance.infoYG.testErrorOfRewardedAdsInEditor)
                timeOnOpenRewardedAds += Time.realtimeSinceStartup + 1;
            else
                timeOnOpenRewardedAds = 0;
#endif
            rewardAdResult = RewardAdResult.None;

            if (Time.realtimeSinceStartup > timeOnOpenRewardedAds + 0.5f)
            {
                if (Instance.infoYG.rewardedAfterClosing)
                {
                    rewardAdResult = RewardAdResult.Success;
                }
                else
                {
                    RewardVideoAd.Invoke();
                    RewardVideoEvent?.Invoke(id);
                }
            }
            else
            {
                if (Instance.infoYG.rewardedAfterClosing)
                    rewardAdResult = RewardAdResult.Error;
                else
                    ErrorVideo();
            }
        }

        public static Action ErrorVideoEvent;
        public void ErrorVideo()
        {
            ErrorVideoAd.Invoke();
            ErrorVideoEvent?.Invoke();
        }
        #endregion Rewarded Video

        #region Review
        public static Action<bool> ReviewSentEvent;
        public void ReviewSent(string feedbackSent)
        {
            EnvironmentData.reviewCanShow = false;

            bool sent = feedbackSent == "true" ? true : false;
            ReviewSentEvent?.Invoke(sent);
            if (sent) ReviewDo?.Invoke();
        }
        #endregion Review

        #region Prompt
        public static Action PromptSuccessEvent;
        public static Action PromptFailEvent;
        public void OnPromptSuccess()
        {
            savesData.promptDone = true;
            SaveProgress();

            PromptDo?.Invoke();
            PromptSuccessEvent?.Invoke();
            EnvironmentData.promptCanShow = false;
        }

        public void OnPromptFail()
        {
            PromptFail?.Invoke();
            PromptFailEvent?.Invoke();
            EnvironmentData.promptCanShow = false;
        }
        #endregion Prompt


        // The rest

        #region Update
        public static float timerShowAd;
#if !UNITY_EDITOR
        static float timerSaveCloud = 62;
#endif

        private void Update()
        {
            // Таймер для обработки показа Fillscreen рекламы
            timerShowAd += Time.unscaledDeltaTime;

            // Таймер для облачных сохранений
#if !UNITY_EDITOR
            if (infoYG.saveCloud)
                timerSaveCloud += Time.unscaledDeltaTime;
#endif
        }
        #endregion Update

        #region Json
        public class JsonLB
        {
            public string technoName;
            public bool isDefault;
            public bool isInvertSortOrder;
            public int decimalOffset;
            public string type;
            public string entries;
            public int[] ranks;
            public string[] photos;
            public string[] names;
            public int[] scores;
            public string[] uniqueIDs;
        }
        #endregion Json
    }
}
