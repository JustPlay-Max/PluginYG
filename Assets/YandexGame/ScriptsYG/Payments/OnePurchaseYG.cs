using System;
using UnityEngine;
using UnityEngine.UI;
using YG.Utils.Pay;
#if YG_TEXT_MESH_PRO
using TMPro;
#endif

namespace YG
{
    public class OnePurchaseYG : MonoBehaviour
    {
        public string id;
        public ImageLoadYG purchaseImageLoad;
        public ImageLoadYG currencyImageLoad;
        public bool showCurrencyCode;

        [Serializable]
        public struct TextLegasy
        {
            public Text title, description, priceValue;
        }
        public TextLegasy textLegasy;

#if YG_TEXT_MESH_PRO
        [Serializable]
        public struct TextMP
        {
            public TextMeshProUGUI title, description, priceValue;
        }
        public TextMP textMP;
#endif

        private void OnEnable() => YandexGame.GetPaymentsEvent += UpdateEntries;
        private void OnDisable() => YandexGame.GetPaymentsEvent -= UpdateEntries;

        private void Start()
        {
            if (YandexGame.SDKEnabled)
                UpdateEntries();
        }

        public void UpdateEntries()
        {
            Purchase data = YandexGame.PurchaseByID(id);

            if (data == null)
            {
                Debug.LogError($"No product with ID found: {id}");
                return;
            }

            if (textLegasy.title) textLegasy.title.text = data.title;
            if (textLegasy.description) textLegasy.description.text = data.description;
            if (textLegasy.priceValue)
            {
                if (showCurrencyCode) textLegasy.priceValue.text = data.price;
                else textLegasy.priceValue.text = data.priceValue;
            }

#if YG_TEXT_MESH_PRO
            if (textMP.title) textMP.title.text = data.title;
            if (textMP.description) textMP.description.text = data.description;
            if (textMP.priceValue)
            {
                if (showCurrencyCode) textMP.priceValue.text = data.price;
                else textMP.priceValue.text = data.priceValue;
            }
#endif
            if (purchaseImageLoad)
                purchaseImageLoad.Load(data.imageURI);

            if (currencyImageLoad && data.currencyImageURL != string.Empty && data.currencyImageURL != null)
                currencyImageLoad.Load(data.currencyImageURL);
        }

        public void BuyPurchase() => YandexGame.BuyPayments(id);
    }
}
