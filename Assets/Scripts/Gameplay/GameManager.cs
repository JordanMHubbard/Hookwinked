using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private Image HurtEffect;
    [SerializeField] private Image ShockedEffect;
    [SerializeField] private GameObject HookedMinigame;
    [SerializeField] private GameObject DeathScreen;
    [SerializeField] private GameObject SurviveScreen;
    [SerializeField] private GameObject DayTransition;
    [SerializeField] private TextMeshProUGUI CurrentDayText;
    [SerializeField] private CanvasGroup DayTransitionGroup;
    [SerializeField] private CanvasGroup CurrentDayGroup;
    [SerializeField] private TextMeshProUGUI RockCountText;
    [SerializeField] private TextMeshProUGUI FragmentsCountText;
    private DeathScreenUI deathScreenUI;

    [Header("Perk Level")]
    [SerializeField] private GameObject PerkSelectionScreen;
    [SerializeField] private GameObject GainedPerkScreen;
    [SerializeField] private GameObject ScreenFade;
    [SerializeField] private GameObject DialogueScreen;
    private GainedPerkUI gainedPerkUI;

    [Header("Spawning")]
    private PreySpawner preySpawner;
    private PredatorSpawner predatorSpawner;
    [SerializeField] private GameObject fishSpawnerBox;

    // Game Data
    public PlayerFishController PlayerController { get; set; }
    private int currentDay;
    private int boatFragmentsCount = 0;
    private int numDayRetries;
    [SerializeField] private List<DaySettings> daySettings;
    [SerializeField] private List<GameObject> boatWaypoints;
    [SerializeField] private List<GameObject> fishWaypoints;
    public PerkSelectionUI PerkUpgrades { get; set; }
    private List<PerkInfo> perkList = new List<PerkInfo>
    {
        { new PerkInfo("NitroFish", "An ethereal spirit hastens your stride. Gain faster top speed when using the swim burst ability", 3) },
        { new PerkInfo("Ocean's Endurance", "A quiet force steadies your soul, causing energy to drain more slowly", 3) },
        { new PerkInfo("Coral-lateral Damage", "Imbued with great strength against your will, you're now able to hurl stones that deal damage to anything they strike", 3) },
        { new PerkInfo("Silent Assassin", "Your aura grows faint in the wild — prey senses you from a shorter distance", 3) }
    };
    [SerializeField] private List<Sprite> perkIcons;
    [SerializeField] private AudioClip ambience;
    [SerializeField] private AudioSource ambienceSource;
    
    // Setters
    public void SetCurrentDay(int day) { currentDay = day; }
    public void IncrementCurrentDay() { currentDay++; }
    public void IncrementNumDayRetries()
    {
        numDayRetries++;
        SaveSystem.Save();
    }
    public void ResetNumDayRetries()
    {
        numDayRetries = 0;
        SaveSystem.Save();
    }
    public void SetBoatFragmentsCount(int amount)
    {
        boatFragmentsCount = amount;
        if (FragmentsCountText) FragmentsCountText.text = boatFragmentsCount.ToString();
    }
    public void SetRockCount(int amount) { RockCountText.text = amount.ToString(); }
    public void SetPerkList(List<PerkInfo> perks) {perkList = perks;}

    // Getters
    public DaySettings GetCurrentDaySettings() { return daySettings[currentDay]; }
    public int GetNumDayRetries() { return numDayRetries; }
    public PreySpawner GetPreySpawner() { return preySpawner; }
    public Vector3 GetRandomFishWaypoint() { 
        return fishWaypoints[UnityEngine.Random.Range(0, fishWaypoints.Count)].transform.position; }
    public PredatorSpawner GetPredatorSpawner() { return predatorSpawner; }
    public int GetCurrentDay() {return currentDay;}
    public int GetBoatFragmentsCount() {return boatFragmentsCount;}
    public Vector3 GetRandomBoatWaypoint() { 
        return boatWaypoints[UnityEngine.Random.Range(0, boatWaypoints.Count)].transform.position; }
    public List<PerkInfo> GetPerkList() {return perkList;}
    public bool GetIsPerkUnlocked(int index) {return perkList[index].isUnlocked;}
    public void PausePlayerEnergy(bool shouldPause) { PlayerController.PauseEnergy(shouldPause); }
    public void EnableHUD() { PlayerHUD.SetActive(true); }
    public void DisableHUD() { PlayerHUD.SetActive(false); }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        SaveSystem.Load();

        InitializeUI();
        InitializeSpawners();
        SetPerkIcons();
        ambienceSource.clip = ambience; 
    }

    private void Start()
    {
        PlayAmbience();

        if (PlayerController)
        {
            InputManager.Instance.isInputPaused = true;
            PlayerController.GetEnergyComp().SetIsPaused(true);
            StartCoroutine(ShowDayTransition());
        }
    }

    public void PlayAmbience() { ambienceSource.Play(); }
    public void PauseAmbience(bool shouldPause)
    {
        if (shouldPause) { ambienceSource.Pause(); }
        else { ambienceSource.UnPause(); }
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
        if (HurtEffect != null) HurtEffect.gameObject.SetActive(false);
        if (ShockedEffect != null) ShockedEffect.gameObject.SetActive(false);

        // Perk Level
        if (PerkSelectionScreen != null) PerkSelectionScreen.SetActive(false);
        if (GainedPerkScreen != null)
        {
            GainedPerkScreen.SetActive(false);
            gainedPerkUI = GainedPerkScreen.GetComponent<GainedPerkUI>();
        }
        if (ScreenFade != null) ScreenFade.SetActive(false);
    }

    private IEnumerator ShowDayTransition()
    {
        CurrentDayText.text = "Day " + currentDay.ToString();
        yield return new WaitForSeconds(2.5f);

        CurrentDayGroup.DOFade(1f, 1f);
        yield return new WaitForSeconds(4f);

        CurrentDayGroup.DOFade(0f, 1f);
        yield return new WaitForSeconds(2f);

        DayTransitionGroup.DOFade(0f, 3f);
        yield return new WaitForSeconds(3f);

        InputManager.Instance.isInputPaused = false;
        DayTransition.SetActive(false);
        if (PlayerController) PlayerController.GetEnergyComp().SetIsPaused(false);

    }

    public void ShowHurtEffect()
    {
        StartCoroutine(ActivateHurtEffect());
    }

    private IEnumerator ActivateHurtEffect()
    {
        HurtEffect.gameObject.SetActive(true);
        HurtEffect.DOFade(1, 0.5f);
        yield return new WaitForSeconds(2f);

        HurtEffect.DOFade(0, 0.5f);
        yield return new WaitForSeconds(0.5f);

        HurtEffect.gameObject.SetActive(false);
    }

    public void ShowShockedEffect()
    {
        StartCoroutine(ActivateShockedEffect());
    }

    private IEnumerator ActivateShockedEffect()
    {
        ShockedEffect.gameObject.SetActive(true);
        ShockedEffect.DOFade(1, 0.5f);
        StartCoroutine(ActivateHurtEffect());
        yield return new WaitForSeconds(2f);

        ShockedEffect.DOFade(0, 0.5f);
        yield return new WaitForSeconds(0.5f);

        ShockedEffect.gameObject.SetActive(false);
    }

    public void ShowDeathScreen(DeathScreenUI.DeathType deathType)
    {
        ambienceSource.Pause();
        PlayerController.PausePlayer();
        deathScreenUI.ChooseRandomMessage(deathType);
        DeathScreen.SetActive(true);
    }

    public void ShowSurviveScreen()
    {
        PlayerController.PausePlayer();
        if (SceneTransitionManager.Instance)
        {
            StartCoroutine(SceneTransitionManager.Instance.TransitionIn(ActivateSurviveScreen));
        }
    }

    private void ActivateSurviveScreen()
    {
        SurviveScreen.SetActive(true);
        if (SceneTransitionManager.Instance)
        {
            StartCoroutine(SceneTransitionManager.Instance.TransitionOut());
        }
    }

    public void ShowPerkScreen()
    {
        StartCoroutine(ActivatePerkScreen());
    }

    private IEnumerator ActivatePerkScreen()
    {
        ScreenFade.SetActive(false);
        yield return new WaitForSeconds(1f);

        PerkSelectionScreen.SetActive(true);
    }

    public GameObject GetScreenFade() { return ScreenFade; }
    public void HideDialogueScreen() { DialogueScreen.SetActive(false); }

    // Minigames
    public void StartHookedMinigame()
    {
        if (HookedMinigame == null || PlayerHUD == null) return;

        StartCoroutine(ActivateHookedMinigame());
    }

    private IEnumerator ActivateHookedMinigame()
    {
        yield return new WaitForSeconds(1f);

        PlayerHUD.SetActive(false);
        HookedMinigame.SetActive(true);
        InputManager.Instance.SwitchCurrentMap(InputManager.ActionMap.HookedMinigame);
    }

    public void ExitHookedMinigame(bool hasEscaped)
    {
        if (HookedMinigame == null ||  PlayerHUD == null) return;

        HookedMinigame.SetActive(false);
        if (!hasEscaped) return;
        
        PlayerHUD.SetActive(true);
        InputManager.Instance.SwitchCurrentMap(InputManager.ActionMap.Player);
        //Increase player speed temporarily
        OnHookedMinigameFinished?.Invoke();
    }

    // Spawning
    private void InitializeSpawners()
    {
        if (fishSpawnerBox != null)
        {
            preySpawner = fishSpawnerBox.GetComponent<PreySpawner>();
            predatorSpawner = fishSpawnerBox.GetComponent<PredatorSpawner>();
        }
        else Debug.LogWarning("fishSpawnerBox has not been set!");
    }

    // Despawns prey and spawns new one after random amount of time
    public void PreyConsumed(GameObject eatenPrey)
    {
        eatenPrey.SetActive(false);

        // Notifies listeners
        OnPreyConsumed?.Invoke(eatenPrey);
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

        if (SceneTransitionManager.Instance)
        {
            StartCoroutine(SceneTransitionManager.Instance.TransitionIn(OpenReefLevel));
        }
    }

    private void OpenReefLevel()
    {
        SceneManager.LoadScene("TheReef");
    }

    #region Save and Load

    public void Save(ref GameSaveData data)
    {
        data.CurrentGameDay = currentDay;
        data.TotalShipFragments = boatFragmentsCount;
        data.perks = perkList;
        data.totalDayRetries = numDayRetries;
    }

    public void Load(GameSaveData data)
    {
        currentDay = data.CurrentGameDay;
        SetBoatFragmentsCount(data.TotalShipFragments);

        if (data.perks.Count > 0)
        {
            perkList = data.perks;
        }
        numDayRetries = data.totalDayRetries;
    }

    #endregion
}

[System.Serializable]
public struct GameSaveData
{
    public int CurrentGameDay;
    public int TotalShipFragments;
    public List<PerkInfo> perks;
    public int totalDayRetries;
}
