using System.IO;
using UnityEngine;

namespace YG.Insides.BuildModify
{
    public static partial class ModifyIndexFile
    {
        static void SetBakcgroundFormat(ref string fileText)
        {
            string searchCode = @"canvas.style.background = ""url('background.png') center / cover"";";

            if (!fileText.Contains(searchCode))
            {
                Debug.LogWarning("Search string not found in index.html");
                return;
            }

            InfoYG infoYG = ConfigYG.GetInfoYG();

            if (infoYG.bakcgroundImage == InfoYG.BakcgroundImage.png)
            {
                DeleteImage("jpg");
                DeleteImage("gif");
            }
            else if (infoYG.bakcgroundImage == InfoYG.BakcgroundImage.jpg)
            {
                fileText = fileText.Replace(searchCode, searchCode.Replace("png", "jpg"));
                DeleteImage("png");
                DeleteImage("gif");
            }
            else if (infoYG.bakcgroundImage == InfoYG.BakcgroundImage.gif)
            {
                fileText = fileText.Replace(searchCode, searchCode.Replace("png", "gif"));
                DeleteImage("png");
                DeleteImage("jpg");
            }
            else if (infoYG.bakcgroundImage == InfoYG.BakcgroundImage.unity)
            {
                fileText = fileText.Replace(searchCode, "canvas.style.background = backgroundUnity;");
                DeleteImage("png");
                DeleteImage("jpg");
                DeleteImage("gif");
            }
            else if (infoYG.bakcgroundImage == InfoYG.BakcgroundImage.no)
            {
                fileText = fileText.Replace(searchCode, string.Empty);
                DeleteImage("png");
                DeleteImage("jpg");
                DeleteImage("gif");
            }

            if (infoYG.bakcgroundImage != InfoYG.BakcgroundImage.unity)
            {
                fileText = fileText.Replace(@"var backgroundUnity = ""url('"" + buildUrl + ""/') center / cover"";", string.Empty);
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
