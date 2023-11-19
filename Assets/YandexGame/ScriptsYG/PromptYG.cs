using UnityEngine;
using UnityEngine.Events;

namespace YG
{
    [HelpURL("https://www.notion.so/PluginYG-d457b23eee604b7aa6076116aab647ed#28b70d48d9be436088f60200c99807cd")]
    public class PromptYG : MonoBehaviour
    {
        [Header("Buttons serialize")]
        [Tooltip("Объект (отключённая кнопка или текст), который будет сообщать о том, что ярлык не поддерживается. Данный объект можно не указывать, тогда, если ярлык не будет поддерживаться - ничего не будет отображаться.")]
        public GameObject notSupported;
        [Tooltip("Объект (отключённая кнопка или текст), который будет сообщать о том, что ярлык уже установлен. Данный объект можно не указывать, тогда, если ярлык уже установлен - ничего не будет отображаться.")]
        public GameObject done;
        [Tooltip("Объект с кнопкой, которая будет предлагать установить ярлык на рабочий стол (возможно, за вознаграждение). При клике на кнопку необходимо запускать метод PromptShow через данный скрипт или через YandexGame скрипт.")]
        public GameObject showDialog;
        [Header("Events")]
        [Space(5)]
        public UnityEvent onPromptSuccess;
        public UnityEvent onPromptFail;

        private void Awake()
        {
            if (notSupported) notSupported.SetActive(false);
            if (done) done.SetActive(false);
            showDialog.SetActive(false);
        }

        private void OnEnable()
        {
            YandexGame.GetDataEvent += UpdateData;
            YandexGame.PromptSuccessEvent += OnPromptSuccess;
            YandexGame.PromptFailEvent += OnPromptFail;

            if (YandexGame.SDKEnabled) UpdateData();
        }
        private void OnDisable()
        {
            YandexGame.GetDataEvent -= UpdateData;
            YandexGame.PromptSuccessEvent -= OnPromptSuccess;
            YandexGame.PromptFailEvent -= OnPromptFail;
        }

        public void UpdateData()
        {
#if UNITY_EDITOR
            YandexGame.EnvironmentData.promptCanShow = true;
#endif
            if (YandexGame.savesData.promptDone)
            {
                if (notSupported) notSupported.SetActive(false);
                if (done) done.SetActive(true);
                showDialog.SetActive(false);
            }
            else if (!YandexGame.EnvironmentData.promptCanShow)
            {
                if (notSupported) notSupported.SetActive(true);
                if (done) done.SetActive(false);
                showDialog.SetActive(false);
            }
            else
            {
                if (notSupported) notSupported.SetActive(false);
                if (done) done.SetActive(false);
                showDialog.SetActive(true);
            }
        }

        public void PromptShow() => YandexGame.PromptShow();

        void OnPromptSuccess()
        {
            onPromptSuccess?.Invoke();
            UpdateData();
        }
        void OnPromptFail()
        {
            YandexGame.EnvironmentData.promptCanShow = false;
            onPromptFail?.Invoke();
            UpdateData();
        }
    }
}