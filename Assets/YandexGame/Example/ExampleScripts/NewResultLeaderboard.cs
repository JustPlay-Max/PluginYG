using UnityEngine;
using UnityEngine.UI;
using YG;

public class NewResultLeaderboard : MonoBehaviour
{
    [SerializeField] LeaderboardYG leaderboardYG;
    [SerializeField] InputField nameLbInputField;
    [SerializeField] InputField scoreLbInputField;

    public void NewName()
    {
        leaderboardYG.nameLB = nameLbInputField.text;
        leaderboardYG.UpdateLB();
    }

    public void NewScore()
    {
        leaderboardYG.NewScore(int.Parse(scoreLbInputField.text));

        // Или можете использовать статический метод:
        //YandexGame.NewLeaderboardScores(leaderboardYG.nameLB, int.Parse(scoreLbInputField.text));
    }
}
