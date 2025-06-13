using System.Collections;
using UnityEngine;

public class AIEatPrey : MonoBehaviour
{
    protected AIFishController controller;
    protected PreyDetection preyDetect;
    [SerializeField] private AudioClip[] eatSounds;

    protected virtual void Start()
    {
        controller = GetComponentInParent<AIFishController>();
        preyDetect = controller.GetComponentInChildren<PreyDetection>();

    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Prey"))
        {
            SoundFXManager.Instance.PlayRandomSoundFXClip(eatSounds, transform, 1f, 1f, 0.2f, 0.1f);
            preyDetect.SetIsPursuingPrey(false);
            preyDetect.RemovePrey(other.transform.parent.gameObject);
            GameManager.Instance.PreyConsumed(other.transform.parent.gameObject);
        }
    }
}
