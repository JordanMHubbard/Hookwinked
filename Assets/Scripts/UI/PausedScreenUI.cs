using UnityEngine.SceneManagement;
using UnityEngine;

public class PausedScreenUI : MonoBehaviour
{
    public void ResumeGame()
    {
        PauseManager.Instance.UnpauseGame();
    }
    public void OpenOptions()
    {
        PauseManager.Instance.ShowOptionsScreen();
        gameObject.SetActive(false);
    }

    public void OpenControls()
    {
        PauseManager.Instance.ShowControlsScreen();
        gameObject.SetActive(false);
    }

    public void ExitToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
