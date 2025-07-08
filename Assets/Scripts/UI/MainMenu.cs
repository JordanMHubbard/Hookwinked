using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;
    [SerializeField] private GameObject OptionsScreen;
    [SerializeField] private GameObject ControlsScreen;
    [SerializeField] private RectTransform TitleImageTransform;
    [SerializeField] private GameObject CreditsScreen;
    [SerializeField] private CanvasGroup canvasGroup;

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
        Hide();
    }

    public void OpenControls()
    {
        ControlsScreen.SetActive(true);
        Hide();
    }

    public void OpenCredits()
    {
        CreditsScreen.SetActive(true);
        Hide();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Show()
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
    }
    public void Hide()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0f;
    }


    private void InitializeUI()
    {
        // Menus
        if (OptionsScreen != null) OptionsScreen.SetActive(false);
        if (ControlsScreen != null) ControlsScreen.SetActive(false);
        if (CreditsScreen != null) CreditsScreen.SetActive(false);
    }

    private void TitleAnim()
    {
        TitleImageTransform.DOAnchorPos(TitleImageTransform.anchoredPosition +
            new Vector2(0, 20), 2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }
}
