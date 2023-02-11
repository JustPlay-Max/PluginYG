using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO.Compression;
using System.IO;

namespace YG.Insides
{
    public class ArchivingBuild : IPostprocessBuildWithReport
    {
        public int callbackOrder { get { return 0; } }

        public void OnPostprocessBuild(BuildReport report)
        {
            string patch = report.summary.outputPath;
            string number = "";

            if (!File.Exists(patch + ".zip"))
            {
                Archiving();
            }
            else
            {
                for (int i = 1; i < 100; i++)
                {
                    if (!File.Exists(patch + "_" + i + ".zip"))
                    {
                        number = "_" + i;
                        Archiving();
                        break;
                    }
                }
            }

            void Archiving()
            {
                ZipFile.CreateFromDirectory(patch, patch + number + ".zip");
            }
        }
    }
}




