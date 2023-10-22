using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.Events;
using System;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using YG.Utils.LB;
using YG.Utils.Pay;
#if YG_NEWTONSOFT_FOR_SAVES
using Newtonsoft.Json;
#endif

namespace YG
{
    [HelpURL("https://ash-message-bf4.notion.site/PluginYG-d457b23eee604b7aa6076116aab647ed")]
    public class YandexGame : MonoBehaviour
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
        public static bool SDKEnabled { get => _SDKEnabled; }
        public static bool auth { get => _auth; }
        public static bool initializedLB { get => _initializedLB; }
        public static string playerName
        {
            get => _playerName;
            set => _playerName = value;
        }
        public static string playerId { get => _playerId; }
        public static string playerPhoto
        {
            get => _playerPhoto;
            set => _playerPhoto = value;
        }
        public static string photoSize
        {
            get => _photoSize;
            set => _photoSize = value;
        }

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


        static bool _SDKEnabled;
        static bool _startGame;
        static bool _auth;
        static bool _initializedLB;
        static string _playerName = "unauthorized";
        static string _playerId;
        static string _playerPhoto;
        static string _photoSize;
        static bool _leaderboardEnable;
        static bool _debug;
        static bool _scopes;
        public static bool nowFullAd;
        public static bool nowVideoAd;
        public static SavesYG savesData = new SavesYG();
        public static JsonEnvironmentData EnvironmentData = new JsonEnvironmentData();
        public static YandexGame Instance;
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
                if (Instance != null) Destroy(gameObject);
                else
                {
                    Instance = this;
                    DontDestroyOnLoad(gameObject);
                }
            }
            else Instance = this;
        }

        [DllImport("__Internal")]
        private static extern void StaticRBTDeactivate();

        private void Start()
        {
            if (infoYG.AdWhenLoadingScene)
                _FullscreenShow();

#if !UNITY_EDITOR
            if (!infoYG.staticRBTInGame)
                StaticRBTDeactivate();
#endif
        }

        static void Message(string message)
        {
            if (_debug) Debug.Log(message);
        }

        void FirstСalls()
        {
            if (!_startGame)
            {
#if UNITY_EDITOR
                if (Instance.infoYG.playerInfoSimulation.isMobile)
                {
                    EnvironmentData.isMobile = true;
                    EnvironmentData.deviceType = "mobile";
                }
#endif
                _debug = infoYG.debug;
                _leaderboardEnable = infoYG.leaderboardEnable;
                _scopes = infoYG.scopes;
                _startGame = true;

                if (infoYG.playerPhotoSize == InfoYG.PlayerPhotoSize.small)
                    _photoSize = "small";
                else if (infoYG.playerPhotoSize == InfoYG.PlayerPhotoSize.medium)
                    _photoSize = "medium";
                else if (infoYG.playerPhotoSize == InfoYG.PlayerPhotoSize.large)
                    _photoSize = "large";

                InitializationGame();
            }
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
            _startGame = false;
            _auth = false;
            _initializedLB = false;
            _playerName = "unauthorized";
            _playerId = null;
            _playerPhoto = null;
            _photoSize = "medium";
            _leaderboardEnable = false;
            _debug = false;
            _scopes = false;
            nowFullAd = false;
            nowVideoAd = false;
            savesData = new SavesYG();
            EnvironmentData = new JsonEnvironmentData();
            purchases = new Purchase[0];
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
            GetPaymentsEvent = null;
            PurchaseSuccessEvent = null;
            PurchaseFailedEvent = null;
            ReviewSentEvent = null;
            PromptSuccessEvent = null;
            PromptFailEvent = null;
        }
#endif
        #endregion For ECS

        #endregion Methods

        #region Player Data
        static string PATH_SAVES_EDITOR = "/YandexGame/WorkingData/Editor/SavesEditorYG.json";
        public static Action GetDataEvent;

#if UNITY_EDITOR
        public static void SaveEditor()
        {
            Message("Save Editor");
            string path = Application.dataPath + PATH_SAVES_EDITOR;
            string directory = Path.GetDirectoryName(path);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            bool fileExits = false;
            if (File.Exists(path))
                fileExits = true;

#if YG_NEWTONSOFT_FOR_SAVES
            string json = JsonConvert.SerializeObject(savesData, Formatting.Indented);
#else
            string json = JsonUtility.ToJson(savesData, true);
#endif
            File.WriteAllText(path, json);

            if (!fileExits && File.Exists(path))
            {
                UnityEditor.AssetDatabase.Refresh();
                Debug.Log("UnityEditor.AssetDatabase.Refresh");
            }
        }

        public static void LoadEditor()
        {
            Message("Load Editor");

            string path = Application.dataPath + PATH_SAVES_EDITOR;

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
#if YG_NEWTONSOFT_FOR_SAVES
                savesData = JsonConvert.DeserializeObject<SavesYG>(json);
#else
                savesData = JsonUtility.FromJson<SavesYG>(json);
#endif
                AfterLoading();
            }
            else
            {
                ResetSaveProgress();
            }
        }
#endif

        [DllImport("__Internal")]
        private static extern void SaveToLocalStorage(string key, string value);
        public static void SaveLocal()
        {
            Message("Save Local");
#if !UNITY_EDITOR
#if YG_NEWTONSOFT_FOR_SAVES
            SaveToLocalStorage("savesData", JsonConvert.SerializeObject(savesData));
#else
            SaveToLocalStorage("savesData", JsonUtility.ToJson(savesData));
#endif
#endif
        }

        [DllImport("__Internal")]
        private static extern string LoadFromLocalStorage(string key);
        public static void LoadLocal()
        {
            Message("Load Local");

            if (!HasKey("savesData"))
                ResetSaveProgress();
            else
            {
#if YG_NEWTONSOFT_FOR_SAVES
                savesData = JsonConvert.DeserializeObject<SavesYG>(LoadFromLocalStorage("savesData"));
#else
                savesData = JsonUtility.FromJson<SavesYG>(LoadFromLocalStorage("savesData"));
#endif
            }

            AfterLoading();
        }


        [DllImport("__Internal")]
        private static extern int HasKeyInLocalStorage(string key);
        public static bool HasKey(string key)
        {
            try
            {
                return HasKeyInLocalStorage(key) == 1;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }

        }

        [DllImport("__Internal")]
        private static extern void RemoveFromLocalStorage(string key);
        public void RemoveLocalSaves() => RemoveFromLocalStorage("savesData");

        static void AfterLoading()
        {
            _SDKEnabled = true;
            GetDataEvent?.Invoke();

            if (Instance.infoYG.LocalizationEnable &&
                Instance.infoYG.callingLanguageCheck == InfoYG.CallingLanguageCheck.EveryGameLaunch)
            {
                LanguageRequest();
            }
            else
            {
                SwitchLangEvent?.Invoke(savesData.language);
            }
        }

        public static Action onResetProgress;
        public void _ResetSaveProgress()
        {
            Message("Reset Save Progress");
            savesData = new SavesYG { isFirstSession = false };
            _SDKEnabled = true;

            if (infoYG.LocalizationEnable &&
                (infoYG.callingLanguageCheck == InfoYG.CallingLanguageCheck.FirstLaunchOnly ||
                infoYG.callingLanguageCheck == InfoYG.CallingLanguageCheck.EveryGameLaunch))
            {
                LanguageRequest();
            }

            GetDataEvent?.Invoke();
            onResetProgress?.Invoke();

        }
        public static void ResetSaveProgress() => Instance._ResetSaveProgress();

        public void _SaveProgress()
        {
            if (_SDKEnabled)
            {
                savesData.idSave++;
#if !UNITY_EDITOR
                if (!infoYG.saveCloud || (infoYG.saveCloud && infoYG.localSaveSync))
                {
                    SaveLocal();
                }

                if (infoYG.saveCloud && timerSaveCloud >= infoYG.saveCloudInterval + 1)
                {
                    timerSaveCloud = 0;
                    SaveCloud();
                }
#else
                SaveEditor();
#endif
            }
            else Debug.LogError("Данные не могут быть сохранены до инициализации SDK!");
        }
        public static void SaveProgress() => Instance._SaveProgress();

        public void _LoadProgress()
        {
#if !UNITY_EDITOR
            if (!infoYG.saveCloud)
            {
                LoadLocal();
            }
            else LoadCloud();
#else
            LoadEditor();
#endif
        }
        public static void LoadProgress() => Instance._LoadProgress();

        #endregion Player Data        


        // Sending messages

        #region Initialization SDK
        [DllImport("__Internal")]
        private static extern void InitGame_Internal(string playerPhotoSize, bool scopes, bool gameReadyApi);

        public void InitializationGame()
        {
#if !UNITY_EDITOR
            InitGame_Internal(_photoSize, infoYG.scopes, infoYG.autoGameReadyAPI);
#else
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
                playerPhoto = Instance.infoYG.playerInfoSimulation.photo
            };

            string json = JsonUtility.ToJson(playerDataSimulation);
            SetInitializationSDK(json);
#endif
        }

        [DllImport("__Internal")]
        private static extern void GameReadyAPI_Internal();

        public static void GameReadyAPI()
        {
            if (!Instance.infoYG.autoGameReadyAPI)
            {
#if !UNITY_EDITOR
                GameReadyAPI_Internal();
#else
                Message("Game Ready API");
#endif
            }
        }
        public void _GameReadyAPI() => GameReadyAPI();
        #endregion Initialization SDK

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
#endif
        }
        #endregion Init Leaderboard

        #region Open Auth Dialog
        [DllImport("__Internal")]
        private static extern void OpenAuthDialog(string playerPhotoSize, bool scopes);

        public void _OpenAuthDialog()
        {
#if !UNITY_EDITOR
            OpenAuthDialog(_photoSize, _scopes);
#endif
#if UNITY_EDITOR
            Message("Open Auth Dialog");
#endif
        }

        public static void AuthDialog()
        {
#if !UNITY_EDITOR
            OpenAuthDialog(_photoSize, _scopes);
#endif
#if UNITY_EDITOR
            Message("Open Auth Dialog");
#endif
        }
        #endregion Open Auth Dialog

        #region Save end Load Cloud
        [DllImport("__Internal")]
        private static extern void SaveYG(string jsonData, bool flush);

        public static void SaveCloud()
        {
            Message("Save Cloud");
#if YG_NEWTONSOFT_FOR_SAVES
            SaveYG(JsonConvert.SerializeObject(savesData), Instance.infoYG.flush);
#else
            SaveYG(JsonUtility.ToJson(savesData), Instance.infoYG.flush);
#endif
        }

        [DllImport("__Internal")]
        private static extern void LoadYG();

        public static void LoadCloud()
        {
            Message("Load Cloud");
            LoadYG();
        }
        #endregion Save end Load Cloud

        #region Fullscren Ad Show
        [DllImport("__Internal")]
        private static extern void FullAdShow();

        public void _FullscreenShow()
        {
            if (!nowAdsShow && timerShowAd >= infoYG.fullscreenAdInterval)
            {
                timerShowAd = 0;
#if !UNITY_EDITOR
                FullAdShow();
#else
                Message("Fullscren Ad");
                OpenFullAd();
                CloseFullAdInEditor();
#endif
            }
            else
            {
                Message($"До запроса к показу Fullscreen рекламы {(infoYG.fullscreenAdInterval - timerShowAd).ToString("00.0")} сек.");
            }
        }

        public static void FullscreenShow() => Instance._FullscreenShow();

#if UNITY_EDITOR
        void CloseFullAdInEditor()
        {
            GameObject errMessage = new GameObject { name = "TestFullAd" };
            DontDestroyOnLoad(errMessage);

            Canvas canvas = errMessage.AddComponent<Canvas>();
            canvas.sortingOrder = 32767;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            errMessage.AddComponent<GraphicRaycaster>();
            errMessage.AddComponent<RawImage>().color = new Color(0, 1, 0, 0.5f);

            Insides.CallingAnEvent call = errMessage.AddComponent(typeof(Insides.CallingAnEvent)) as Insides.CallingAnEvent;
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
#if !UNITY_EDITOR
                RewardedShow(id);
#else
                OpenVideo();
                CloseVideoInEditor(id);
#endif
            }
        }

        public static void RewVideoShow(int id) => Instance._RewardedShow(id);

#if UNITY_EDITOR
        void CloseVideoInEditor(int id)
        {
            GameObject errMessage = new GameObject { name = "TestVideoAd" };
            DontDestroyOnLoad(errMessage);

            Canvas canvas = errMessage.AddComponent<Canvas>();
            canvas.sortingOrder = 32767;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            errMessage.AddComponent<GraphicRaycaster>();
            errMessage.AddComponent<RawImage>().color = new Color(0, 0, 1, 0.5f);
            DontDestroyOnLoad(errMessage);

            Insides.CallingAnEvent call = errMessage.AddComponent(typeof(Insides.CallingAnEvent)) as Insides.CallingAnEvent;
            call.StartCoroutine(call.CallingAd(infoYG.durationOfAdSimulation, id));
        }
#endif
        #endregion Rewarded Video Show

        #region Language
        [DllImport("__Internal")]
        private static extern void LanguageRequestInternal();

        public void _LanguageRequest()
        {
#if !UNITY_EDITOR
            LanguageRequestInternal();
#else
            string langSimulate = Instance.infoYG.playerInfoSimulation.language;
            if (langSimulate != null && langSimulate != "")
            {
                EnvironmentData.language = langSimulate;
            }
            SetLanguage(EnvironmentData.language);
#endif
        }
        public static void LanguageRequest() => Instance._LanguageRequest();

        public static Action<string> SwitchLangEvent;

        public static void SwitchLanguage(string language)
        {
            savesData.language = language;
            SwitchLangEvent?.Invoke(language);
        }

        public void _SwitchLanguage(string language)
        {
            SwitchLanguage(language);
            SaveProgress();
        }
        #endregion Language

        #region Requesting Environment Data
        [DllImport("__Internal")]
        private static extern void RequestingEnvironmentData();

        public void _RequestingEnvironmentData()
        {
#if !UNITY_EDITOR
            RequestingEnvironmentData();
#endif
        }
        #endregion Requesting Environment Data

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
            if (_leaderboardEnable)
            {
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
            if (_leaderboardEnable)
            {
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
            if (_leaderboardEnable)
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

            if (_leaderboardEnable)
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

        #region Payments
        [DllImport("__Internal")]
        private static extern void BuyPaymentsInternal(string id);

        public static void BuyPayments(string id)
        {
#if !UNITY_EDITOR
            BuyPaymentsInternal(id);
#else
            Message($"Buy Payment. ID: {id}");
            Instance.OnPurchaseSuccess(id);
#endif
        }

        public void _BuyPayments(string id) => BuyPayments(id);


        [DllImport("__Internal")]
        private static extern void GetPaymentsInternal();

        public static void GetPayments()
        {
            Message("Get Payments");
#if !UNITY_EDITOR
            GetPaymentsInternal();
#else
            Instance.PaymentsEntries("");
#endif
        }

        public void _GetPayments() => GetPayments();

        public static Purchase PurchaseByID(string ID)
        {
            for (int i = 0; i < purchases.Length; i++)
            {
                if (purchases[i].id == ID)
                {
                    return purchases[i];
                }
            }

            return null;
        }

        [DllImport("__Internal")]
        private static extern void ConsumePurchaseInternal(string id);

        public static void ConsumePurchaseByID(string id)
        {
#if !UNITY_EDITOR
            ConsumePurchaseInternal(id);
#endif
        }

        [DllImport("__Internal")]
        private static extern void ConsumePurchasesInternal();

        public static void ConsumePurchases()
        {
#if !UNITY_EDITOR
            ConsumePurchasesInternal();
#endif
        }

        #endregion Payments

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
#if !UNITY_EDITOR
            if (wasShown == "true")
            {
                Message("Closed Fullscreen Ad");
            }
            else
            {
                if (infoYG.adDisplayCalls == InfoYG.AdCallsMode.until)
                {
                    Message("The fullscreen ad was not shown! The next time the method is executed to display an ad, the ad will be called because you have selected the method (Until Ad Is Shown)");
                    ResetTimerFullAd();
                }
                else Message("The fullscreen ad was not shown! The next time the method is executed to display an ad, the ad will not be called, since you have selected the method (Resetting Time After Any Ad Display).");
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
            timeOnOpenRewardedAds = Time.unscaledTime;
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
            else if(rewardAdResult == RewardAdResult.Error)
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
            if (!Instance.infoYG.testErrorOfRewardedAdsInEditor)
                timeOnOpenRewardedAds -= 3;
#endif
            rewardAdResult = RewardAdResult.None;

            if (Time.unscaledTime > timeOnOpenRewardedAds + 2)
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

        #region Set Initialization SDK
        JsonAuth jsonAuth = new JsonAuth();

        public void SetInitializationSDK(string data)
        {
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

            _playerName = jsonAuth.playerName.ToString();
            _playerId = jsonAuth.playerId.ToString();
            _playerPhoto = jsonAuth.playerPhoto.ToString();

            Message("Authorization - " + jsonAuth.playerAuth.ToString());

            _RequestingEnvironmentData();
            LoadProgress();
            GetPayments();

            if (_leaderboardEnable)
            {
#if !UNITY_EDITOR
                _InitLeaderboard();
#else
                InitializedLB();
#endif
            }
        }
        #endregion Set Initialization SDK

        #region Loading progress
        enum DataState { Exist, NotExist, Broken };
        public void SetLoadSaves(string data)
        {
            DataState cloudDataState = DataState.Exist;
            DataState localDataState = DataState.Exist;
            SavesYG cloudData = new SavesYG();
            SavesYG localData = new SavesYG();

            if (data != "noData")
            {
                data = data.Remove(0, 2);
                data = data.Remove(data.Length - 2, 2);
                data = data.Replace(@"\\\", '\u0002'.ToString());
                data = data.Replace(@"\", "");
                data = data.Replace('\u0002'.ToString(), @"\");
                try
                {
#if YG_NEWTONSOFT_FOR_SAVES
                    cloudData = JsonConvert.DeserializeObject<SavesYG>(data);
#else
                    cloudData = JsonUtility.FromJson<SavesYG>(data);
#endif
                }
                catch (Exception e)
                {
                    Debug.LogError("Cloud Load Error: " + e.Message);
                    cloudDataState = DataState.Broken;
                }
            }
            else cloudDataState = DataState.NotExist;

            if (infoYG.localSaveSync == false)
            {
                if (cloudDataState == DataState.NotExist)
                {
                    Message("No cloud saves. Local saves are disabled.");
                    ResetSaveProgress();
                }
                else
                {
                    if (cloudDataState == DataState.Broken)
                        Message("Load Cloud Broken! But we tried to restore and load cloud saves. Local saves are disabled.");
                    else Message("Load Cloud Complete! Local saves are disabled.");

                    savesData = cloudData;
                    AfterLoading();
                }
                return;
            }

            if (HasKey("savesData"))
            {
                try
                {
#if YG_NEWTONSOFT_FOR_SAVES
                    localData = JsonConvert.DeserializeObject<SavesYG>(LoadFromLocalStorage("savesData"));
#else
                    localData = JsonUtility.FromJson<SavesYG>(LoadFromLocalStorage("savesData"));
#endif
                }
                catch (Exception e)
                {
                    Debug.LogError("Local Load Error: " + e.Message);
                    localDataState = DataState.Broken;
                }
            }
            else localDataState = DataState.NotExist;

            if (cloudDataState == DataState.Exist && localDataState == DataState.Exist)
            {
                if (cloudData.idSave >= localData.idSave)
                {
                    Message($"Load Cloud Complete! ID Cloud Save: {cloudData.idSave}, ID Local Save: {localData.idSave}");
                    savesData = cloudData;
                }
                else
                {
                    Message($"Load Local Complete! ID Cloud Save: {cloudData.idSave}, ID Local Save: {localData.idSave}");
                    savesData = localData;
                }
                AfterLoading();
            }
            else if (cloudDataState == DataState.Exist)
            {
                savesData = cloudData;
                Message("Load Cloud Complete! Local Data - " + localDataState);
                AfterLoading();
            }
            else if (localDataState == DataState.Exist)
            {
                savesData = localData;
                Message("Load Local Complete! Cloud Data - " + cloudDataState);
                AfterLoading();
            }
            else if (cloudDataState == DataState.Broken ||
                (cloudDataState == DataState.Broken && localDataState == DataState.Broken))
            {
                Message("Local Saves - " + localDataState);
                Message("Cloud Saves - Broken! Data Recovering...");
                ResetSaveProgress();
#if YG_NEWTONSOFT_FOR_SAVES
                savesData = JsonConvert.DeserializeObject<SavesYG>(data);
#else
                savesData = JsonUtility.FromJson<SavesYG>(data);
#endif
                Message("Cloud Saves Partially Restored!");
                AfterLoading();
            }
            else if (localDataState == DataState.Broken)
            {
                Message("Cloud Saves - " + cloudDataState);
                Message("Local Saves - Broken! Data Recovering...");
                ResetSaveProgress();
#if YG_NEWTONSOFT_FOR_SAVES
                savesData = JsonConvert.DeserializeObject<SavesYG>(LoadFromLocalStorage("savesData"));
#else
                savesData = JsonUtility.FromJson<SavesYG>(LoadFromLocalStorage("savesData"));
#endif
                Message("Local Saves Partially Restored!");
                AfterLoading();
            }
            else
            {
                Message("No Saves");
                ResetSaveProgress();
            }
        }
        #endregion Loading progress

        #region Language
        public void SetLanguage(string language)
        {
            string lang = "en";

            switch (language)
            {
                case "ru":
                    if (infoYG.languages.ru)
                        lang = language;
                    break;
                case "en":
                    if (infoYG.languages.en)
                        lang = language;
                    break;
                case "tr":
                    if (infoYG.languages.tr)
                        lang = language;
                    else lang = "ru";
                    break;
                case "az":
                    if (infoYG.languages.az)
                        lang = language;
                    else lang = "en";
                    break;
                case "be":
                    if (infoYG.languages.be)
                        lang = language;
                    else lang = "ru";
                    break;
                case "he":
                    if (infoYG.languages.he)
                        lang = language;
                    else lang = "en";
                    break;
                case "hy":
                    if (infoYG.languages.hy)
                        lang = language;
                    else lang = "en";
                    break;
                case "ka":
                    if (infoYG.languages.ka)
                        lang = language;
                    else lang = "en";
                    break;
                case "et":
                    if (infoYG.languages.et)
                        lang = language;
                    else lang = "en";
                    break;
                case "fr":
                    if (infoYG.languages.fr)
                        lang = language;
                    else lang = "en";
                    break;
                case "kk":
                    if (infoYG.languages.kk)
                        lang = language;
                    else lang = "ru";
                    break;
                case "ky":
                    if (infoYG.languages.ky)
                        lang = language;
                    else lang = "en";
                    break;
                case "lt":
                    if (infoYG.languages.lt)
                        lang = language;
                    else lang = "en";
                    break;
                case "lv":
                    if (infoYG.languages.lv)
                        lang = language;
                    else lang = "en";
                    break;
                case "ro":
                    if (infoYG.languages.ro)
                        lang = language;
                    else lang = "en";
                    break;
                case "tg":
                    if (infoYG.languages.tg)
                        lang = language;
                    else lang = "en";
                    break;
                case "tk":
                    if (infoYG.languages.tk)
                        lang = language;
                    else lang = "en";
                    break;
                case "uk":
                    if (infoYG.languages.uk)
                        lang = language;
                    else lang = "ru";
                    break;
                case "uz":
                    if (infoYG.languages.uz)
                        lang = language;
                    else lang = "ru";
                    break;
                case "es":
                    if (infoYG.languages.es)
                        lang = language;
                    else lang = "en";
                    break;
                case "pt":
                    if (infoYG.languages.pt)
                        lang = language;
                    else lang = "en";
                    break;
                case "ar":
                    if (infoYG.languages.ar)
                        lang = language;
                    else lang = "en";
                    break;
                case "id":
                    if (infoYG.languages.id)
                        lang = language;
                    else lang = "en";
                    break;
                case "ja":
                    if (infoYG.languages.ja)
                        lang = language;
                    else lang = "en";
                    break;
                case "it":
                    if (infoYG.languages.it)
                        lang = language;
                    else lang = "en";
                    break;
                case "de":
                    if (infoYG.languages.de)
                        lang = language;
                    else lang = "en";
                    break;
                case "hi":
                    if (infoYG.languages.hi)
                        lang = language;
                    else lang = "en";
                    break;
                default:
                    lang = "en";
                    break;
            }

            if (lang == "en" && !infoYG.languages.en)
                lang = "ru";
            else if (lang == "ru" && !infoYG.languages.ru)
                lang = "en";

            Message("Language Request: Lang - " + lang);
            savesData.language = lang;
            SwitchLangEvent?.Invoke(lang);
        }
        #endregion Language

        #region Environment Data
        public void SetEnvironmentData(string data)
        {
#if YG_NEWTONSOFT_FOR_SAVES
            EnvironmentData = JsonConvert.DeserializeObject<JsonEnvironmentData>(data);
#else
            EnvironmentData = JsonUtility.FromJson<JsonEnvironmentData>(data);
#endif
        }
        #endregion Environment Data

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

        #region Payments
        public static Action GetPaymentsEvent;
        public static Purchase[] purchases = new Purchase[0];

        public void PaymentsEntries(string data)
        {
#if !UNITY_EDITOR
            JsonPayments paymentsData = JsonUtility.FromJson<JsonPayments>(data);
            purchases = new Purchase[paymentsData.id.Length];

            for (int i = 0; i < purchases.Length; i++)
            {
                purchases[i] = new Purchase();
                purchases[i].id = paymentsData.id[i];
                purchases[i].title = paymentsData.title[i];
                purchases[i].description = paymentsData.description[i];
                purchases[i].imageURI = paymentsData.imageURI[i];
                purchases[i].priceValue = paymentsData.priceValue[i];
                purchases[i].consumed = paymentsData.consumed[i];
            }
#else
            purchases = Instance.infoYG.purshasesSimulation;
#endif
            GetPaymentsEvent?.Invoke();
        }

        public static Action<string> PurchaseSuccessEvent;
        public void OnPurchaseSuccess(string id)
        {
            PurchaseByID(id).consumed = true;
            PurchaseSuccess?.Invoke();
            PurchaseSuccessEvent?.Invoke(id);
        }

        public static Action<string> PurchaseFailedEvent;
        public void OnPurchaseFailed(string id)
        {
            PurchaseFailed?.Invoke();
            PurchaseFailedEvent?.Invoke(id);
        }
        #endregion Payments

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
        int delayFirstCalls = -1;
        public static float timerShowAd;
#if !UNITY_EDITOR
        static float timerSaveCloud = 62;
#endif

        private void Update()
        {
            // Таймер для обработки показа Fillscreen рекламы
            timerShowAd += Time.unscaledDeltaTime;

            // Задержка вызова метода FirstСalls
            if (delayFirstCalls < infoYG.SDKStartDelay)
            {
                delayFirstCalls++;
                if (delayFirstCalls == infoYG.SDKStartDelay)
                    FirstСalls();
            }

            // Таймер для облачных сохранений
#if !UNITY_EDITOR
            if (infoYG.saveCloud)
                timerSaveCloud += Time.unscaledDeltaTime;
#endif
        }
        #endregion Update

        #region Json
        public class JsonAuth
        {
            public string playerAuth;
            public string playerName;
            public string playerId;
            public string playerPhoto;
        }

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

        public class JsonEnvironmentData
        {
            public string language = "ru";
            public string domain = "ru";
            public string deviceType = "desktop";
            public bool isMobile;
            public bool isDesktop = true;
            public bool isTablet;
            public bool isTV;
            public string appID;
            public string browserLang;
            public string payload;
            public bool promptCanShow;
            public bool reviewCanShow;
        }

        public class JsonPayments
        {
            public string[] id;
            public string[] title;
            public string[] description;
            public string[] imageURI;
            public string[] priceValue;
            public bool[] consumed;
        }
        #endregion Json
    }
}
