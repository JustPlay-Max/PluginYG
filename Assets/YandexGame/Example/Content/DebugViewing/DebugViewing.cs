using UnityEngine;
using UnityEngine.UI;

public class DebugViewing : MonoBehaviour
{
    [SerializeField] Transform cubeRotation;
    [SerializeField] Text timeScale, audioPause;

    void Update()
    {
        cubeRotation.Rotate(Vector3.up * 30 * Time.deltaTime);

        timeScale.text = "timeScale: " + Time.timeScale;
        audioPause.text = "audioPause: " + AudioListener.pause;
    }
}
