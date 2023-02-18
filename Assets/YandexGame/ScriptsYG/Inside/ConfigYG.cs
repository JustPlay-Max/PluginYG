using UnityEditor;
using UnityEngine;

namespace YG.Insides
{
    public class ConfigYG : MonoBehaviour
    {
#if UNITY_EDITOR
        public static string patchYGPrefab = "Assets/YandexGame/Prefabs/YandexGame.prefab";

        public static InfoYG GetInfoYG()
        {
            GameObject ygPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(patchYGPrefab, typeof(GameObject));
            if (ygPrefab == null)
            {
                Debug.LogError($"Префаб YandexGame не был найден по пути: {patchYGPrefab}");
                return null;
            }

            YandexGame ygScr = ygPrefab.GetComponent<YandexGame>();
            if (ygScr == null)
            {
                Debug.LogError($"На объекте YandexGame не был найден компонент YandexGame! Префаб объекта расположен по пути: {patchYGPrefab}");
                return null;
            }

            InfoYG infoYG = ygScr.infoYG;
            if (ygScr == null)
            {
                Debug.LogError($"На компоненте YandexGame не определено поле InfoYG! Префаб YandexGame расположен по пути: {patchYGPrefab}");
                return null;
            }

            return infoYG;
        }
#endif
    }
}