using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;

namespace YG
{
    public class ImageLoadYG : MonoBehaviour
    {
        public bool startLoad = true;
        public RawImage rawImage;
        public Image spriteImage;
        public string urlImage;
        public GameObject loadAnimObj;
        [Tooltip("Вы можете выключить запись лога в консоль.")]
        [SerializeField] bool debug;

        private void Awake()
        {
            if (rawImage) rawImage.enabled = false;
            if (spriteImage) spriteImage.enabled = false;

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
                {
                    if (debug)
                        Debug.LogError("Error: " + webRequest.error);
                }
                else
                {
                    DownloadHandlerTexture handlerTexture = webRequest.downloadHandler as DownloadHandlerTexture;

                    if (rawImage)
                    {
                        if (handlerTexture.isDone)
                            rawImage.texture = handlerTexture.texture;
                        rawImage.enabled = true;
                    }

                    if (spriteImage)
                    {
                        if (handlerTexture.isDone)
                            spriteImage.sprite = Sprite.Create((Texture2D)handlerTexture.texture,
                                new Rect(0, 0, handlerTexture.texture.width, handlerTexture.texture.height), Vector2.zero);

                        spriteImage.enabled = true;
                    }

                    if (loadAnimObj)
                        loadAnimObj.SetActive(false);
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
