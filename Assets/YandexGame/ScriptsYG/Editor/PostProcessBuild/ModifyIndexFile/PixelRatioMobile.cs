
namespace YG.EditorScr.BuildModify
{
    public partial class ModifyBuildManager
    {
        public static void SetPixelRatioMobile()
        {
            if (infoYG.pixelRatioEnable)
            {
                string value = infoYG.pixelRatioValue.ToString();
                value = value.Replace(",", ".");

                indexFile = indexFile.Replace("config.devicePixelRatio = 1", "config.devicePixelRatio = " + value);
            }
            else
            {
                indexFile = indexFile.Replace("config.devicePixelRatio = 1;", string.Empty);
            }
        }
    }
}
