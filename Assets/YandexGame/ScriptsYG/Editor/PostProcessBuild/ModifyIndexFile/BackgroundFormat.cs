using System.IO;
using UnityEngine;

namespace YG.EditorScr.BuildModify
{
    public partial class ModifyBuildManager
    {
        public static void SetBackgroundFormat()
        {
            string searchCode = @"canvas.style.background = ""url('background.png') center / cover"";";

            if (!indexFile.Contains(searchCode))
            {
                Debug.LogWarning("Search string not found in index.html");
                return;
            }

            if (infoYG.backgroundImageFormat == InfoYG.BackgroundImageFormat.png)
            {
                DeleteImage("jpg");
                DeleteImage("gif");
            }
            else if (infoYG.backgroundImageFormat == InfoYG.BackgroundImageFormat.jpg)
            {
                indexFile = indexFile.Replace(searchCode, searchCode.Replace("png", "jpg"));
                DeleteImage("png");
                DeleteImage("gif");
            }
            else if (infoYG.backgroundImageFormat == InfoYG.BackgroundImageFormat.gif)
            {
                indexFile = indexFile.Replace(searchCode, searchCode.Replace("png", "gif"));
                DeleteImage("png");
                DeleteImage("jpg");
            }
            else if (infoYG.backgroundImageFormat == InfoYG.BackgroundImageFormat.unity)
            {
                indexFile = indexFile.Replace(searchCode, "canvas.style.background = backgroundUnity;");
                DeleteImage("png");
                DeleteImage("jpg");
                DeleteImage("gif");
            }
            else if (infoYG.backgroundImageFormat == InfoYG.BackgroundImageFormat.no)
            {
                indexFile = indexFile.Replace(searchCode, string.Empty);
                DeleteImage("png");
                DeleteImage("jpg");
                DeleteImage("gif");
            }

            if (infoYG.backgroundImageFormat != InfoYG.BackgroundImageFormat.unity)
            {
                indexFile = indexFile.Replace(@"var backgroundUnity = ""url('"" + buildUrl + ""/') center / cover"";", string.Empty);
            }

            void DeleteImage(string format)
            {
                string pathImage = BUILD_PATCH + "/background." + format;

                if (File.Exists(pathImage))
                {
                    File.Delete(pathImage);
                }
            }
        }

    }
}
