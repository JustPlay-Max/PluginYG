using UnityEngine;

namespace YG
{
    [HelpURL("https://www.notion.so/PluginYG-d457b23eee604b7aa6076116aab647ed#10e7dfffefdc42ec93b39be0c78e77cb")]
    public class ConsumePurchasesYG : MonoBehaviour
    {
        private void OnEnable() => YandexGame.GetDataEvent += ConsumePurchases;
        private void OnDisable() => YandexGame.GetDataEvent -= ConsumePurchases;

        private static bool consume;

        private void Start()
        {
            if (YandexGame.SDKEnabled)
                ConsumePurchases();
        }

        private void ConsumePurchases()
        {
            if (!consume)
            {
                consume = true;
                YandexGame.ConsumePurchases();
            }
        }
    }
}