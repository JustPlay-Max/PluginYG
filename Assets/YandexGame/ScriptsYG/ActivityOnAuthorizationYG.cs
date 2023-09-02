using System;
using UnityEngine;
using UnityEngine.Events;

namespace YG
{
    public class ActivityOnAuthorizationYG : MonoBehaviour
    {
        public GameObject activityObjAuth;

        [Serializable]
        public class Events
        {
            public UnityEvent authorizationTrue;
            public UnityEvent authorizationFalse;
        }
        public Events events;

        private void OnEnable()
        {
            YandexGame.GetDataEvent += DetermineAuthorization;

            if (YandexGame.SDKEnabled)
                DetermineAuthorization();
        }
        private void OnDisable() => YandexGame.GetDataEvent -= DetermineAuthorization;

        private void DetermineAuthorization()
        {
            if (YandexGame.auth)
            {
                if (activityObjAuth)
                    activityObjAuth.SetActive(true);

                events.authorizationTrue?.Invoke();
            }
            else
            {
                if (activityObjAuth)
                    activityObjAuth.SetActive(false);

                events.authorizationFalse?.Invoke();
            }
        }
    }
}