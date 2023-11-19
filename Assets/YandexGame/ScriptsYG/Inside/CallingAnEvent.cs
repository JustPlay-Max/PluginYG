#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace YG.Insides
{
    public class CallingAnEvent : MonoBehaviour
    {
        public IEnumerator CallingAd(float duration)
        {
            yield return new WaitForSecondsRealtime(YandexGame.Instance.infoYG.loadAdWithDelaySimulation);
            YandexGame.Instance.OpenFullAd();
            DrawScreen(new Color(0, 1, 0, 0.5f));
            yield return new WaitForSecondsRealtime(duration);
            YandexGame.Instance.CloseFullAd();
            Destroy(gameObject);
        }

        public IEnumerator CallingAd(float duration, int id)
        {
            yield return new WaitForSecondsRealtime(YandexGame.Instance.infoYG.loadAdWithDelaySimulation);

            YandexGame.Instance.OpenVideo();
            if (!YandexGame.Instance.infoYG.testErrorOfRewardedAdsInEditor)
                DrawScreen(new Color(0, 0, 1, 0.5f));
            else 
                DrawScreen(new Color(1, 0, 0, 0.5f));

            yield return new WaitForSecondsRealtime(duration);
            YandexGame.Instance.RewardVideo(id);
            YandexGame.Instance.CloseVideo();
            Destroy(gameObject);
        }

        private void DrawScreen(Color color)
        {
            GameObject obj = gameObject;
            Canvas canvas = obj.AddComponent<Canvas>();
            canvas.sortingOrder = 32767;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            obj.AddComponent<GraphicRaycaster>();
            obj.AddComponent<RawImage>().color = color;
        }
    }
}
#endif