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
    [SerializeField] private AudioClip finishedSound;
    public bool isDayOver { get; private set; }

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
        GameManager.Instance.FadeOutSuspenseMusic();
        GameManager.Instance.PausePlayerEnergy(true);
        ShowWaypoint();
        StartCoroutine(ManageMessage());
    }

    private IEnumerator ManageMessage()
    {
        HomeMessageGroup.gameObject.SetActive(true);
        HomeMessageGroup.DOFade(1f, 1f);
        SoundFXManager.Instance.PlaySoundFXClip(finishedSound, transform, transform.position, 0.75f, 0f);

        yield return new WaitForSeconds(5f);

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

    private void ShowWaypoint()
    {
        HomeWaypoint.SetActive(true);
        Vector3 waypointPos = HomeWaypoint.transform.position;
        HomeWaypoint.transform.DOMove(waypointPos + new Vector3(0f, 1f, 0f), 2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    public void GameOver()
    {
        OnDayFinished?.Invoke();
    }
    
}
