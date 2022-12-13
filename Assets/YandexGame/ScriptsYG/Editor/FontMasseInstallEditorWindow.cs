using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace YG
{
    public class FontMasseInstallEditorWindow : EditorWindow
    {
        [MenuItem("Tools/PluginYG/Localization/Font Default Masse")]
        public static void ShowWindow()
        {
            GetWindow<FontMasseInstallEditorWindow>("Font Default Masse");
        }

        Vector2 scrollPosition = Vector2.zero;
        List<GameObject> objectsTranlate = new List<GameObject>();

        private void OnGUI()
        {
            GUILayout.Space(10);

            if (GUILayout.Button("Search for all objects on the scene by type LanguageYG", GUILayout.Height(30)))
            {
                objectsTranlate.Clear();

                foreach (LanguageYG obj in SceneAsset.FindObjectsOfType<LanguageYG>())
                {
                    objectsTranlate.Add(obj.gameObject);
                }
            }

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Add selected"))
            {
                foreach (GameObject obj in Selection.gameObjects)
                {
                    if (obj.GetComponent<LanguageYG>())
                    {
                        bool check = false;
                        for (int i = 0; i < objectsTranlate.Count; i++)
                            if (obj == objectsTranlate[i])
                                check = true;

                        if (!check)
                            objectsTranlate.Add(obj);
                    }
                }
            }

            if (GUILayout.Button("Remove selected"))
            {
                foreach (GameObject obj in Selection.gameObjects)
                {
                    objectsTranlate.Remove(obj);
                }
            }

            GUILayout.EndHorizontal();

            if (objectsTranlate.Count > 0)
            {
                if (GUILayout.Button("Clear"))
                {
                    objectsTranlate.Clear();
                }
            }

            if (objectsTranlate.Count > 0)
            {
                GUILayout.Space(10);

                if (GUILayout.Button("Put a standard font on all selected texts", GUILayout.Height(30)))
                {
                    int completeObjCoint = 0;

                    foreach (GameObject obj in objectsTranlate)
                    {
                        LanguageYG scr = obj.GetComponent<LanguageYG>();

                        if (scr.infoYG.fonts.defaultFont.Length >= scr.fontNumber + 1 && scr.infoYG.fonts.defaultFont[scr.fontNumber])
                        {
                            scr.ChangeFont(scr.infoYG.fonts.defaultFont);
                            completeObjCoint++;
                        }
                        else
                        {
                            Debug.LogError("The standard font is not specified! Specify it in the InfoYG plugin settings.  (ru) Не указан стандартный шрифт! Укажите его в настройках плагина InfoYG", scr.gameObject);
                        }
                    }

                    if (completeObjCoint == objectsTranlate.Count)
                    {
                        Debug.Log("The font was replaced successfully!  (ru) Замена шрифтов произведена успешно!");
                    }
                    else if (completeObjCoint == 0)
                    {
                        Debug.Log("Fonts have not been replaced!  (ru) Замена шрифтов не произведена!");
                    }
                    else
                    {
                        Debug.Log($"Fonts have been partially replaced! Replaced {completeObjCoint} fonts from {objectsTranlate.Count} (ru) Замена шрифтов произведена частично! Заменено {completeObjCoint} шрифтов из {objectsTranlate.Count}");
                    }
                }
            }

            var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
            GUILayout.Label($"({objectsTranlate.Count} objects in the list)", style, GUILayout.ExpandWidth(true));

            if (objectsTranlate.Count > 10 && position.height < objectsTranlate.Count * 20.6f + 150)
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.Height(position.height - 150));

            for (int i = 0; i < objectsTranlate.Count; i++)
            {
                objectsTranlate[i] = (GameObject)EditorGUILayout.ObjectField($"{i + 1}. { objectsTranlate[i].name}", objectsTranlate[i], typeof(GameObject), false);
            }

            if (objectsTranlate.Count > 10 && position.height < objectsTranlate.Count * 20.6f + 150)
                GUILayout.EndScrollView();
        }
    }
}
