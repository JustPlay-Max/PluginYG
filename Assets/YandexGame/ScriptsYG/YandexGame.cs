//#define JSON_NET_ENABLED
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.Events;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;
#if JSON_NET_ENABLED
using Newtonsoft.Json;
#endif

namespace YG
{
    public class YandexGame : MonoBehaviour
    {
        public InfoYG infoYG;
        [Tooltip("Объект YandexGame не будет удаляться при смене сцены. При выборе опции singleton, объект YandexGame необходимо поместить только на одну сцену, которая первая загружается при запуске игры.\n\n •  При выборе опции singleton, полноэкранная реклама не будет автоматически показываться при загрузке новой сцены, даже при выборе параметра Ad When Loading Scene = true в InfoYG.")]
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
        public static JsonPayments PaymentsData = new JsonPayments();
        public static YandexGame Instance;
        static string pathSaves;
        #endregion Data Fields

        #region Methods
        private void Awake()
        {
            pathSaves = Application.dataPath + "/YandexGame/WorkingData/saveyg.yg";
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

                InitializationSDK();
            }
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
            PaymentsData = new JsonPayments();
            Instance = null;
            pathSaves = null;
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
            UpdateLbEvent = null;
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
        public static Action GetDataEvent;

        public static void SaveEditor()
        {
            Message("Save Editor");
            FileStream fs = new FileStream(pathSaves, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fs, savesData);
            fs.Close();
        }

        [DllImport("__Internal")]
        private static extern void SaveToLocalStorage(string key, string value);
        public static void SaveLocal()
        {
            Message("Save Local");
#if !UNITY_EDITOR
#if JSON_NET_ENABLED
            SaveToLocalStorage("savesData", JsonConvert.SerializeObject(savesData));
#else
            SaveToLocalStorage("savesData", JsonUtility.ToJson(savesData));
#endif
#endif
        }

        public static void LoadEditor()
        {
            Message("Load Editor");
            if (File.Exists(pathSaves))
            {
                FileStream fs = new FileStream(pathSaves, FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                try
                {
                    savesData = (SavesYG)formatter.Deserialize(fs);
                    AfterLoading();
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    ResetSaveProgress();
                }
                finally
                {
                    fs.Close();
                }
            }
            else ResetSaveProgress();
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
#if JSON_NET_ENABLED
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
                LanguageRequest();
            else SwitchLangEvent?.Invoke(savesData.language);
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
                LanguageRequest();

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
        private static extern void InitSDK_Internal(string playerPhotoSize, bool scopes);

        public void InitializationSDK()
        {
#if !UNITY_EDITOR
            InitSDK_Internal( _photoSize, infoYG.scopes);
#else
            SetInitializationSDK(@"{""playerAuth""" + ": " + @"""resolved""," + @"""playerName""" + ": " + @"""Ivan"", " + @"""playerId""" + ": " + @"""tOpLpSh7i8QG8Voh/SuPbeS4NKTj1OxATCTKQF92H4c="", " + @"""playerPhoto""" + ": " + @"""https://s381vla.storage.yandex.net/rdisk/6abebb2a2211159542df567e57bfd89c1a255305976455254fa0868910ffee57/6411b0ef/MemHQzsnZ2QE1ElANeLrWFkY7msmjkjvvw3wr5Q3giJMw53O6EAdzMrOYQICwbZg-LoS5wxafS5y5wTAMD_Fvg==?uid=325055514&filename=Player1.png&disposition=attachment&hash=&limit=0&content_type=image%2Fpng&owner_uid=325055514&fsize=13889&hid=3e22053d8e718b72893d54dd40d4a9a4&media_type=image&tknv=v2&etag=580b6bd8bc6fece28dc421e843492530&rtoken=FpFeHH6hXRuP&force_default=yes&ycrid=na-9b1d66ffc5d1eb5916adefb0f09568f7-downloader6e&ts=5f6eef20ad9c0&s=18045c1538c3df7e6a7ed187a974d6187e60b38016c6954192293e28d75f6137&pb=U2FsdGVkX1_6u1ORoSa9iHDfKDutmhz2XF4JKGHGS0cFuGWQRGb1QHcsjQ5iIU5jatOlaEy_94BwSLyElTEu-mmPLqkq2KONLzj9yGv2Yes""}");
#endif
        }
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
#if JSON_NET_ENABLED
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
            if (!nowFullAd && !nowVideoAd &&
                timerShowAd >= infoYG.fullscreenAdInterval)
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
            else Message($"До запроса к показу Fullscreen рекламы {(infoYG.fullscreenAdInterval - timerShowAd).ToString("00.0")} сек.");
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
            SetLanguage("ru");
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
#if !UNITY_EDITOR
            if (_leaderboardEnable) 
                SetLeaderboardScores(nameLB, score);
#endif
            if (_leaderboardEnable)
                Message("New Scores Leaderboard " + nameLB + ": " + score);
        }

        public static void NewLBScoreTimeConvert(string nameLB, float secondsScore)
        {
            int result;
            int indexComma = secondsScore.ToString().IndexOf(",");

            if (secondsScore < 1)
            {
                Debug.LogError("You can't record a record below zero!");
                return;
            }
            else if (indexComma <= 0) result = (int)(secondsScore * 1000f);
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

        [DllImport("__Internal")]
        private static extern void GetLeaderboardScores(string nameLB, int maxQuantityPlayers, int quantityTop, int quantityAround, string photoSizeLB, bool auth);

        public static void GetLeaderboard(string nameLB, int maxQuantityPlayers, int quantityTop, int quantityAround, string photoSizeLB)
        {
            int[] rank = new int[3];
            string[] photo = new string[3];
            string[] playersName = new string[3];
            int[] scorePlayers = new int[3];

#if !UNITY_EDITOR
            if (_leaderboardEnable)
            {
                GetLeaderboardScores(nameLB, maxQuantityPlayers, quantityTop, quantityAround, photoSizeLB, _auth);
            }
            else
            {
                rank = new int[1];
                photo = new string[1];
                playersName = new string[1];
                scorePlayers = new int[1];

                UpdateLbEvent?.Invoke(nameLB, "No data", rank, photo, playersName, scorePlayers, auth);
            }
#endif
#if UNITY_EDITOR
            if (_leaderboardEnable)
            {
                rank[0] = 1; rank[1] = 2; rank[2] = 3;
                photo[0] = "nonePhoto";
                photo[1] = "https://s353vla.storage.yandex.net/rdisk/cb05378ff110cdba2703d685f24c5bcef255f4c9b50be01764b967ee6931af38/6411b05f/MemHQzsnZ2QE1ElANeLrWFyMjEfJYMOv4LLKQNZ7NZh4KtAdJiUJPMJx22gH8N-QCvzNnhyyRKL_r176Lm-ggA==?uid=325055514&filename=Player2.png&disposition=attachment&hash=&limit=0&content_type=image%2Fpng&owner_uid=325055514&fsize=13694&hid=c3ae331c249ea81c7ae7fc88347996d3&media_type=image&tknv=v2&etag=dfa5341ed88e15cc160897c6aa3079e7&rtoken=xN31JKAIMuia&force_default=yes&ycrid=na-89b63a427d140083d6823e719d323d37-downloader6e&ts=5f6eee97595c0&s=fe7a8c4c6c6bc5d2afaeabcc9d0154b858aea3e9578743cfee979c1128c9e478&pb=U2FsdGVkX1_rZbVSnI-xs5_XupMDSqDYDv4FsdMjefn8XrkgopEUDeU7q6yyWZknfWxVP5wx_YPrGKv2AO9h6vy5h7635zN03O7xx9F1Hfs";
                photo[2] = "https://s274iva.storage.yandex.net/rdisk/788c34c55506ffbcf6bb8b461ad42131c1a9907c0746f183421504fc7ca639f7/6411b07b/MemHQzsnZ2QE1ElANeLrWMmCASB3apr1ON-o8R4hoNetsDiQrcPz4ZFpXANYGMN1TvKvzYr9OvC3Il4-GrZz2A==?uid=325055514&filename=Player3.png&disposition=attachment&hash=&limit=0&content_type=image%2Fpng&owner_uid=325055514&fsize=13589&hid=4562adf31f9dd3bab5cb9ab4a969b514&media_type=image&tknv=v2&etag=d22c9399def88a852cbdb16606eeb599&rtoken=fct0tnJZBzmn&force_default=yes&ycrid=na-62c2544b17f2f422e86012ceb0257166-downloader6e&ts=5f6eeeb20d4c0&s=4bd8f74b1f46f96b7dd07580d791304920f661fa23813ea4835ab1415bd09b64&pb=U2FsdGVkX1_NfTvCiN6sAX0D0qORCZvFQEnZKjYZjRDD7ahYox1KZd4UD_h7jxn900561r6dGVJWhiPJNDNJXg7XIXYUk7fodgP8Tb2_Iq8";
                playersName[0] = "anonymous"; playersName[1] = "Ivan"; playersName[2] = "Maria";
                scorePlayers[0] = 23101; scorePlayers[1] = 115202; scorePlayers[2] = 185303;

                UpdateLbEvent?.Invoke(nameLB, $"Test LeaderBoard\nName: {nameLB}\n1. anonymous: 10\n2. Ivan: 15\n3. Maria: 23",
                    rank, photo, playersName, scorePlayers, true);
            }
            else
            {
                rank = new int[1];
                rank[0] = 0;
                playersName[0] = "No data";

                UpdateLbEvent?.Invoke(nameLB, "No data", rank, photo, playersName, scorePlayers, auth);
            }
            Message("Get Leaderboard - " + nameLB);
#endif
        }
        #endregion Leaderboard

        #region Payments
        [DllImport("__Internal")]
        private static extern void BuyPaymentsInternal(string id);

        public static void BuyPayments(string id)
        {
            Message("Buy Payment. ID: " + id);
#if !UNITY_EDITOR
            BuyPaymentsInternal(id);
#else
            Instance.OnPurchaseSuccess(id);
            GetPayments();
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
            GetPaymentsEvent?.Invoke();
#endif
        }

        public void _GetPayments() => GetPayments();

        public static Purchase PurchaseByID(string ID)
        {
            Purchase purchase = null;

            for (int i = 0; i < PaymentsData.id.Length; i++)
            {
                if (PaymentsData.id[i] == ID)
                {
                    purchase = new Purchase
                    {
                        numArray = i,
                        id = PaymentsData.id[i],
                        title = PaymentsData.title[i],
                        description = PaymentsData.description[i],
                        imageURI = PaymentsData.imageURI[i],
                        priceValue = PaymentsData.priceValue[i],
                        purchased = PaymentsData.purchased[i]
                    };

                    break;
                }
            }

            return purchase;
        }

        [DllImport("__Internal")]
        private static extern void DeletePurchaseInternal(string id);

        public static void DeletePurchase(string id)
        {
            Message("Delete Purchase. id - " + id);
#if !UNITY_EDITOR
            DeletePurchaseInternal(id);
#endif
            if (PurchaseByID(id) != null)
            {
                PaymentsData.purchased[PurchaseByID(id).numArray] = 0;
                GetPaymentsEvent?.Invoke();
            }
            else Debug.LogError(@$"No purchase with this id ""{id}"" was found!");
        }

        public void _DeletePurchase(string id) => DeletePurchase(id);


        [DllImport("__Internal")]
        private static extern void DeleteAllPurchasesInternal();

        public static void DeleteAllPurchases()
        {
            Message("Delete All Purchases");
#if !UNITY_EDITOR
            DeleteAllPurchasesInternal();
#endif
            for (int i = 0; i < PaymentsData.purchased.Length; i++)
                PaymentsData.purchased[i] = 0;

            GetPaymentsEvent?.Invoke();
        }

        public void _DeleteAllPurchases() => DeleteAllPurchases();

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
        public static Action OpenVideoEvent;
        public void OpenVideo()
        {
            OpenVideoEvent?.Invoke();
            OpenVideoAd.Invoke();
            nowVideoAd = true;
        }

        public static Action CloseVideoEvent;
        public void CloseVideo()
        {
            nowVideoAd = false;

            CloseVideoAd.Invoke();
            CloseVideoEvent?.Invoke();
        }

        public static Action<int> RewardVideoEvent;
        public void RewardVideo(int id)
        {
            RewardVideoAd.Invoke();
            RewardVideoEvent?.Invoke(id);
        }

        public static Action ErrorVideoEvent;
        public void ErrorVideo()
        {
            ErrorVideoAd.Invoke();
            ErrorVideoEvent?.Invoke();
        }
        #endregion Rewarded Video

        #region Authorization
        JsonAuth jsonAuth = new JsonAuth();

        public void SetInitializationSDK(string data)
        {
#if JSON_NET_ENABLED
            jsonAuth = JsonConvert.DeserializeObject<JsonAuth>(data);
#else
            jsonAuth = JsonUtility.FromJson<JsonAuth>(data);
#endif

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

#if !UNITY_EDITOR
            GetPayments();
#else
            PaymentsEntries("");
#endif

            if (_leaderboardEnable)
            {
#if !UNITY_EDITOR
                _InitLeaderboard();
#else
                InitializedLB();
#endif
            }
        }
        #endregion Set Authorization

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
#if JSON_NET_ENABLED
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
#if JSON_NET_ENABLED
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
#if JSON_NET_ENABLED
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
#if JSON_NET_ENABLED
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
#if JSON_NET_ENABLED
            EnvironmentData = JsonConvert.DeserializeObject<JsonEnvironmentData>(data);
#else
            EnvironmentData = JsonUtility.FromJson<JsonEnvironmentData>(data);
#endif
        }
        #endregion Environment Data

        #region Leaderboard
        public delegate void UpdateLB(
            string name,
            string description,
            int[] rank,
            string[] photo,
            string[] playersName,
            int[] scorePlayers,
            bool auth);

        public static event UpdateLB UpdateLbEvent;

        JsonLB jsonLB = new JsonLB();

        int[] rank;
        string[] photo;
        string[] playersName;
        int[] scorePlayers;

        public void LeaderboardEntries(string data)
        {
#if JSON_NET_ENABLED
            jsonLB = JsonConvert.DeserializeObject<JsonLB>(data);
#else
            jsonLB = JsonUtility.FromJson<JsonLB>(data);
#endif

            rank = jsonLB.rank;
            photo = jsonLB.photo;
            playersName = jsonLB.playersName;
            scorePlayers = jsonLB.scorePlayers;

            UpdateLbEvent?.Invoke(
                jsonLB.nameLB.ToString(),
                jsonLB.entries.ToString(),
                rank,
                photo,
                playersName,
                scorePlayers,
                _auth);
        }

        public void InitializedLB()
        {
            UpdateLbEvent?.Invoke("initialized", "no data", rank, photo, playersName, scorePlayers, _auth);
            _initializedLB = true;
        }
        #endregion Leaderboard

        #region Payments
        public static Action GetPaymentsEvent;

        public void PaymentsEntries(string data)
        {
#if !UNITY_EDITOR
#if JSON_NET_ENABLED
            PaymentsData = JsonConvert.DeserializeObject<JsonPayments>(data);
#else
            PaymentsData = JsonUtility.FromJson<JsonPayments>(data);
#endif
#else
            PaymentsData.id = new string[3];
            PaymentsData.id[0] = "test";
            PaymentsData.id[1] = "test2";
            PaymentsData.id[2] = "test3";

            PaymentsData.title = new string[3];
            PaymentsData.title[0] = "Gun";
            PaymentsData.title[1] = "Armor";
            PaymentsData.title[2] = "Grenade";

            PaymentsData.description = new string[3];
            PaymentsData.description[0] = "Testing purchases in the editor";
            PaymentsData.description[1] = "Second testing of purchases in the editor";
            PaymentsData.description[2] = "Third test purchase";

            PaymentsData.imageURI = new string[3];
            PaymentsData.imageURI[0] = "https://s519sas.storage.yandex.net/rdisk/9f44ec194b4191b7e17617f639b0b76b94cabf1ff143d88b7551feba74cd22db/6411b150/MemHQzsnZ2QE1ElANeLrWNAqG5jmQeoI2Ak8Pjpt9qClu9sXhrYVR9NQDA-ko6qqS_bVF2yaPWSX8joEz5hbFQ==?uid=325055514&filename=Paymant1.png&disposition=attachment&hash=&limit=0&content_type=image%2Fpng&owner_uid=325055514&fsize=9657&hid=aff6f37df525445a57a5a8e275a1343d&media_type=image&tknv=v2&etag=af7e3f011c909f5f23b66a608741f2ae&rtoken=U9I764unsrwy&force_default=yes&ycrid=na-efac5bc05f4425777beb391038334e0e-downloader6e&ts=5f6eef7d2f400&s=53ca12d81af7c952700ec74bee4931847b272f383377c4b1ac35bf606c51d517&pb=U2FsdGVkX19kujHs416E638jZiiiEH2sUj2uX90WjKMqq7rHHXIGYS_ulwS7nEEuRm6jeWsjuM9E55syzcN4bmU_Lsgk403vUYkB-hL710w";
            PaymentsData.imageURI[1] = "https://s143vlx.storage.yandex.net/rdisk/edb4958fb6d38aa53d6af80344d71984f4cc58bd744d5d92878c8d9f86f49ac6/6411b16e/MemHQzsnZ2QE1ElANeLrWIJ4NLi2iHwXhduvffLTTSMozI334a79BJ2XWalF4iMkt57YzX4yS7aERXVhHM6ByQ==?uid=325055514&filename=Paymant2.png&disposition=attachment&hash=&limit=0&content_type=image%2Fpng&owner_uid=325055514&fsize=8643&hid=af61b500062e3bce82a2af81bc99fd95&media_type=image&tknv=v2&etag=4433a7729fc614eead08d0553f76f241&rtoken=LTZ1Ouw4eewL&force_default=yes&ycrid=na-ac7e228f6d7c127f65042a4caabbcfd0-downloader6e&ts=5f6eef99cb780&s=27721ecbbc706e4f4316c7326b42fb5d60518e0a3ce85f2be8b0540c33fe8bb2&pb=U2FsdGVkX1-8QJy-cHizt1j7iWI_ex5YF9J_CJwzfvRH6SF0XJcOBjIfyJF0ygoNnWno_2Xv1vJIaBX1d2_UUkz81rndk0ejipI6eUttSN8";
            PaymentsData.imageURI[2] = "https://s40vlx.storage.yandex.net/rdisk/f770a2cfe699d4b8fc1eb02d0f524e85f01c2000988965cde1368786ba9f3a3e/6411b17d/MemHQzsnZ2QE1ElANeLrWMIvA2YN0F2ULGbBMFAzq9tDxNz4G8RQrD5pgvOhY5REdUWGjcB18wx3i7Q9V88o1w==?uid=325055514&filename=Paymant3.png&disposition=attachment&hash=&limit=0&content_type=image%2Fpng&owner_uid=325055514&fsize=11193&hid=29014250d2df1bf630e0f8e1dfde94e0&media_type=image&tknv=v2&etag=95355903dc59e9476ba7d383027f5ee0&rtoken=MP0SXOIaAEmQ&force_default=yes&ycrid=na-8f23fb56f59830778916e63f1dc58240-downloader6e&ts=5f6eefa819940&s=17b5dc5012f8cbc6ce6388af10b46c230df53b3cd8236860e7ed66774e7e9cf3&pb=U2FsdGVkX1_UndYGQNr3RRKceeb-9zcOaPEv7dJ_OB8272AkSSQBlesfVj3frzpWamNbTSb5tIc-6GZOO7L6CigryNZIus8mfm-XtPs7irE";

            PaymentsData.priceValue = new string[3];
            PaymentsData.priceValue[0] = "10";
            PaymentsData.priceValue[1] = "15";
            PaymentsData.priceValue[2] = "20";

            PaymentsData.purchased = new int[3];
#endif
            GetPaymentsEvent?.Invoke();
        }

        public static Action<string> PurchaseSuccessEvent;
        public void OnPurchaseSuccess(string id)
        {
            Purchase purchase = PurchaseByID(id);
            if (purchase != null)
                PaymentsData.purchased[purchase.numArray] += 1;

            PurchaseSuccess?.Invoke();
            PurchaseSuccessEvent?.Invoke(id);
            GetPaymentsEvent?.Invoke();
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
        static float timerShowAd;
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
            public string nameLB;
            public string entries;
            public int[] rank;
            public string[] photo;
            public string[] playersName;
            public int[] scorePlayers;
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
            public int[] purchased;
        }
        #endregion Json
    }

    public class Purchase
    {
        public int numArray;
        public string id;
        public string title;
        public string description;
        public string imageURI;
        public string priceValue;
        public int purchased;
    }
}