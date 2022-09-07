using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;

namespace YG
{
    public class ImageLoadYG : MonoBehaviour
    {
        [SerializeField] bool startLoad = true;
        [SerializeField] RawImage rawImage;
        [SerializeField] string urlImage;
        [SerializeField] GameObject loadAnimObj;

        private void Awake()
        {
            rawImage.enabled = false;
            if (startLoad) Load();
            else if (loadAnimObj) loadAnimObj.SetActive(false);
        }

        public void Load()
        {
            if (loadAnimObj) loadAnimObj.SetActive(true);
            StartCoroutine(SwapPlayerPhoto(urlImage));
        }

        public void Load(string url)
        {
            if (url != "null")
            {
                if (loadAnimObj) loadAnimObj.SetActive(true);
                StartCoroutine(SwapPlayerPhoto(url));
            }
        }

        IEnumerator SwapPlayerPhoto(string url)
        {
#if UNITY_2020_1_OR_NEWER
            using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                    webRequest.result == UnityWebRequest.Result.DataProcessingError)
                    Debug.LogError("Error: " + webRequest.error);
                else
                {
                    DownloadHandlerTexture handlerTexture = webRequest.downloadHandler as DownloadHandlerTexture;
                    rawImage.texture = handlerTexture.texture;

                    rawImage.enabled = true;
                    if (loadAnimObj) loadAnimObj.SetActive(false);
                }
            }
#endif
#if !UNITY_2020_1_OR_NEWER
#pragma warning disable CS0618 // Тип или член устарел
            using (WWW www = new WWW(url))
#pragma warning restore CS0618 // Тип или член устарел
            {
                yield return www;
                Texture2D texture = www.texture;

                rawImage.texture = texture;

                byte[] bytes = texture.EncodeToJPG();

                File.WriteAllBytes(Application.persistentDataPath + "LoadImage.jpg", bytes);
            }

            rawImage.enabled = true;
            if (loadAnimObj) loadAnimObj.SetActive(false);
#endif
        }
    }
}
