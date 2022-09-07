using UnityEngine;
using UnityEngine.UI;
using UnityToolbag;

namespace YG
{
    public class PaymentsYG : MonoBehaviour
    {
        [Tooltip("Возможность купить один товар много раз.")]
        public bool multiplePurchase;

        [Tooltip("Приписка YAN в конце цены (валюта Яндекс Игр).")]
        public bool YANAddToPrice;

        [Tooltip("Отображение лишь одной покупки")]
        public bool onePurchase;

        [Tooltip("Введите ID покупки.\nВ Unity Editor будет выводиться тестовая покупка для симуляции. В черновике Я.И. будет выводиться ваша покупука созданная в Я.И.")]
        [ConditionallyVisible(nameof(onePurchase))]
        public string IDPurchase;

        private void OnEnable()
        {
            YandexGame.GetPaymentsEvent += UpdateCatalog;

            if (YandexGame.SDKEnabled)
            {
                UpdateCatalog();
            }
        }
        
        private void OnDisable() => YandexGame.GetPaymentsEvent -= UpdateCatalog;

        Transform tr;
        Transform group;
        Transform purchase;
        GameObject buyButton;

        public void UpdateCatalog()
        {
            if (!tr) tr = transform;

            if (onePurchase)
            {
#if !UNITY_EDITOR
                Purchase samplePurchase = YandexGame.PurchaseByID(IDPurchase);
                tr.Find("Description").GetComponent<Text>().text = samplePurchase.description;
#else
                Purchase samplePurchase = YandexGame.PurchaseByID("test");
                tr.Find("Description").GetComponent<Text>().text = "ID - " + IDPurchase;
#endif
                tr.GetComponentInChildren<ImageLoadYG>().Load(samplePurchase.imageURI);
                tr.Find("Title").GetComponent<Text>().text = samplePurchase.title;

                if (!buyButton)
                    buyButton = tr.Find("BuyButton").gameObject;
                if (!multiplePurchase && samplePurchase.purchased > 0)
                    buyButton.SetActive(false);
                else buyButton.SetActive(true);

                string price = samplePurchase.priceValue;
                if (YANAddToPrice) price += " YAN";
                tr.Find("Price").GetComponent<Text>().text = price;
            }
            else
            {
                group = tr.Find("Group");
                purchase = group.GetChild(0);

                if (!buyButton)
                    buyButton = purchase.Find("BuyButton").gameObject;

                for (int i = 0; i < group.childCount; i++)
                    if (i != 0) Destroy(group.GetChild(i).gameObject);

                if (YandexGame.PaymentsData.id.Length == 0)
                {
                    purchase.gameObject.SetActive(false);
                }
                else
                {
                    purchase.gameObject.SetActive(true);
                    buyButton.SetActive(true);

                    for (int i = 1; i < YandexGame.PaymentsData.id.Length; i++)
                    {
                        CreatePurchase(purchase, i, true);
                    }

                    CreatePurchase(purchase, 0, false);
                }
            }
        }

        public void GetPayments()
        {
            YandexGame.GetPayments();
        }

        void CreatePurchase(Transform copy, int num, bool instantiate)
        {
            if (instantiate)
                copy = Instantiate(copy, copy.parent);

            if (!multiplePurchase && YandexGame.PaymentsData.purchased[num] > 0)
                copy.Find("BuyButton").gameObject.SetActive(false);

            copy.GetComponentInChildren<ImageLoadYG>().Load(YandexGame.PaymentsData.imageURI[num]);
            copy.Find("Title").GetComponent<Text>().text = YandexGame.PaymentsData.title[num];
            copy.Find("Description").GetComponent<Text>().text = YandexGame.PaymentsData.description[num];

            string price = YandexGame.PaymentsData.priceValue[num];
            if (YANAddToPrice) price += " YAN";
            copy.Find("Price").GetComponent<Text>().text = price;
        }

        public void BuyFromCatalog(Transform purchase)
        {
            string id = null;

            for (int i = 0; i < group.childCount; i++)
            {
                if (group.GetChild(i) == purchase)
                {
                    id = YandexGame.PaymentsData.id[i];
                    break;
                }
            }

            if (id != null)
                YandexGame.BuyPayments(id);
        }

        public void BuyOnePurchase()
        {
            YandexGame.BuyPayments(IDPurchase);
        }
    }
}
