using UnityEngine;
using System.Runtime.InteropServices;
using System;

namespace YG
{
    public partial class YandexGame
    {
        [DllImport("__Internal")]
        private static extern void SaveToLocalStorage(string key, string value);

        [DllImport("__Internal")]
        private static extern string LoadFromLocalStorage(string key);


        [DllImport("__Internal")]
        private static extern int HasKeyInLocalStorage(string key);

        public static bool HasKey(string key)
        {
            try
            {
                return HasKeyInLocalStorage(key) == 1;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }

        [DllImport("__Internal")]
        private static extern void RemoveFromLocalStorage(string key);
        public static void RemoveLocalSaves() => RemoveFromLocalStorage("savesData");
    }
}
