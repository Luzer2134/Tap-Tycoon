using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void OnPlayButton()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void OnExitButton()
    {
        Application.Quit();
    }
}