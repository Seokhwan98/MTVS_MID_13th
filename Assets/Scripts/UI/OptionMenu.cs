using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionMenu : MonoBehaviour
{
    public void CloseOptions()
    {
        SceneManager.UnloadSceneAsync("Option");
    }
}