#if UNITY_EDITOR
using System;
using UnityEngine;
using YG.Utils.OpenURL;

namespace YG
{
    public partial class InfoYG
    {
        public OpenURLSettings OpenURL;

        [Serializable]
        public partial class OpenURLSettings
        {
            [Header("Simulation")]
            public string developerURL = "https://yandex.ru/games/developer/25834";
            public GameInfo[] allGames = new GameInfo[]
            {
                new GameInfo()
                {
                    appID = 177895,
                    title = "Падение Тела 3D",
                    url = "https://yandex.ru/games/app/177895",
                    coverURL = "https://avatars.mds.yandex.net/get-games/1881957/2a0000018ab6f8e6b8c3ec2b8c3bf7e5d596/pjpg128x128",
                    iconURL = "https://avatars.mds.yandex.net/get-games/1881957/2a0000018ab6f8e6b8c3ec2b8c3bf7e5d596/pjpg128x128"
                },
                new GameInfo()
                {
                    appID = 341205,
                    title = "Кто победит? Создай битву!",
                    url = "https://yandex.ru/games/app/341205",
                    coverURL = "https://avatars.mds.yandex.net/get-games/1881957/2a00000190318058b9a4d207de946818eaff/pjpg128x128",
                    iconURL = "https://avatars.mds.yandex.net/get-games/1881957/2a00000190318058b9a4d207de946818eaff/pjpg128x128"
                },
                new GameInfo()
                {
                    appID = 191541,
                    title = "ФСБ Открывай!",
                    url = "https://yandex.ru/games/app/191541",
                    coverURL = "https://avatars.mds.yandex.net/get-games/11385414/2a00000190db848f31b9058ad84bfd435ed4/pjpg128x128",
                    iconURL = "https://avatars.mds.yandex.net/get-games/11385414/2a00000190db848f31b9058ad84bfd435ed4/pjpg128x128"
                },
                new GameInfo()
                {
                    appID = 232236,
                    title = "Крутой Спуск ГТА",
                    url = "https://yandex.ru/games/app/232236",
                    coverURL = "https://avatars.mds.yandex.net/get-games/1890793/2a00000189c00383b5000f00c723ed0fa77c/pjpg128x128",
                    iconURL = "https://avatars.mds.yandex.net/get-games/1890793/2a00000189c00383b5000f00c723ed0fa77c/pjpg128x128"
                }
            };
        }
    }
}
#endif