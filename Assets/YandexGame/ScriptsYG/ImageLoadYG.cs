using System.Collections;
using System.Collections.Generic;
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

        private struct LoadTextures { public string link; public Texture2D texture; }
        private static List<LoadTextures> saveTextures = new List<LoadTextures>();

        private void Awake()
        {
            if (rawImage)
                rawImage.enabled = false;
            if (spriteImage)
                spriteImage.enabled = false;

            if (startLoad)
                Load();
            else if (loadAnimObj)
                loadAnimObj.SetActive(false);
        }

        public void Load(string url)
        {
            if (url != "null" && url != null && url != string.Empty)
            {
                Texture2D existingTexture = ExistingTexture(url);
                if (existingTexture)
                    SetTexture(existingTexture);
                else
                    StartCoroutine(SwapPlayerPhoto(url));
            }
        }
        public void Load() => Load(urlImage);

        private Texture2D ExistingTexture(string url)
        {
            List<LoadTextures> images = saveTextures;

            for (int i = 0; i < images.Count; i++)
            {
                if (url == images[i].link)
                    return images[i].texture;
            }

            return null;
        }

        public void ClearImage()
        {
            if (rawImage)
            {
                rawImage.texture = null;
                rawImage.enabled = false;
            }

            if (spriteImage)
            {
                spriteImage.sprite = null;
                spriteImage.enabled = false;
            }

            if (loadAnimObj)
                loadAnimObj.SetActive(false);
        }

        public void PutSprite(Sprite sprite)
        {
            if (rawImage)
            {
                rawImage.texture = sprite.texture;
                rawImage.enabled = true;
            }

            if (spriteImage)
            {
                spriteImage.sprite = sprite;
                spriteImage.enabled = true;
            }

            if (loadAnimObj)
                loadAnimObj.SetActive(false);
        }

        IEnumerator SwapPlayerPhoto(string url)
        {
            if (loadAnimObj)
                loadAnimObj.SetActive(true);

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

                    if (handlerTexture.isDone)
                    {
                        Texture2D existingTexture = ExistingTexture(url);
                        if (existingTexture)
                        {
                            SetTexture(existingTexture);
                        }
                        else
                        {
                            SetTexture(handlerTexture.texture);
                            saveTextures.Add(new LoadTextures
                            {
                                link = url,
                                texture = handlerTexture.texture
                            });
                        }
                    }
                }
            }
        }

        private void SetTexture(Texture2D texture)
        {
            if (rawImage)
            {
                rawImage.texture = texture;
                rawImage.enabled = true;
            }

            if (spriteImage)
            {
                Rect rect = new Rect(0, 0, texture.width, texture.height);
                spriteImage.sprite = Sprite.Create(texture, rect, Vector2.zero);
                spriteImage.enabled = true;
            }

            if (loadAnimObj)
                loadAnimObj.SetActive(false);
        }
    }
}
