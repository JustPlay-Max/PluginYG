using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.SceneManagement;
using YG.Utils.Lang;
#if YG_TEXT_MESH_PRO
using TMPro;
#endif

namespace YG.Insides
{
    [CustomEditor(typeof(LanguageYG))]
    public class LanguageYGEditor : Editor
    {
        LanguageYG scr;

        GUIStyle red;
        GUIStyle green;

        int processTranslateLabel;

        private void OnEnable()
        {
            scr = (LanguageYG)target;
            scr.Serialize();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            scr = (LanguageYG)target;
            Undo.RecordObject(scr, "Undo LanguageYG");

            red = new GUIStyle(EditorStyles.label);
            red.normal.textColor = Color.red;
            green = new GUIStyle(EditorStyles.label);
            green.normal.textColor = Color.green;

            bool isNullTextComponent = false;

#if YG_TEXT_MESH_PRO
            if (scr.textLComponent == null && scr.textMPComponent == null)
                isNullTextComponent = true;
#else
            if (scr.textLComponent == null)
                isNullTextComponent = true;
#endif

            if (isNullTextComponent)
            {
                if (GUILayout.Button("Add component - Text Legasy", GUILayout.Height(23)))
                {
                    scr.textLComponent = scr.gameObject.AddComponent<Text>();
                    Undo.RecordObject(scr.textLComponent, "Undo textUIComponent");
                }
#if YG_TEXT_MESH_PRO
                if (GUILayout.Button("Add component - Text Mesh Pro UGUI", GUILayout.Height(23)))
                {
                    scr.textMPComponent = scr.gameObject.AddComponent<TextMeshProUGUI>();
                }
#endif
                return;
            }

            if (scr.infoYG == null)
            {
                if (GUILayout.Button("Identify infoYG", GUILayout.Height(35)))
                {
                    scr.infoYG = scr.GetInfoYG();
                    if (scr.infoYG == null)
                        Debug.LogError("InfoYG not found!");
                }
            }

            if (scr.infoYG)
            {
                if (scr.infoYG.translateMethod == InfoYG.TranslateMethod.CSVFile)
                {
                    GUILayout.BeginVertical("HelpBox");

                    scr.componentTextField = EditorGUILayout.ToggleLeft("Component Text/TextMeshPro Translate", scr.componentTextField);

                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button(">", GUILayout.Width(20)))
                    {
                        TranslationTableEditorWindow.ShowWindow();
                    }

                    bool availableStr = true;

                    if (scr.componentTextField)
                    {
                        if (scr.textLComponent)
                        {
                            GUILayout.Label(scr.textLComponent.text);

                            if (scr.textLComponent == null || scr.textLComponent.text.Length == 0)
                                availableStr = false;
                        }
#if YG_TEXT_MESH_PRO
                        else if (scr.textMPComponent)
                        {
                            GUILayout.Label(scr.textMPComponent.text);

                            if (scr.textMPComponent == null || scr.textMPComponent.text.Length == 0)
                                availableStr = false;
                        }
#endif
                    }
                    else
                    {
                        if (scr.text == null || scr.text.Length == 0)
                            availableStr = false;

                        scr.text = EditorGUILayout.TextField(scr.text, GUILayout.Height(20));
                    }

                    if (availableStr)
                    {
                        GUILayout.Label("ID Translate");
                    }
                    else
                    {
                        GUILayout.Label("ID Translate (necessarily)", red);
                    }

                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button("Import"))
                    {
                        string[] translfers = CSVManager.ImportTransfersByKey(scr);
                        if (translfers != null)
                            scr.languages = CSVManager.ImportTransfersByKey(scr);
                    }
                    if (GUILayout.Button("Export"))
                    {
                        CSVManager.SetIDLineFile(scr.infoYG, scr);
                    }

                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();

                    scr.textHeight = EditorGUILayout.Slider("Text Height", scr.textHeight, 20f, 400f);
                    UpdateLanguages(true);
                }
                else
                {
                    //scr.textHeight = EditorGUILayout.Slider("Text Height", scr.textHeight, 20f, 400f);

                    if (scr.infoYG.translateMethod == InfoYG.TranslateMethod.AutoLocalization)
                    {
                        GUILayout.BeginVertical("HelpBox");

                        scr.componentTextField = EditorGUILayout.ToggleLeft("Component Text/TextMeshPro Translate", scr.componentTextField);
                        scr.textHeight = EditorGUILayout.Slider("Text Height", scr.textHeight, 20f, 400f);

                        if (!scr.componentTextField)
                            scr.text = EditorGUILayout.TextArea(scr.text, GUILayout.Height(scr.textHeight));
                        else
                        {
                            if (scr.textLComponent)
                                GUILayout.Label(scr.textLComponent.text);
#if YG_TEXT_MESH_PRO
                            else if (scr.textMPComponent)
                                GUILayout.Label(scr.textMPComponent.text);
#endif
                        }

                        GUILayout.BeginHorizontal();

                        if (scr.componentTextField)
                        {
                            if (scr.textLComponent)
                            {
                                if (scr.textLComponent.text.Length > 0)
                                {
                                    GUILayout.Label("Text Component", green);

                                    if (GUILayout.Button("TRANSLATE"))
                                        TranslateButton();
                                }
                                else
                                    GUILayout.Label("Text Component", red);
                            }
#if YG_TEXT_MESH_PRO
                            else if (scr.textMPComponent)
                            {
                                if (scr.textMPComponent.text != null && scr.textMPComponent.text.Length > 0)
                                {
                                    GUILayout.Label("TextMeshPro Component", green);

                                    if (GUILayout.Button("TRANSLATE"))
                                        TranslateButton();
                                }
                                else
                                    GUILayout.Label("TextMeshPro Component", red);
                            }
#endif
                        }
                        else
                        {
                            if (scr.componentTextField || (scr.text == null || scr.text.Length == 0))
                            {
                                GUILayout.Label("FILL IN THE FIELD", red);
                            }
                            else if (GUILayout.Button("TRANSLATE"))
                                TranslateButton();
                        }

                        if (GUILayout.Button("CLEAR"))
                        {
                            scr.ru = "";
                            scr.en = "";
                            scr.tr = "";
                            scr.az = "";
                            scr.be = "";
                            scr.he = "";
                            scr.hy = "";
                            scr.ka = "";
                            scr.et = "";
                            scr.fr = "";
                            scr.kk = "";
                            scr.ky = "";
                            scr.lt = "";
                            scr.lv = "";
                            scr.ro = "";
                            scr.tg = "";
                            scr.tk = "";
                            scr.uk = "";
                            scr.uz = "";
                            scr.es = "";
                            scr.pt = "";
                            scr.ar = "";
                            scr.id = "";
                            scr.ja = "";
                            scr.it = "";
                            scr.de = "";
                            scr.hi = "";

                            scr.processTranslateLabel = "";
                            scr.countLang = processTranslateLabel;
                        }

                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                    }

                    GUILayout.BeginVertical("box");
                    GUILayout.BeginHorizontal();

                    bool labelProcess = false;

                    if (scr.infoYG.translateMethod == InfoYG.TranslateMethod.AutoLocalization)
                    {
                        if (scr.processTranslateLabel != "")
                        {
                            if (scr.countLang == processTranslateLabel)
                            {
                                GUILayout.Label(scr.processTranslateLabel, green, GUILayout.Height(20));
                                labelProcess = true;
                            }
                            else if (scr.processTranslateLabel == "")
                            {
                                labelProcess = true;
                            }
                            else
                            {
                                GUILayout.Label(scr.processTranslateLabel, GUILayout.Height(20));
                                labelProcess = true;
                            }

                            try
                            {
                                if (scr.processTranslateLabel.Contains("error"))
                                {
                                    GUILayout.Label(scr.processTranslateLabel, red, GUILayout.Height(20));
                                    labelProcess = true;
                                }
                            }
                            catch
                            {
                            }
                        }
                    }

                    if (labelProcess == false)
                        GUILayout.Label(processTranslateLabel + " Languages", GUILayout.Height(20));

                    try
                    {
                        if (!scr.processTranslateLabel.Contains("completed"))
                            GUILayout.Label("Go back to the inspector!", GUILayout.Height(20));
                    }
                    catch
                    {
                    }

                    GUILayout.EndHorizontal();

                    UpdateLanguages(false);
                    GUILayout.EndVertical();
                }
            }

            if (scr.textLComponent
#if YG_TEXT_MESH_PRO
                || scr.textMPComponent
#endif
                )
            {
                GUILayout.Space(10);
                GUILayout.BeginVertical("box");

                if (scr.textLComponent)
                {
                    scr.uniqueFont = (Font)EditorGUILayout.ObjectField("Unique Font", scr.uniqueFont, typeof(Font), false);
                    FontSettingsDraw();
                }
#if YG_TEXT_MESH_PRO
                else if (scr.textMPComponent)
                {
                    scr.uniqueFontTMP = (TMP_FontAsset)EditorGUILayout.ObjectField("Unique Font", scr.uniqueFontTMP, typeof(TMP_FontAsset), false);
                    FontTMPSettingsDraw();
                }
#endif
                GUILayout.EndVertical();
            }

            if (scr.additionalText != null)
                scr.additionalText = (LangYGAdditionalText)EditorGUILayout.ObjectField("Additional Text", scr.additionalText, typeof(LangYGAdditionalText), false);


            if (GUI.changed)
            {
                EditorUtility.SetDirty(scr.gameObject);
                EditorSceneManager.MarkSceneDirty(scr.gameObject.scene);
            }
        }

        readonly string buttonText_ReplaseFont = "Replace the font with the standard one";

        void FontSettingsDraw()
        {
            if (scr.infoYG.fonts.defaultFont.Length == 0)
                return;

            scr.fontNumber = Mathf.Clamp(scr.fontNumber, 0, scr.infoYG.fonts.defaultFont.Length - 1);
            if (scr.infoYG.fonts.defaultFont.Length > 1)
                scr.fontNumber = EditorGUILayout.IntField("Font Index (in array default fonts)", scr.fontNumber);

            if (scr.textLComponent.font == scr.infoYG.fonts.defaultFont[scr.fontNumber])
                return;

            if (GUILayout.Button(buttonText_ReplaseFont))
            {
                Undo.RecordObject(scr.textLComponent, "Undo TextLComponent.font");
                scr.textLComponent.font = scr.infoYG.fonts.defaultFont[scr.fontNumber];
            }
        }

#if YG_TEXT_MESH_PRO
        void FontTMPSettingsDraw()
        {
            if (scr.infoYG.fontsTMP.defaultFont.Length == 0)
                return;

            scr.fontNumber = Mathf.Clamp(scr.fontNumber, 0, scr.infoYG.fontsTMP.defaultFont.Length - 1);
            if (scr.infoYG.fontsTMP.defaultFont.Length > 1)
                scr.fontNumber = EditorGUILayout.IntField("Font Index (in array default fonts)", scr.fontNumber);

            if (scr.textMPComponent.font == scr.infoYG.fontsTMP.defaultFont[scr.fontNumber])
                return;

            if (GUILayout.Button(buttonText_ReplaseFont))
            {
                Undo.RecordObject(scr.textMPComponent, "Undo TextMPComponent.font");
                scr.textMPComponent.font = scr.infoYG.fontsTMP.defaultFont[scr.fontNumber];
            }
        }
#endif

        void TranslateButton()
        {
            scr.processTranslateLabel = "";
            scr.Translate(processTranslateLabel);
        }

        void UpdateLanguages(bool CSVFile)
        {
            processTranslateLabel = 0;
            bool[] langArr = LangMethods.LangArr(scr.infoYG);

            for (int i = 0; i < langArr.Length; i++)
            {
                if (langArr[i])
                {
                    processTranslateLabel++;
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(new GUIContent(LangMethods.LangName(i), CSVManager.FullNameLanguages()[i]), GUILayout.Width(20), GUILayout.Height(20));

                    if (i == 0) scr.ru = EditorGUILayout.TextArea(scr.ru, GUILayout.Height(scr.textHeight));
                    else if (i == 1) scr.en = EditorGUILayout.TextArea(scr.en, GUILayout.Height(scr.textHeight));
                    else if (i == 2) scr.tr = EditorGUILayout.TextArea(scr.tr, GUILayout.Height(scr.textHeight));
                    else if (i == 3) scr.az = EditorGUILayout.TextArea(scr.az, GUILayout.Height(scr.textHeight));
                    else if (i == 4) scr.be = EditorGUILayout.TextArea(scr.be, GUILayout.Height(scr.textHeight));
                    else if (i == 5) scr.he = EditorGUILayout.TextArea(scr.he, GUILayout.Height(scr.textHeight));
                    else if (i == 6) scr.hy = EditorGUILayout.TextArea(scr.hy, GUILayout.Height(scr.textHeight));
                    else if (i == 7) scr.ka = EditorGUILayout.TextArea(scr.ka, GUILayout.Height(scr.textHeight));
                    else if (i == 8) scr.et = EditorGUILayout.TextArea(scr.et, GUILayout.Height(scr.textHeight));
                    else if (i == 9) scr.fr = EditorGUILayout.TextArea(scr.fr, GUILayout.Height(scr.textHeight));
                    else if (i == 10) scr.kk = EditorGUILayout.TextArea(scr.kk, GUILayout.Height(scr.textHeight));
                    else if (i == 11) scr.ky = EditorGUILayout.TextArea(scr.ky, GUILayout.Height(scr.textHeight));
                    else if (i == 12) scr.lt = EditorGUILayout.TextArea(scr.lt, GUILayout.Height(scr.textHeight));
                    else if (i == 13) scr.lv = EditorGUILayout.TextArea(scr.lv, GUILayout.Height(scr.textHeight));
                    else if (i == 14) scr.ro = EditorGUILayout.TextArea(scr.ro, GUILayout.Height(scr.textHeight));
                    else if (i == 15) scr.tg = EditorGUILayout.TextArea(scr.tg, GUILayout.Height(scr.textHeight));
                    else if (i == 16) scr.tk = EditorGUILayout.TextArea(scr.tk, GUILayout.Height(scr.textHeight));
                    else if (i == 17) scr.uk = EditorGUILayout.TextArea(scr.uk, GUILayout.Height(scr.textHeight));
                    else if (i == 18) scr.uz = EditorGUILayout.TextArea(scr.uz, GUILayout.Height(scr.textHeight));
                    else if (i == 19) scr.es = EditorGUILayout.TextArea(scr.es, GUILayout.Height(scr.textHeight));
                    else if (i == 20) scr.pt = EditorGUILayout.TextArea(scr.pt, GUILayout.Height(scr.textHeight));
                    else if (i == 21) scr.ar = EditorGUILayout.TextArea(scr.ar, GUILayout.Height(scr.textHeight));
                    else if (i == 22) scr.id = EditorGUILayout.TextArea(scr.id, GUILayout.Height(scr.textHeight));
                    else if (i == 23) scr.ja = EditorGUILayout.TextArea(scr.ja, GUILayout.Height(scr.textHeight));
                    else if (i == 24) scr.it = EditorGUILayout.TextArea(scr.it, GUILayout.Height(scr.textHeight));
                    else if (i == 25) scr.de = EditorGUILayout.TextArea(scr.de, GUILayout.Height(scr.textHeight));
                    else if (i == 26) scr.hi = EditorGUILayout.TextArea(scr.hi, GUILayout.Height(scr.textHeight));

                    GUILayout.EndHorizontal();
                }
            }
        }
    }
}
