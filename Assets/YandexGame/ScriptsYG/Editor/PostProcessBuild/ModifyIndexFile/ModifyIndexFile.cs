using System.IO;

namespace YG.Insides.BuildModify
{
    public static partial class ModifyIndexFile
    {
        private static string BUILD_PATCH;

        public static void ModifyIndex(string buildPatch)
        {
            BUILD_PATCH = buildPatch;
            string filePath = Path.Combine(buildPatch, "index.html");
            string fileText = File.ReadAllText(filePath);

            // Методы модификации index файла:
            SetBakcgroundFormat(ref fileText);
            SetAdWhenLoadGameValue(ref fileText);
            SetPixelRatioMobile(ref fileText);
            SetMetricaCounterID(ref fileText);

            File.WriteAllText(filePath, fileText);
        }
    }
}