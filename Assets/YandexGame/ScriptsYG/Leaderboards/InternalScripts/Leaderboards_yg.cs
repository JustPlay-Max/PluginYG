using System;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using YG.Utils.LB;

namespace YG
{
    public partial class YandexGame
    {
        public static Action<LBData> onGetLeaderboard;

        [DllImport("__Internal")]
        private static extern void SetLeaderboardScores(string nameLB, int score);

        public static void NewLeaderboardScores(string nameLB, long score)
        {
            score = (long)Mathf.Clamp(score, 0, 9007199254740991);

            if (Instance.infoYG.leaderboardEnable && auth)
            {
                if (Instance.infoYG.saveScoreAnonymousPlayers == false &&
                    playerName == "anonymous")
                    return;

#if !UNITY_EDITOR
                Message("New Leaderboard Record: " + score);
                SetLeaderboardScores(nameLB, (int)score);
#else
                Message($"New Leaderboard '{nameLB}' Record: {score}");
#endif
            }
        }

        public static void NewLBScoreTimeConvert(string nameLB, float secondsScore)
        {
            if (Instance.infoYG.leaderboardEnable && auth)
            {
                if (Instance.infoYG.saveScoreAnonymousPlayers == false &&
                    playerName == "anonymous")
                    return;

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
#if !UNITY_EDITOR
            if (Instance.infoYG.leaderboardEnable)
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

            if (Instance.infoYG.leaderboardEnable)
            {
                LBData lb = null;
                LBData[] LBs = new LBData[Instance.infoYG.leaderboardSimulation.Length];

                for (int i = 0; i < Instance.infoYG.leaderboardSimulation.Length; i++)
                {
                    LBs[i] = new LBData();
                    LBMethods.CopyLBData(out LBs[i], Instance.infoYG.leaderboardSimulation[i]);
                }

                for (int i = 0; i < LBs.Length; i++)
                {
                    if (nameLB == LBs[i].technoName)
                    {
                        lb = LBs[i];
                        break;
                    }
                }

                if (lb != null)
                {
                    lb.players = lb.players.OrderBy(item => item.score).ToArray();

                    if (!lb.isInvertSortOrder)
                        Array.Reverse(lb.players);

                    for (int i = 0; i < lb.players.Length; i++)
                    {
                        lb.players[i].rank = i + 1;
                    }

                    onGetLeaderboard?.Invoke(lb);
                }
                else
                {
                    NoData();
                }
            }
            else
            {
                NoData();
            }
#endif

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
        }

        // Receiving messages

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
    }
}
