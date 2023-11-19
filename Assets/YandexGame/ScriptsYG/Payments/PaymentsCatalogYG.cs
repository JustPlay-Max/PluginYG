using System;
using UnityEngine;
using UnityToolbag;
using YG.Utils.Pay;

namespace YG
{
    [HelpURL("https://www.notion.so/PluginYG-d457b23eee604b7aa6076116aab647ed#10e7dfffefdc42ec93b39be0c78e77cb")]
    public class PaymentsCatalogYG : MonoBehaviour
    {
        [SerializeField]
        private bool spawnPurchases = true;

        [SerializeField, ConditionallyVisible(nameof(spawnPurchases)), Tooltip("Родительский объект для спавна в нём покупок")]
        private Transform rootSpawnPurchases;
        [SerializeField, ConditionallyVisible(nameof(spawnPurchases)), Tooltip("Префаб покупки (объект со компонентом PurchaseYG)")]
        private GameObject purchasePrefab;
        public enum UpdateListMethod { OnEnable, Start, DoNotUpdate };
        [Tooltip("Когда следует обновлять список покупок?\nStart - Обновлять в методе Start.\nOnEnable - Обновлять при каждой активации объекта (в методе OnEnable)\nDoNotUpdate - Не обновлять.")]
        public UpdateListMethod updateListMethod;

        [SerializeField, Tooltip("Список покупок (PurchaseYG)")]
        public PurchaseYG[] purchases = new PurchaseYG[0];

        public Action onUpdatePurchasesList;

        private void OnEnable()
        {
            if (updateListMethod != UpdateListMethod.DoNotUpdate)
                YandexGame.GetPaymentsEvent += UpdatePurchasesList;

            if (YandexGame.SDKEnabled && updateListMethod == UpdateListMethod.OnEnable)
                UpdatePurchasesList();
        }

        private void OnDisable()
        {
            if (updateListMethod != UpdateListMethod.DoNotUpdate)
                YandexGame.GetPaymentsEvent -= UpdatePurchasesList;
        }

        private void Start()
        {
            if (YandexGame.purchases.Length > 0 &&
                updateListMethod == UpdateListMethod.Start)
            {
                UpdatePurchasesList();
            }
        }

        public void UpdatePurchasesList()
        {
            if (spawnPurchases)
            {
                DestroyPurchasesList();
                SpawnPurchasesList();
            }
            else
            {
                SetDataPurchasesListByID();
            }
            onUpdatePurchasesList?.Invoke();
        }

        private void DestroyPurchasesList()
        {
            int childCount = rootSpawnPurchases.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                Destroy(rootSpawnPurchases.GetChild(i).gameObject);
            }
        }

        private void SpawnPurchasesList()
        {
            purchases = new PurchaseYG[YandexGame.purchases.Length];
            for (int i = 0; i < YandexGame.purchases.Length; i++)
            {
                GameObject purchaseObj = Instantiate(purchasePrefab, rootSpawnPurchases);

                purchases[i] = purchaseObj.GetComponent<PurchaseYG>();
                purchases[i].data = YandexGame.purchases[i];
                purchases[i].UpdateEntries();
            }
        }

        private void SetDataPurchasesListByID()
        {
            for (int i = 0; i < purchases.Length; i++)
            {
                Purchase purchase = YandexGame.PurchaseByID(purchases[i].data.id);
                if (purchase != null)
                {
                    purchases[i].data = purchase;
                }
                else
                {
                    Debug.LogError($"Purchase with ID: {purchases[i].data.id} not found!");
                    continue;
                }

                purchases[i].UpdateEntries();
            }
        }

        public void BuyPurchase(string id)
        {
            YandexGame.BuyPayments(id);
        }
    }
}