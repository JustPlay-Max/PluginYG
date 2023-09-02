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
            YandexGame.Instance.RewardVideo(id);
            YandexGame.Instance.CloseVideo();
            Destroy(gameObject);
        }
    }
}
#endif