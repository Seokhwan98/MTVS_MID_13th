using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TapToStart : MonoBehaviour
{
    private string nextSceneName = "ARium";

    private bool hasStarted = false;

    void Update()
    {
        if (hasStarted || Touchscreen.current == null)
            return;

        if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            hasStarted = true;

            // 씬 이동
            SceneManager.LoadScene(nextSceneName);
        }
    }
}