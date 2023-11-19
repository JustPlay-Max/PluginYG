using System.Runtime.InteropServices;

namespace YG
{
    public partial class YandexGame
    {
        private static bool gameReadyUsed;

        [StartYG]
        public static void InitGRA()
        {
            if (Instance.infoYG.autoGameReadyAPI)
                GameReadyAPI();
        }

        [DllImport("__Internal")]
        private static extern void GameReadyAPI_js();

        public static void GameReadyAPI()
        {
            if (!gameReadyUsed)
            {
                gameReadyUsed = true;
#if !UNITY_EDITOR
                GameReadyAPI_js();
#endif
            }
        }
        public void _GameReadyAPI() => GameReadyAPI();
    }
}
