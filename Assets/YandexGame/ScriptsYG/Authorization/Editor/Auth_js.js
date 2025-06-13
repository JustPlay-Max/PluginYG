
var playerData = 'noData';

async function InitPlayer(sendback) {
    return new Promise(async (resolve) => {
        try {
            if (ysdk == null) {
                NotAuthorized();
                if (sendback)
                    myGameInstance.SendMessage('YandexGame', 'SetInitializationSDK', NotAuthorized());
                return resolve(NotAuthorized());
            }
            else {
                let _scopes = ___scopes___;

                player = await ysdk.getPlayer({ scopes: _scopes });

                if (!player.isAuthorized()) {
                    return resolve(NotAuthorized());
                }

                let playerName = player.getName();
                let playerPhoto = player.getPhoto('___photoSize___');

                if (!_scopes) {
                    playerName = "anonymous";
                    playerPhoto = "null";
                }

                let authJson = {
                    "playerAuth": "resolved",
                    "playerName": playerName,
                    "playerId": player.getUniqueID(),
                    "playerPhoto": playerPhoto,
                    "payingStatus": player.getPayingStatus()
                };

                if (sendback)
                    myGameInstance.SendMessage('YandexGame', 'SetInitializationSDK', JSON.stringify(authJson));

                return resolve(JSON.stringify(authJson));
            }
        } catch (e) {
            console.error('CRASH init Player: ', e.message);
            if (sendback)
                myGameInstance.SendMessage('YandexGame', 'SetInitializationSDK', NotAuthorized());
            return resolve(NotAuthorized());
        }
    });
}

function NotAuthorized() {
    let authJson = {
        "playerAuth": "rejected",
        "playerName": "unauthorized",
        "playerId": "unauthorized",
        "playerPhoto": "unknown",
        "payingStatus": player.getPayingStatus()
    };
    return JSON.stringify(authJson);
}

function OpenAuthDialog() {
    if (ysdk !== null) {
        try {
            ysdk.auth.openAuthDialog().then(() => {
                InitPlayer(true)
                    .then(() => {
                        myGameInstance.SendMessage('YandexGame', 'GetDataInvoke');
                    });
            });
        } catch (e) {
            console.log('CRASH Open Auth Dialog: ', e.message);
        }
    }
}