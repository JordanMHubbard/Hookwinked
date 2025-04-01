using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Instance {get; private set;}

    // Event Delegates
    public event System.Action<GameObject> OnPreyConsumed;
    public event System.Action OnHookedMinigameFinished;

    // UI
    [Header("Main Levels")]
    [SerializeField] private GameObject PlayerHUD;
    [SerializeField] private GameObject HookedMinigame;
    [SerializeField] private GameObject DeathScreen;
    [SerializeField] private GameObject SurviveScreen;
    private DeathScreenUI deathScreenUI;

    [Header("Perk Level")]
    [SerializeField] private GameObject PerkSelectionScreen;
    
    [Header("General")]
    [SerializeField] private GameObject SceneTransition;
    [SerializeField] private RectTransform transitionMask;
    private Vector3 originalMaskScale = new Vector3(7f, 7f, 7f);

    // Game Data
    private int currentDay;
    private int shipFragmentsCount = 5;
    public PlayerFishController Player { get; set; }
    public PerkSelectionUI PerkUpgrades { get; set; }
    
    // Setters
    public void SetCurrentDay(int day) {currentDay = day;}
    public void SetShipFragmentsCount(int amount) {shipFragmentsCount = amount;}
    public void IncrementShipFragments() {shipFragmentsCount++;}
    
    // Getters
    public int GetCurrentDay() {return currentDay;}
    public int GetShipFragmentsCount() {return shipFragmentsCount;}

    public void EnableHUD() { PlayerHUD.SetActive(true); }
    public void DisableHUD() { PlayerHUD.SetActive(false); }
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        SaveSystem.Load();

        InitializeUI();
        StartCoroutine(TestPerks());
    }

    // Despawns prey and spawns new one after random amount of time
    public void PreyConsumed(GameObject eatenPrey)
    {
        eatenPrey.SetActive(false);
        
        // Notifies listeners
        OnPreyConsumed?.Invoke(eatenPrey);
    }

    // Minigames
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

    private IEnumerator TestPerks()
    {
        yield return new WaitForSeconds(2f);
        PerkSelectionScreen.SetActive(true);
    }
    // UI
    private void InitializeUI()
    {
        // Main Level
        if (HookedMinigame != null) HookedMinigame.SetActive(false);
        if (SurviveScreen != null) SurviveScreen.SetActive(false);
        if (DeathScreen != null) 
        {
            DeathScreen.SetActive(false);
            deathScreenUI = DeathScreen.GetComponent<DeathScreenUI>();
        }

        // Perk Level
        if (PerkSelectionScreen != null) PerkSelectionScreen.SetActive(false);

        // General
        if (SceneTransition != null) SceneTransition.SetActive(false);
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

    #region Save and Load

    public void Save(ref GameSaveData data)
    {
        data.CurrentGameDay = currentDay;
        data.TotalShipFragments = shipFragmentsCount;
    }

    public void Load(GameSaveData data)
    {
        currentDay = data.CurrentGameDay;
        shipFragmentsCount = data.TotalShipFragments;
    }

    #endregion

    private void OnApplicationQuit()
    {
        SaveSystem.ResetDays();
    }

}

[System.Serializable]
public struct GameSaveData
{
    public int CurrentGameDay;
    public int TotalShipFragments;
}
