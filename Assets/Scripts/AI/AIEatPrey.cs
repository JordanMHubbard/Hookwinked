using UnityEngine;

public class AIEatPrey : MonoBehaviour
{
    private AIFishController controller;
    private PreyDetection preyDetect;
    [SerializeField] private AudioClip[] eatSounds;

    void Start()
    {   
        controller = GetComponentInParent<AIFishController>();
        preyDetect = controller.GetComponentInChildren<PreyDetection>();
        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Prey"))
        {
            SoundFXManager.Instance.PlayRandomSoundFXClip(eatSounds, transform, 1f, 0.2f, 0.1f);
            preyDetect.SetIsPursuingPrey(false);
            preyDetect.RemovePrey(other.transform.parent.gameObject);
            GameManager.Instance.PreyConsumed(other.transform.parent.gameObject);
        }
    }
}
