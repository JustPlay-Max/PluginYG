using System;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;
using UnityToolbag;
using YG.Utils.LB;
using YG.Utils.Lang;

namespace YG
{
    [DefaultExecutionOrder(-101), HelpURL("https://www.notion.so/PluginYG-d457b23eee604b7aa6076116aab647ed#7f075606f6c24091926fa3ad7ab59d10")]
    public class LeaderboardYG : MonoBehaviour
    {
        [Tooltip("Техническое название соревновательной таблицы")]
        public string nameLB;

        [Tooltip("Максимальное кол-во получаемых игроков")]
        public int maxQuantityPlayers = 20;

        [Tooltip("Кол-во получения верхних топ игроков")]
        [Range(1, 20)]
        public int quantityTop = 3;

        [Tooltip("Кол-во получаемых записей возле пользователя")]
        [Range(1, 10)]
        public int quantityAround = 6;

        public enum UpdateLBMethod { Start, OnEnable, DoNotUpdate };
        [Tooltip("Когда следует обновлять лидерборд?\nStart - Обновлять в методе Start.\nOnEnable - Обновлять при каждой активации объекта (в методе OnEnable)\nDoNotUpdate - Не обновлять лидерборд с помощью данного скрипта (подразоумивается, что метод обновления 'UpdateLB' вы будете запускать сами, когда вам потребуется.")]
        public UpdateLBMethod updateLBMethod = UpdateLBMethod.OnEnable;

        [Tooltip("Перетащите компонент Text для записи описания таблицы, если вы не выбрали продвинутую таблицу (advanced)")]
        public Text entriesText;

        [SerializeField, Tooltip("Продвинутая таблица. Поддерживает подгрузку авата и конвертацию рекордов в тип Time. Подгружает все данные в отдельные элементы интерфейса.")]
        private bool advanced;

        [SerializeField, ConditionallyVisible(nameof(advanced)), Tooltip("Родительский объект для спавна в нём объектов 'playerDataPrefab'")]
        private Transform rootSpawnPlayersData;
        [ConditionallyVisible(nameof(advanced)), Tooltip("Префаб отображаемых данных игрока (объект со компонентом LBPlayerDataYG)")]
        public GameObject playerDataPrefab;

        public enum PlayerPhoto
        { NonePhoto, Small, Medium, Large };
        [Tooltip("Размер подгружаемых изображений игроков. NonePhoto = не подгружать изображение")]
        [ConditionallyVisible(nameof(advanced))]
        public PlayerPhoto playerPhoto = PlayerPhoto.Small;

        [Tooltip("Использовать кастомный спрайт для отображения аваторок скрытых пользователей")]
        [ConditionallyVisible(nameof(advanced))]
        public Sprite isHiddenPlayerPhoto;

        [SerializeField, ConditionallyVisible(nameof(advanced)), Tooltip("Конвертация полученных рекордов в Time тип")]
        private bool timeTypeConvert;

        [SerializeField, ConditionallyVisible("timeTypeConvert"),
            Range(0, 3), Tooltip("Размер десятичной части счёта (при использовании Time type).\n  Например:\n  0 = 00:00\n  1 = 00:00.0\n  2 = 00:00.00\n  3 = 00:00.000\nВы можете проверить это в Unity не прибегая к тестированию в сборке.")]
        private int decimalSize = 1;

        [SerializeField]
        private UnityEvent onUpdateData;

        private string photoSize;
        private LBPlayerDataYG[] players = new LBPlayerDataYG[0];

        void Start()
        {
            if (playerPhoto == PlayerPhoto.NonePhoto)
                photoSize = "nonePhoto";
            if (playerPhoto == PlayerPhoto.Small)
                photoSize = "small";
            else if (playerPhoto == PlayerPhoto.Medium)
                photoSize = "medium";
            else if (playerPhoto == PlayerPhoto.Large)
                photoSize = "large";

            if (updateLBMethod == UpdateLBMethod.Start && YandexGame.initializedLB)
            {
                UpdateLB();
            }
        }

        private void OnEnable()
        {
            YandexGame.onGetLeaderboard += OnUpdateLB;

            if (updateLBMethod == UpdateLBMethod.OnEnable && YandexGame.initializedLB)
            {
                UpdateLB();
            }
        }

        private void OnDisable() => YandexGame.onGetLeaderboard -= OnUpdateLB;

        void OnUpdateLB(LBData lb)
        {
            if (lb.entries == "initialized")
            {
                UpdateLB();
            }
            else if (lb.technoName == nameLB)
            {
                string noData = "...";

                if (lb.entries == "no data")
                {
                    noData = YandexGame.savesData.language switch
                    {
                        "ru" => "Нет данных",
                        "en" => "No data",
                        "tr" => "Veri yok",
                        _ => "...",
                    };
                }
                if (!advanced)
                {
                    lb.entries = lb.entries.Replace("anonymous", LangMethods.IsHiddenTextTranslate(YandexGame.Instance.infoYG));
                    entriesText.text = lb.entries;
                }
                else
                {
                    DestroyLBList();

                    if (lb.entries == "no data")
                    {
                        players = new LBPlayerDataYG[1];
                        GameObject playerObj = Instantiate(playerDataPrefab, rootSpawnPlayersData);

                        players[0] = playerObj.GetComponent<LBPlayerDataYG>();
                        players[0].data.name = noData;
                        players[0].data.photoUrl = null;
                        players[0].data.rank = null;
                        players[0].data.score = null;
                        players[0].data.inTop = false;
                        players[0].data.thisPlayer = false;
                        players[0].data.photoSprite = null;
                        players[0].UpdateEntries();
                    }
                    else
                    {
                        SpawnPlayersList(lb);
                    }
                }
                onUpdateData.Invoke();
            }
        }

        private void DestroyLBList()
        {
            int childCount = rootSpawnPlayersData.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                Destroy(rootSpawnPlayersData.GetChild(i).gameObject);
            }
        }

        private void SpawnPlayersList(LBData lb)
        {
            players = new LBPlayerDataYG[lb.players.Length];
            for (int i = 0; i < players.Length; i++)
            {
                GameObject playerObj = Instantiate(playerDataPrefab, rootSpawnPlayersData);

                players[i] = playerObj.GetComponent<LBPlayerDataYG>();

                int rank = lb.players[i].rank;

                players[i].data.name = LBMethods.AnonimName(lb.players[i].name);
                players[i].data.rank = rank.ToString();

                if (rank <= quantityTop)
                {
                    players[i].data.inTop = true;
                }
                else
                {
                    players[i].data.inTop = false;
                }

                if (lb.players[i].uniqueID == YandexGame.playerId)
                {
                    players[i].data.thisPlayer = true;
                }
                else
                {
                    players[i].data.thisPlayer = false;
                }

                if (timeTypeConvert)
                {
                    string timeScore = TimeTypeConvert(lb.players[i].score);
                    players[i].data.score = timeScore;
                }
                else
                {
                    players[i].data.score = lb.players[i].score.ToString();
                }

                if (playerPhoto != PlayerPhoto.NonePhoto)
                {
                    if (lb.players[i].photo == "nonePhoto")
                    {
                        if (isHiddenPlayerPhoto)
                        {
                            players[i].data.photoSprite = isHiddenPlayerPhoto;
                        }
                        else
                        {
                            players[i].data.photoUrl = null;
                        }
                    }
                    else
                    {
                        players[i].data.photoUrl = lb.players[i].photo;
                    }
                }

                players[i].UpdateEntries();
            }
        }

        public void UpdateLB()
        {
            YandexGame.GetLeaderboard(nameLB, maxQuantityPlayers, quantityTop, quantityAround, photoSize);
        }

        public void NewScore(int score) => YandexGame.NewLeaderboardScores(nameLB, score);

        public void NewScoreTimeConvert(float score) => YandexGame.NewLBScoreTimeConvert(nameLB, score);

        public string TimeTypeConvert(int score)
        {
            return LBMethods.TimeTypeConvertStatic(score, decimalSize);
        }

        public void SetNameLB(string name)
        {
            nameLB = name;
        }
    }
}

