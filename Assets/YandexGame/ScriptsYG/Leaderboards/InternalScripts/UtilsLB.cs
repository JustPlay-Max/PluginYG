using System;
using System.Collections.Generic;
using UnityEngine;
using YG.Utils.Lang;

namespace YG.Utils.LB
{
    [Serializable]
    public class LBData
    {
        public string technoName;
        public string entries;
        public bool isDefault;
        public bool isInvertSortOrder;
        public int decimalOffset;
        public string type;
        public LBPlayerData[] players;
        public LBThisPlayerData thisPlayer;

#if UNITY_EDITOR
        public LBData()
        {
            technoName = "new";
            entries = "records of records";
            players = new LBPlayerData[6]
                {
                    new LBPlayerData { name = "anonymous", rank = 1, score = 10, uniqueID = "123", photo = InfoYG.avatarExample},
                    new LBPlayerData { name = "Ivan", rank = 2, score = 15, uniqueID = "321", photo = InfoYG.avatarExample },
                    new LBPlayerData { name = "Tanya", rank = 3, score = 23, uniqueID = "456", photo = InfoYG.avatarExample },
                    new LBPlayerData { name = "player4", rank = 4, score = 30, uniqueID = "321", photo = InfoYG.avatarExample },
                    new LBPlayerData { name = "playerThis", rank = 5, score = 40, uniqueID = "000", photo = InfoYG.avatarExample },
                    new LBPlayerData { name = "player6", rank = 6, score = 50, uniqueID = "321", photo = InfoYG.avatarExample }
                };
            type = "numeric";
        }
#endif
    }

    [Serializable]
    public class LBPlayerData
    {
        public int rank;
        public string name;
        public int score;
        public string photo;
        public string uniqueID;

#if UNITY_EDITOR
        public LBPlayerData()
        {
            rank = 0;
            name = "Player";
            score = 15;
            photo = InfoYG.avatarExample;
            uniqueID = "123";
        }
#endif
    }

    [Serializable]
    public class LBThisPlayerData
    {
        public int rank;
        public int score;
    }

    public static class LBMethods
    {
        public static string TimeTypeConvertStatic(int score, int decimalSize)
        {
            if (score < 1000)
                return "00:00";

            if (decimalSize == 1)
                return TimeSpan.FromMilliseconds(score).ToString("mm':'ss'.'f");
            else if (decimalSize == 2)
                return TimeSpan.FromMilliseconds(score).ToString("mm':'ss'.'ff");
            else if (decimalSize == 3)
                return TimeSpan.FromMilliseconds(score).ToString("mm':'ss'.'fff");
            else
                return TimeSpan.FromMilliseconds(score).ToString("mm':'ss");
        }

        public static string TimeTypeConvertStatic(int score)
        {
            return TimeTypeConvertStatic(score, 0);
        }

        public static string AnonimName(string origName)
        {
            if (origName != "anonymous") return origName;
            else return LangMethods.IsHiddenTextTranslate(YandexGame.Instance.infoYG);
        }

        public static void CopyLBData(out LBData copy, LBData original)
        {
            if (original == null)
            {
                copy = null;
                Debug.LogError("Original leaderboard data null!");
                return;
            }

            copy = new LBData()
            {
                type = original.type,
                technoName = original.technoName,
                entries = original.entries,
                isDefault = original.isDefault,
                isInvertSortOrder = original.isInvertSortOrder,
                decimalOffset = original.decimalOffset,
                thisPlayer = new LBThisPlayerData
                {
                    rank = original.thisPlayer.rank,
                    score = original.thisPlayer.score
                },
                players = new LBPlayerData[original.players.Length]
            };

            for (int i = 0; i < copy.players.Length; i++)
            {
                copy.players[i] = new LBPlayerData
                {
                    rank = original.players[i].rank,
                    name = original.players[i].name,
                    score = original.players[i].score,
                    photo = original.players[i].photo,
                    uniqueID = original.players[i].uniqueID
                };
            }
        }

        public static LBData SortLB(LBData lbData, int quantityTop, int quantityAround, int maxQuantityPlayers)
        {
            CopyLBData(out LBData lb, lbData);

            LBPlayerData thisPlayer = null;

            for (int i = 0; i < lb.players.Length; i++)
            {
                if (lb.players[i].uniqueID == YandexGame.playerId)
                    thisPlayer = lb.players[i];
            }

            if (thisPlayer != null)
            {
                List<LBPlayerData> top = new List<LBPlayerData>();

                int topMaxCount = quantityTop;
                topMaxCount = Mathf.Clamp(topMaxCount, 0, lb.players.Length);

                for (int i = 0; i < topMaxCount; i++)
                    top.Add(lb.players[i]);

                List<LBPlayerData> around = new List<LBPlayerData>();
                thisPlayer = null;
                int tPlayerIndex = 0;

                if (top.Count == quantityTop)
                {
                    for (int i = quantityTop; i < lb.players.Length; i++)
                    {
                        around.Add(lb.players[i]);

                        if (lb.players[i].uniqueID == YandexGame.playerId)
                        {
                            thisPlayer = lb.players[i];
                            tPlayerIndex = around.Count - 1;
                        }
                    }
                }

                if (around.Count > 0 && thisPlayer != null)
                {
                    List<LBPlayerData> beforePlayers = new List<LBPlayerData>();
                    List<LBPlayerData> afterPlayers = new List<LBPlayerData>();

                    for (int i = 0; i < tPlayerIndex; i++)
                        beforePlayers.Add(around[i]);

                    for (int i = tPlayerIndex + 1; i < around.Count; i++)
                        afterPlayers.Add(around[i]);

                    int beforeCountResult = quantityAround;
                    int afterCountResult = quantityAround;

                    if (beforePlayers.Count < quantityAround || afterPlayers.Count < quantityAround)
                    {
                        if (beforePlayers.Count < afterPlayers.Count)
                        {
                            beforeCountResult = beforePlayers.Count;

                            afterCountResult = around.Count - 1 - beforeCountResult;
                            afterCountResult = Mathf.Clamp(afterCountResult, 0, quantityAround * 2 - beforeCountResult);
                        }
                        else
                        {
                            afterCountResult = afterPlayers.Count;

                            beforeCountResult = around.Count - 1 - afterCountResult;
                            beforeCountResult = Mathf.Clamp(beforeCountResult, 0, quantityAround * 2 - afterCountResult);
                        }
                    }

                    int beforePlayersCount = beforePlayers.Count;
                    beforePlayers = new List<LBPlayerData>();

                    for (int i = beforePlayersCount - beforeCountResult; i < beforePlayersCount; i++)
                        beforePlayers.Add(around[i]);

                    afterPlayers = new List<LBPlayerData>();
                    for (int i = tPlayerIndex + 1; i < tPlayerIndex + afterCountResult + 1; i++)
                        afterPlayers.Add(around[i]);

                    around = beforePlayers;
                    around.Add(thisPlayer);
                    around.AddRange(afterPlayers);
                }

                top.AddRange(around);

                if (top.Count > maxQuantityPlayers)
                {
                    List<LBPlayerData> trimmedList = top.GetRange(0, maxQuantityPlayers);
                    lb.players = trimmedList.ToArray();
                }
                else
                {
                    lb.players = top.ToArray();
                }
                return lb;
            }
            return lb;
        }
    }

}