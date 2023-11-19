using System.IO;
using UnityEngine;

namespace YG.EditorScr.BuildModify
{
    public partial class ModifyBuildManager
    {
        public static void SetLogoImageFormat()
        {
            string searchCode = "logo.png";

            if (!indexFile.Contains(searchCode))
            {
                Debug.LogWarning("Search string not found in index.html");
                return;
            }

            if (infoYG.logoImageFormat == InfoYG.LogoImgFormat.png)
            {
                DeleteLogo("jpg");
                DeleteLogo("gif");
            }
            else if (infoYG.logoImageFormat == InfoYG.LogoImgFormat.jpg)
            {
                indexFile = indexFile.Replace(searchCode, searchCode.Replace("png", "jpg"));
                DeleteLogo("png");
                DeleteLogo("gif");
            }
            else if (infoYG.logoImageFormat == InfoYG.LogoImgFormat.gif)
            {
                indexFile = indexFile.Replace(searchCode, searchCode.Replace("png", "gif"));
                DeleteLogo("png");
                DeleteLogo("jpg");
            }
            else if (infoYG.logoImageFormat == InfoYG.LogoImgFormat.no)
            {
                indexFile = indexFile.Replace(@"<div id=""unity-logo""><img src=""logo.png""></div>", string.Empty);
                DeleteLogo("png");
                DeleteLogo("jpg");
                DeleteLogo("gif");
            }

            void DeleteLogo(string format)
            {
                string pathImage = BUILD_PATCH + "/logo." + format;

                if (File.Exists(pathImage))
                {
                    File.Delete(pathImage);
                }
            }
        }

    }
}
