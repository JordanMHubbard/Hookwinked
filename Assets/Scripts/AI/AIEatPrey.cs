using System.Collections;
using UnityEngine;

public class AIEatPrey : MonoBehaviour
{
    protected AIFishController controller;
    protected PreyDetection preyDetect;
    protected bool canEat;
    [SerializeField] private AudioClip[] eatSounds;

    private void OnEnable()
    {
        HomeManager.Instance.OnDayFinished += StopEat;
        canEat = true;
    }

    protected virtual void Start()
    {
        controller = GetComponentInParent<AIFishController>();
        preyDetect = controller.GetComponentInChildren<PreyDetection>();

    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!canEat) return;

        if (other.CompareTag("Prey"))
        {
            SoundFXManager.Instance.PlayRandomSoundFXClip(eatSounds, transform, transform.position, 1f, 1f, 0.2f, 0.1f);
            preyDetect.SetIsPursuingPrey(false);
            preyDetect.RemovePrey(other.transform.parent.gameObject);
            GameManager.Instance.PreyConsumed(other.transform.parent.gameObject);
        }
    }

    private void StopEat() { canEat = false; }
}
