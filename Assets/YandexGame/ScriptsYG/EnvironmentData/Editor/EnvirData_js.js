
var environmentData = 'null';

function RequestingEnvironmentData(sendback) {
    return new Promise((resolve, reject) => {
        if (ysdk == null) {
            resolve('');
            return;
        }
        try {
            var promptCanShow = false;
            var reviewCanShow = false;

            ysdk.feedback.canReview()
                .then(({ value }) => {
                    if (value)
                        reviewCanShow = true;

                    ysdk.shortcut.canShowPrompt().then(prompt => {
                        if (prompt.canShow)
                            promptCanShow = true;

                        let jsonEnvir = {
                            "language": ysdk.environment.i18n.lang,
                            "domain": ysdk.environment.i18n.tld,
                            "deviceType": ysdk.deviceInfo.type,
                            "isMobile": ysdk.deviceInfo.isMobile(),
                            "isDesktop": ysdk.deviceInfo.isDesktop(),
                            "isTablet": ysdk.deviceInfo.isTablet(),
                            "isTV": ysdk.deviceInfo.isTV(),
                            "appID": ysdk.environment.app.id,
                            "browserLang": ysdk.environment.browser.lang,
                            "payload": ysdk.environment.payload,
                            "promptCanShow": promptCanShow,
                            "reviewCanShow": reviewCanShow
                        };
                        if (sendback)
                            myGameInstance.SendMessage('YandexGame', 'SetEnvirData', JSON.stringify(jsonEnvir));
                        resolve(JSON.stringify(jsonEnvir));
                        console.log("Environment Data: " + JSON.stringify(jsonEnvir));
                    });
                });
        } catch (e) {
            console.error('CRASH Requesting Environment Data: ', e.message);
            reject(e);
        }
    });
}