
let flasgsData = "null";

function GetFlags() {
    return new Promise((resolve) => {
        if (ysdk == null) {
            resolve('null');
            return;
        }
        try {
            ysdk.getFlags().then(flags => {
                let names = [];
                let values = [];

                for (let key in flags) {
                    if (flags.hasOwnProperty(key)) {
                        names.push(key);
                        values.push(flags[key]);
                    }
                }

                let jsonFlags = {
                    "names": names,
                    "values": values
                };

                resolve(JSON.stringify(jsonFlags));
            });
        } catch (e) {
            console.error('CRASH Get Flags: ', e.message);
            resolve('null');
        }
    });
}