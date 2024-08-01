using System;
using UnityEngine;
using UnityEngine.UI;
using YG.Utils.Pay;
#if YG_TEXT_MESH_PRO
using TMPro;
#endif

namespace YG
{
    [HelpURL("https://www.notion.so/PluginYG-d457b23eee604b7aa6076116aab647ed#10e7dfffefdc42ec93b39be0c78e77cb")]
    public class PurchaseYG : MonoBehaviour
    {
        [Tooltip("Добавить код валюты к строке цены. (Например YAN)")]
        public bool showCurrencyCode;
        [Tooltip("Компонент ImageLoadYG для загрузки изображения покупки.")]
        public ImageLoadYG purchaseImageLoad;
        [Tooltip("Компонент ImageLoadYG для загрузки изображения валюты.")]
        public ImageLoadYG currencyImageLoad;

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
        public Purchase data = new Purchase();

        [ContextMenu(nameof(UpdateEntries))]
        public void UpdateEntries()
        {
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

        public void BuyPurchase()
        {
            YandexGame.BuyPayments(data.id);
        }
    }
}
