using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityToolbag;

namespace YG
{
    public class LeaderboardYG : MonoBehaviour
    {
        [Tooltip("Техническое название соревновательной таблицы")]
        public string nameLB;
        [Tooltip("Максимальное кол-во получаемых игроков")]
        public int maxQuantityPlayers = 20;
        [Tooltip("Кол-во получения верхних топ игроков")]
        [Range(0, 10)]
        public int quantityTop = 3;
        [Tooltip("Кол-во получаемых записей возле пользователя")]
        [Range(0, 20)]
        public int quantityAround = 3;
        public enum UpdateLBMethod { Start, OnEnable, DoNotUpdate };
        [Tooltip(@"Когда следует обновлять лидерборд?\nStart - Обновлять в методе Start.\nOnEnable - Обновлять при каждой активации объекта (в методе OnEnable)\nDoNotUpdate - Не обновлять лидерборд с помощью данного скрипта (подразоумивается, что метод обновления ""UpdateLB"" вы будете запускать сами, когда вам потребуется.")]
        public UpdateLBMethod updateLBMethod = UpdateLBMethod.OnEnable;
        [Tooltip("Перетащите компонент Text для записи описания таблицы, если вы не выбрали продвинутую таблицу (advanced)")]
        public Text entriesText;
        [Tooltip("Продвинутая таблица. Поддерживает подгрузку авата и конвертацию рекордов в тип Time. Подгружает все данные в отдельные элементы интерфейса.")]
        public bool advanced;
        public enum PlayerPhoto { NonePhoto, Small, Medium, Large };
        [Tooltip("Размер подгружаемых изображений игроков. NonePhoto = не подгружать изображение")]
        [ConditionallyVisible(nameof(advanced))]
        public PlayerPhoto playerPhoto = PlayerPhoto.Small;
        [Tooltip("Конвертация полученных рекордов в Time тип")]
        [ConditionallyVisible(nameof(advanced))]
        public bool timeTypeConvert;
        [ConditionallyVisible("timeTypeConvert"), Range(0, 3), Tooltip("Сколько показывать сотых (цифр после запятой)? (при использовании Time type)\n  Например:\n  0 = 00:00\n  1 = 00:00.0\n  2 = 00:00.00\n  3 = 00:00.000\nВы можете проверить это в Unity не прибегая к тестированию в WebGL!")]
        public int decimalSize = 1;

        string photoSize;

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

            if (updateLBMethod == UpdateLBMethod.Start
                && YandexGame.initializedLB)
                UpdateLB();
        }

        private void OnEnable()
        {
            YandexGame.UpdateLbEvent += OnUpdateLB;

            if (updateLBMethod == UpdateLBMethod.OnEnable
                && YandexGame.initializedLB)
                UpdateLB();
        }

        private void OnDisable() => YandexGame.UpdateLbEvent -= OnUpdateLB;

        void OnUpdateLB(string _name, string entriesLB, int[] rank, string[] photo, string[] playersName, int[] scorePlayers, bool auth)
        {
            if (_name == "initialized"){
                UpdateLB();
            }

            else if (_name == nameLB)
            {
                string error = "...";

                if (entriesLB == "No data")
                {
                    switch (YandexGame.savesData.language)
                    {
                        case "ru":
                            error = "Нет данных";
                            break;
                        case "tr":
                            error = "Veri yok";
                            break;
                        default:
                            error = "No data";
                            break;
                    }
                }                

                if (!advanced)
                {
                    if (entriesLB == "No data") entriesText.text = error;
                    else entriesText.text = entriesLB;
                }
                    
                else
                {
                    GameObject sampleContainer = transform.GetComponentInChildren<GridLayoutGroup>().transform.GetChild(0).gameObject;

                    for (int i = 0; i < sampleContainer.transform.parent.childCount; i++)
                        if (i != 0) Destroy(sampleContainer.transform.parent.GetChild(i).gameObject);

                    if (entriesLB == "No data")
                    {
                        sampleContainer.transform.Find("Rank").GetComponentInChildren<Text>().text = "";
                        sampleContainer.transform.Find("Score").GetComponentInChildren<Text>().text = "";
                        sampleContainer.transform.Find("Name").GetComponentInChildren<Text>().text = error;
                    }
                    else
                    {
                        for (int i = 0; i < rank.Length; i++)
                        {
                            if (i != 0) sampleContainer = Instantiate(sampleContainer, sampleContainer.transform.parent);

                            sampleContainer.transform.Find("Rank").GetComponentInChildren<Text>().text = rank[i].ToString();
                            sampleContainer.transform.Find("Name").GetComponentInChildren<Text>().text = playersName[i];

                            if (!timeTypeConvert)
                                sampleContainer.transform.Find("Score").GetComponentInChildren<Text>().text = scorePlayers[i].ToString();

                            else
                            {
                                string res = scorePlayers[i].ToString();
                                string milSec = decimalSize == 0 ? "" : "." + res.Remove(0, res.Length - decimalSize);

                                int secReal = int.Parse(res.Remove(res.Length - 3));
                                int min = (int)(secReal / 60.0f);
                                int sec = (int)secReal - min * 60;

                                string minStr;
                                if (min.ToString().Length == 1) minStr = "0" + min.ToString();
                                else minStr = min.ToString();

                                string secStr;
                                if (sec.ToString().Length == 1) secStr = "0" + sec.ToString();
                                else secStr = sec.ToString();

                                res = minStr + ":" + secStr + milSec;

                                sampleContainer.transform.Find("Score").GetComponentInChildren<Text>().text = res;
                            }

                            if (playerPhoto != PlayerPhoto.NonePhoto && photo[i] != "nonePhoto")
                                sampleContainer.transform.Find("Photo").GetComponentInChildren<ImageLoadYG>().Load(photo[i]);
                        }
                    }
                }
            }
        }

        public void UpdateLB()
        {
            YandexGame.GetLeaderboard(nameLB, maxQuantityPlayers, quantityTop, quantityAround, photoSize);
        }

        public void NewScore(int score) => YandexGame.NewLeaderboardScores(nameLB, score);

        public void NewScoreTimeConvert(float score) => YandexGame.NewLBScoreTimeConvert(nameLB, score);
    }
}

