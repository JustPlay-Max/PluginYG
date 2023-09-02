using UnityEngine;
using UnityEngine.Events;

namespace YG
{
    [HelpURL("https://www.notion.so/PluginYG-d457b23eee604b7aa6076116aab647ed#28b70d48d9be436088f60200c99807cd")]
    public class PromptYG : MonoBehaviour
    {
        [Header("Buttons serialize")]
        [Tooltip("ќбъект (отключЄнна€ кнопка или текст), который будет сообщать о том, что €рлык не поддерживаетс€. ƒанный объект можно не указывать, тогда, если €рлык не будет поддерживатьс€ - ничего не будет отображатьс€.")]
        public GameObject notSupported;
        [Tooltip("ќбъект (отключЄнна€ кнопка или текст), который будет сообщать о том, что €рлык уже установлен. ƒанный объект можно не указывать, тогда, если €рлык уже установлен - ничего не будет отображатьс€.")]
        public GameObject done;
        [Tooltip("ќбъект с кнопкой, котора€ будет предлагать установить €рлык на рабочий стол (возможно, за вознаграждение). ѕри клике на кнопку необходимо запускать метод PromptShow через данный скрипт или через YandexGame скрипт.")]
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