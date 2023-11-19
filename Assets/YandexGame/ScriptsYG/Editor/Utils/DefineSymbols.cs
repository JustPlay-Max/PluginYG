﻿using System.Collections.Generic;
using UnityEditor;

namespace YG.Insides.Utils
{
    [InitializeOnLoad]
    public class DefineSymbols
    {
        public static bool CheckDefine(string define)
        {
            BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

            if (defines.Contains(define))
            {
                return true;
            }
            else return false;
        }

        public static void AddDefine(string define)
        {
            BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

            if (defines.Contains(define))
                return;

            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, (defines + ";" + define));
        }

        public static void RemoveDefine(string define)
        {
            BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

            if (defines.Contains(define))
            {
                string[] defineArray = defines.Split(';');

                List<string> updatedDefines = new List<string>();
                foreach (string d in defineArray)
                {
                    if (d != define)
                    {
                        updatedDefines.Add(d);
                    }
                }

                string newDefines = string.Join(";", updatedDefines);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, newDefines);
            }
        }

        static DefineSymbols()
        {
            AddDefine("YG_PLUGIN_YANDEX_GAME");
        }
    }
}
