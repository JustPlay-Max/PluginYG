using System;
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
                    new LBPlayerData { name = "anonymous", rank = 1, score = 10, uniqueID = "123", photo = InfoYG.photoExample},
                    new LBPlayerData { name = "Ivan", rank = 2, score = 15, uniqueID = "321", photo = InfoYG.photoExample },
                    new LBPlayerData { name = "Tanya", rank = 3, score = 23, uniqueID = "456", photo = InfoYG.photoExample },
                    new LBPlayerData { name = "player4", rank = 4, score = 30, uniqueID = "321", photo = InfoYG.photoExample },
                    new LBPlayerData { name = "playerThis", rank = 5, score = 40, uniqueID = "000", photo = InfoYG.photoExample },
                    new LBPlayerData { name = "player6", rank = 6, score = 50, uniqueID = "321", photo = InfoYG.photoExample }
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
            photo = InfoYG.photoExample;
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

    public class LBMethods
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
    }

}