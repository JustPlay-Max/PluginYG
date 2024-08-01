
var paymentsData = 'none';

function GetPayments(sendback) {
    return new Promise((resolve) => {
        try {
            if (ysdk == null) {
                resolve('none');
                return;
            }

            ysdk.getPayments().then(_payments => {
                payments = _payments;

                payments.getCatalog()
                    .then(products => {
                        let productID = [];
                        let title = [];
                        let description = [];
                        let imageURI = [];
                        let price = [];
                        let priceValue = [];
                        let priceCurrencyCode = [];
                        let currencyImageURL = [];
                        let consumed = [];

                        payments.getPurchases().then(purchases => {
                            for (let i = 0; i < products.length; i++) {
                                productID[i] = products[i].id;
                                title[i] = products[i].title;
                                description[i] = products[i].description;
                                imageURI[i] = products[i].imageURI;
                                price[i] = products[i].price;
                                priceValue[i] = products[i].priceValue;
                                priceCurrencyCode[i] = products[i].priceCurrencyCode;
                                currencyImageURL[i] = products[i].getPriceCurrencyImage("medium");

                                consumed[i] = true;
                                for (i2 = 0; i2 < purchases.length; i2++) {
                                    if (purchases[i2].productID === productID[i]) {
                                        consumed[i] = false;
                                        break;
                                    }
                                }
                            }

                            let jsonPayments = {
                                "id": productID,
                                "title": title,
                                "description": description,
                                "imageURI": imageURI,
                                "price": price,
                                "priceValue": priceValue,
                                "priceCurrencyCode": priceCurrencyCode,
                                "currencyImageURL": currencyImageURL,
                                "consumed": consumed,
                                "language": ysdk.environment.i18n.lang
                            };

                            if (sendback)
                                myGameInstance.SendMessage('YandexGame', 'PaymentsEntries', JSON.stringify(jsonPayments));
                            resolve(JSON.stringify(jsonPayments));
                        });
                    });

            }).catch(e => {
                console.log('Purchases are not available', e.message);
                resolve('none');
            })
        } catch (e) {
            console.error('CRASH Init Payments: ', e.message);
            resolve('none');
        }
    });
}

function BuyPayments(id) {
    try {
        if (payments != null) {
            payments.purchase(id).then(() => {
                console.log('Purchase Success');
                ConsumePurchase(id);
                FocusGame();
            }).catch(e => {
                console.error('Purchase Failed', e.message);
                myGameInstance.SendMessage('YandexGame', 'OnPurchaseFailed', id);
                FocusGame();
            })
        } else {
            console.log('Payments == null');
        }
    } catch (e) {
        console.error('CRASH Buy Payments: ', e.message);
        FocusGame();
    }
}

function ConsumePurchase(id) {
    try {
        if (payments != null) {
            payments.getPurchases().then(purchases => {
                for (i = 0; i < purchases.length; i++) {
                    if (purchases[i].productID === id) {
                        payments.consumePurchase(purchases[i].purchaseToken);
                        myGameInstance.SendMessage('YandexGame', 'OnPurchaseSuccess', id);
                    }
                }
            });
        }
        else console.log('Consume purchase: payments null');
    } catch (e) {
        console.error('CRASH Consume Purchase: ', e.message);
    }
}

function ConsumePurchases() {
    try {
        if (payments != null) {
            payments.getPurchases().then(purchases => {
                console.log('Unprocessed purchases: ', purchases.length);
                for (i = 0; i < purchases.length; i++) {
                    payments.consumePurchase(purchases[i].purchaseToken);
                    myGameInstance.SendMessage('YandexGame', 'OnPurchaseSuccess', purchases[i].productID);
                }
            });
        }
        else console.log('Consume purchases: payments null');
    } catch (e) {
        console.error('CRASH Consume purchases: ', e.message);
    }
}