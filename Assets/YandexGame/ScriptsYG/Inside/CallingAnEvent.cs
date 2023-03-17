#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

namespace YG.Insides
{
    public class CallingAnEvent : MonoBehaviour
    {
        public IEnumerator CallingAd(float duration)
        {
            yield return new WaitForSecondsRealtime(duration);
            YandexGame.Instance.CloseFullAd();
            Destroy(gameObject);
        }

        public IEnumerator CallingAd(float duration, int id)
        {
            yield return new WaitForSecondsRealtime(duration);
            YandexGame.Instance.CloseVideo();
            YandexGame.Instance.RewardVideo(id);
            Destroy(gameObject);
        }
    }
}
#endif