using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Instance {get; private set;}
    public event System.Action<GameObject> OnPreyConsumed;
    [SerializeField] private GameObject HookedMinigame;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        HookedMinigame.SetActive(false);
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
        if (HookedMinigame == null) return;

        HookedMinigame.SetActive(true);
        InputManager.instance.SwitchCurrentMap(InputManager.ActionMap.HookedMinigame);
    }

   


}
