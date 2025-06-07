using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;
    [SerializeField] private GameObject OptionsScreen;
    [SerializeField] private GameObject ControlsScreen;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        InitializeUI();
    }

    private void Start()
    {
        SaveSystem.ResetSaveData();
        Cursor.visible = true;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("TheReef");
    }

    public void OpenOptions()
    {
        OptionsScreen.SetActive(true);
        gameObject.SetActive(false);
    }

    public void OpenControls()
    {
        ControlsScreen.SetActive(true);
        gameObject.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    private void InitializeUI()
    {
        // Menus
        if (OptionsScreen != null) OptionsScreen.SetActive(false);
        if (ControlsScreen != null) ControlsScreen.SetActive(false);
    }
}
