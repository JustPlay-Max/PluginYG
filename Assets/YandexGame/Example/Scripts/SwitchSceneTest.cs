using UnityEngine;
using UnityEngine.SceneManagement;

namespace YG.Example
{
    public class SwitchSceneTest : MonoBehaviour
    {
        public void SwitchScene(int sceneID)
        {
            SceneManager.LoadScene(sceneID);
        }
    }
}