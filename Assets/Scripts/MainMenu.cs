using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("TheReef");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
