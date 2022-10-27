using UnityEngine;
using UnityEngine.Events;

namespace YG {
    public class ReviewYG : MonoBehaviour
    {
        [Tooltip("Открывать окно авторизации, если пользователь не авторизован.")]
        public bool authDialog;
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
