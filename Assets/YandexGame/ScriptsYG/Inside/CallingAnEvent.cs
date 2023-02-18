#if UNITY_EDITOR
using UnityEngine;

namespace YG.Insides
{
    public class CallingAnEvent : MonoBehaviour
    {
        public int idRewarded;
        public bool closeFull, closeVideo, rewardVideo;

        private void OnDestroy()
        {
            if (closeFull) YandexGame.Instance.CloseFullscreen();
            if (closeVideo) YandexGame.Instance.CloseVideo();
            if (rewardVideo) YandexGame.Instance.RewardVideo(idRewarded);
        }
    }
}
#endif