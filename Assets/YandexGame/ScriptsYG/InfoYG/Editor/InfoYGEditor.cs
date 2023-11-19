using UnityEngine;
using UnityEditor;
using YG.Insides.Utils;
using YG.EditorScr;

namespace YG.Insides
{
    [CustomEditor(typeof(InfoYG))]
    public class InfoYGEditor : Editor
    {
        bool localization_IsActive;
        bool saves_IsActive;
        bool textMP_IsActive;

        const string newtonsoftUrl = "com.unity.nuget.newtonsoft-json";
        const string textMPUrl = "com.unity.textmeshpro";

        const string define_Localization = "YG_NEWTONSOFT_FOR_AUTOLOCALIZATION";
        const string define_Saves = "YG_NEWTONSOFT_FOR_SAVES";
        const string define_TextMP = "YG_TEXT_MESH_PRO";

        private void OnEnable()
        {
            localization_IsActive = DefineSymbols.CheckDefine(define_Localization);
            saves_IsActive = DefineSymbols.CheckDefine(define_Saves);
            textMP_IsActive = DefineSymbols.CheckDefine(define_TextMP);
        }

        public override void OnInspectorGUI()
        {
            DocumentationEditor.DocButton();

            GUILayout.Space(10);
            base.OnInspectorGUI();
            GUILayout.Space(10);

            if (localization_IsActive)
            {
                if (GUILayout.Button("Deactivate Newtonsoft for AUTO_LOCALIZATION"))
                {
                    DefineSymbols.RemoveDefine(define_Localization);
                }
            }
            else
            {
                if (GUILayout.Button("Activate Newtonsoft for AUTO_LOCALIZATION"))
                {
                    if (!PackageDownloader.IsPackageImported(newtonsoftUrl))
                    {
                        PackageDownloader.DownloadPackage(newtonsoftUrl);
                    }

                    DefineSymbols.AddDefine(define_Localization);
                }
            }

            if (localization_IsActive)
            {
                GUILayout.Label("Newtonsoft is currently ACTIVE for auto_localization");
            }
            else
            {
                GUILayout.Label("Newtonsoft is currently DEACTIVE for auto_localization");
            }

            GUILayout.Space(7);

            if (saves_IsActive)
            {
                if (GUILayout.Button("Deactivate Newtonsoft for SAVES_DATA"))
                {
                    DefineSymbols.RemoveDefine(define_Saves);
                }
            }
            else
            {
                if (GUILayout.Button("Activate Newtonsoft for SAVES_DATA"))
                {
                    if (!PackageDownloader.IsPackageImported(newtonsoftUrl))
                    {
                        PackageDownloader.DownloadPackage(newtonsoftUrl);
                    }

                    DefineSymbols.AddDefine(define_Saves);
                }
            }

            if (saves_IsActive)
            {
                GUILayout.Label("Newtonsoft is currently ACTIVE for saves_data");
            }
            else
            {
                GUILayout.Label("Newtonsoft is currently DEACTIVE for saves_data");
            }

            GUILayout.Space(7);

            if (textMP_IsActive)
            {
                if (GUILayout.Button("Deactivate TEXT_MESH_PRO"))
                {
                    DefineSymbols.RemoveDefine(define_TextMP);
                }
            }
            else
            {
                if (GUILayout.Button("Activate TEXT_MESH_PRO"))
                {
                    if (!PackageDownloader.IsPackageImported(textMPUrl))
                    {
                        PackageDownloader.DownloadPackage(textMPUrl);
                    }

                    DefineSymbols.AddDefine(define_TextMP);
                }
            }

            if (textMP_IsActive)
            {
                GUILayout.Label("TextMeshPro is ACTIVE for pluginYG");
            }
            else
            {
                GUILayout.Label("TextMeshPro is DEACTIVE for pluginYG");
            }
        }
    }
}