using UnityEngine;
using System.IO;

namespace YG.EditorScr.BuildModify
{
    public class BuildLog
    {
        private readonly static string filePath = Application.dataPath + "/YandexGame/ScriptsYG/Editor/PostProcessBuild/ModifyIndexFile/BuildLogYG.txt",
            separator = ": ";

        public static void WritingLog(string buildPath)
        {
            string[] linesBasic = new string[]
            {
                "Build path", // 0
                "Build number", // 1
            };

            if (!File.Exists(filePath))
            {
                string newLines = "";
                for (int i = 0; i < linesBasic.Length; i++)
                {
                    newLines += linesBasic[i] + separator + "\n";
                }

                File.WriteAllText(filePath, newLines);
            }

            string[] lines = File.ReadAllLines(filePath);


            // Write lines log:
            // Build patch
            WritingLine(linesBasic[0], buildPath);

            // Buld number
            string readBuildNumber = ReadingLine(linesBasic[1]);
            int oldBuildNumber = 0;

            if (readBuildNumber != null && readBuildNumber != "")
                oldBuildNumber = int.Parse(ReadingLine(linesBasic[1]));

            string newBuildNumber = (oldBuildNumber + 1).ToString();
            WritingLine(linesBasic[1], newBuildNumber);


            File.WriteAllLines(filePath, lines);


            void WritingLine(string searchString, string write)
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains(searchString + separator))
                    {
                        string newLine = searchString + separator + write;
                        lines[i] = newLine;
                        break;
                    }
                }
            }

            string ReadingLine(string searchString)
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains(searchString))
                    {
                        return lines[i].Replace(searchString + separator, string.Empty);
                    }
                }
                return null;
            }
        }

        public static string ReadProperty(string property)
        {
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains(property + separator))
                    {
                        return lines[i].Replace(property + separator, string.Empty);
                    }
                }
            }

            return null;
        }
    }
}