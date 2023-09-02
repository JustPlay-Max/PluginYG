using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using YG.Utils.Lang;
#if YG_TEXT_MESH_PRO
using TMPro;
#endif

namespace YG.Insides
{
    public class CSVFileEditorWindow : EditorWindow
    {
        [MenuItem("Tools/PluginYG/Localization/Import\\Export Language Translations")]
        public static void ShowWindow()
        {
            GetWindow<CSVFileEditorWindow>("Import\\Export Language Translations");
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
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Import", GUILayout.Height(30)))
                {
                    int countInpObj = 0;

                    for (int i = 0; i < objectsTranlate.Count; i++)
                    {
                        LanguageYG scr = objectsTranlate[i].GetComponent<LanguageYG>();

                        if (CSVManager.GetKeyForLangYG(scr) == null || CSVManager.GetKeyForLangYG(scr) == "")
                        {
                            Debug.LogError("(en) The text field is not filled in in the Text/TextMesh component, fill it in. (ru) На данном объекте не указан ID! В компоненте Text/TextMesh не заполнено поле text, заполните его.", scr);
                            continue;
                        }

                        string[] translfers = CSVManager.ImportTransfersByKey(scr);

                        if (translfers != null)
                        {
                            scr.languages = CSVManager.ImportTransfersByKey(scr);
                            countInpObj++;
                        }
                    }

                    Debug.Log($"The import has been made! {countInpObj} from {objectsTranlate.Count} objects processed. (ru) Импорт произведен! {countInpObj} из {objectsTranlate.Count} объектов обработано.");
                }

                if (GUILayout.Button("Export", GUILayout.Height(30)))
                {
                    List<LanguageYG> langObj = new List<LanguageYG>();

                    for (int i = 0; i < objectsTranlate.Count; i++)
                    {
                        LanguageYG scr = objectsTranlate[i].GetComponent<LanguageYG>();
                        string textScr = null;

                        if (scr.componentTextField)
                        {
                            if (scr.textLComponent)
                            {
                                textScr = scr.textLComponent.text;
                                scr.text = textScr;
                            }
#if YG_TEXT_MESH_PRO
                            else if (scr.textMPComponent)
                            {
                                textScr = scr.textMPComponent.text;
                                scr.text = textScr;
                            }
#endif

                            if (scr.text == null || scr.text == "")
                            {
                                Debug.LogError("(en) The text field is not filled in in the Text/TextMesh component, fill it in. (ru) На данном объекте не указан ID! В компоненте Text/TextMesh не заполнено поле text, заполните его.", scr);
                                continue;
                            }
                        }
                        else
                        {
                            if (scr.text == null || scr.text == "")
                            {
                                Debug.LogError("The data object is not specified Apostille! In the component parts of the undeclared Field, The Undeclared egos. (ru) На данном объекте не указан ID! В компоненте LanguageYG не заполнено поле ID, заполните его.", scr);
                                continue;
                            }
                            else
                                textScr = scr.text;
                        }

                        bool clon = false;
                        foreach (LanguageYG l in langObj)
                        {
                            if (l != null)
                            {
                                if (textScr == l.text)
                                    clon = true;
                            }
                        }

                        if (!clon)
                        {
                            langObj.Add(scr);
                        }
                    }

                    string[] idArr = new string[langObj.Count];
                    for (int i = 0; i < idArr.Length; i++)
                    {
                        idArr[i] = CSVManager.GetKeyForLangYG(langObj[i]);
                    }

                    if (langObj.Count > 0)
                    {
                        InfoYG infoYG = langObj[0].infoYG;

                        string[,] keys = new string[langObj.Count, LangMethods.LangArr(infoYG).Length + 1];

                        for (int i = 0; i < langObj.Count; i++)
                        {
                            for (int i2 = 0; i2 < LangMethods.LangArr(infoYG).Length + 1; i2++)
                            {
                                if (i2 == 0)
                                    keys[i, 0] = idArr[i];

                                else
                                    keys[i, i2] = langObj[i].languages[i2 - 1];
                            }
                        }

                        CSVManager.WriteCSVFile(infoYG, keys, idArr);

                        Debug.Log($"The export was successful! {langObj.Count} from {objectsTranlate.Count} objects processed. (ru) Экспорт произошёл успешно! {langObj.Count} из {objectsTranlate.Count} объектов обработано.");
                    }
                }

                GUILayout.EndHorizontal();
            }

            var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
            GUILayout.Label($"({objectsTranlate.Count} objects in the list)", style, GUILayout.ExpandWidth(true));

            if (objectsTranlate.Count > 10 && position.height < objectsTranlate.Count * 20.6f + 150)
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.Height(position.height - 150));

            for (int i = 0; i < objectsTranlate.Count; i++)
            {
                objectsTranlate[i] = (GameObject)EditorGUILayout.ObjectField($"{i + 1}. {objectsTranlate[i].name}", objectsTranlate[i], typeof(GameObject), false);
            }

            if (objectsTranlate.Count > 10 && position.height < objectsTranlate.Count * 20.6f + 150)
                GUILayout.EndScrollView();
        }
    }
}
