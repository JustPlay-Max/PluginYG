using UnityEngine;
using System.Runtime.InteropServices;
using System;

namespace YG
{
    public partial class YandexGame
    {
        public static class LocalStorage
        {
            [DllImport("__Internal")]
            public static extern void SetKey_LocalStorage_js(string key, string value);

            public static void SetKey(string key, string value)
            {
#if PLATFORM_WEBGL
#if UNITY_EDITOR
                PlayerPrefs.SetString(key, value);
#else
                SetKey_LocalStorage_js(key, value);
#endif
#else
                PlayerPrefs.SetString(key, value);
#endif
            }

            [DllImport("__Internal")]
            public static extern string GetKey_LocalStorage_js(string key);

            public static string GetKey(string key)
            {
#if PLATFORM_WEBGL
#if UNITY_EDITOR
                return PlayerPrefs.GetString(key);
#else
                return GetKey_LocalStorage_js(key);
#endif
#else
                return PlayerPrefs.GetString(key);
#endif
            }


            [DllImport("__Internal")]
            private static extern int HasKey_LocalStorage_js(string key);

            public static bool HasKey(string key)
            {
#if PLATFORM_WEBGL
#if UNITY_EDITOR
                return PlayerPrefs.HasKey(key);
#else
                try
                {
                    return HasKey_LocalStorage_js(key) == 1;
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    return false;
                }
#endif
#else
                return PlayerPrefs.HasKey(key);
#endif
            }


            [DllImport("__Internal")]
            public static extern void DeleteKey_LocalStorage_js(string key);

            public static void DeleteKey(string key)
            {
#if PLATFORM_WEBGL
#if UNITY_EDITOR
                PlayerPrefs.DeleteKey(key);
#else
                DeleteKey_LocalStorage_js(key);
#endif
#else
                PlayerPrefs.DeleteKey(key);
#endif
            }
        }
    }
}
