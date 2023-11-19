using UnityEngine;
using System.IO;

namespace YG.EditorScr.BuildModify
{
    public partial class ModifyBuildManager
    {
        public static void EnvirData()
        {
            string initFunction = "environmentData = await RequestingEnvironmentData();\nconsole.log('Init Envir ysdk');";
            AddIndexCode(initFunction, CodeType.init);

            string donorPatch = Application.dataPath + "/YandexGame/ScriptsYG/EnvironmentData/Editor/EnvirData_js.js";
            string donorText = File.ReadAllText(donorPatch);

            AddIndexCode(donorText, CodeType.js);
        }
    }
}
