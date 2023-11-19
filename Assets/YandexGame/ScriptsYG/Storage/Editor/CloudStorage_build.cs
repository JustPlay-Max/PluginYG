using UnityEngine;
using System.IO;

namespace YG.EditorScr.BuildModify
{
    public partial class ModifyBuildManager
    {
        public static void CloudStorage()
        {
            string initFunction = "cloudSaves = await LoadCloud();\nconsole.log('Init Storage ysdk');";
            AddIndexCode(initFunction, CodeType.init);

            string donorPatch = Application.dataPath + "/YandexGame/ScriptsYG/Storage/Editor/CloudStorage_js.js";
            string donorText = File.ReadAllText(donorPatch);

            AddIndexCode(donorText, CodeType.js);
        }
    }
}
