using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace YG
{
    public class TimerBeforeAdsYG : MonoBehaviour
    {
        [SerializeField,
            Tooltip("Объект таймера перед показом рекламы. Он будет активироваться и деактивироваться в нужное время.")]
        private GameObject secondsPanelObject;
        [SerializeField,
            Tooltip("Массив объектов, которые будут показываться по очереди через секунду. Сколько объектов вы поместите в массив, столько секунд будет отчитываться перед показом рекламы.\n\nНапример, поместите в массив три объекта: певый с текстом '3', второй с текстом '2', третий с текстом '1'.\nВ таком случае произойдёт отчет трёх секунд с показом объектов с цифрами перед рекламой.")]
        private GameObject[] secondObjects;

        [SerializeField, Tooltip("Пазуа с помощью компонента ViewingAdsYG.")]
        private bool pauseTo_ViewingAdsYG = true;

        [Space(20)]
        [SerializeField] private UnityEvent onShowTimer;
        [SerializeField] private UnityEvent onHideTimer;
        [SerializeField] private UnityEvent doPause;

        private int objSecCounter;

        private void Start()
        {
            if (secondsPanelObject)
                secondsPanelObject.SetActive(false);

            for (int i = 0; i < secondObjects.Length; i++)
                secondObjects[i].SetActive(false);

            if (secondObjects.Length > 0)
                StartCoroutine(CheckTimerAd());
            else
                Debug.LogError("Fill in the array 'secondObjects'");
        }

        IEnumerator CheckTimerAd()
        {
            while (true)
            {
                if (YandexGame.timerShowAd >= YandexGame.Instance.infoYG.fullscreenAdInterval
                    && Time.timeScale != 0)
                {
                    onShowTimer?.Invoke();
                    objSecCounter = 0;
                    if (secondsPanelObject)
                        secondsPanelObject.SetActive(true);

                    StartCoroutine(TimerAdShow());
                    yield break;
                }

                yield return new WaitForSeconds(1.0f); yield return new WaitForSecondsRealtime(1.0f);
            }
        }

        IEnumerator TimerAdShow()
        {
            if (pauseTo_ViewingAdsYG)
                ViewingAdsYG.onPause?.Invoke(true);

            doPause?.Invoke();

            while (true)
            {
                if (objSecCounter < secondObjects.Length)
                {
                    for (int i2 = 0; i2 < secondObjects.Length; i2++)
                        secondObjects[i2].SetActive(false);

                    secondObjects[objSecCounter].SetActive(true);
                    objSecCounter++;

                    yield return new WaitForSecondsRealtime(1.0f);
                }

                if (objSecCounter == secondObjects.Length)
                {
                    YandexGame.FullscreenShow();
                    StartCoroutine(BackupTimerClosure());

                    while (!YandexGame.nowFullAd)
                        yield return null;

                    RestartTimer();
                    yield break;
                }
            }
        }

        IEnumerator BackupTimerClosure()
        {
            yield return new WaitForSecondsRealtime(2.5f);

            if (objSecCounter != 0)
            {
                RestartTimer();
            }
        }

        private void RestartTimer()
        {
            secondsPanelObject.SetActive(false);
            onHideTimer?.Invoke();
            objSecCounter = 0;
            StartCoroutine(CheckTimerAd());
        }
    }
}
