using UnityEngine;
using UnityEngine.UI;
using YG;

public class RewardedAd : MonoBehaviour
{
    [SerializeField] int AdID;
    [SerializeField] Text textMoney;

    int moneyCount = 0;

    void Start() => AdMoney(0);

    private void OnEnable() => YandexGame.CloseVideoEvent += Rewarded;
    private void OnDisable() => YandexGame.CloseVideoEvent -= Rewarded;

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