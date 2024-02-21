using UnityEditor;
using UnityEditor.PackageManager;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace YG.Insides.Utils
{
    public class PackageDownloader
    {
        public static bool IsPackageImported(string packageName)
        {
            string packagePath = "Packages/" + packageName;
            PackageInfo packageInfo = PackageInfo.FindForAssetPath(packagePath);
            return packageInfo != null;
        }

        public static bool DownloadPackage(string packageName)
        {
            if (!IsPackageImported(packageName))
            {
                if (DialogDownloadPackage(packageName))
                {
                    Client.Add(packageName);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        private static bool DialogDownloadPackage(string packageName)
        {
            int option = EditorUtility.DisplayDialogComplex("Download package", $"To continue, you need to install the package: {packageName}\nInstall it?", "Yes", "No", "");

            if (option == 0)
                return true;
            else
                return false;
        }
    }
}