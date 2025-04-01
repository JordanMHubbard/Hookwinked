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

    private Vector3 confettiLPosition;
    private Vector3 confettiRPosition;
    private Vector3 partyFishPosition;

    private void Awake()
    {
        confettiLPosition = confettiLeft.transform.position;
        confettiRPosition = confettiLeft.transform.position;
        partyFishPosition = partyFishImage.transform.position;
        numDaysText.text = GameManager.Instance.GetCurrentDay().ToString();
    }
    private void OnEnable()
    {
        StartCoroutine(SurivedStatsAnim());
    }

    private IEnumerator SurivedStatsAnim()
    {
        yield return new WaitForSeconds(2f);
        
        partyFishImage.transform.DOMove(partyFishPosition + new Vector3(700f, 150f, 0f), 0.5f);
        yield return new WaitForSeconds(1f);
        
        youSurvivedImage.transform.DOScaleX(1f, 0.5f);
        yield return new WaitForSeconds(0.25f);

        StartCoroutine(ConfettiAnim());
        yield return new WaitForSeconds(2f);

        daysSurvivedImage.transform.DOScaleX(1f, 0.75f);
        yield return new WaitForSeconds(2f);

        numDaysObj.transform.DOScaleX(1f, 0.25f);
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
        SceneManager.LoadScene("TheReef");
    }
    public void ExitToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    
}

