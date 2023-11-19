
namespace YG.EditorScr.BuildModify
{
    public partial class ModifyBuildManager
    {
        public static void LoadGameRun()
        {
            if (infoYG.loadGameRun == false)
            {
                indexFile = indexFile.Replace("if (LocalHost()) // Delete when setting up: Load Game Run", "if (LocalHost())");
            }
            else
            {
                indexFile = indexFile.Replace("if (LocalHost()) // Delete when setting up: Load Game Run", "// Load Game Run = true");
            }
        }
    }
}
