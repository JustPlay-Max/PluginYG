using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchSceneTest : MonoBehaviour
{
    public void SwitchScene(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }
}