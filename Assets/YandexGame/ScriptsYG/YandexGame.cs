using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.Events;
using System;
using UnityEngine.SceneManagement;
using YG.Utils.LB;

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
        public static bool initializedLB { get => _initializedLB; }

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
        private static bool _initializedLB;

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
        }
        private void OnDisable()
        {
            if (singleton)
                SceneManager.sceneLoaded -= OnSceneLoaded;
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
                if (infoYG.leaderboardEnable)
                {
#if !UNITY_EDITOR
                    Debug.Log("Init Leaderbords inGame");
                    _InitLeaderboard();
#else
                    InitializedLB();
#endif
                }

                CallStartYG();
                _SDKEnabled = true;
                GetDataInvoke();
#if !UNITY_EDITOR
                InitGame_js();
#endif
            }
        }

        static void Message(string message)
        {
            if (Instance.infoYG.debug)
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
            _initializedLB = false;
            _playerName = "unauthorized";
            _playerId = null;
            _playerPhoto = null;
            _photoSize = "medium";
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

        #region Init Leaderboard
        [DllImport("__Internal")]
        private static extern void InitLeaderboard();

        public void _InitLeaderboard()
        {
#if !UNITY_EDITOR
            InitLeaderboard();
#endif
#if UNITY_EDITOR
            Message("Initialization Leaderboards");
            InitializedLB();
#endif
        }
        #endregion Init Leaderboard

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

        #region Leaderboard
        [DllImport("__Internal")]
        private static extern void SetLeaderboardScores(string nameLB, int score);

        public static void NewLeaderboardScores(string nameLB, int score)
        {
            if (Instance.infoYG.leaderboardEnable && auth)
            {
                if (Instance.infoYG.saveScoreAnonymousPlayers == false &&
                    playerName == "anonymous")
                    return;

#if !UNITY_EDITOR
                Message("New Liderboard Record: " + score);
                SetLeaderboardScores(nameLB, score);
#else
                Message($"New Liderboard '{nameLB}' Record: {score}");
#endif
            }
        }

        public static void NewLBScoreTimeConvert(string nameLB, float secondsScore)
        {
            if (Instance.infoYG.leaderboardEnable && auth)
            {
                if (Instance.infoYG.saveScoreAnonymousPlayers == false &&
                    playerName == "anonymous")
                    return;

                int result;
                int indexComma = secondsScore.ToString().IndexOf(",");

                if (secondsScore < 1)
                {
                    Debug.LogError("You can't record a record below zero!");
                    return;
                }
                else if (indexComma <= 0)
                {
                    result = (int)(secondsScore);
                }
                else
                {
                    string rec = secondsScore.ToString();
                    string sec = rec.Remove(indexComma);
                    string milSec = rec.Remove(0, indexComma + 1);
                    if (milSec.Length > 3) milSec = milSec.Remove(3);
                    else if (milSec.Length == 2) milSec += "0";
                    else if (milSec.Length == 1) milSec += "00";
                    rec = sec + milSec;
                    result = int.Parse(rec);
                }

                NewLeaderboardScores(nameLB, result);
            }
        }

        [DllImport("__Internal")]
        private static extern void GetLeaderboardScores(string nameLB, int maxQuantityPlayers, int quantityTop, int quantityAround, string photoSizeLB, bool auth);

        public static void GetLeaderboard(string nameLB, int maxQuantityPlayers, int quantityTop, int quantityAround, string photoSizeLB)
        {
            void NoData()
            {
                LBData lb = new LBData()
                {
                    technoName = nameLB,
                    entries = "no data",
                    players = new LBPlayerData[1]
                    {
                        new LBPlayerData()
                        {
                            name = "no data",
                            photo = null
                        }
                    }
                };
                onGetLeaderboard?.Invoke(lb);
            }

#if !UNITY_EDITOR
            if (Instance.infoYG.leaderboardEnable)
            {
                Message("Get Leaderboard");
                GetLeaderboardScores(nameLB, maxQuantityPlayers, quantityTop, quantityAround, photoSizeLB, _auth);
            }
            else
            {
                NoData();
            }
#else
            Message("Get Leaderboard - " + nameLB);

            if (Instance.infoYG.leaderboardEnable)
            {
                int indexLB = -1;
                LBData[] lb = Instance.infoYG.leaderboardSimulation;
                for (int i = 0; i < lb.Length; i++)
                {
                    if (nameLB == lb[i].technoName)
                    {
                        indexLB = i;
                        break;
                    }
                }

                if (indexLB >= 0)
                    onGetLeaderboard?.Invoke(lb[indexLB]);
                else
                    NoData();
            }
            else
            {
                NoData();
            }
#endif
        }
        #endregion Leaderboard

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


        // Receiving messages

        #region Fullscren Ad
        public static Action OpenFullAdEvent;
        public void OpenFullAd()
        {
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

        #region Leaderboard
        public static Action<LBData> onGetLeaderboard;

        public void LeaderboardEntries(string data)
        {
            JsonLB jsonLB = JsonUtility.FromJson<JsonLB>(data);

            LBData lbData = new LBData()
            {
                technoName = jsonLB.technoName,
                isDefault = jsonLB.isDefault,
                isInvertSortOrder = jsonLB.isInvertSortOrder,
                decimalOffset = jsonLB.decimalOffset,
                type = jsonLB.type,
                entries = jsonLB.entries,
                players = new LBPlayerData[jsonLB.names.Length],
                thisPlayer = null
            };

            for (int i = 0; i < jsonLB.names.Length; i++)
            {
                lbData.players[i] = new LBPlayerData();
                lbData.players[i].name = jsonLB.names[i];
                lbData.players[i].rank = jsonLB.ranks[i];
                lbData.players[i].score = jsonLB.scores[i];
                lbData.players[i].photo = jsonLB.photos[i];
                lbData.players[i].uniqueID = jsonLB.uniqueIDs[i];

                if (jsonLB.uniqueIDs[i] == playerId)
                {
                    lbData.thisPlayer = new LBThisPlayerData
                    {
                        rank = jsonLB.ranks[i],
                        score = jsonLB.scores[i]
                    };
                }
            }

            onGetLeaderboard?.Invoke(lbData);
        }

        public void InitializedLB()
        {
            LBData lb = new LBData()
            {
                entries = "initialized"
            };
            onGetLeaderboard?.Invoke(lb);
            _initializedLB = true;
        }
        #endregion Leaderboard

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
