using System.Collections;
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
    private DeathScreenManager deathScreenManager;

    public void EnableHUD() { PlayerHUD.SetActive(true); }
    public void DisableHUD() { PlayerHUD.SetActive(false); }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        HookedMinigame.SetActive(false);
        DeathScreen.SetActive(false);
        deathScreenManager = DeathScreen.GetComponent<DeathScreenManager>();
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
        InputManager.instance.SwitchCurrentMap(InputManager.ActionMap.HookedMinigame);
    }


    public void ExitHookedMinigame()
    {
        if (HookedMinigame == null ||  PlayerHUD == null) return;

        HookedMinigame.SetActive(false);
        PlayerHUD.SetActive(true);
        InputManager.instance.SwitchCurrentMap(InputManager.ActionMap.Player);
        //Increase player speed temporarily
        OnHookedMinigameFinished?.Invoke();
    }

    public void ShowDeathScreen(DeathScreenManager.DeathType deathType)
    {
        deathScreenManager.ChooseRandomMessage(deathType);
        DeathScreen.SetActive(true);
    }

}
