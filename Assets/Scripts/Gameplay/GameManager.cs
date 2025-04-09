using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.SceneManagement;
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
    [SerializeField] private GameObject GainedPerkScreen;
    private GainedPerkUI gainedPerkUI;
    
    [Header("General")]
    [SerializeField] private GameObject SceneTransition;
    [SerializeField] private RectTransform transitionMask;
    [SerializeField] private bool shouldTransitionAtStart;
    private Vector3 originalMaskScale = new Vector3(7f, 7f, 7f);


    // Game Data
    private int currentDay;
    private int shipFragmentsCount = 5;
    public PlayerFishController PlayerController { get; set; }
    public PerkSelectionUI PerkUpgrades { get; set; }
    private List<PerkInfo> perkList = new List<PerkInfo>
    {
        { new PerkInfo("NitroFish", "Longer Speed Boost", 3) },
        { new PerkInfo("Ocean's Endurance", "Slower energy depletion", 3) },
        { new PerkInfo("Coral-lateral Damage", "Shoot rocks faster and deal more damage", 3) },
        { new PerkInfo("Silent Assassin", "Prey's detection range gets smaller", 3) }
    };
    [SerializeField] private List<Sprite> perkIcons;
    public bool GetIsPerkUnlocked(int index) {return perkList[index].isUnlocked;}
    
    // Setters
    public void SetCurrentDay(int day) {currentDay = day;}
    public void SetShipFragmentsCount(int amount) {shipFragmentsCount = amount;}
    public void IncrementShipFragments() {shipFragmentsCount++;}
    public void SetPerkList(List<PerkInfo> perks) {perkList = perks;}
    
    // Getters
    public int GetCurrentDay() {return currentDay;}
    public int GetShipFragmentsCount() {return shipFragmentsCount;}
    public List<PerkInfo> GetPerkList() {return perkList;}

    public void EnableHUD() { PlayerHUD.SetActive(true); }
    public void DisableHUD() { PlayerHUD.SetActive(false); }
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        SaveSystem.Load();

        InitializeUI();
        SetPerkIcons();
        //StartCoroutine(TestPerks()); 
    }

    private void Start()
    {
        if (shouldTransitionAtStart) 
        {
            StartCoroutine(TransitionOut());
        }
        
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
        if (GainedPerkScreen != null) 
        {
            GainedPerkScreen.SetActive(false);
            gainedPerkUI = GainedPerkScreen.GetComponent<GainedPerkUI>();
        }

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

    private void ActivateSurviveScreen()
    {
        SurviveScreen.SetActive(true);
        StartCoroutine(TransitionOut());
    }

    private IEnumerator TransitionIn(Action callback)
    {
        SceneTransition.SetActive(true);
        transitionMask.DOScale(new Vector3(0f, 0f, 0f), 2f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(3f);
        callback?.Invoke();
    }

    private IEnumerator TransitionOut()
    {
        SceneTransition.SetActive(true);
        transitionMask.localScale = new Vector3(0f, 0f, 0f);
        yield return new WaitForSeconds(0.5f);

        transitionMask.DOScale(originalMaskScale, 2f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(2f);
        
        SceneTransition.SetActive(false);
    }

    // Perks
    private void SetPerkIcons()
    {
        for (int i = 0; i < perkList.Count; i++)
        {
            if (i < perkIcons.Count)
            {
                perkList[i].icon = perkIcons[i];
            }
        }
    }

    public void UnlockPerk(int index)
    {
        StartCoroutine(UnlockPerkAnim(index));
    }

    private IEnumerator UnlockPerkAnim(int index)
    {
        yield return new WaitForSeconds(1f);

        perkList[index].isUnlocked = true;
        Debug.Log("Unlocked perk: " + perkList[index].perkName);
        GainedPerkScreen.SetActive(true);
        gainedPerkUI.InitializePerk(perkList[index].perkName, perkList[index].description, perkList[index].icon);
        SaveSystem.Save();
        yield return new WaitForSeconds(7f);

        StartCoroutine(TransitionIn(OpenReefLevel));
    }

    private void OpenReefLevel()
    {
        SceneManager.LoadScene("TheReef");
    }

    #region Save and Load

    public void Save(ref GameSaveData data)
    {
        data.CurrentGameDay = currentDay;
        data.TotalShipFragments = shipFragmentsCount;
        data.perks = perkList;
    }

    public void Load(GameSaveData data)
    {
        currentDay = data.CurrentGameDay;
        shipFragmentsCount = data.TotalShipFragments;
        
        if (data.perks != null) 
        {
            perkList = data.perks;
        }
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
    public List<PerkInfo> perks;
}
