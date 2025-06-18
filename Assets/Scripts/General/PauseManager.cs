using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;
    public bool IsPaused { get; private set; }
    public bool IsOnPauseMenu { get; private set; }

    [Header("Menus")]
    [SerializeField] private GameObject PausedScreen;
    [SerializeField] private GameObject OptionsScreen;
    [SerializeField] private GameObject ControlsScreen;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InitializeUI();
    }
    
    private void Update()
    {
        if (!IsPaused)
        {
            if (InputManager.Instance.MenuOpenInput)
            {
                PauseGame();
            }
        }
        else
        {
            if (InputManager.Instance.UIMenuCloseInput && IsOnPauseMenu)
            {
                UnpauseGame();
            }
        }
    }

    public void PauseGame()
    {
        IsPaused = true;
        GameManager.Instance.PausePlayerEnergy(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true; 
        Time.timeScale = 0f;

        InputManager.Instance.SwitchCurrentMap(InputManager.ActionMap.UI);

        ShowPausedScreen();
    }

    public void UnpauseGame()
    {
        IsPaused = false;
        GameManager.Instance.PausePlayerEnergy(false);
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; 

        InputManager.Instance.SwitchCurrentMap(InputManager.ActionMap.Player);

        HidePausedScreen();
    }

    private void InitializeUI()
    {
        // Menus
        if (PausedScreen != null) PausedScreen.SetActive(false);
        if (OptionsScreen != null)
        {
            //optionsScreenUI = OptionsScreen.GetComponent<OptionsScreenUI>();
            OptionsScreen.SetActive(false);
        }
        if (ControlsScreen != null) ControlsScreen.SetActive(false);
    }

    public void ShowPausedScreen()
    {
        PausedScreen.SetActive(true);
        IsOnPauseMenu = true;
    }
    public void HidePausedScreen()
    {
        PausedScreen.SetActive(false);
    }
    public void ShowOptionsScreen()
    {
        OptionsScreen.SetActive(true);
        IsOnPauseMenu = false;
    }
    public void ShowControlsScreen()
    {
        ControlsScreen.SetActive(true);
        IsOnPauseMenu = false;
    }
}
