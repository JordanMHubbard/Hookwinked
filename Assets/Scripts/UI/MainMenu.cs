using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;
    [SerializeField] private GameObject OptionsScreen;
    [SerializeField] private GameObject ControlsScreen;
    [SerializeField] private GameObject TitleImage;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InitializeUI();
    }

    private void Start()
    {
        SaveSystem.ResetSaveData();
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        TitleAnim();
    }

    public void StartGame()
    {
        if (SceneTransitionManager.Instance)
        {
            StartCoroutine(SceneTransitionManager.Instance.TransitionIn(OpenReefLevel));
        }
    }

    private void OpenReefLevel()
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

    private void TitleAnim()
    {
        Vector3 titlePos = TitleImage.transform.position;
        TitleImage.transform.DOMove(titlePos + new Vector3(0f, 20f, 0f), 2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }
}
