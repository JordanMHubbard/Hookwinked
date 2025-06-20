using System.Collections;
using DG.Tweening;
using UnityEngine;

public class HomeManager : MonoBehaviour
{
    public static HomeManager Instance { get; private set; }

    //Event Delegates
    public event System.Action OnDayFinished;

    [SerializeField] private GameObject HomeWaypoint;
    [SerializeField] private CanvasGroup HomeMessageGroup;
    private bool isDayOver;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (HomeWaypoint != null) HomeWaypoint.SetActive(false);
        if (HomeMessageGroup != null) HomeMessageGroup.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isDayOver)
            {
                StartCoroutine(EndDay());
            }
        }
    }

    public void StartDayEnd()
    {
        isDayOver = true;
        GameManager.Instance.PausePlayerEnergy(true);
        HomeWaypoint.SetActive(true);
        StartCoroutine(ManageMessage());
    }

    private IEnumerator ManageMessage()
    {
        HomeMessageGroup.gameObject.SetActive(true);
        HomeMessageGroup.DOFade(1f, 1f);
        yield return new WaitForSeconds(3f);

        HomeMessageGroup.DOFade(0f, 1f);
    }

    private IEnumerator EndDay()
    {
        isDayOver = false;
        OnDayFinished?.Invoke();
        yield return new WaitForSeconds(2f);

        HomeMessageGroup.gameObject.SetActive(false);
        GameManager.Instance.ShowSurviveScreen();
        GameManager.Instance.PauseAmbience(true);
    }
    
}
