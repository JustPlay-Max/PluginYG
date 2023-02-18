using System.IO;
using System.IO.Compression;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace YG.Insides
{
    public class ArchivingBuild : IPostprocessBuildWithReport
    {
        public int callbackOrder { get { return 0; } }

        public void OnPostprocessBuild(BuildReport report)
        {
            InfoYG infoYG = ConfigYG.GetInfoYG();

            if (infoYG.autoBuildArchiving)
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
}




