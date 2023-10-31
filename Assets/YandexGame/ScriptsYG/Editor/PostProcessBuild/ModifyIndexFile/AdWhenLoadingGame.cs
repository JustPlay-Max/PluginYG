
namespace YG.EditorScr.BuildModify
{
    public partial class ModifyBuildManager
    {
        public static void SetAdWhenLoadGameValue()
        {
            if (infoYG.showFirstAd == false)
            {
                indexFile = indexFile.Replace("FullAdShow(); // First ad true", "// First ad false");
            }
        }
    }
}
