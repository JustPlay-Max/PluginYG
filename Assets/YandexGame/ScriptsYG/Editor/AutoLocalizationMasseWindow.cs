using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;

namespace YG.Insides
{
    public class AutoLocalizationMasse : EditorWindow
    {
        [MenuItem("Tools/PluginYG/Localization/Auto Localization Masse")]
        public static void ShowWindow()
        {
            GetWindow<AutoLocalizationMasse>("Auto Localization Masse");
        }

        Vector2 scrollPosition = Vector2.zero;
        List<GameObject> objectsTranlate = new List<GameObject>();

        private void OnGUI()
        {
            if (GameObject.FindObjectOfType<YandexGame>().infoYG.translateMethod == InfoYG.TranslateMethod.AutoLocalization)
            {
                GUILayout.Space(10);

                if (GUILayout.Button("Search for all objects on the scene by type Text/TextMesh", GUILayout.Height(30)))
                {
                    objectsTranlate.Clear();

                    foreach (Text obj in SceneAsset.FindObjectsOfType<Text>())
                    {
                        objectsTranlate.Add(obj.gameObject);
                    }

                    foreach (TextMesh obj in SceneAsset.FindObjectsOfType<TextMesh>())
                    {
                        objectsTranlate.Add(obj.gameObject);
                    }
                }

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
                        if (obj.GetComponent<Text>() || obj.GetComponent<TextMesh>())
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

                    if (GUILayout.Button("TRANSLATE", GUILayout.Height(30)))
                    {
                        foreach (GameObject obj in objectsTranlate)
                        {
                            LanguageYG scrAL = obj.GetComponent<LanguageYG>();

                            if (scrAL == null)
                                scrAL = obj.AddComponent<LanguageYG>();

                            scrAL.Serialize();
                            scrAL.componentTextField = true;
                            scrAL.Translate(19);
                        }
                    }

                    if (GUILayout.Button("Remove a component LanguageYG"))
                    {
                        foreach (GameObject obj in objectsTranlate)
                        {
                            LanguageYG scrAL = obj.GetComponent<LanguageYG>();

                            if (scrAL)
                                DestroyImmediate(scrAL);
                        }
                    }
                }

                var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
                GUILayout.Label($"({objectsTranlate.Count} objects in the list)", style, GUILayout.ExpandWidth(true));

                if (objectsTranlate.Count > 10 && position.height < objectsTranlate.Count * 20.6f + 170)
                    scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.Height(position.height - 170));

                for (int i = 0; i < objectsTranlate.Count; i++)
                {
                    objectsTranlate[i] = (GameObject)EditorGUILayout.ObjectField($"{i + 1}. {objectsTranlate[i].name}", objectsTranlate[i], typeof(GameObject), false);
                }

                if (objectsTranlate.Count > 10 && position.height < objectsTranlate.Count * 20.6f + 170)
                    GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Label("Select Auto Location Inspector in the plugin settings\nInfoYG -> Translate Metod -> AutoLocalization", GUILayout.ExpandWidth(true));
            }
        }
    }
}

