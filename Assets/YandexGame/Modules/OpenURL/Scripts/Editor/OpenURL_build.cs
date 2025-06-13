using UnityEngine;
using System.IO;

namespace YG.EditorScr.BuildModify
{
    public partial class ModifyBuildManager
    {
        public static void OpenURL()
        {
            string initFunction = "allGamesData = await GetAllGames();\nconsole.log('Init OpenURL ysdk');";
            AddIndexCode(initFunction, CodeType.init);

            string donorPatch = Application.dataPath + "/YandexGame/Modules/OpenURL/Scripts/Editor/OpenURL_js.js";
            string donorText = File.ReadAllText(donorPatch);

            AddIndexCode(donorText, CodeType.js);
        }
    }
}
