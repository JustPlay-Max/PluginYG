using UnityEngine;
using UnityEditor;

namespace YG.EditorScr
{
    public class DocumentationEditor : Editor
    {
        public static readonly string DOC_URL = "https://ash-message-bf4.notion.site/PluginYG-d457b23eee604b7aa6076116aab647ed";

        [MenuItem("Tools/PluginYG/Documentation", false, 90)]
        public static void DocMenuItem()
        {
            Application.OpenURL(DOC_URL);
        }

        public static void DocButton()
        {
            if (GUILayout.Button("Documentation"))
            {
                Application.OpenURL(DOC_URL);
            }
        }
    }
}