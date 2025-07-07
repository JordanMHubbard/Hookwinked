using System.Collections;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SurviveScreenUI : MonoBehaviour
{ 
    [SerializeField] private GameObject youSurvivedImage;
    [SerializeField] private GameObject daysSurvivedImage;
    [SerializeField] private GameObject numDaysObj;
    [SerializeField] private TextMeshProUGUI numDaysText;
    [SerializeField] private GameObject confettiLeft;
    [SerializeField] private GameObject confettiRight;
    [SerializeField] private GameObject partyFishImage;
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
        confettiLPosition = confettiLeft.transform.position;
        confettiRPosition = confettiLeft.transform.position;
        partyFishPosition = partyFishImage.transform.position;

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
        
        partyFishImage.transform.DOMove(partyFishPosition + new Vector3(700f, 150f, 0f), 0.5f);
        yield return new WaitForSeconds(1f);
        
        youSurvivedImage.transform.DOScaleX(1f, 0.5f);
        yield return new WaitForSeconds(0.25f);

        StartCoroutine(ConfettiAnim());
        yield return new WaitForSeconds(0.5f);
        
        UISoundFXManager.Instance.PlaySoundFXClip(partySound, null, transform.position, 0.5f, 0f);
        yield return new WaitForSeconds(1.5f);

        daysSurvivedImage.transform.DOScaleX(1f, 0.75f);
        yield return new WaitForSeconds(2f);

        numDaysObj.transform.DOScaleX(1f, 0.25f);
        UISoundFXManager.Instance.PlaySoundFXClip(daySound, null, transform.position, 0.5f, 0f);
        yield return new WaitForSeconds(2f);

        StartCoroutine(ButtonsAnim());

    }

    private IEnumerator ConfettiAnim()
    {
        yield return new WaitForSeconds(0.5f);

        confettiLeft.transform.DOScale(1f, 0.5f);
        confettiLeft.transform.DOMove(confettiLeft.transform.position + new Vector3(-50f, 50f, 0f), 0.75f).SetEase(Ease.OutCubic);
        confettiRight.transform.DOScale(1f, 0.5f);
        confettiRight.transform.DOMove(confettiRight.transform.position + new Vector3(50f, 50f, 0f), 0.75f).SetEase(Ease.OutCubic);
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

