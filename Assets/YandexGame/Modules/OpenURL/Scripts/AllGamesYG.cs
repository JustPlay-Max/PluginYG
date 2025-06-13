using System;
using System.Collections.Generic;
using UnityEngine;
using UnityToolbag;
using YG.Utils.OpenURL;

namespace YG
{
    public class AllGamesYG : MonoBehaviour
    {
        public Transform rootSpawnGames;
        public GameObject gameInfoPrefab;
        public GameInfo.ImageURL imageURLType;
        public int maxGameSpawn = 10;
#if Flags_yg
        public bool sortUsingFlags;
        [ConditionallyVisible(nameof(sortUsingFlags))]
        public string nameFlag = "SortListGamesYG";
        [ConditionallyVisible(nameof(sortUsingFlags))]
        public bool onlyGamesFromFlag;
#endif
        private void Start()
        {
            UpdateList();
        }

        public void UpdateList()
        {
            DestroyList();
            SpawnList();
        }

        public void DestroyList()
        {
            int childCount = rootSpawnGames.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                Destroy(rootSpawnGames.GetChild(i).gameObject);
            }
        }

        private void SpawnList()
        {
            int count = Math.Clamp(YandexGame.allGames.Length, 0, maxGameSpawn);

            void DefaultSpawnList()
            {
                for (int i = 0; i < count; i++)
                {
                    GameObject obj = Instantiate(gameInfoPrefab, rootSpawnGames);
                    obj.GetComponent<GameYG>().Setup(YandexGame.allGames[i], imageURLType);
                }
            }
#if !Flags_yg
            DefaultSpawnList();
#else
            if (sortUsingFlags)
            {
                string flag = YandexGame.GetFlag(nameFlag);

                if (flag == null)
                {
                    DefaultSpawnList();
                    return;
                }

                string[] gamesStr = flag.Split(new[] { ", ", ",", ",  ", " , ", " ", "  " }, StringSplitOptions.RemoveEmptyEntries);

                if (gamesStr.Length == 0)
                {
                    DefaultSpawnList();
                    return;
                }

                int[] gamesID = new int[gamesStr.Length];

                for (int i = 0; i < gamesStr.Length; i++)
                {
                    if (!int.TryParse(gamesStr[i], out gamesID[i]))
                        gamesID[i] = 0;
                }

                List<int> resGames = new List<int>();

                for (int i = 0; i < gamesID.Length; i++)
                {
                    for (int j = 0; j < YandexGame.allGames.Length; j++)
                    {
                        if (gamesID[i] == YandexGame.allGames[j].appID)
                        {
                            resGames.Add(gamesID[i]);
                            break;
                        }
                    }
                }

                if (!onlyGamesFromFlag)
                {
                    List<int> otherGames = new List<int>();
                    HashSet<int> existingGames = new HashSet<int>(resGames);

                    for (int a = 0; a < YandexGame.allGames.Length; a++)
                    {
                        if (!existingGames.Contains(YandexGame.allGames[a].appID))
                        {
                            otherGames.Add(YandexGame.allGames[a].appID);
                        }
                    }
                    resGames.AddRange(otherGames);
                }

                count = Math.Clamp(resGames.Count, 0, maxGameSpawn);

                for (int i = 0; i < count; i++)
                {
                    GameInfo game = YandexGame.GetGameByID(resGames[i]);
                    GameObject obj = Instantiate(gameInfoPrefab, rootSpawnGames);
                    obj.GetComponent<GameYG>().Setup(game, imageURLType);
                }
            }
            else
            {
                DefaultSpawnList();
            }
#endif
        }

        public void OnDeveloperURL() => YandexGame.OnDeveloperURL();
        public void OnGameURL(int id) => YandexGame.OnGameURL(id);
    }
}