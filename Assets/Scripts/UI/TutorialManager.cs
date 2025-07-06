using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] GameObject[] tutorials;
    private int index;

    private void Start()
    {
        tutorials[0].SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Continue()
    {
        tutorials[index++].SetActive(false);
        if (index < tutorials.Length)
        {
            tutorials[index].SetActive(true);
        }
        else
        {

            InputManager.Instance.isInputPaused = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            GameManager.Instance.PausePlayerEnergy(false);
            DayNightCycle.Instance.StartDay();

            gameObject.SetActive(false);
        }
    }
}
