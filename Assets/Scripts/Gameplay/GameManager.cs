using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Instance {get; private set;}
    public event System.Action<GameObject> OnPreyConsumed;
    public event System.Action OnHookedMinigameFinished;
    [SerializeField] private GameObject PlayerHUD;
    [SerializeField] private GameObject HookedMinigame;
    [SerializeField] private GameObject DeathScreen;
    [SerializeField] private GameObject SurviveScreen;
    [SerializeField] private GameObject SceneTransition;
    [SerializeField] private RectTransform transitionMask;
    private Vector3 originalMaskScale = new Vector3(7f, 7f, 7f);
    private DeathScreenUI deathScreenUI;

    public void EnableHUD() { PlayerHUD.SetActive(true); }
    public void DisableHUD() { PlayerHUD.SetActive(false); }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        HookedMinigame.SetActive(false);
        DeathScreen.SetActive(false);
        SurviveScreen.SetActive(false);
        SceneTransition.SetActive(false);
        deathScreenUI = DeathScreen.GetComponent<DeathScreenUI>();
    }

    // Despawns prey and spawns new one after random amount of time
    public void PreyConsumed(GameObject eatenPrey)
    {
        eatenPrey.SetActive(false);
        
        // Notifies listeners
        OnPreyConsumed?.Invoke(eatenPrey);
    }

    public void StartHookedMinigame()
    {
        if (HookedMinigame == null ||  PlayerHUD == null) return;
        
        StartCoroutine(ActivateHookedMinigame());
    }

    private IEnumerator ActivateHookedMinigame()
    {
        yield return new WaitForSeconds(1f);

        PlayerHUD.SetActive(false);
        HookedMinigame.SetActive(true);
        InputManager.Instance.SwitchCurrentMap(InputManager.ActionMap.HookedMinigame);
    }


    public void ExitHookedMinigame()
    {
        if (HookedMinigame == null ||  PlayerHUD == null) return;

        HookedMinigame.SetActive(false);
        PlayerHUD.SetActive(true);
        InputManager.Instance.SwitchCurrentMap(InputManager.ActionMap.Player);
        //Increase player speed temporarily
        OnHookedMinigameFinished?.Invoke();
    }

    public void ShowDeathScreen(DeathScreenUI.DeathType deathType)
    {
        deathScreenUI.ChooseRandomMessage(deathType);
        DeathScreen.SetActive(true);
    }

    public void ShowSurviveScreen()
    {
        StartCoroutine(TransitionIn(ActivateSurviveScreen));
    }

    private IEnumerator TransitionIn(Action callback)
    {
        SceneTransition.SetActive(true);
        transitionMask.DOScale(new Vector3(0f, 0f, 0f), 2f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(3f);

        transitionMask.DOScale(originalMaskScale, 2f).SetEase(Ease.Linear);
        callback?.Invoke();
        yield return new WaitForSeconds(2f);
        
        SceneTransition.SetActive(false);
    }

    private void ActivateSurviveScreen()
    {
        SurviveScreen.SetActive(true);
    }

}
