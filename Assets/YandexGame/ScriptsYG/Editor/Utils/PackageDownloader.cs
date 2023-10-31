using UnityEditor.PackageManager;
using UnityEngine;

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

        public static void DownloadPackage(string packageName)
        {
            Client.Add(packageName);
            Debug.Log($"Downloading package: {packageName}");
        }
    }
}