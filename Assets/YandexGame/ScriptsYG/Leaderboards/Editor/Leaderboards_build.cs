using UnityEngine;
using System.IO;

namespace YG.EditorScr.BuildModify
{
    public partial class ModifyBuildManager
    {
        public static void Leaderboards()
        {
            string initFunction = "InitLeaderboards();\nconsole.log('Init Leaderboards');";
            AddIndexCode(initFunction, CodeType.init);

            string donorPatch = Application.dataPath + "/YandexGame/ScriptsYG/Leaderboards/Editor/Leaderboards_js.js";
            string donorText = File.ReadAllText(donorPatch);

            AddIndexCode(donorText, CodeType.js);
        }
    }
}
