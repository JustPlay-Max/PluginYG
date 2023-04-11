using UnityEngine;
using System.IO;
using System;

namespace YG.Insides.BuildModify
{
    public static partial class ModifyIndexFile
    {
        static void SetMetricaCounterID(ref string fileText)
        {
            InfoYG infoYG = ConfigYG.GetInfoYG();
            string replaceCode = "<!-- Yandex.Metrika counter -->";

            if (infoYG.metricaEnable)
            {
                string donorPatch = Application.dataPath + "/YandexGame/ScriptsYG/Editor/PostProcessBuild/ModifyIndexFile/MetricaDonor.html";
                string donorText = File.ReadAllText(donorPatch);
                donorText = donorText.Replace("METRICA_COUNTER_ID", infoYG.metricaCounterID.ToString());

                int index = fileText.IndexOf(replaceCode);
                fileText = fileText.Insert(index + replaceCode.Length, donorText);
            }
            else
            {
                fileText = fileText.Replace(replaceCode, string.Empty);
            }
        }
    }
}
