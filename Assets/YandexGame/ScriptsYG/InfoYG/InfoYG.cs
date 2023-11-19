using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityToolbag;
using YG.Utils.LB;
using YG.Utils.Pay;
#if YG_TEXT_MESH_PRO
using TMPro;
#endif

namespace YG
{
    [/*CreateAssetMenu(fileName = "YandexGameData", menuName = "InfoYG"),*/
        HelpURL("https://ash-message-bf4.notion.site/PluginYG-d457b23eee604b7aa6076116aab647ed")]
    public partial class InfoYG : ScriptableObject
    {
        [Header("———————  Basic settings  ———————")]

        [Tooltip("При инициализации объекта Player авторизованному игроку будет показано диалоговое окно с запросом на предоставление доступа к персональным данным. Запрашивается доступ только к аватару и имени, идентификатор пользователя всегда передается автоматически. Примерное содержание: Игра запрашивает доступ к вашему аватару и имени пользователя на сервисах Яндекса.\nЕсли вам достаточно знать идентификатор, а имя и аватар пользователя не нужны, используйте опциональный параметр scopes: false. В этом случае диалоговое окно не будет показано.")]
        public bool scopes = true;
        public enum PlayerPhotoSize { small, medium, large };
        [ConditionallyVisible(nameof(scopes))]
        [Tooltip("Размер подкачанного изображения пользователя.")]
        public PlayerPhotoSize playerPhotoSize;

        public string GetPlayerPhotoSize()
        {
            if (playerPhotoSize == PlayerPhotoSize.small)
                return "small";
            else if (playerPhotoSize == PlayerPhotoSize.medium)
                return "medium";
            else if (playerPhotoSize == PlayerPhotoSize.large)
                return "large";

            return null;
        }

        [Tooltip("Метод из SDK Яндекс при выполнении которого отражается момент, когда игра загрузила все ресурсы и готова к взаимодействию с пользователем.\r\n\r\nЕсли данный параметр 'Auto Game Ready API' включен, то плагин сам выполнит метод Game Ready API сразу после загрузки игры.\r\n\r\nЕсли в Вашей игре имеются свои реализации загрузки игры, например, загрузка первой сцены, то Вам необходимо снять галку 'Auto Game Ready API' и самостоятельно выполнять этот метод (по желанию), когда игра будет полностью загружена. Выполнение метода: `YandexGame.GameReadyAPI();`")]
        public bool autoGameReadyAPI;

#if UNITY_EDITOR
        [Serializable]
        public class PlayerInfoSimulation
        {
            public bool authorized = true;
            public bool isMobile;
            public string language = "ru";
            public string name = "Player this";
            public string uniqueID = "000";
            public string photo = photoExample;
        }
        public PlayerInfoSimulation playerInfoSimulation;
#endif

        [Header("———————  Advertisement  ———————")]

        [Tooltip("Показывать рекламу при переключении сцены? (после загрузки сцен)\n\nПо умолчанию = true — это значит, что показ рекламы будет вызываться при загрузке любой сцены в игре. Значение false — реклама не будет показываться при загрузке сцен.")]
        public bool AdWhenLoadingScene = true;

        [Tooltip("Показывать рекламу при загрузке игры? (Первая реклама при открытии страницы игры)")]
        public bool showFirstAd = true;

        [Tooltip("Выдавать вознаграждение за просмотр рекламы только после закрытия рекламы?\n(true = после закрытия, false = сразу после того как таймер закончит свой отчёт)")]
        public bool rewardedAfterClosing = true;

        public enum AdCallsMode
        {
            [InspectorName("Until Ad Is Shown")] until,
            [InspectorName("Resetting Timer After Any Ad Display")] reset
        }
        [Tooltip("Обработка вызовов показа рекламы\n\n •  Until Ad Is Shown - Не включать ограничительный таймер плагина, пока реклама не будет успешно показана. Если запрос на показ рекламы был отклонён по различным причинам, то вызовы к показу рекламы будут выполняться пока она не покажется.\n\n •  Resetting Timer After Any Ad Display - Включать ограничительный таймер плагина после любого вызова рекламы. Даже если реклама не была показана, запросы на рекламу смогут вновь выполняться только через указанный вами временной промежуток (Fullscreen Ad Interval).")]
        public AdCallsMode adDisplayCalls = AdCallsMode.until;

        [Tooltip("Интервал запросов на вызов полноэкранной рекламы."), Min(5)]
        public int fullscreenAdInterval = 60;

#if UNITY_EDITOR
        [Tooltip("Длительность симуляции показа рекламы."), Min(0)]
        public float durationOfAdSimulation = 0.5f;

        [Tooltip("Задержка открытия полноэкранной рекламы. Может быть полезна для тестирования уведомления о том, что скоро откроется реклама, перед ёё показом (в момент ожидания рекламы)."), Min(0)]
        public float loadAdWithDelaySimulation = 0.0f;

        [Tooltip("Нажмите галочку, чтобы сэмулировать вызов ошибки при просмотре рекламы за вознаграждение. (Только для Unity Editor)")]
        public bool testErrorOfRewardedAdsInEditor;
#endif

        [Header("———————  Saves  ———————")]

        [Tooltip(@"Вкл/Выкл облачные сохранения (сохранения Яндекса). При включении облачных сохранений, локальные сохранения всё равно будут работать. При использовании метода сохранения ""SaveProgress"", сохранения будут происходить локально - если таймер не достиг значения ""Save Cloud Interval"". Если же таймер достиг интервала - то сохранения запишутся в облако. При загрузке сохранений, будут загружены более актуальные данные (из локальных или облачных сохранений).")]
        public bool saveCloud = true;

        [ConditionallyVisible(nameof(saveCloud))]
        [Tooltip("Flush — определяет очередность отправки данных. При значении «true» данные будут отправлены на сервер немедленно; «false» (значение по умолчанию) — запрос на отправку данных будет поставлен в очередь.")]
        public bool flush;

        [ConditionallyVisible(nameof(saveCloud))]
        [Tooltip("Синхронизировать облачные сохранения с локальными? Если localSaveSync = false при включенных облачных сохранениях, то локальные просто не будут использоваться.")]
        public bool localSaveSync = true;

        [ConditionallyVisible(nameof(saveCloud))]
        [Tooltip("Интервал облачных сохранений.\nПри использовании метода сохранения (SaveProgress), сохранения будут происходить локально, если таймер не достиг значения (Save Cloud Interval. По умолчанию = 5). Если же таймер достиг интервала, то сохранения запишутся в облако.\nПри загрузке сохранений, будут загружены более актуальные данные (из локальных или облачных сохранений).")]
        [Min(0)]
        public int saveCloudInterval = 5;

        [Tooltip("Выполнять определённый метод при закрытии или обновлении страницы игры.")]
        public bool saveOnQuitGame = false;

        [Serializable]
        public class QuitGameMethod
        {
            [Tooltip("Имя объекта, который содержит нужный метод для выполнения после закрытия игры.")]
            public string objectName = "YandexGame";

            [Tooltip("Имя метода. Подходит публичный метод без перегрузок.")]
            public string methodName = "_SaveProgress";
        }
        [ConditionallyVisible(nameof(saveOnQuitGame))]
        public QuitGameMethod quitGameParameters = new QuitGameMethod
        {
            objectName = "YandexGame",
            methodName = "_SaveProgress"
        };

        [Header("———————  Leaderboards  ———————")]

        [Tooltip("Вкл/Выкл лидерборды")]
        public bool leaderboardEnable = true;

        [ConditionallyVisible(nameof(leaderboardEnable)), Tooltip("Записывать рекорд анонимных игроков?")]
        public bool saveScoreAnonymousPlayers = true;

        #region LeaderboardSimulation
#if UNITY_EDITOR
        public static string photoExample = "https://justplaygames.ru/public/icon_player.png";

        public LBData[] leaderboardSimulation = new LBData[]
        {
            new LBData
            {
                technoName = "advanced",
                entries = "Test LeaderBoard\nName: advanced\n1. anonymous: 10\n2. Ivan: 15\n3. Tanya: 23",
                players = new LBPlayerData[6]
                {
                    new LBPlayerData { name = "anonymous", rank = 1, score = 10, uniqueID = "123", photo = photoExample},
                    new LBPlayerData { name = "Ivan", rank = 2, score = 15, uniqueID = "321", photo = photoExample },
                    new LBPlayerData { name = "Tanya", rank = 3, score = 23, uniqueID = "456", photo = photoExample },
                    new LBPlayerData { name = "Player4", rank = 4, score = 30, uniqueID = "321", photo = photoExample },
                    new LBPlayerData { name = "Player this", rank = 5, score = 40, uniqueID = "000", photo = photoExample },
                    new LBPlayerData { name = "Player6", rank = 6, score = 50, uniqueID = "321", photo = photoExample }
                },
                type = "numeric",
                thisPlayer = new LBThisPlayerData
                {
                    rank = 2,
                    score = 10,
                }
            },
            new LBData
            {
                technoName = "simple",
                entries = "Test LeaderBoard\nName: simple\n1. anonymous: 10\n2. Max: 15\n3. Maria: 23"
            },
            new LBData
            {
                technoName = "time",
                entries = "Test LeaderBoard\nName: time\n1. anonymous: 10\n2. Max: 15\n3. Maria: 23",
                players = new LBPlayerData[6]
                {
                    new LBPlayerData { name = "anonymous", rank = 1, score = 7123, uniqueID = "789", photo = photoExample},
                    new LBPlayerData { name = "Max", rank = 2, score = 15321, uniqueID = "987", photo = photoExample },
                    new LBPlayerData { name = "Maria", rank = 3, score = 62000, uniqueID = "891", photo = photoExample },
                    new LBPlayerData { name = "Player4", rank = 4, score = 122000, uniqueID = "321", photo = photoExample },
                    new LBPlayerData { name = "Player this", rank = 5, score = 127000, uniqueID = "000", photo = photoExample },
                    new LBPlayerData { name = "Player6", rank = 6, score = 340000, uniqueID = "321", photo = photoExample }
                },
                type = "numeric",
                thisPlayer = new LBThisPlayerData
                {
                    rank = 2,
                    score = 10,
                }
            }
        };
#endif
        #endregion LeaderboardSimulation

        [Header("———————  Language Translation  ———————")]

        [Tooltip("Вкл/Выкл локализацию с помощью плагина.")]
        public bool LocalizationEnable;

        public enum CallingLanguageCheck { FirstLaunchOnly, EveryGameLaunch, DoNotChangeLanguageStartup };
        [Tooltip("Менять язык игры в соответствии с языком установленным Я.Играми:\nFirstLaunchOnly - Только при первом запуске игры\nEveryGameLaunch - Каждый раз при запуске игры\nDoNotChangeLanguageStartup - Не менять язык при запуске игры.")]
        [ConditionallyVisible(nameof(LocalizationEnable))]
        public CallingLanguageCheck callingLanguageCheck = CallingLanguageCheck.EveryGameLaunch;

        public enum TranslateMethod { AutoLocalization, Manual, CSVFile };
        [Tooltip("Метод перевода. \nAutoLocalization - Автоматический перевод через интернет с помощью Google Translate \nManual - Ручной режим. Вы сами записываете перевод в компоненте LanguageYG \nCSVFile - Перевод с помощью Excel файла.")]
        [ConditionallyVisible(nameof(LocalizationEnable))]
        public TranslateMethod translateMethod;

        [Tooltip("Домен с которого будет скачиваться перевод. Если у вас возникли проблемы с авто-переводом, попробуйте поменять домен.")]
        [ConditionallyVisible(nameof(LocalizationEnable))]
        public string domainAutoLocalization = "com";

        [Serializable]
        public class CSVTranslate
        {
            public enum FileFormat { GoogleSheets, ExcelOffice };
            [Tooltip("Формат scv файла. \nGoogleSheets - Создаст файл с разделительной запятой (,) \nExcelOffice - Создаст файл с разделительной точкой с запятой (;).")]
            [ConditionallyVisible(nameof(LocalizationEnable))]
            public FileFormat format;

            [Tooltip("Имя CSV файла.")]
            [ConditionallyVisible(nameof(LocalizationEnable))]
            public string name = "TranslateCSV";
        }
        [ConditionallyVisible(nameof(LocalizationEnable))]
        [Tooltip("Настройки для метода локализации с помощью CSV файла. Это подразоумивает перевод по ключам всех текстов игры в таблице Excel или Google Sheets.")]
        public CSVTranslate CSVFileTranslate;

        #region LanguagesEnumeration
        [Serializable]
        public class Languages
        {
            [Tooltip("RUSSIAN")] public bool ru = true;
            [Tooltip("ENGLISH")] public bool en = true;
            [Tooltip("TURKISH")] public bool tr = true;
            [Tooltip("AZERBAIJANIAN")] public bool az;
            [Tooltip("BELARUSIAN")] public bool be;
            [Tooltip("HEBREW")] public bool he;
            [Tooltip("ARMENIAN")] public bool hy;
            [Tooltip("GEORGIAN")] public bool ka;
            [Tooltip("ESTONIAN")] public bool et;
            [Tooltip("FRENCH")] public bool fr;
            [Tooltip("KAZAKH")] public bool kk;
            [Tooltip("KYRGYZ")] public bool ky;
            [Tooltip("LITHUANIAN")] public bool lt;
            [Tooltip("LATVIAN")] public bool lv;
            [Tooltip("ROMANIAN")] public bool ro;
            [Tooltip("TAJICK")] public bool tg;
            [Tooltip("TURKMEN")] public bool tk;
            [Tooltip("UKRAINIAN")] public bool uk;
            [Tooltip("UZBEK")] public bool uz;
            [Tooltip("SPANISH")] public bool es;
            [Tooltip("PORTUGUESE")] public bool pt;
            [Tooltip("ARABIAN")] public bool ar;
            [Tooltip("INDONESIAN")] public bool id;
            [Tooltip("JAPANESE")] public bool ja;
            [Tooltip("ITALIAN")] public bool it;
            [Tooltip("GERMAN")] public bool de;
            [Tooltip("HINDI")] public bool hi;
        }

        [ConditionallyVisible(nameof(LocalizationEnable))]
        [Tooltip("Выберите языки, на которые будет переведена Ваша игра.")]
        public Languages languages;

        [Serializable]
        public class Fonts
        {
            [Tooltip("Стандартный шрифт")] public Font[] defaultFont;
            [Tooltip("RUSSIAN")] public Font[] ru;
            [Tooltip("ENGLISH")] public Font[] en;
            [Tooltip("TURKISH")] public Font[] tr;
            [Tooltip("AZERBAIJANIAN")] public Font[] az;
            [Tooltip("BELARUSIAN")] public Font[] be;
            [Tooltip("HEBREW")] public Font[] he;
            [Tooltip("ARMENIAN")] public Font[] hy;
            [Tooltip("GEORGIAN")] public Font[] ka;
            [Tooltip("ESTONIAN")] public Font[] et;
            [Tooltip("FRENCH")] public Font[] fr;
            [Tooltip("KAZAKH")] public Font[] kk;
            [Tooltip("KYRGYZ")] public Font[] ky;
            [Tooltip("LITHUANIAN")] public Font[] lt;
            [Tooltip("LATVIAN")] public Font[] lv;
            [Tooltip("ROMANIAN")] public Font[] ro;
            [Tooltip("TAJICK")] public Font[] tg;
            [Tooltip("TURKMEN")] public Font[] tk;
            [Tooltip("UKRAINIAN")] public Font[] uk;
            [Tooltip("UZBEK")] public Font[] uz;
            [Tooltip("SPANISH")] public Font[] es;
            [Tooltip("PORTUGUESE")] public Font[] pt;
            [Tooltip("ARABIAN")] public Font[] ar;
            [Tooltip("INDONESIAN")] public Font[] id;
            [Tooltip("JAPANESE")] public Font[] ja;
            [Tooltip("ITALIAN")] public Font[] it;
            [Tooltip("GERMAN")] public Font[] de;
            [Tooltip("HINDI")] public Font[] hi;
        }

        [ConditionallyVisible(nameof(LocalizationEnable)),
            Tooltip("Здесь вы можете выбрать одельные шрифты для каждого языка.")]
        public Fonts fonts;

#if YG_TEXT_MESH_PRO
        [Serializable]
        public class FontsTMP
        {
            [Tooltip("Стандартный шрифт")] public TMP_FontAsset[] defaultFont;
            [Tooltip("RUSSIAN")] public TMP_FontAsset[] ru;
            [Tooltip("ENGLISH")] public TMP_FontAsset[] en;
            [Tooltip("TURKISH")] public TMP_FontAsset[] tr;
            [Tooltip("AZERBAIJANIAN")] public TMP_FontAsset[] az;
            [Tooltip("BELARUSIAN")] public TMP_FontAsset[] be;
            [Tooltip("HEBREW")] public TMP_FontAsset[] he;
            [Tooltip("ARMENIAN")] public TMP_FontAsset[] hy;
            [Tooltip("GEORGIAN")] public TMP_FontAsset[] ka;
            [Tooltip("ESTONIAN")] public TMP_FontAsset[] et;
            [Tooltip("FRENCH")] public TMP_FontAsset[] fr;
            [Tooltip("KAZAKH")] public TMP_FontAsset[] kk;
            [Tooltip("KYRGYZ")] public TMP_FontAsset[] ky;
            [Tooltip("LITHUANIAN")] public TMP_FontAsset[] lt;
            [Tooltip("LATVIAN")] public TMP_FontAsset[] lv;
            [Tooltip("ROMANIAN")] public TMP_FontAsset[] ro;
            [Tooltip("TAJICK")] public TMP_FontAsset[] tg;
            [Tooltip("TURKMEN")] public TMP_FontAsset[] tk;
            [Tooltip("UKRAINIAN")] public TMP_FontAsset[] uk;
            [Tooltip("UZBEK")] public TMP_FontAsset[] uz;
            [Tooltip("SPANISH")] public TMP_FontAsset[] es;
            [Tooltip("PORTUGUESE")] public TMP_FontAsset[] pt;
            [Tooltip("ARABIAN")] public TMP_FontAsset[] ar;
            [Tooltip("INDONESIAN")] public TMP_FontAsset[] id;
            [Tooltip("JAPANESE")] public TMP_FontAsset[] ja;
            [Tooltip("ITALIAN")] public TMP_FontAsset[] it;
            [Tooltip("GERMAN")] public TMP_FontAsset[] de;
            [Tooltip("HINDI")] public TMP_FontAsset[] hi;
        }

        [ConditionallyVisible(nameof(LocalizationEnable)),
            Tooltip("Здесь вы можете выбрать одельные шрифты TextMeshPro для каждого языка.")]
        public FontsTMP fontsTMP;
#endif

        [Serializable]
        public class FontsSizeCorrect
        {
            [Tooltip("RUSSIAN")] public int[] ru;
            [Tooltip("ENGLISH")] public int[] en;
            [Tooltip("TURKISH")] public int[] tr;
            [Tooltip("AZERBAIJANIAN")] public int[] az;
            [Tooltip("BELARUSIAN")] public int[] be;
            [Tooltip("HEBREW")] public int[] he;
            [Tooltip("ARMENIAN")] public int[] hy;
            [Tooltip("GEORGIAN")] public int[] ka;
            [Tooltip("ESTONIAN")] public int[] et;
            [Tooltip("FRENCH")] public int[] fr;
            [Tooltip("KAZAKH")] public int[] kk;
            [Tooltip("KYRGYZ")] public int[] ky;
            [Tooltip("LITHUANIAN")] public int[] lt;
            [Tooltip("LATVIAN")] public int[] lv;
            [Tooltip("ROMANIAN")] public int[] ro;
            [Tooltip("TAJICK")] public int[] tg;
            [Tooltip("TURKMEN")] public int[] tk;
            [Tooltip("UKRAINIAN")] public int[] uk;
            [Tooltip("UZBEK")] public int[] uz;
            [Tooltip("SPANISH")] public int[] es;
            [Tooltip("PORTUGUESE")] public int[] pt;
            [Tooltip("ARABIAN")] public int[] ar;
            [Tooltip("INDONESIAN")] public int[] id;
            [Tooltip("JAPANESE")] public int[] ja;
            [Tooltip("ITALIAN")] public int[] it;
            [Tooltip("GERMAN")] public int[] de;
            [Tooltip("HINDI")] public int[] hi;
        }

        [ConditionallyVisible(nameof(LocalizationEnable))]
        [Tooltip("Вы можете скорректировать размер шрифта для каждого языка. Допустим, для Японского языка вы можете указать -3. В таком случае, если бы базовый размер был бы, например, 10, то для японского языка он бы стал равен 7.")]
        public FontsSizeCorrect fontsSizeCorrect;
        #endregion LanguagesEnumeration

        [Header("———————  Purchases  ———————")]

        #region PurchasesSimulation
        public Purchase[] purshasesSimulation = new Purchase[]
        {
            new Purchase
            {
                id = "gun",
                title = "Gun",
                description = "Product - Gun",
                imageURI = "https://justplaygames.ru/public/Paymant1.png",
                priceValue = "5",
                consumed = true
            },
            new Purchase
            {
                id = "armor",
                title = "Armor",
                description = "Product - Armor",
                imageURI = "https://justplaygames.ru/public/Paymant2.png",
                priceValue = "10",
                consumed = true
            },
            new Purchase
            {
                id = "grenade",
                title = "Grenade",
                description = "Product - Grenade",
                imageURI = "https://justplaygames.ru/public/Paymant3.png",
                priceValue = "30",
                consumed = true
            }
        };
        #endregion PurchasesSimulation

        [Header("———————  Metrica  ———————")]

        public bool metricaEnable;
        [ConditionallyVisible(nameof(metricaEnable))]
        [Min(0)]
        public int metricaCounterID;

        [Header("———————  Other settings  ———————")]

        [Tooltip("Запись лога в консоль")]
        public bool debug = true;

        [FormerlySerializedAs("autoBuildArchiving"), Tooltip("Включить автоматическую архивацию билда?\n\n •  После успешного создания билда игры, папка с содержанием билда пакуется в zip архив. При повторной сборке игры, архив не перезапишется, но создастся новый пакет с приписанным номером в названии файла.")]
        public bool archivingBuild = true;

        public enum LogoImgFormat
        {
            [InspectorName("No Logo")] no,
            [InspectorName("PNG")] png,
            [InspectorName("JPG")] jpg,
            [InspectorName("GIF")] gif
        }
        [Tooltip("Заднее изображение при загрузке игры")]
        public LogoImgFormat logoImageFormat = LogoImgFormat.png;

        public enum BackgroundImageFormat
        {
            [InspectorName("Player Settings")] unity,
            [InspectorName("No Background")] no,
            [InspectorName("PNG")] png,
            [InspectorName("JPG")] jpg,
            [InspectorName("GIF")] gif
        }
        [Tooltip("Заднее изображение при загрузке игры")]
        public BackgroundImageFormat backgroundImageFormat = BackgroundImageFormat.jpg;

        [Tooltip("Снижение качества изображения игры в угоду оптимизации для мобильных устройств")]
        public bool pixelRatioEnable;
        [ConditionallyVisible(nameof(pixelRatioEnable))]
        [Min(0)]
        public float pixelRatioValue = 1;

        //[Tooltip(" •  Load Game Run = false:\nСначала будет происходить полная инициализация SDK Яндекса, затем только загрузка игры.\n\n •  Load Game Run = true:\nИгра будет загружаться вместе с инициализацией SDK Яндекса. В таком случае, игра может загрузиться раньше инициализации.")]
        [HideInInspector]
        public bool loadGameRun = false;

        [Tooltip("(Для кастомных баннеров, не для рекламных. Они устарели и их нельзя использовать в Я.Играх!). Если данный параметр выключен, то статические баннеры будут отображаться только во время загрузки игры. Если данный параметр включен, то статические баннеры будут отображаться и во время загрузки игры и в самой игре они тоже будут присутствовать.")]
        public bool staticRBTInGame;
    }
}
