using UnityEngine;

public class AIEatPrey : MonoBehaviour
{
    private SoundRandomizer eatSoundComp;
    private AIFishController controller;
    private PreyDetection preyDetect;

     void Awake()
    {
        eatSoundComp = GetComponent<SoundRandomizer>();
    }

    void Start()
    {   
        controller = GetComponentInParent<AIFishController>();
        preyDetect = controller.GetComponentInChildren<PreyDetection>();
        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Prey"))
        {
            eatSoundComp.PlayRandomSound();
            preyDetect.RemovePrey(other.gameObject);
            GameManager.Instance.PreyConsumed(other.gameObject);
        }
    }
}
