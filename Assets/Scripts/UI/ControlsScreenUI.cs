using UnityEngine;

public class ControlsScreenUI : MonoBehaviour
{
    public void Back()
    {
        PauseManager.Instance.ShowPausedScreen();
        gameObject.SetActive(false);
    }
}
