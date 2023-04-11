using UnityEditor;
using UnityEngine;
using UnityEditor.Callbacks;
using UnityEditor.Build.Reporting;
using UnityEditor.Build;
using System.IO;

namespace YG.Insides.BuildModify
{
    public class PostProcessBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder { get { return 0; } }
        public void OnPreprocessBuild(BuildReport report)
        {
            string buildPatch = report.summary.outputPath + "/index.html";

            if (File.Exists(buildPatch))
            {
                File.Delete(buildPatch);
            }
        }

        [PostProcessBuild]
        public static void ModifyIndex(BuildTarget target, string pathToBuiltProject)
        {
            ModifyIndexFile.ModifyIndex(pathToBuiltProject);
            ArchivingBuild.Archiving(pathToBuiltProject);
        }
    }
}