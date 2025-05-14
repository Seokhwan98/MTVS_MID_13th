using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void OpenGameScene()
    {
        SceneManager.LoadScene("Arium");
    }
}
