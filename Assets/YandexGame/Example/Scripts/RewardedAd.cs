using UnityEngine;
using UnityEngine.UI;

namespace YG.Example
{
    public class RewardedAd : MonoBehaviour
    {
        [SerializeField] int AdID;
        [SerializeField] Text textMoney;

        int moneyCount = 0;

        void Start() => AdMoney(0);

        private void OnEnable() => YandexGame.RewardVideoEvent += Rewarded;
        private void OnDisable() => YandexGame.RewardVideoEvent -= Rewarded;

        void Rewarded(int id)
        {
            if (id == AdID)
                AdMoney(1);
        }

        void AdMoney(int count)
        {
            moneyCount += count;
            textMoney.text = "" + moneyCount;
        }
    }
}