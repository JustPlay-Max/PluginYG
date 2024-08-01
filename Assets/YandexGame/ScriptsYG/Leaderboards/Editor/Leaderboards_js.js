
let leaderboard = null;

function InitLeaderboards() {
    ysdk.getLeaderboards()
        .then(_lb => leaderboard = _lb);
}

function WaitForLeaderboard() {
    return new Promise((resolve, reject) => {
        const interval = setInterval(() => {
            if (leaderboard !== null) {
                clearInterval(interval);
                clearTimeout(timeout);
                resolve();
            }
        }, 100);

        const timeout = setTimeout(() => {
            clearInterval(interval);
            reject(new Error("Leaderboard initialization timeout"));
        }, 5000);
    });
}

async function GetLeaderboardScores(nameLB, maxPlayers, quantityTop, quantityAround, photoSize, auth) {
    try {
        if (leaderboard === null) {
            await WaitForLeaderboard();
        }

        var jsonEntries = {
            technoName: '',
            isDefault: false,
            isInvertSortOrder: false,
            decimalOffset: 0,
            type: '' // , title: ''
        };

        ysdk.getLeaderboards()
            .then(leaderboard => leaderboard.getLeaderboardDescription(nameLB))
            .then(res => {
                jsonEntries.technoName = nameLB;
                jsonEntries.isDefault = res.default;
                jsonEntries.isInvertSortOrder = res.description.invert_sort_order;
                jsonEntries.decimalOffset = res.description.score_format.options.decimal_offset;
                jsonEntries.type = res.description.type; // Не определяется на момент 18.07.23
                //jsonEntries.title = res.title; // Реализуйте по предпочтениям

                return leaderboard.getLeaderboardEntries(nameLB, {
                    quantityTop: quantityTop,
                    includeUser: auth,
                    quantityAround: quantityAround
                });
            })
            .then(res => {
                let jsonPlayers = EntriesLB(res, maxPlayers, photoSize);
                let combinedJson = { ...jsonEntries, ...jsonPlayers };

                myGameInstance.SendMessage('YandexGame', 'LeaderboardEntries', JSON.stringify(combinedJson));
            })
            .catch(error => {
                console.error(error);
            });
    }
    catch (e) {
        console.error('CRASH Get Leaderboard: ', e.message);
    }
}

async function SetLeaderboardScores(_name, score) {
    try {
        if (leaderboard === null) {
            await WaitForLeaderboard();
        }

        ysdk.getLeaderboards()
            .then(leaderboard => {
                leaderboard.setLeaderboardScore(_name, score);
            });
    } catch (e) {
        console.error('CRASH Set Leader board Scores: ', e.message);
    }
}

function EntriesLB(res, maxPlayers, photoSize) {
    let LeaderboardEntriesText = '';
    let playersCount;

    if (res.entries.length < maxPlayers) {
        playersCount = res.entries.length;
    } else {
        playersCount = maxPlayers;
    }

    let ranks = new Array(playersCount);
    let photos = new Array(playersCount);
    let mames = new Array(playersCount);
    let scores = new Array(playersCount);
    let uniqueIDs = new Array(playersCount);

    for (i = 0; i < playersCount; i++) {
        ranks[i] = res.entries[i].rank;
        scores[i] = res.entries[i].score;
        uniqueIDs[i] = res.entries[i].player.uniqueID;
        photos[i] = res.entries[i].player.getAvatarSrc(photoSize);

        if (res.entries[i].player.scopePermissions.public_name !== "allow")
            mames[i] = "anonymous";
        else
            mames[i] = res.entries[i].player.publicName;

        LeaderboardEntriesText += ranks[i] + '. ' + mames[i] + ": " + scores[i] + '\n';
    }

    if (playersCount === 0) {
        LeaderboardEntriesText = 'no data';
    }

    let jsonPlayers = {
        "entries": LeaderboardEntriesText,
        "ranks": ranks,
        "photos": photos,
        "names": mames,
        "scores": scores,
        "uniqueIDs": uniqueIDs
    };

    return jsonPlayers;
}