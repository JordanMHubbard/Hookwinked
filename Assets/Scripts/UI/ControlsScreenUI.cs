using UnityEngine;

public class ControlsScreenUI : MonoBehaviour
{
    public void Back()
    {
        if (PauseManager.Instance) PauseManager.Instance.ShowPausedScreen();
        if (MainMenu.Instance) MainMenu.Instance.Show();
        gameObject.SetActive(false);
    }
}
