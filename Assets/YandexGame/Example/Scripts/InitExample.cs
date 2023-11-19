using UnityEngine;
using UnityEngine.UI;

namespace YG.Example
{
    public class InitExample : MonoBehaviour
    {
        public Text text;

        private string initEventText, 
            initAwakeText;
        private static int initEventCounter;

        private void Awake()
        {
            initAwakeText = $"Awake initialization data:" +
                $"\nPlayer - {YandexGame.playerName}" +
                $"\nDevice - {YandexGame.EnvironmentData.deviceType}" +
                $"\nLanguage - {YandexGame.EnvironmentData.language}" +
                $"\nID Save - {YandexGame.savesData.idSave}";

            ShowText();
        }

        private void OnEnable() => YandexGame.GetDataEvent += GetData;
        private void OnDisable() => YandexGame.GetDataEvent -= GetData;

        private void GetData()
        {
            initEventCounter++;
            initEventText = $"GetData event initialization data:" +
                $"\nGetData event counter - {initEventCounter}" +
                $"\nPlayer - {YandexGame.playerName}" +
                $"\nDevice - {YandexGame.EnvironmentData.deviceType}" +
                $"\nLanguage - {YandexGame.EnvironmentData.language}" +
                $"\nID Save - {YandexGame.savesData.idSave}";

            ShowText();
        }

        private void ShowText()
        {
            text.text = initEventText + "\n\n" + initAwakeText;
        }
    }
}