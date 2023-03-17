using UnityEngine;

namespace YG.Example
{
    public class GetLederboardData : MonoBehaviour
    {
        public LeaderboardYG leaderboardYG;

        public void DebugLederboardData()
        {
            string debugData = $"Debug players data from the leaderboard '{leaderboardYG.name}':\n";

            foreach (LeaderboardYG.PlayersData data in leaderboardYG.playersData)
            {
                string scoreTimeConvert = leaderboardYG.TimeTypeConvert(data.score);
                debugData += $"{data.rank}. {data.name} {scoreTimeConvert}\n";
            }

            Debug.Log(debugData);
        }
    }
}
