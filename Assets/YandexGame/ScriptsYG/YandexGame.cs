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
        [Space(10)]
        public UnityEvent ResolvedAuthorization;
        public UnityEvent RejectedAuthorization;
        [Space(30)]
        public UnityEvent OpenFullscreenAd;
        public UnityEvent CloseFullscreenAd;
        [Space(30)]
        public UnityEvent OpenVideoAd;
        public UnityEvent CloseVideoAd;
        public UnityEvent CheaterVideoAd;
        [Space(30)]
        public UnityEvent PurchaseSuccess;
        public UnityEvent PurchaseFailed;
        [Space(30)]
        public UnityEvent PromptDo;

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
        public static bool adBlock
        {
            get => _adBlock;
            set => _adBlock = value;
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
        static bool _adBlock;
        static string _photoSize;
        static bool _leaderboardEnable;
        static bool _debug;
        static bool _scopes;
        public static bool nowFullAd;
        public static bool nowVideoAd;
        public static SavesYG savesData = new();
        public static JsonEnvironmentData EnvironmentData = new();
        public static JsonPayments PaymentsData = new();
        #endregion Data Fields

        #region Methods
        private void Awake()
        {
            transform.SetParent(null);
            gameObject.name = "YandexGame";

            onFullAdShow = null;
            onFullAdShow += _FullscreenShow;

            onRewAdShow = null;
            onRewAdShow += _RewardedShow;
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
                if (infoYG.siteLock)
                    Invoke("SiteLock", 1);
#endif
            }
        }
        #endregion Methods

        #region Player Data
        public static Action GetDataEvent;

#if UNITY_EDITOR
        
        public static void SaveLocal()
        {
            string path = Application.dataPath + "/YandexGame/WorkingData/";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Message("Save Unity Editor: Create New Directory");
            }
            else Message("Save Unity Editor");

            FileStream fs = new FileStream(path + "saveyg.yg", FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fs, savesData);
            fs.Close();
        }

        public static void LoadLocal()
        {
            string path = Application.dataPath + "/YandexGame/WorkingData/";
            Message("Load Unity Editor");

            if (File.Exists(path + "saveyg.yg")) // если файл есть
            {
                FileStream fs = new FileStream(path + "saveyg.yg", FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                try // загрузка
                {
                    savesData = (SavesYG)formatter.Deserialize(fs);
                    _SDKEnabled = true;
                    GetDataEvent?.Invoke();
                    SwitchLangEvent?.Invoke(savesData.language);
                }
                catch (Exception e) // если файл поломан
                {
                    Debug.Log(e.Message);
                    ResetSaveProgress();
                }
                finally
                {
                    fs.Close();
                } 
            }
            else ResetSaveProgress();
        }
#endif

        public static void ResetSaveProgress()
        {
            Message("Reset Save Progress");
#if UNITY_EDITOR
            savesData = new SavesYG { isFirstSession = false };

            _SDKEnabled = true;
            SwitchLangEvent?.Invoke(savesData.language);
            GetDataEvent?.Invoke();
#else
            GameObject.Find("YandexGame").GetComponent<YandexGame>().ResetSaveCloud();
#endif
        }
        public void _ResetSaveProgress() => ResetSaveProgress();

        public static void SaveProgress()
        {
            if (_SDKEnabled)
            {
#if !UNITY_EDITOR
                SaveCloud(false);
#else
                SaveLocal();
#endif
            }
        }

        public static void LoadProgress()
        {
#if !UNITY_EDITOR
            LoadCloud();
#else
            LoadLocal();
#endif
        }
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

        public static void SaveCloud(bool flush)
        {
            Message("Load YG");
            SaveYG(JsonUtility.ToJson(savesData), flush);
        }

        [DllImport("__Internal")]
        private static extern void LoadYG();

        public static void LoadCloud()
        {
            Message("Load YG");
            LoadYG();
        }
        #endregion Save end Load Cloud

        #region Fullscren Ad Show
        [DllImport("__Internal")]
        private static extern void FullAdShow();

        public void _FullscreenShow()
        {
            if (timerShowAd >= 31)
            {
                timerShowAd = 0;
#if !UNITY_EDITOR
                FullAdShow();
#else
                Message("Fullscren Ad");
                OpenFullscreen();
                StartCoroutine(TestCloseFullAd());
#endif
            }
            else Message("(ru) Отображение полноэкранной рекламы заблокировано! Еще рано.  (en) The display of full-screen ads is blocked! It's still early.");
        }

        static Action onFullAdShow;
        public static void FullscreenShow()
        {
            onFullAdShow?.Invoke();
        }

#if UNITY_EDITOR
        IEnumerator TestCloseFullAd()
        {
            GameObject errMessage = new GameObject { name = "TestFullAd" };
            Canvas canvas = errMessage.AddComponent<Canvas>();
            canvas.sortingOrder = 9995;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            errMessage.AddComponent<GraphicRaycaster>();
            errMessage.AddComponent<RawImage>().color = new Color(0, 1, 0, 0.5f);

            yield return new WaitForSecondsRealtime(1);

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
            Message("Rewarded Ad");
#if !UNITY_EDITOR
            if (infoYG.checkAdblock)
            {
                if (!adBlock)
                {
                    adBlock = true;
                    StartCoroutine(MissAdBlock(3));
                    RewardedShow(id);
                }
            }
            else RewardedShow(id);
#else
            if (!infoYG.checkAdblock)
            {
                Message("Cheater!");

                CheaterVideoAd.Invoke();
                CheaterVideoEvent?.Invoke();
            }
            else
            {
                OpenVideo(id);
                StartCoroutine(TestCloseVideo(id));
            }
#endif
        }

        static Action<int> onRewAdShow;
        public static void RewVideoShow(int id)
        {
            onRewAdShow?.Invoke(id);
        }

        IEnumerator MissAdBlock(float timer)
        {
            yield return new WaitForSecondsRealtime(timer);
            _adBlock = false;
        }

#if UNITY_EDITOR
        IEnumerator TestCloseVideo(int id)
        {
            GameObject errMessage = new GameObject { name = "TestVideoAd" };
            Canvas canvas = errMessage.AddComponent<Canvas>();
            canvas.sortingOrder = 9995;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            errMessage.AddComponent<GraphicRaycaster>();
            errMessage.AddComponent<RawImage>().color = new Color(0, 0, 1, 0.5f);

            yield return new WaitForSecondsRealtime(1);

            Destroy(errMessage);
            CloseVideo(id);
        }
#endif
        #endregion Rewarded Video Show

        #region Language
        [DllImport("__Internal")]
        private static extern void LanguageRequest();

        public void _LanguageRequest()
        {
#if !UNITY_EDITOR
            LanguageRequest();
#endif
#if UNITY_EDITOR
            SetLanguage("ru");
#endif
        }

        public static Action<string> SwitchLangEvent;

        public void _SwitchLanguage(string language)
        {
            savesData.language = language;
            SaveProgress();

            SwitchLangEvent?.Invoke(language);
        }

        public static void SwitchLanguage(string language)
        {
            savesData.language = language;
            SaveProgress();

            SwitchLangEvent?.Invoke(language);
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
            Message("Requesting Envirolopment Data");
        }
        #endregion Requesting Environment Data

        #region URL
        public void _OnURL(string url)
        {
#if !UNITY_EDITOR
            Application.OpenURL("https://yandex." + EnvironmentData.domain + "/games/" + url);
#endif
#if UNITY_EDITOR
            Application.OpenURL("https://yandex." + "ru/games/" + url);
#endif
            Message("URL");
        }

        public void _OnURL_ru(string url)
        {
            Application.OpenURL("https://yandex.ru/games/" + url);
            Message("URL.ru");
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
                scorePlayers[0] = 23; scorePlayers[1] = 115; scorePlayers[2] = 1053;

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
            GameObject.Find("YandexGame").GetComponent<YandexGame>().OnPurchaseSuccess(id);
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

        #region Review
        [DllImport("__Internal")]
        private static extern void Review();

        public void _Review()
        {
#if !UNITY_EDITOR
                    if (_auth)
                      Review();
                  else
                      _OpenAuthDialog();
#endif
            Message("Review");
        }
#endregion Review

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

            GameObject.Find("YandexGame").GetComponent<YandexGame>().PromptDo?.Invoke();
            PromptSuccessEvent?.Invoke();
#endif
        }
        public void _PromptShow() => PromptShow();
        #endregion Prompt


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
        public static Action<int> OpenVideoEvent;

        public void OpenVideo(int id)
        {
            OpenVideoEvent?.Invoke(id);
            OpenVideoAd.Invoke();
            nowVideoAd = true;
        }

        public static Action<int> CloseVideoEvent;
        public static Action CheaterVideoEvent;

        public void CloseVideo(int id)
        {
            nowVideoAd = false;
            if (infoYG.checkAdblock && _adBlock)
            {
                CheaterVideoAd.Invoke();
                CheaterVideoEvent?.Invoke();

                StopAllCoroutines();
                _adBlock = false;
            }
            else
            {
                CloseVideoAd.Invoke();
                CloseVideoEvent?.Invoke(id);
            }
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
        public void SetLoadSaves(string data)
        {
            data = data.Remove(0, 2);
            data = data.Remove(data.Length - 2, 2);
            data = data.Replace(@"\", "");

            savesData = JsonUtility.FromJson<SavesYG>(data);
            Message("Load YG Complete");

            _SDKEnabled = true;
            GetDataEvent?.Invoke();

            if (infoYG.LocalizationEnable && infoYG.callingLanguageCheck == InfoYG.CallingLanguageCheck.EveryGameLaunch)
                _LanguageRequest();
            else
                SwitchLangEvent?.Invoke(savesData.language);
        }

        public void ResetSaveCloud()
        {
            Message("Reset Save Progress");
            savesData = new SavesYG { isFirstSession = false };

            if (infoYG.LocalizationEnable &&
                (infoYG.callingLanguageCheck == InfoYG.CallingLanguageCheck.FirstLaunchOnly ||
                infoYG.callingLanguageCheck == InfoYG.CallingLanguageCheck.EveryGameLaunch))
                _LanguageRequest();

            _SDKEnabled = true;
            GetDataEvent?.Invoke();
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

        #region Prompt
        public static Action PromptSuccessEvent;
        public void OnPromptSuccess()
        {
            savesData.promptDone = true;
            SaveProgress();

            PromptDo?.Invoke();
            PromptSuccessEvent?.Invoke();
        }
        #endregion Prompt


        // The rest

        #region Update
        int delayFirstCalls = -1;
        static float timerShowAd;

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