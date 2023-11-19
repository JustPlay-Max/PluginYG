
var cloudSaves = 'noData';

function SaveCloud(jsonData, flush) {
    if (player == null) {
        console.error('CRASH Save Cloud: ', 'Didnt have time to load');
        return;
    }
    try {
        player.setData({
            saves: [jsonData],
        }, flush);
    } catch (e) {
        console.error('CRASH Save Cloud: ', e.message);
    }
}

function LoadCloud(sendback) {
    return new Promise((resolve) => {
        if (ysdk == null) {
            if (sendback)
                myGameInstance.SendMessage('YandexGame', 'SetLoadSaves', 'noData');
            resolve('noData');
            return;
        }
        try {
            ysdk.getPlayer({ scopes: false })
                .then(_player => {
                    _player.getData(["saves"]).then(data => {
                        if (data.saves) {
                            if (sendback)
                                myGameInstance.SendMessage('YandexGame', 'SetLoadSaves', JSON.stringify(data.saves));
                            resolve(JSON.stringify(data.saves));
                        } else {
                            if (sendback)
                                myGameInstance.SendMessage('YandexGame', 'SetLoadSaves', 'noData');
                            resolve('noData');
                        }
                    }).catch(() => {
                        console.error('Load Cloud Error!');
                        if (sendback)
                            myGameInstance.SendMessage('YandexGame', 'SetLoadSaves', 'noData');
                        resolve('noData');
                    });
                }).catch(e => {
                    console.error('Load Cloud Error!', e.message);
                    if (sendback)
                        myGameInstance.SendMessage('YandexGame', 'SetLoadSaves', 'noData');
                    resolve('noData');
                });
        }
        catch (e) {
            console.error('CRASH Load saves Cloud: ', e.message);
            if (sendback)
                myGameInstance.SendMessage('YandexGame', 'SetLoadSaves', 'noData');
            resolve('noData');
        }
    });
}