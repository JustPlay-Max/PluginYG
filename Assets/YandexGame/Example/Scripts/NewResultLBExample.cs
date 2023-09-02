using UnityEngine;
using UnityEngine.UI;

namespace YG.Example
{
    public class NewResultLBExample : MonoBehaviour
    {
        [SerializeField] LeaderboardYG leaderboardYG;
        [SerializeField] InputField nameLbInputField;
        [SerializeField] InputField scoreLbInputField;

        // Код для примера! Смена технического названия таблицы и её обновление в компоненте LeaderboardYG
        public void NewName()
        {
            leaderboardYG.nameLB = nameLbInputField.text;
            leaderboardYG.UpdateLB();
        }

        public void NewScore()
        {
            // Статический метод добавление нового рекорда
            YandexGame.NewLeaderboardScores(leaderboardYG.nameLB, int.Parse(scoreLbInputField.text));

            // Метод добавление нового рекорда обращением к компоненту LeaderboardYG
            // leaderboardYG.NewScore(int.Parse(scoreLbInputField.text));
        }

        public void NewScoreTimeConvert()
        {
            // Статический метод добавление нового рекорда конвертированного в time тип
            YandexGame.NewLBScoreTimeConvert(leaderboardYG.nameLB, float.Parse(scoreLbInputField.text));

            // Метод добавление нового рекорда обращением к компоненту LeaderboardYG
            // leaderboardYG.NewScoreTimeConvert(float.Parse(scoreLbInputField.text));
        }
    }
}