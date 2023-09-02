using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Build.Reporting;
using UnityEditor.Build;
using System.IO;

namespace YG.EditorScr.BuildModify
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
        public static void ModifyBuildDo(BuildTarget target, string pathToBuiltProject)
        {
            ModifyBuildManager.ModifyIndex(pathToBuiltProject);
            ArchivingBuild.Archiving(pathToBuiltProject);
            BuildLog.WritingLog(pathToBuiltProject);
        }

        [MenuItem("Tools/PluginYG/Run Modify Build", false)]
        public static void ModifyBuildMenuItem()
        {
            string buildPatch = BuildLog.ReadProperty("Build path");

            if (buildPatch != null)
                ModifyBuildManager.ModifyIndex(buildPatch);
            else
                Debug.LogError("Log is missing build patch");
        }
    }
}