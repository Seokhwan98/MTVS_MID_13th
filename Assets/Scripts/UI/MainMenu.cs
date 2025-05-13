using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OpenOptions()
    {
        SceneManager.LoadScene("Option", LoadSceneMode.Additive);
    }
}