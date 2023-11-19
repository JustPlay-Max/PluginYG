
var playerData = 'noData';

function InitPlayer(sendback) {
    return new Promise((resolve) => {
        try {
            if (ysdk == null) {
                NotAuthorized();
                if (sendback)
                    myGameInstance.SendMessage('YandexGame', 'SetInitializationSDK', NotAuthorized());
                resolve(NotAuthorized());
            }
            else {
                let _scopes = ___scopes___;
                ysdk.getPlayer({ scopes: _scopes })
                    .then(_player => {
                        player = _player;

                        let playerName = player.getName();
                        let playerPhoto = player.getPhoto('___photoSize___');

                        if (!_scopes) {
                            playerName = "anonymous";
                            playerPhoto = "null";
                        }

                        if (player.getMode() === 'lite') {

                            console.log('Not Authorized');
                            if (sendback)
                                myGameInstance.SendMessage('YandexGame', 'SetInitializationSDK', NotAuthorized());
                            resolve(NotAuthorized());
                        } else {
                            let authJson = {
                                "playerAuth": "resolved",
                                "playerName": playerName,
                                "playerId": player.getUniqueID(),
                                "playerPhoto": playerPhoto
                            };
                            if (sendback)
                                myGameInstance.SendMessage('YandexGame', 'SetInitializationSDK', JSON.stringify(authJson));
                            resolve(JSON.stringify(authJson));
                        }
                    }).catch(e => {
                        console.error('Authorized err: ', e.message);
                        if (sendback)
                            myGameInstance.SendMessage('YandexGame', 'SetInitializationSDK', NotAuthorized());
                        resolve(NotAuthorized());
                    });
            }
        } catch (e) {
            console.error('CRASH init Player: ', e.message);
            if (sendback)
                myGameInstance.SendMessage('YandexGame', 'SetInitializationSDK', NotAuthorized());
            resolve(NotAuthorized());
        }
    });
}

function NotAuthorized() {
    let authJson = {
        "playerAuth": "rejected",
        "playerName": "unauthorized",
        "playerId": "unauthorized",
        "playerPhoto": "null"
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