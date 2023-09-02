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

                AddIndexCode(donorText, CodeType.js);
            }
        }
    }
}
