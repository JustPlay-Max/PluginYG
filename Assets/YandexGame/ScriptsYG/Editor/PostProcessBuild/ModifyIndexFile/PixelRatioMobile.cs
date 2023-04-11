
namespace YG.Insides.BuildModify
{
    public static partial class ModifyIndexFile
    {
        static void SetPixelRatioMobile(ref string fileText)
        {
            InfoYG infoYG = ConfigYG.GetInfoYG();

            if (infoYG.pixelRatioEnable)
            {
                string value = infoYG.pixelRatioValue.ToString();
                value = value.Replace(",", ".");

                fileText = fileText.Replace("config.devicePixelRatio = 1", "config.devicePixelRatio = " + value);
            }
            else
            {
                fileText = fileText.Replace("config.devicePixelRatio = 1;", string.Empty);
            }
        }
    }
}
