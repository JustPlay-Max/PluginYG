using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.Events;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using UnityEngine.UI;

namespace YG
{
    public class YandexGame : MonoBehaviour
    {
        public InfoYG infoYG;
        public bool singleton;
        [Space(10)]
        public UnityEvent ResolvedAuthorization;
        public UnityEvent RejectedAuthorization;
        [Space(30)]
        public UnityEvent OpenFullscreenAd;
        public UnityEvent CloseFullscreenAd;
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
            if (infoYG.fullscreenAdChallenge == InfoYG.FullscreenAdChallenge.atStartupEndSwitchScene)
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

                _AuthorizationCheck();
                _RequestingEnvironmentData();

#if !UNITY_EDITOR
                if (infoYG.sitelock)
                    Invoke("SiteLock", 1);
#endif
            }
        }
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
            SaveToLocalStorage("savesData", JsonUtility.ToJson(savesData));
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
            else savesData = JsonUtility.FromJson<SavesYG>(LoadFromLocalStorage("savesData"));

            AfterLoading();
        }


        [DllImport("__Internal")]
        private static extern int HasKeyInLocalStorage(string key);
        public static bool HasKey(string key)
        {
            return HasKeyInLocalStorage(key) == 1;
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
                SaveLocal();
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

        #region SiteLock
        [DllImport("__Internal")]
        private static extern string GetURLFromPage();

        void SiteLock()
        {
            try 
            {
                string urlOrig = GetURLFromPage();

                string localhost = "http://localhost";
                if (urlOrig.Remove(localhost.Length) != localhost)
                {
                    string plaedLinks = urlOrig.Remove(0, 15);
                    plaedLinks = plaedLinks.Remove(0, EnvironmentData.domain.Length + 1);
                    string[] plaedSplit = plaedLinks.Split('/');
                    plaedLinks = $"{plaedSplit[0]}/{plaedSplit[1]}";

                    string urlCheck = $"https://yandex.{EnvironmentData.domain}/{plaedLinks}/{EnvironmentData.appID}";

                    if (urlOrig.Remove(urlCheck.Length) != urlCheck)
                    {
                        Crash();
                    }
                }
            }
            catch
            {
                Crash();
            }
        }

        void Crash()
        {
            GameObject errMessage = new GameObject { name = "siteLock" };
            errMessage.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            errMessage.AddComponent<GraphicRaycaster>();
            errMessage.AddComponent<RawImage>();

            Time.timeScale = 0;
            AudioListener.volume = 0;
            AudioListener.pause = true;
        }
        #endregion SiteLock


        // Sending messages

        #region Authorization Check
        [DllImport("__Internal")]
        private static extern void AuthorizationCheck(string playerPhotoSize, bool scopes);

        public void _AuthorizationCheck()
        {
#if !UNITY_EDITOR
            AuthorizationCheck( _photoSize, infoYG.scopes);
#else
            SetAuthorization(@"{""playerAuth""" + ": " + @"""resolved""," + @"""playerName""" + ": " + @"""Ivan"", " + @"""playerId""" + ": " + @"""tOpLpSh7i8QG8Voh/SuPbeS4NKTj1OxATCTKQF92H4c="", " + @"""playerPhoto""" + ": " + @"""https://drive.google.com/u/0/uc?id=1TCoEwiiUvIiQwAMbKcBssneWkmsoofuI&export=download""}");
#endif
        }
        #endregion Authorization Check

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
            SaveYG(JsonUtility.ToJson(savesData), Instance.infoYG.flush);
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
                timerShowAd >= infoYG.fullscreenAdInterval + 1)
            {
                timerShowAd = 0;
#if !UNITY_EDITOR
                FullAdShow();
#else
                Message("Fullscren Ad");
                OpenFullscreen();
                StartCoroutine(CloseFullAdInEditor());
#endif
            }
            else Message($"До показа Fullscreen рекламы {infoYG.fullscreenAdInterval + 1 - timerShowAd} сек.");
        }

        public static void FullscreenShow() => Instance._FullscreenShow();

#if UNITY_EDITOR
        IEnumerator CloseFullAdInEditor()
        {
            GameObject errMessage = new GameObject { name = "TestFullAd" };
            Canvas canvas = errMessage.AddComponent<Canvas>();
            canvas.sortingOrder = 32767;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            errMessage.AddComponent<GraphicRaycaster>();
            errMessage.AddComponent<RawImage>().color = new Color(0, 1, 0, 0.5f);

            yield return new WaitForSecondsRealtime(infoYG.durationOfAdSimulation);

            Destroy(errMessage);
            CloseFullscreen();
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
                StartCoroutine(CloseVideoInEditor(id));
#endif
            }
        }

        public static void RewVideoShow(int id) => Instance._RewardedShow(id);

#if UNITY_EDITOR
        IEnumerator CloseVideoInEditor(int id)
        {
            GameObject errMessage = new GameObject { name = "TestVideoAd" };
            Canvas canvas = errMessage.AddComponent<Canvas>();
            canvas.sortingOrder = 32767;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            errMessage.AddComponent<GraphicRaycaster>();
            errMessage.AddComponent<RawImage>().color = new Color(0, 0, 1, 0.5f);

            yield return new WaitForSecondsRealtime(infoYG.durationOfAdSimulation);

            Destroy(errMessage);
            CloseVideo();
            RewardVideo(id);
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
        public void _OnURL_Yandex_DefineDomain(string url)
        {
            Message("URL yandex.DefineDomain");
#if !UNITY_EDITOR
            Application.OpenURL("https://yandex." + EnvironmentData.domain + "/games/" + url);
#endif
#if UNITY_EDITOR
            Application.OpenURL("https://yandex." + "ru/games/" + url);
#endif
        }

        public void _OnAnyURL(string url)
        {
            Message("Any URL");
            Application.OpenURL(url);
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
                photo[0] = "https://drive.google.com/u/0/uc?id=1TCoEwiiUvIiQwAMbKcBssneWkmsoofuI&export=download";
                photo[1] = "https://drive.google.com/u/0/uc?id=1MlVQuyQTKMjoX3FDJYnsLKhEb4_M9FQB&export=download"; 
                photo[2] = "https://drive.google.com/u/0/uc?id=11ZwzHDXm_UNxqnMke2ONo6oJaGVp7VgP&export=download";
                playersName[0] = "Player"; playersName[1] = "Ivan"; playersName[2] = "Maria";
                scorePlayers[0] = 23101; scorePlayers[1] = 115202; scorePlayers[2] = 185303;

                UpdateLbEvent?.Invoke(nameLB, $"Test LeaderBoard\nName: {nameLB}\n1. Player: 10\n2. Ivan: 15\n3. Maria: 23",
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

            for(int i = 0; i < PaymentsData.id.Length; i++)
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
        public void OpenFullscreen()
        {
            OpenFullscreenAd.Invoke();
            OpenFullAdEvent?.Invoke();
            nowFullAd = true;
        }

        public static Action CloseFullAdEvent;
        public void CloseFullscreen()
        {
            nowFullAd = false;
            CloseFullscreenAd.Invoke();
            CloseFullAdEvent?.Invoke();
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

        public void SetAuthorization(string data)
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
                data = data.Replace(@"\", "");
                try
                {
                    cloudData = JsonUtility.FromJson<SavesYG>(data);
                }
                catch (Exception e)
                {
                    Debug.LogError("Cloud Load Error: " + e.Message);
                    cloudDataState = DataState.Broken;
                }
            }
            else cloudDataState = DataState.NotExist;

            if (HasKey("savesData"))
            {
                try
                {
                    localData = JsonUtility.FromJson<SavesYG>(LoadFromLocalStorage("savesData"));
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
                savesData = JsonUtility.FromJson<SavesYG>(data);
                Message("Cloud Saves Partially Restored!");
                AfterLoading();
            }
            else if (localDataState == DataState.Broken)
            {
                Message("Cloud Saves - " + cloudDataState);
                Message("Local Saves - Broken! Data Recovering...");
                ResetSaveProgress();
                savesData = JsonUtility.FromJson<SavesYG>(LoadFromLocalStorage("savesData"));
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
            EnvironmentData = JsonUtility.FromJson<JsonEnvironmentData>(data);
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
            jsonLB = JsonUtility.FromJson<JsonLB>(data);

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
            PaymentsData = JsonUtility.FromJson<JsonPayments>(data);
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
            PaymentsData.imageURI[0] = "https://drive.google.com/u/0/uc?id=1WLAXG3U1taoC0EQGNtsan7pejy-ada4Y&export=download";
            PaymentsData.imageURI[1] = "https://drive.google.com/u/0/uc?id=1bDj5v6yFe4M9gezD71FI7tDwC9a7Pdip&export=download";
            PaymentsData.imageURI[2] = "https://drive.google.com/u/0/uc?id=1uSQKQo4gctLQ_XOd7kd5ul9R-qBWAtEN&export=download";

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
        public void OnPromptSuccess()
        {
            savesData.promptDone = true;
            SaveProgress();

            PromptDo?.Invoke();
            PromptSuccessEvent?.Invoke();
            EnvironmentData.promptCanShow = false;
        }
        #endregion Prompt


        // The rest

        #region Update
        int delayFirstCalls = -1;
        static float timerShowAd;
        static float timerSaveCloud = 62;

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
            public string language;
            public string domain;
            public string deviceType = "desktop";
            public bool isMobile;
            public bool isDesktop;
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