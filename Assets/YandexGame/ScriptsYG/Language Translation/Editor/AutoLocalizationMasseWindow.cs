using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
#if YG_TEXT_MESH_PRO
using TMPro;
#endif

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

                if (GUILayout.Button("Search for all objects on the scene by type TEXT (Legasy)", GUILayout.Height(30)))
                {
                    objectsTranlate.Clear();

                    foreach (Text obj in SceneAsset.FindObjectsOfType<Text>(includeInactive: true))
                    {
                        objectsTranlate.Add(obj.gameObject);
                    }
                }
#if YG_TEXT_MESH_PRO
                if (GUILayout.Button("Search for all objects on the scene by type TEXT_MESH_PRO", GUILayout.Height(30)))
                {
                    objectsTranlate.Clear();

                    foreach (TMP_Text obj in SceneAsset.FindObjectsOfType<TMP_Text>(includeInactive: true))
                    {
                        objectsTranlate.Add(obj.gameObject);
                    }
                }
#endif
                if (GUILayout.Button("Search for all objects on the scene by type LANGUAGE_YG", GUILayout.Height(30)))
                {
                    objectsTranlate.Clear();

                    foreach (LanguageYG obj in SceneAsset.FindObjectsOfType<LanguageYG>(includeInactive: true))
                    {
                        objectsTranlate.Add(obj.gameObject);
                    }
                }

                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Add selected from list", GUILayout.Height(22)))
                {
                    foreach (GameObject obj in Selection.gameObjects)
                    {
                        if (obj.GetComponent<Text>()
#if YG_TEXT_MESH_PRO
                            || obj.GetComponent<TMP_Text>()
#endif
                            )
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

                if (GUILayout.Button("Remove selected from list", GUILayout.Height(22)))
                {
                    foreach (GameObject obj in Selection.gameObjects)
                    {
                        objectsTranlate.Remove(obj);
                    }
                }

                if (objectsTranlate.Count > 0)
                {
                    if (GUILayout.Button("Clear list", GUILayout.Height(22)))
                    {
                        objectsTranlate.Clear();
                    }
                }

                GUILayout.EndHorizontal();

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

                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button("Remove component LanguageYG", GUILayout.Height(22)))
                    {
                        foreach (GameObject obj in objectsTranlate)
                        {
                            LanguageYG scrAL = obj.GetComponent<LanguageYG>();

                            if (scrAL)
                                DestroyImmediate(scrAL);
                        }
                    }

                    if (GUILayout.Button("Reserialize LanguageYG components", GUILayout.Height(22)))
                    {
                        foreach (GameObject obj in objectsTranlate)
                        {
                            LanguageYG scrAL = obj.GetComponent<LanguageYG>();

                            if (scrAL)
                                scrAL.Serialize();
                        }
                    }

                    GUILayout.EndHorizontal();
                }

                var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
                GUILayout.Label($"({objectsTranlate.Count} objects in the list)", style, GUILayout.ExpandWidth(true));

                if (objectsTranlate.Count > 10 && position.height < objectsTranlate.Count * 20.6f + 190)
                    scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.Height(position.height - 190));

                for (int i = 0; i < objectsTranlate.Count; i++)
                {
                    objectsTranlate[i] = (GameObject)EditorGUILayout.ObjectField($"{i + 1}. {objectsTranlate[i].name}", objectsTranlate[i], typeof(GameObject), false);
                }

                if (objectsTranlate.Count > 10 && position.height < objectsTranlate.Count * 20.6f + 190)
                    GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Label("Select Auto Location Inspector in the plugin settings\nInfoYG -> Translate Metod -> AutoLocalization", GUILayout.ExpandWidth(true));
            }

            if (GUI.changed && objectsTranlate.Count > 0)
            {
                EditorUtility.SetDirty(objectsTranlate[0].gameObject);
                EditorSceneManager.MarkSceneDirty(objectsTranlate[0].gameObject.scene);
            }
        }
    }
}

