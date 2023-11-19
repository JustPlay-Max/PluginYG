using UnityEngine;
using System.IO;

namespace YG.EditorScr.BuildModify
{
    public partial class ModifyBuildManager
    {
        public static void SetMetricaCounterID()
        {
            if (infoYG.metricaEnable)
            {
                string donorPatch = Application.dataPath + "/YandexGame/ScriptsYG/Editor/PostProcessBuild/ModifyIndexFile/MetricaDonor.html";
                string donorText = File.ReadAllText(donorPatch);
                donorText = donorText.Replace("METRICA_COUNTER_ID", infoYG.metricaCounterID.ToString());

                AddIndexCode(donorText, CodeType.head);
            }
        }
    }
}
