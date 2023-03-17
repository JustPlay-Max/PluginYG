using UnityEngine;
using UnityEngine.Events;

namespace YG
{
    public class ReviewYG : MonoBehaviour
    {
        [Tooltip("Открывать окно авторизации, если пользователь не авторизован.")]
        public bool authDialog;
        [Tooltip("Активировать оценку игры на мобильных устройствах? На мобильных устройствах открытие окна для оценки может вызывать зависание игры!")]
        public bool showOnMobileDevice;
        [Space(15)]
        public UnityEvent ReviewAvailable;
        public UnityEvent ReviewNotAvailable;
        public UnityEvent LeftReview;
        public UnityEvent NotLeftReview;

        private void Awake() => ReviewNotAvailable.Invoke();

        private void OnEnable()
        {
            YandexGame.GetDataEvent += UpdateData;
            YandexGame.ReviewSentEvent += ReviewSent;

            if (YandexGame.SDKEnabled) UpdateData();
        }
        private void OnDisable()
        {
            YandexGame.GetDataEvent -= UpdateData;
            YandexGame.ReviewSentEvent -= ReviewSent;
        }

        public void UpdateData()
        {
#if UNITY_EDITOR
            YandexGame.EnvironmentData.reviewCanShow = true;
#endif

            if (!showOnMobileDevice && (YandexGame.EnvironmentData.isMobile || YandexGame.EnvironmentData.isTablet))
                YandexGame.EnvironmentData.reviewCanShow = false;

            if (!authDialog && !YandexGame.auth)
            {
                ReviewNotAvailable.Invoke();
                return;
            }

            if (YandexGame.EnvironmentData.reviewCanShow)
                ReviewAvailable.Invoke();
            else ReviewNotAvailable.Invoke();
        }

        void ReviewSent(bool sent)
        {
            if (sent) LeftReview.Invoke();
            else NotLeftReview.Invoke();

            ReviewNotAvailable.Invoke();
        }

        public void ReviewShow() => YandexGame.ReviewShow(authDialog);
    }
}
