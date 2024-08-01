using System.Collections;
using UnityEngine;
using YG;

public class AdNotificationYG : MonoBehaviour
{
    [Tooltip("Объект, который будет активироваться перед открытием рекламы. И деактивироваться при открытии.")]
    public GameObject notificationObj;
    [Min(0.1f), Tooltip("Максимальное время показа объекта нотификации. Если реклама так и не будет показана, то объект скроется через указанное в данном параметре время.")]
    public float waitingForAds = 1;

    public static bool isShowNotification;
    public static AdNotificationYG Instance;

    private Coroutine closeNotifCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            YandexGame.onAdNotification += OnAdNotification;
            YandexGame.OpenFullAdEvent += OnOpenAd;
            YandexGame.OpenVideoEvent += OnOpenAd;
            notificationObj.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        YandexGame.onAdNotification -= OnAdNotification;
        YandexGame.OpenFullAdEvent -= OnOpenAd;
        YandexGame.OpenVideoEvent -= OnOpenAd;
    }

    private void OnAdNotification()
    {
        ViewingAdsYG.onPause?.Invoke(true);
        notificationObj.SetActive(true);
        isShowNotification = true;
        closeNotifCoroutine = StartCoroutine(CloseNotification());
    }

    private IEnumerator CloseNotification()
    {
        yield return new WaitForSecondsRealtime(waitingForAds);
        notificationObj.SetActive(false);
        isShowNotification = false;
        ViewingAdsYG.onPause?.Invoke(false);
    }

    private void OnOpenAd()
    {
        notificationObj.SetActive(false);
        isShowNotification = false;

        if (closeNotifCoroutine != null)
        {
            StopCoroutine(closeNotifCoroutine);
        }
    }
}
