using System.Collections;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SurviveScreenUI : MonoBehaviour
{ 
    [SerializeField] private RectTransform youSurvivedImage;
    [SerializeField] private RectTransform daysSurvivedImage;
    [SerializeField] private RectTransform numDaysObj;
    [SerializeField] private TextMeshProUGUI numDaysText;
    [SerializeField] private RectTransform confettiLeft;
    [SerializeField] private RectTransform confettiRight;
    [SerializeField] private RectTransform partyFishImage;
    [SerializeField] private CanvasGroup continueGroup;
    [SerializeField] private CanvasGroup exitGroup;
    [SerializeField] private AudioClip partySound;
    [SerializeField] private AudioClip daySound;
    [SerializeField] private AudioClip transitionSound;

    private Vector3 confettiLPosition;
    private Vector3 confettiRPosition;
    private Vector3 partyFishPosition;

    private void Awake()
    {
        confettiLPosition = confettiLeft.anchoredPosition;
        confettiRPosition = confettiRight.anchoredPosition;
        partyFishPosition = partyFishImage.anchoredPosition;

        SaveSystem.Save();
        int dayNum = GameManager.Instance.GetCurrentDay() + 1;
        numDaysText.text = dayNum.ToString();
    }
    private void OnEnable()
    {
        StartCoroutine(SurivedStatsAnim());
    }

    private IEnumerator SurivedStatsAnim()
    {
        UISoundFXManager.Instance.PlaySoundFXClip(transitionSound, null, transform.position, 0.6f, 0f);
        yield return new WaitForSeconds(2f);
        
        partyFishImage.DOAnchorPos(partyFishPosition + new Vector3(1400f, 400f, 0f), 0.5f);
        yield return new WaitForSeconds(1f);
        
        youSurvivedImage.DOScaleX(1f, 0.5f);
        yield return new WaitForSeconds(0.25f);

        StartCoroutine(ConfettiAnim());
        yield return new WaitForSeconds(0.5f);
        
        UISoundFXManager.Instance.PlaySoundFXClip(partySound, null, transform.position, 0.5f, 0f);
        yield return new WaitForSeconds(1.5f);

        daysSurvivedImage.DOScaleX(1f, 0.75f);
        yield return new WaitForSeconds(2f);

        numDaysObj.DOScaleX(1f, 0.25f);
        UISoundFXManager.Instance.PlaySoundFXClip(daySound, null, transform.position, 0.5f, 0f);
        yield return new WaitForSeconds(2f);

        StartCoroutine(ButtonsAnim());

    }

    private IEnumerator ConfettiAnim()
    {
        yield return new WaitForSeconds(0.5f);

        confettiLeft.DOScale(1f, 0.5f);
        confettiLeft.DOAnchorPos(confettiLPosition + new Vector3(-180f, 80f, 0f), 0.75f).SetEase(Ease.OutCubic);
        confettiRight.DOScale(1f, 0.5f);
        confettiRight.DOAnchorPos(confettiRPosition + new Vector3(180f, 80f, 0f), 0.75f).SetEase(Ease.OutCubic);
    }

    private IEnumerator ButtonsAnim()
    {
        continueGroup.DOFade(1f, 1f);
        exitGroup.DOFade(1f, 1f);
        yield return new WaitForSeconds(1f);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ContinueGame()
    {
        SoundFXManager.Instance.Unmute();
        GameManager.Instance.ResetNumDayRetries();
        if (SceneTransitionManager.Instance)
        {
            StartCoroutine(SceneTransitionManager.Instance.TransitionIn(OpenPerkLevel));
        }
    }
    private void OpenPerkLevel()
    {
        SceneManager.LoadScene("Perks");
    }

    public void ExitToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    
}

