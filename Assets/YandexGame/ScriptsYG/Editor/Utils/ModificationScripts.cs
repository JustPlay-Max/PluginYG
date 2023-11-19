using UnityEngine;
using UnityEditor;
using System.IO;

namespace YG.Insides.Utils
{
    public static class ModificationScripts
    {
        public static void AddCommentInClass(string path)
        {
            path = Application.dataPath + "/" + path;
            if (File.Exists(path))
            {
                string[] lines = File.ReadAllLines(path);

                if (lines.Length > 0 && !lines[0].StartsWith("//"))
                {
                    lines[0] = "//" + lines[0];
                    File.WriteAllLines(path, lines);
                }
            }
            else
            {
                Debug.LogError($"{path} file was not found!");
            }

            AssetDatabase.Refresh();
        }

        public static void RemoveCommentInClass(string path)
        {
            path = Application.dataPath + "/" + path;
            if (File.Exists(path))
            {
                string[] lines = File.ReadAllLines(path);

                if (lines.Length > 0 && lines[0].StartsWith("//"))
                {
                    lines[0] = lines[0].Substring(2);
                    File.WriteAllLines(path, lines);
                }
            }
            else
            {
                Debug.LogError($"{path} file was not found!");
            }

            AssetDatabase.Refresh();
        }

        public static bool CheckIfCommentExists(string path)
        {
            path = Application.dataPath + "/" + path;
            if (File.Exists(path))
            {
                string[] lines = File.ReadAllLines(path);

                if (lines.Length > 0 && lines[0].StartsWith("//"))
                {
                    return false; // Комментарий найден в начале файла
                }
            }

            return true; // Комментарий отсутствует в начале файла
        }
    }
}