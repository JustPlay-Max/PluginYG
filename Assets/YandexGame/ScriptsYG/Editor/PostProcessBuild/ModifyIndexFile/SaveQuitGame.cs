using UnityEngine;
using System.IO;

namespace YG.EditorScr.BuildModify
{
    public partial class ModifyBuildManager
    {
        public static void SetSaveQuitGame()
        {
            if (infoYG.saveOnQuitGame)
            {
                string donorPatch = Application.dataPath + "/YandexGame/ScriptsYG/Editor/PostProcessBuild/ModifyIndexFile/SaveQuitGame.js";
                string donorText = File.ReadAllText(donorPatch);

                donorText = donorText.Replace("{{{ObjectName}}}", infoYG.quitGameParameters.objectName);
                donorText = donorText.Replace("{{{MethodName}}}", infoYG.quitGameParameters.methodName);

                AddIndexCode(donorText, CodeType.js);
            }
        }
    }
}
